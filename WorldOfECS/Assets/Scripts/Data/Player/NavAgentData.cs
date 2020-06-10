using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.AI;

namespace WOrldOfECS.Data
{
    [GenerateAuthoringComponent]
    public struct NavAgentData : IComponentData
    {
        public float3 destination;
        public float3 normal;
        public float distance;
    }
}