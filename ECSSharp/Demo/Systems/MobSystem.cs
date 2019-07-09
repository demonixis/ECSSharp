using ECSSharp.Framework;
using ECSSharp.Demo.Components;
using System.Numerics;
using System;

namespace ECSSharp.Demo.Systems
{
    public class MobSystem : Framework.System
    {
        public override void Execute(World world)
        {
            var entities = world.GetArchetypes(typeof(Mob), typeof(Weapon), typeof(Health), typeof(Transform));

            foreach (var entity in entities)
            {
                var mob = world.GetComponent<Mob>(entity);
                var weapon = world.GetComponent<Weapon>(entity);
                var health = world.GetComponent<Health>(entity);
                var transform = world.GetComponent<Transform>(entity);
                var targetTransform = world.GetComponent<Transform>(mob.Target);
                var playerHealth = world.GetComponent<Health>(mob.Target);

                if (health.HP <= 0 || playerHealth.HP <= 0)
                    continue;

                // Can I Attack?
                if (Vector3.Distance(transform.Position, targetTransform.Position) < 2)
                {
                    if (playerHealth.Shield > 0)
                    {
                        playerHealth.Shield -= weapon.Damage;

                        if (playerHealth.Shield < 0)
                        {
                            Console.WriteLine($"A Mob has destroyed the shield of the player");

                            var value = Math.Abs(playerHealth.Shield);
                            playerHealth.Shield = 0;

                            playerHealth.HP -= value;
                        }
                    }
                    else
                        playerHealth.HP -= weapon.Damage;

                    if (playerHealth.HP < 0)
                        playerHealth.HP = 0;

                    if (playerHealth.HP == 0)
                        Console.WriteLine("A mob has killed the player!");
                }
                // Move in direction of the player.
                else
                {
                    var direction = targetTransform.Position - transform.Position;
                    transform.Position += direction;
                }
            }
        }
    }
}
