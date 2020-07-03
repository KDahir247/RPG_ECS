using System;
using Unity.Entities;

namespace WorldOfECS.Data
{
    [Serializable]
    [GenerateAuthoringComponent]
    public struct MovementData : IComponentData
    {
        //Tag for all entity that can move and get called by NavMeshControllerSystem.
    }
}