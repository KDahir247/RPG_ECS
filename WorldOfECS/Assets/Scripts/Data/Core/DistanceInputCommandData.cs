using System;
using Unity.Entities;
using Unity.Physics;

namespace WorldOfECS.Data
{
    [Serializable]
    [GenerateAuthoringComponent]
    public struct DistanceInputCommandData : IComponentData
    {
        public float maxDistance;
        public CollisionFilter collision;
    }
}