using System;
using UniRx;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics.Systems;
using UnityEngine;
using UnityEngine.InputSystem;
using WorldOfECS.Data;
using WorldOfECS.Event;

namespace WorldOfECS.ComponentBufferSystem
{
// since the terrain collider isn't integrated in unity ecs/dots we are going to set up a raycast buffer for raycast job scheduling
//TODO Might convert from standard gameObject raycast to entity raycast. Since gameObject raycast can't pickup Entity and vis versa.


    public class RaycastCommandBufferSystem : Unity.Entities.ComponentSystem
    {
        private readonly BehaviourEventBus _event = new BehaviourEventBus();
        private readonly Mouse _mouse = Mouse.current;
        private Camera _camera;
        
        
        
        protected override void OnStartRunning()
        {
            _event.OfType<object, Camera>()
                .Where(cam => cam != null)
                .Subscribe(camera => { _camera = camera; });
            
        }

        protected override void OnUpdate()
        {
//            Unity.Assertions.Assert.IsNotNull(_camera);

            var commands = new NativeArray<RaycastCommand>(1, Allocator.TempJob);
            var results = new NativeArray<RaycastHit>(1, Allocator.TempJob);
            
            JobHandle job = default;


            Entities.ForEach((Entity entity,
                ref PhysicsCommandData physicsCommandData,
                ref NavAgentData navAgentData) =>
            {

                if (physicsCommandData.minimumCommandPerJob > Environment.ProcessorCount - 1)
                {
                    throw new Exception($"minimumCommandPerJob from {physicsCommandData.ToString()} cant be greater then your PC processor count.");
                }
                
                if (_mouse.leftButton.isPressed)
                {
                    Ray ray = _camera.ScreenPointToRay(_mouse.position.ReadValue());

                    commands[0] = new RaycastCommand(ray.origin,
                        ray.direction,
                        physicsCommandData.distance,
                        physicsCommandData.layerMask,
                        physicsCommandData.maxHits);

                    job =
                        RaycastCommand.ScheduleBatch(commands, results, physicsCommandData.minimumCommandPerJob);


                    job.Complete();

                    if (results[0].collider != null)
                    {
                        navAgentData.destination = results[0].point;
                        navAgentData.normal = results[0].normal;
                        navAgentData.distance = results[0].distance;
                        navAgentData.hasPath = true;
                    }
                    else
                    {
                        navAgentData.hasPath = false;
                    }
                }
            });


            if (!job.IsCompleted) return;
            
            commands.Dispose(job);
            results.Dispose(job);
        }

        protected override void OnStopRunning()
        {
            if (!BehaviourEventBus.IsClear)
            {
                _event.ClearEventBusCache();
            }
        }
    }
}