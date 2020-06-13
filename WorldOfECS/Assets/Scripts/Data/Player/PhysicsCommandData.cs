using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace WorldOfECS.Data
{
    [GenerateAuthoringComponent]
    public struct PhysicsCommandData : IComponentData
    {

        public float distance;
        public LayerMask layerMask;
        public int maxHits;
        public int minimumCommandPerJob;
    }
}