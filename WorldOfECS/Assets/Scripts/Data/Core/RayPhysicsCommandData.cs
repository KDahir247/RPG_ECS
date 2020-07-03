using Unity.Entities;
using UnityEngine;

namespace WorldOfECS.Data
{
    [GenerateAuthoringComponent]
    public struct RayPhysicsCommandData : IComponentData
    {
        public float distance;
        public LayerMask layerMask;
        public int maxHits;
        public int minimumCommandPerJob;
    }
}