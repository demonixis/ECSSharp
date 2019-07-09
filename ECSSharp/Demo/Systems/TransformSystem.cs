using ECSSharp.Framework;
using ECSSharp.Demo.Components;
using System.Numerics;
using System.Threading.Tasks;
using System.Diagnostics;
using System;

namespace ECSSharp.Demo.Systems
{
    public class TransformSystem : Framework.System
    {
        public bool Multithread { get; set; } = true;

        public override void Execute(World world)
        {
            var entities = world.GetArchetypes(typeof(Transform));

            var watch = new Stopwatch();
            watch.Start();

            if (Multithread)
            {
                Parallel.ForEach(entities, (entity, index) => Update(world, entity));
            }
            else
            {
                foreach (var entity in entities)
                    Update(world, entity);
            }

            watch.Stop();
            Console.WriteLine($"Transform updated in {watch.ElapsedMilliseconds}ms. Multithreading sets to {Multithread}.");
        }

        private void UpdateMultiThreads(World world, uint[] entities)
        {
            Transform transform;
            Vector3 rotation;

            Parallel.ForEach(entities, (entity, index) =>
            {
                transform = world.GetComponent<Transform>(entity);
                rotation = transform.Rotation;

                transform.WorldMatrix =
                    Matrix4x4.CreateScale(transform.Scale) *
                    Matrix4x4.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z) *
                    Matrix4x4.CreateTranslation(transform.Position);

                if (transform.ParentEntity > 0)
                {
                    var parentTransform = world.GetComponent<Transform>(transform.ParentEntity);

                    if (parentTransform != null)
                        transform.WorldMatrix *= parentTransform.WorldMatrix;
                }
            });
        }

        private void Update(World world, uint entity)
        {
            var transform = world.GetComponent<Transform>(entity);
            var rotation = transform.Rotation;

            transform.WorldMatrix =
                Matrix4x4.CreateScale(transform.Scale) *
                Matrix4x4.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z) *
                Matrix4x4.CreateTranslation(transform.Position);

            if (transform.ParentEntity > 0)
            {
                var parentTransform = world.GetComponent<Transform>(transform.ParentEntity);

                if (parentTransform != null)
                    transform.WorldMatrix *= parentTransform.WorldMatrix;
            }
        }

        private void UpdateSingleThread(World world, uint[] entities)
        {

            
        }
    }
}
