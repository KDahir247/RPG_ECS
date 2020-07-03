using UniRx;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using WorldOfECS.Data;

namespace WorldOfECS.ComponentSystem
{
    /// <summary>
    ///     This is experimental for later replacing RaycastCommandBufferSystem
    /// </summary>
    [BurstCompile(FloatPrecision.Low, FloatMode.Fast, FloatMode = FloatMode.Fast, CompileSynchronously = false)]
    public class RaycastCommandBufferSystem : JobComponentSystem
    {
        private readonly Mouse _mouse = Mouse.current;

        private Camera _camera;

        protected override void OnCreate()
        {
            MessageBroker
                .Default
                .Receive<Camera>()
                .Subscribe(cam => { _camera = cam; });
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var currentTarget = new NativeArray<RaycastHit>(1, Allocator.TempJob);

            Entities
                .WithoutBurst()
                .ForEach((Entity entity,
                    ref PhysicsCastData phyiscsCastData,
                    in RayPhysicsCommandData physicsCommandData) =>
                {
                    if (_mouse.leftButton.isPressed)
                    {
                        var commands = new NativeArray<RaycastCommand>(1, Allocator.TempJob);
                        var results = new NativeArray<RaycastHit>(1, Allocator.TempJob);

                        var ray = _camera.ScreenPointToRay(_mouse.position.ReadValue());

                        commands[0] = new RaycastCommand
                        {
                            from = ray.origin,
                            direction = ray.direction,
                            distance = physicsCommandData.distance,
                            layerMask = physicsCommandData.layerMask,
                            maxHits = physicsCommandData.maxHits
                        };

                        var raycastCommandJobHandle =
                            RaycastCommand.ScheduleBatch(commands, results, physicsCommandData.minimumCommandPerJob);
                        
                        raycastCommandJobHandle.Complete();
                        
                        currentTarget[0] = results[0].collider != null ? results[0] : phyiscsCastData.hit;
                        
                        if(currentTarget[0].collider)
                        {
                            if (currentTarget[0].collider.CompareTag("Movable"))
                            {
                                phyiscsCastData.status = Status.Movable;
                                phyiscsCastData.stoppingDistance = 0;
                            }
                            else if(currentTarget[0].collider.CompareTag("Targetable"))
                            {
                                phyiscsCastData.status = Status.Targetable;
                                phyiscsCastData.stoppingDistance = 1.2f;
                            }
                        }
                        
                        commands.Dispose(raycastCommandJobHandle);
                        results.Dispose(raycastCommandJobHandle);
                    }
                }).Run();


            var jobHandle = Entities
                .WithAll<RayPhysicsCommandData>()
                .ForEach((Entity burstEntity,
                    ref PhysicsCastData physicsCastBurstData,
                    in Translation position) =>
                {
                    
                    if (currentTarget[0].point != Vector3.zero)
                    {
                        if (math.distance(currentTarget[0].point, position.Value) >
                            physicsCastBurstData.stoppingDistance)
                        {
                            physicsCastBurstData.hasTarget = true;
                            physicsCastBurstData.hasReached = false;

                            physicsCastBurstData.hit.distance = currentTarget[0].distance;
                            physicsCastBurstData.hit.point = currentTarget[0].point;
                        }
                    }

                }).Schedule(inputDeps);

            jobHandle.Complete();

            currentTarget.Dispose(jobHandle);

            return jobHandle;
        }
    }
}