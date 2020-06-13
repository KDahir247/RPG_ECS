using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.AI;

namespace WorldOfECS.Data
{
    [GenerateAuthoringComponent]
    public struct NavAgentData : IComponentData
    {
        public float3 destination;
        public float3 normal;
        public float distance;
        public bool hasPath;
    }
}