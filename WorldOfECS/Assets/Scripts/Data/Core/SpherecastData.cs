using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


namespace WorldOfECS.Data
{
    [System.Serializable]
    [GenerateAuthoringComponent]
    public struct SpherecastData : IComponentData
    {
        public Status status;
        public RaycastHit hit;
        public float stoppingDistance; //TODO move to a separate component
        public bool hasReached; //TODO move to a separate component
    }
}