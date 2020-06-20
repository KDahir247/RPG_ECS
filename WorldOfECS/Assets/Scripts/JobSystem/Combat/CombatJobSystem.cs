using System;
using UniRx;
using Unity.Entities;
using Unity.Jobs;
using Unity.Kinematica;
using UnityEngine;
using WorldOfECS.Animation.Annotation;
using WorldOfECS.ComponentBufferSystem;
using WorldOfECS.Data;

namespace WorldOfECS.ComponentSystem
{



    [UpdateAfter(typeof(RaycastCommandBufferSystem))]
    public class CombatJobSystem : JobComponentSystem
    {

        private readonly BoolReactiveProperty _isInRange = new BoolReactiveProperty(false);


        private bool _lockedToTarget;

        //Event for playing and stopping attack cycle
        public static event Action<RaycastHit> startAttackEvent;
        public static event Action endAttackEvent;

        protected override void OnStartRunning()
        {

            Entities
                .WithoutBurst()
                .ForEach((Entity entity,
                    Kinematica kinematica,
                    ref RaycastData raycastData) =>
                {
                    var hit = raycastData.hit;

                    _isInRange
                        .ObserveEveryValueChanged(reactive => reactive.Value)
                        .Subscribe(inRange =>
                        {

                            //ref can't be used in closure (lambda body)
                            ref MotionSynthesizer lambdaMotionSynthesizer = ref kinematica.Synthesizer.Ref;

                            if (inRange)
                            {

                                startAttackEvent?.Invoke(hit);

                                lambdaMotionSynthesizer
                                    .Action()
                                    .Push(lambdaMotionSynthesizer.Query
                                        .Where(Locomotion.Default)
                                        .And(Combat.Default));
                            }
                            else
                            {
                                endAttackEvent?.Invoke();
                            }

                        });
                }).Run();

        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {

            Entities
                .WithoutBurst()
                .ForEach((ref RaycastData raycastData) =>
                {
                    if (raycastData.status == Status.Targetable)
                    {
                        _lockedToTarget = true;
                    }
                    else if (raycastData.status == Status.Movable)
                    {
                        _lockedToTarget = false;
                    }


                    _isInRange.Value = raycastData.hasReached && _lockedToTarget;

                }).Run();


            return inputDeps;
        }


        protected override void OnStopRunning()
        {
            if (startAttackEvent != null)
                foreach (var startD in startAttackEvent.GetInvocationList())
                {
                    startAttackEvent -= startD as Action<RaycastHit>;
                }

            if (endAttackEvent != null)
                foreach (var endD in endAttackEvent.GetInvocationList())
                {
                    endAttackEvent -= endD as Action;
                }

            _isInRange.Dispose();
        }
    }
}