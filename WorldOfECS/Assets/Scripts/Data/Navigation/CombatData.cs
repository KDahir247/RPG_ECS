using System;
using Unity.Entities;
using UnityEngine;

namespace WorldOfECS.Data
{
    [Serializable]
    [GenerateAuthoringComponent]
    public struct CombatData : IComponentData
    {
        //Tag for all entity can trigger combat and get called from CombatJobSystem
    }
}