using ECSSharp.Framework;
using System.Numerics;

namespace ECSSharp.Demo.Components
{
    public sealed class Transform : Component
    {
        public Matrix4x4 WorldMatrix;
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;
        public uint ParentEntity;
    }
}
