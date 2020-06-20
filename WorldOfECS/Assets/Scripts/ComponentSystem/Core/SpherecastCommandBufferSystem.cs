//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Jobs;
//using UnityEngine;
//using WorldOfECS.Data;
//
//public class SpherecastCommandBufferSystem : ComponentSystem
//{
//
//    protected override void OnUpdate()
//    {
//        Entities
//            .ForEach((ref SpherePhysicsCommandData spherecastCommand,
//                ref SpherecastData sphereCastData) =>
//            {
//                
//                var commands = new NativeArray<SpherecastCommand>(1, Allocator.TempJob);
//                var results = new NativeArray<RaycastHit>(1, Allocator.TempJob);
//
//                commands[0] = new SpherecastCommand(spherecastCommand.origin, spherecastCommand.radius, spherecastCommand.direction, spherecastCommand.distance, spherecastCommand.layerMask);
//
//                if (spherecastCommand.minimumCommandPerJob > Environment.ProcessorCount - 1)
//                {
//                    throw new Exception($"minimumCommandPerJob from {spherecastCommand.ToString()} cant be greater then your PC processor count.");
//                }
//
//                JobHandle job =
//                    SpherecastCommand.ScheduleBatch(commands, results, 1);
//                
//                job.Complete();
//                
//
//                if (results[0].transform != null)
//                {
//                    sphereCastData.stoppingDistance = 1;
//                    sphereCastData.status = Status.Targetable;
//                    sphereCastData.hit = results[0];
//                }
//                
//                
//                results.Dispose(job);
//                commands.Dispose(job);
//            });
//    }
//}
