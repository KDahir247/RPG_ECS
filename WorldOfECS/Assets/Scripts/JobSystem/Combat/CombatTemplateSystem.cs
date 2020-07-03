using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using WorldOfECS.Binding;
using WorldOfECS.Data;

namespace WorldOfECS.ComponentSystem
{
    [UpdateAfter(typeof(RaycastCommandBufferSystem))]
    [UpdateAfter(typeof(DistanceJobSystem))]
    [BurstCompile(FloatPrecision.Low, FloatMode.Fast, CompileSynchronously = false)]
    public class CombatTemplateSystem : JobComponentSystem
    {
        protected override void OnStartRunning()
        {
            Entities
                .WithAll<CombatData>()
                .WithoutBurst()
                .ForEach((Entity entity,
                    EventBinding binding) => binding.DstManager = EntityManager).Run();
        }


        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            Entities
                .WithAll<CombatData>()
                .WithoutBurst()
                .ForEach((Entity entity,
                    EventBinding binding,
                    in PhysicsCastData physicsCastData) =>
                {
                    if (physicsCastData.hasReached && physicsCastData.status == Status.Targetable)
                    {
                        binding.CastData = physicsCastData;
                        binding.combatEventBind.Value  = true;
                      
                    }else if (!physicsCastData.hasReached)
                    {
                        binding.combatEventBind.Value = false;
                    }
                    
                }).Run();
           

            return inputDeps;
        }
    }
}