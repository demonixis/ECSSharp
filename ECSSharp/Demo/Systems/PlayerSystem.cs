using ECSSharp.Demo.Components;
using ECSSharp.Framework;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace ECSSharp.Demo.Systems
{
    public class PlayerSystem : Framework.System
    {
        public object Parallele { get; private set; }

        public override void Execute(World world)
        {
            var mobArchtypes = world.GetArchetypes(typeof(Transform), typeof(Mob), typeof(Health));
            var playerArchtypes = world.GetArchetypes(typeof(Player), typeof(Transform), typeof(Weapon), typeof(Health));
            var player = playerArchtypes[0];
            var me = world.GetComponent<Transform>(player);
            var weapon = world.GetComponent<Weapon>(player);
            var health = world.GetComponent<Health>(player);

            if (health.HP <= 0)
                return;

            foreach (var mobEntity in mobArchtypes)
            {
                var transform = world.GetComponent<Transform>(mobEntity);
                var mob = world.GetComponent<Mob>(mobEntity);
                var mobHealth = world.GetComponent<Health>(mobEntity);

                if (mobHealth.HP <= 0)
                    continue;

                if (Vector3.Distance(me.Position, transform.Position) < 2)
                {
                    if (mobHealth.Shield > 0)
                    {
                        mobHealth.Shield -= weapon.Damage;

                        if (mobHealth.Shield < 0)
                        {
                            var value = Math.Abs(mobHealth.Shield);
                            mobHealth.Shield = 0;

                            mobHealth.HP -= value;
                        }
                    }
                    else
                        mobHealth.HP -= weapon.Damage;

                    if (mobHealth.HP < 0)
                        mobHealth.HP = 0;

                    if (mobHealth.HP == 0)
                        Console.WriteLine("The player has killed a mob!");
                }
            }
        }
    }
}
