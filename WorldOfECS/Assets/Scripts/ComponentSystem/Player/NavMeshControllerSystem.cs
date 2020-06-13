using System;
using UniRx;
using Unity.Entities;
using Unity.Jobs;
using Unity.Kinematica;
using UnityEngine.AI;
using WorldOfECS.ComponentBufferSystem;
using WorldOfECS.Data;
using Unity.Assertions;
using UnityEngine;
using WorldOfECS.Animation.Annotation;

namespace WorldOfECS.ComponentSystem
{
    //TODO remove nav mesh agent in place for NavMeshPath & Move the object through root motion from Kinematica
    //TODO Implement Navigation task & Condition task to move the player from Kinematica by Nav mesh predication trajectory.
    //Remove navmesh agent once NavMeshPath is implemented
    
    [UpdateAfter(typeof(RaycastCommandBufferSystem))]
    public class NavMeshControllerSystem : JobComponentSystem
    {
        readonly NavMeshPath _path = new NavMeshPath();
        
        private readonly BoolReactiveProperty reactiveIdle = new BoolReactiveProperty(true);
        
        private Kinematica _kinematica;
        private Identifier<SelectorTask> locomtion;
        
        
        protected override void OnStartRunning()
        {
            Entities
                .WithoutBurst()
                .ForEach((Entity entity,
                    Kinematica kinematica) =>
                {
                    _kinematica = kinematica;

                    Assert.IsNotNull(_kinematica, $"_kinematica != null \t located: {this}");

                    ref MotionSynthesizer motionSynthesizer = ref kinematica.Synthesizer.Ref;


                    motionSynthesizer.Push(
                        motionSynthesizer.Query.Where(
                            Locomotion.Default).And(Idle.Default));
                    
                    
                    reactiveIdle
                            .ObserveEveryValueChanged(condition => condition.Value)
                            .Subscribe(isIdle =>
                            {
                                ref MotionSynthesizer lamdaSynthesizer = ref _kinematica.Synthesizer.Ref;

                                if (isIdle)
                                {
                                    lamdaSynthesizer
                                        .Action()
                                        .Push(lamdaSynthesizer.Query
                                            .Where(Locomotion.Default)
                                            .And(Idle.Default));
                                }
                                else
                                {
                                    lamdaSynthesizer
                                        .Action()
                                        .Push(lamdaSynthesizer.Query
                                            .Where(Locomotion.Default)
                                            .Except(Idle.Default));
                                }
                                
                            });
                    }).Run();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            Entities
                .WithoutBurst()
                .ForEach((Entity entity,
                    NavMeshAgent navAgent,
                    ref NavAgentData agentData) =>
                {
                    if (!agentData.hasPath)
                    {
                        return;
                    }
                    
                    Assert.IsNotNull(_kinematica, $"_kinematica != null \t located: {this}");
                    
                    navAgent.CalculatePath(agentData.destination, _path);
                    if (_path.status != NavMeshPathStatus.PathPartial && _path.status != NavMeshPathStatus.PathInvalid)
                    {
                        
                        navAgent.SetDestination(agentData.destination);
                    }
                    
                    reactiveIdle.Value = navAgent.remainingDistance <= Mathf.Epsilon;

                }).Run();
            
            return inputDeps;
        }
    }
}