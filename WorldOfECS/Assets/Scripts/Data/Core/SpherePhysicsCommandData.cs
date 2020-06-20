using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace WorldOfECS.Data
{
    [System.Serializable]
    [GenerateAuthoringComponent]
    public struct SpherePhysicsCommandData : IComponentData
    {
        public float3 direction;
        public float3 origin;
        public float radius;
        public float distance;
        public LayerMask layerMask;
        public int minimumCommandPerJob;
    }
}