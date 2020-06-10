using System;
using UniRx;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.InputSystem;
using WOrldOfECS.Data;
using WOrldOfECS.Event;

namespace WOrldOfECS.ComponentBufferSystem
{
// since the terrain collider isn't integrated in unity ecs/dots we are going to set up a raycast buffer for raycast job scheduling
    public class RaycastCommandBufferSystem : Unity.Entities.ComponentSystem
    {
        readonly BehaviourEventBus _event = new BehaviourEventBus();
        readonly Mouse _mouse = Mouse.current;

        private Camera _camera;

        protected override void OnCreate()
        {
            _event.OfType<object, Camera>()
                .Where(cam => cam != null)
                .Subscribe(camera => { _camera = camera; });
        }

        protected override void OnUpdate()
        {
            Unity.Assertions.Assert.IsNotNull(_camera);

            var commands = new NativeArray<RaycastCommand>(1, Allocator.TempJob);
            var results = new NativeArray<RaycastHit>(1, Allocator.TempJob);

            JobHandle job = new JobHandle();

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

                    navAgentData.destination = results[0].point;
                    navAgentData.normal = results[0].normal;
                    navAgentData.distance = results[0].distance;
                }
            });

            commands.Dispose(job);
            results.Dispose(job);
        }
    }
}