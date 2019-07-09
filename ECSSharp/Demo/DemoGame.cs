using ECSSharp.Demo.Components;
using ECSSharp.Demo.Systems;
using ECSSharp.Framework;
using System;
using System.Numerics;
using System.Threading;

namespace ECSSharp.Demo
{
    public class DemoGame
    {
        private Thread _thread;
        private bool _running;

        public void Start()
        {
            Stop();

            _thread = new Thread(new ThreadStart(GameLoop));
            _thread.Start();
        }

        public void Stop()
        {
            _running = false;

            if (_thread != null && _thread.IsAlive)
                _thread.Join();

            _thread = null;
        }

        private void GameLoop()
        {
            Console.WriteLine($"Starting the ECS Demo");

            var world = World.Get();

            // Create a Player.
            var player = CreatePlayer();

            for (var i = 0; i < 10000; i++)
                CreateMob(player);

            world.AddSystem(new MobSystem());
            world.AddSystem(new PlayerSystem());
            world.AddSystem(new TransformSystem());

            _running = true;

            while (_running)
            {
                world.Update();
                Thread.Sleep(15);
            }
        }

        private uint CreatePlayer()
        {
            var world = World.Get();
            var player = world.CreateEntity();

            world.AddComponent(player, new Transform()
            {
                Scale = Vector3.One
            });

            world.AddComponent(player, new Health()
            {
                HP = 100,
                Shield = 100
            });

            world.AddComponent(player, new Weapon()
            {
                Owner = player,
                Damage = 25
            });

            world.AddComponent(player, new Player());

            var weapon = world.CreateEntity();

            world.AddComponent(weapon, new Transform()
            {
                Position = new Vector3(0.5f, 1.0f, 0.5f),
                ParentEntity = player
            });

            return player;
        }

        private void CreateMob(uint target)
        {
            var world = World.Get();
            var mob = world.CreateEntity();
            var random = new Random();

            world.AddComponent(mob, new Transform()
            {
                Position = new Vector3(random.Next(-50, 50), 0.0f, random.Next(-50, 50)),
                Rotation = Vector3.Zero,
                Scale = Vector3.One
            });

            world.AddComponent(mob, new Health()
            {
                HP = 15,
                Shield = 10
            });

            world.AddComponent(mob, new Mob()
            {
                KillScore = 20,
                Target = target
            });

            world.AddComponent(mob, new Weapon()
            {
                Damage = 15,
                Owner = mob
            });
        }
    }
}
