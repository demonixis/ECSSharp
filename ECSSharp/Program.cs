using ECSSharp.Demo;
using ECSSharp.Framework;
using System;
using System.Numerics;
using System.Threading;

namespace ECSSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var demo = new DemoGame();
            demo.Start();

            Console.ReadKey();

            demo.Stop();
        }
    }
}
