using System;
using Unity.Entities;
using UnityEngine;

namespace WorldOfECS.Data
{
    public enum Status
    {
        Nil,
        Idle, // for npc, and enemies
        Movable,
        Targetable
    }


    //TODO used by every script
    [Serializable]
    [GenerateAuthoringComponent]
    public struct PhysicsCastData : IComponentData
    {
        public Status status;

        public Entity entity;
        public RaycastHit hit;
        public bool hasTarget;

        public float stoppingDistance; //TODO move to a separate component
        public bool hasReached; //TODO move to a separate component
    }
}