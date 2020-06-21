using System;
using UniRx;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using WorldOfECS.ComponentBufferSystem;
using WorldOfECS.Data;

namespace WorldOfECS.ComponentSystem
{
    [UpdateAfter(typeof(RaycastCommandBufferSystem))]
    public class CombatJobSystem : JobComponentSystem
    {
        private readonly BoolReactiveProperty _isInRange = new BoolReactiveProperty(false);

        private bool _lockedToTarget;

        private RaycastHit target;

        //Event for playing and stopping attack cycle
        public static event Action<RaycastHit> startAttackEvent;
        public static event Action endAttackEvent;

        protected override void OnStartRunning()
        {
            Entities
                .WithoutBurst()
                .ForEach((Entity entity,
                    ref RaycastData raycastData) =>
                {
                    _isInRange
                        .ObserveEveryValueChanged(reactive => reactive.Value)
                        .Subscribe(inRange =>
                        {
                            if (inRange)
                                startAttackEvent?.Invoke(target);
                            else
                                endAttackEvent?.Invoke();
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
                        _lockedToTarget = true;
                    else
                        _lockedToTarget = false;

                    _isInRange.Value = raycastData.hasReached && _lockedToTarget;

                    if (_isInRange.Value)
                        if (target.collider != raycastData.hit.collider)
                            target = raycastData.hit;
                }).Run();


            return inputDeps;
        }


        protected override void OnStopRunning()
        {
            if (startAttackEvent != null)
                foreach (var startD in startAttackEvent.GetInvocationList())
                    startAttackEvent -= startD as Action<RaycastHit>;

            if (endAttackEvent != null)
                foreach (var endD in endAttackEvent.GetInvocationList())
                    endAttackEvent -= endD as Action;

            _isInRange.Dispose();
        }
    }
}