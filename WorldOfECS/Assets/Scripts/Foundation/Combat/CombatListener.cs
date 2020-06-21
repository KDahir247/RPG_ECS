using System;
using UniRx;
using Unity.Kinematica;
using UnityEngine;
using UnityEngine.GameFoundation;
using UnityEngine.GameFoundation.DataAccessLayers;
using WorldOfECS.Animation.Annotation;
using WorldOfECS.ComponentSystem;
using WorldOfECS.Core;

namespace WorldOfECS.Foundation
{
    [RequireComponent(typeof(Kinematica))]
    public class CombatListener : MonoBehaviour
    {
        private IDisposable _attackEventCall;

        private Kinematica _kinematica;

        private InventoryItem _mainWeapon;

        private MemoryDataLayer _memoryDataLayer;
        [SerializeField] private string weaponRef = "unarmed";


        private void Awake()
        {
            GameFoundationInitialization();
            AnimationInitialization();
        }

        private void AnimationInitialization()
        {
            _kinematica = gameObject.GetComponent<Kinematica>();
        }

        private void GameFoundationInitialization()
        {
            _memoryDataLayer = new MemoryDataLayer();

            if (!GameFoundation.IsInitialized)
                GameFoundation.Initialize(_memoryDataLayer,
                    () => { Debug.Log("Initializing GameFoundation Success CombatListener"); },
                    e =>
                    {
                        Debug.LogError(
                            $"Error in CombatListener GameFoundation Initialization \n {e.Source} \t {e.Message}");
                    });

            _mainWeapon = InventoryManager.CreateItem(weaponRef);

            CombatJobSystem.startAttackEvent += StartAttack;
            CombatJobSystem.endAttackEvent += EndAttack;
        }


        /// <summary>
        ///     Called event within ECS combat
        /// </summary>
        /// <param name="hit">target hit</param>
        private void StartAttack(RaycastHit hit)
        {
            transform.LookAt(hit.transform);

            AttackEvent(hit);
            AnimationQuery();
        }

        private void AnimationQuery()
        {
            ref var motionSynthesizer = ref _kinematica.Synthesizer.Ref;

            motionSynthesizer
                .Action()
                .Push(motionSynthesizer.Query
                    .Where(Locomotion.Default)
                    .And(Combat.Default));
        }

        private void AttackEvent(RaycastHit hit)
        {
            var @event = hit.collider.GetComponent<RPGAttribute>();

            if (@event == null) return;

            float attackSpeed = 1;

            if (_mainWeapon.HasStat("attackSpeed")) attackSpeed = _mainWeapon.GetStat("attackSpeed");

            _attackEventCall = Observable.Interval(TimeSpan.FromSeconds(attackSpeed))
                .Select(currentTick =>
                {
                    float damage = 0;
                    if (_mainWeapon.HasStat("damage")) damage = _mainWeapon.GetStat("damage");

                    return damage;
                })
                .Where(damage => @event.TakeDamage(damage) == false) //if can't take damage meaning that hp is zero.
                .Subscribe(damage =>
                {
                    ref var motionSynthesizer = ref _kinematica.Synthesizer.Ref;

                    motionSynthesizer
                        .Action()
                        .Push(motionSynthesizer.Query
                            .Where(Locomotion.Default)
                            .And(Idle.Default));

                    //Dispose of this interval event since the enemy is dead
                    EndAttack();
                });
        }


        /// <summary>
        ///     Called event within ECS combat
        /// </summary>
        private void EndAttack()
        {
            _attackEventCall?.Dispose();
        }
    }
}