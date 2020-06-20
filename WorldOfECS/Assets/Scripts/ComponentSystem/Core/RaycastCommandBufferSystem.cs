using System;
using UniRx;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.InputSystem;
using WorldOfECS.Data;

namespace WorldOfECS.ComponentBufferSystem
{
// since the terrain collider isn't integrated in unity ecs/dots we are going to set up a raycast buffer for raycast job scheduling
//TODO Might convert from standard gameObject raycast to entity raycast. Since gameObject raycast can't pickup Entity and vis versa.


    public class RaycastCommandBufferSystem : Unity.Entities.ComponentSystem
    {
        private readonly Mouse _mouse = Mouse.current;
        private Camera _camera;
        
        
        protected override void OnCreate()
        {
            MessageBroker
                .Default
                .Receive<Camera>()
                .Subscribe(cam =>
                {
                    _camera = cam;
                });
        }

        protected override void OnUpdate()
        {
            Entities
                .ForEach((Entity entity,
                ref RayPhysicsCommandData physicsCommandData,
                ref RaycastData raycastData) =>
            {
                if (physicsCommandData.minimumCommandPerJob > Environment.ProcessorCount - 1)
                {
                    throw new Exception($"minimumCommandPerJob from {physicsCommandData.ToString()} cant be greater then your PC processor count.");
                }
                
                if (_mouse.leftButton.isPressed)
                {
                    var commands = new NativeArray<RaycastCommand>(1, Allocator.TempJob);
                    var results = new NativeArray<RaycastHit>(1, Allocator.TempJob);

                    Ray ray = _camera.ScreenPointToRay(_mouse.position.ReadValue());

                    commands[0] = new RaycastCommand(
                        ray.origin,
                        ray.direction,
                        physicsCommandData.distance,
                        physicsCommandData.layerMask,
                        physicsCommandData.maxHits);

                    JobHandle job =
                        RaycastCommand.ScheduleBatch(commands, results, physicsCommandData.minimumCommandPerJob);
                    
                    job.Complete();

                    if(results[0].transform != null)
                        raycastData.hit = results[0];

                    RaycastEvaluation(ref raycastData);
                    
                    commands.Dispose(job);
                    results.Dispose(job);

                }
            });

        }

        private static void RaycastEvaluation(ref RaycastData raycastData)
        {
            //Re-Evaluate the Status for the raycast data
            raycastData.status = Status.Nil;
            
            if (raycastData.status == Status.Nil && raycastData.hit.collider)
            {
                if (raycastData.hit.collider.CompareTag("Movable"))
                {
                    raycastData.stoppingDistance = 0f;
                    raycastData.status = Status.Movable;
                }
                else if (raycastData.hit.collider.CompareTag("Targetable"))
                {
                    raycastData.stoppingDistance = 1;
                    raycastData.status = Status.Targetable;
                }
            }
        }
    }
}