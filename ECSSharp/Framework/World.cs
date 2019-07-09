using System;
using System.Collections.Generic;

namespace ECSSharp.Framework
{
    public class World
    {
        private static World _instance = null;
        private uint _entityCounter = 0;
        private Dictionary<Type, List<Component>> _components = new Dictionary<Type, List<Component>>();
        private Dictionary<uint, List<Component>> _entityComponents = new Dictionary<uint, List<Component>>();
        private List<System> _systems = new List<System>();

        public static World Get()
        {
            if (_instance == null)
                _instance = new World();

            return _instance;
        }

        public void Update()
        {
            foreach (var system in _systems)
                system.Execute(this);
        }

        #region Entity Management

        public uint CreateEntity()
        {
            var id = _entityCounter++;

            _entityComponents.Add(id, new List<Component>());

            return id;
        }

        public void RemoveEntity(uint id)
        {
        }

        public uint GetEntity(Type component, out bool found)
        {
            List<Component> components = null;

            foreach (var keyValue in _entityComponents)
            {
                components = keyValue.Value;

                foreach (var c in components)
                {
                    if (c.GetType() == component)
                    {
                        found = true;
                        return keyValue.Key;
                    }
                }
            }

            found = false;
            return 0;
        }

        #endregion

        #region System Management

        public void AddSystem(System system)
        {
            if (!_systems.Contains(system))
                _systems.Add(system);
        }

        public void RemoveSystem(System system)
        {
            if (_systems.Contains(system))
                _systems.Remove(system);
        }

        public T GetSystem<T>() where T : System
        {
            foreach (var system in _systems)
            {
                if (system is T)
                    return (T)system;
            }

            return null;
        }

        #endregion

        #region Component Management

        public bool AddComponent(uint entity, Component component)
        {
            var entityComponents = _entityComponents[entity];
            var add = !entityComponents.Contains(component);

            if (add)
            {
                entityComponents.Add(component);

                var type = component.GetType();

                if (!_components.ContainsKey(type))
                {
                    var components = new List<Component>();
                    components.Add(component);

                    _components.Add(type, components);
                }
                else
                    _components[type].Add(component);
            }

            return add;
        }

        public void RemoveComponent(uint entity, Component component)
        {
            _entityComponents[entity].Remove(component);
            _components[component.GetType()].Remove(component);
        }

        public T GetComponent<T>() where T : Component
        {
            var type = typeof(T);

            if (_components.ContainsKey(type))
                return (T)_components[type][0];

            return null;
        }

        public T[] GetComponents<T>() where T : Component
        {
            var type = typeof(T);
            var list = new List<T>();

            foreach (var keyValue in _components)
            {
                if (keyValue.Key == type)
                    list.AddRange((T[])keyValue.Value.ToArray());
            }

            return list.ToArray();
        }

        public T GetComponent<T>(uint entity) where T : Component
        {
            foreach (var component in _entityComponents[entity])
            {
                if (component is T)
                    return (T)component;
            }

            return null;
        }

        public T[] GetComponents<T>(uint entity) where T : Component
        {
            var list = new List<T>();

            foreach (var component in _entityComponents[entity])
            {
                if (component is T)
                    list.Add((T)component);
            }

            return list.ToArray();
        }

        public bool HasComponent(uint entity, Type type)
        {
            foreach (var component in _entityComponents[entity])
            {
                if (component.GetType() == type)
                    return true;
            }

            return false;
        }

        public uint[] GetArchetypes(params Type[] types)
        {
            var list = new List<uint>();
            var isValid = true;

            foreach (var entity in _entityComponents)
            {
                isValid = true;

                foreach (var type in types)
                    isValid &= HasComponent(entity.Key, type);

                if (isValid)
                    list.Add(entity.Key);
            }

            return list.ToArray();
        }

        #endregion
    }
}
