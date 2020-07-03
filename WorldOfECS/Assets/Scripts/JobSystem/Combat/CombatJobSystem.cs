//using System;
//using UniRx;
//using Unity.Entities;
//using Unity.Jobs;
//using UnityEngine;
//using WorldOfECS.Data;
////TODO next to fix up
//namespace WorldOfECS.ComponentSystem
//{
//    [UpdateAfter(typeof(RaycastCommandBufferSystem))]
//    public class CombatJobSystem : JobComponentSystem
//    {
//        private readonly BoolReactiveProperty _isInRange = new BoolReactiveProperty(false);
//
//        private bool _lockedToTarget;
//
//        private RaycastHit _target;
//
//        //Event for playing and stopping attack cycle
//        public static event Action<RaycastHit> StartAttackEvent;
//        public static event Action EndAttackEvent;
//
//        protected override void OnStartRunning()
//        {
//            Entities
//                .WithoutBurst()
//                .ForEach((Entity entity,
//                    ref PhysicsCastData physicsCastData,
//                    ref CombatData combatData) =>
//                {
//                    _isInRange
//                        .ObserveEveryValueChanged(reactive => reactive.Value)
//                        .Subscribe(inRange =>
//                        {
//                            if (inRange)
//                                StartAttackEvent?.Invoke(_target);
//                            else
//                                EndAttackEvent?.Invoke();
//                        });
//                }).Run();
//        }
//
//        protected override JobHandle OnUpdate(JobHandle inputDeps)
//        {
//            Entities
//                .WithoutBurst()
//                .ForEach((ref PhysicsCastData physicsCastData,
//                    ref CombatData combatData) =>
//                {
//                    if (physicsCastData.status == Status.Targetable)
//                        _lockedToTarget = true;
//                    else
//                        _lockedToTarget = false;
//
//                    _isInRange.Value = physicsCastData.hasReached && _lockedToTarget;
//
//                    if (_isInRange.Value)
//                        if (_target.collider != physicsCastData.hit.collider)
//                            _target = physicsCastData.hit;
//                }).Run();
//
//
//            return inputDeps;
//        }
//
//
//        protected override void OnStopRunning()
//        {
//            if (StartAttackEvent != null)
//                foreach (var startD in StartAttackEvent.GetInvocationList())
//                    StartAttackEvent -= startD as Action<RaycastHit>;
//
//            if (EndAttackEvent != null)
//                foreach (var endD in EndAttackEvent.GetInvocationList())
//                    EndAttackEvent -= endD as Action;
//
//            _isInRange.Dispose();
//        }
//    }
//}
//
