using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AI;
using WorldOfECS.Binding;
using WorldOfECS.Data;

namespace WorldOfECS.ComponentSystem
{
    [UpdateAfter(typeof(RaycastCommandBufferSystem))]
    [UpdateAfter(typeof(DistanceJobSystem))]
    [BurstCompile(FloatPrecision.Low, FloatMode.Fast, FloatMode = FloatMode.Fast, CompileSynchronously = false)]
    public class NavMeshControllerSystem : JobComponentSystem
    {
        private readonly NavMeshPath _path = new NavMeshPath();

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            Entities
                .WithoutBurst()
                .WithAll<MovementData>()
                .ForEach((Entity entity,
                    EventBinding binding,
                    NavMeshAgent agent,
                    in PhysicsCastData physicsCastData) =>
                {
                    if (physicsCastData.hasTarget && !physicsCastData.hasReached)
                    {
                        agent.CalculatePath(physicsCastData.hit.point, _path);

                        if (_path.status == NavMeshPathStatus.PathComplete)
                        {
                            agent.stoppingDistance = physicsCastData.stoppingDistance;
                            agent.SetDestination(physicsCastData.hit.point);
                        }
                    }
                    
                    if(physicsCastData.status == Status.Targetable && !physicsCastData.hasReached)
                        binding.movementEventBind.Value = false;
                    else if (physicsCastData.status != Status.Targetable)
                        binding.movementEventBind.Value = physicsCastData.hasReached;
                    
                    
                }).Run();

            var jobHandle = Entities
                .WithAll<MovementData>()
                .ForEach((Entity entity,
                    ref Translation position,
                    ref Rotation rotation,
                    ref PhysicsCastData physicsCastData,
                    in LocalToWorld matrix) =>
                {
                    if (math.round(math.distance(position.Value, physicsCastData.hit.point)) <=
                        physicsCastData.stoppingDistance)
                    {
                        physicsCastData.hasReached = true;
                        physicsCastData.hasTarget = false;
                    }
                    else
                    {
                        position.Value = matrix.Position;
                        rotation.Value = matrix.Rotation;
                    }
                }).Schedule(inputDeps);
            
            return jobHandle;
        }
    }
}