using UniRx;
using Unity.Entities;
using Unity.Jobs;
using Unity.Kinematica;
using Unity.Mathematics;
using UnityEngine.AI;
using WorldOfECS.Animation.Annotation;
using WorldOfECS.ComponentBufferSystem;
using WorldOfECS.Data;

namespace WorldOfECS.ComponentSystem
{
    //TODO remove nav mesh agent in place for NavMeshPath & Move the object through root motion from Kinematica
    //TODO Implement Navigation task & Condition task to move the player from Kinematica by Nav mesh predication trajectory.
    //Remove navmesh agent once NavMeshPath is implemented

    [UpdateAfter(typeof(RaycastCommandBufferSystem))]
    public class NavMeshControllerSystem : JobComponentSystem
    {
        private readonly NavMeshPath _path = new NavMeshPath();

        private readonly BoolReactiveProperty _reactiveIdle = new BoolReactiveProperty(true);

        protected override void OnStartRunning()
        {
            Entities
                .WithoutBurst()
                .ForEach((Kinematica kinematica,
                    ref RaycastData raycastData) =>
                {
                    ref var motionSynthesizer = ref kinematica.Synthesizer.Ref;

                    motionSynthesizer.Push(
                        motionSynthesizer.Query.Where(
                            Locomotion.Default).And(Idle.Default));


                    _reactiveIdle
                        .ObserveEveryValueChanged(condition => condition.Value)
                        .Subscribe(isIdle =>
                        {
                            ref var lambdaMotionSynthesizer = ref kinematica.Synthesizer.Ref;

                            if (isIdle)
                                lambdaMotionSynthesizer
                                    .Action()
                                    .Push(lambdaMotionSynthesizer.Query
                                        .Where(Locomotion.Default)
                                        .And(Idle.Default));
                            else
                                lambdaMotionSynthesizer
                                    .Action()
                                    .Push(lambdaMotionSynthesizer.Query
                                        .Where(Locomotion.Default)
                                        .Except(Idle.Default));
                        });
                }).Run();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            Entities
                .WithoutBurst()
                .ForEach((NavMeshAgent navAgent,
                    ref RaycastData raycastData) =>
                {
                    if (!raycastData.hasReached)
                    {
                        navAgent.CalculatePath(raycastData.hit.point, _path);

                        if (_path.status != NavMeshPathStatus.PathPartial ||
                            _path.status != NavMeshPathStatus.PathInvalid)
                        {
                            navAgent.stoppingDistance = raycastData.stoppingDistance;

                            navAgent.SetDestination(raycastData.hit.point);

                            if (navAgent.remainingDistance <= raycastData.stoppingDistance)
                            {
                                navAgent.velocity = float3.zero;
                                raycastData.hasReached = true;
                            }
                        }
                        else
                        {
                            navAgent.isStopped = true;
                            raycastData.hasReached = true;
                        }
                    }

                    _reactiveIdle.Value = navAgent.remainingDistance <= raycastData.stoppingDistance;
                }).Run();

            return inputDeps;
        }


        protected override void OnStopRunning()
        {
            _reactiveIdle.Dispose();
        }
    }
}