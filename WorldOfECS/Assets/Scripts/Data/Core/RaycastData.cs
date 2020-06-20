using Unity.Entities;
using UnityEngine;
namespace WorldOfECS.Data
{
    public enum Status
    {
        Nil,
        Idle, // for npc, and enemies
        Movable,
        Targetable,
    }
    
    
    [GenerateAuthoringComponent]
    public struct RaycastData : IComponentData
    {
        public Status status;
        public RaycastHit hit;
        public  float stoppingDistance; //TODO move to a separate component
        public bool hasReached; //TODO move to a separate component
    }
}
