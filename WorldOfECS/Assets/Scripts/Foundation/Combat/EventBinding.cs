using System;
using UniRx;
using Unity.Entities;
using Unity.Kinematica;
using UnityEngine;
using UnityEngine.GameFoundation;
using UnityEngine.GameFoundation.DataAccessLayers;
using WorldOfECS.Animation.Annotation;
using WorldOfECS.Core;
using WorldOfECS.Data;

namespace WorldOfECS.Binding
{
    public class EventBinding : MonoBehaviour
    {
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        private IDisposable _attackEvent;
        private Kinematica _kinematica;

        private InventoryItem _mainWeapon;

        //game foundation
        private MemoryDataLayer _memoryDataLayer;

        private RPGAttribute _targetAttribute;
        public ReactiveProperty<bool> combatEventBind = new ReactiveProperty<bool>(false);

        [SerializeField] private ScriptableEquipement equipement;

        public ReactiveProperty<bool> movementEventBind = new ReactiveProperty<bool>(true);
        public EntityManager DstManager { get; set; }
        public PhysicsCastData CastData { get; set; }

        private void Awake()
        {
            CombatEvent();
            GameFoundationInitialization();
        }

        private void GameFoundationInitialization()
        {
            _memoryDataLayer = new MemoryDataLayer();

            if (!GameFoundation.IsInitialized)
                GameFoundation.Initialize(_memoryDataLayer,
                    () => { Debug.Log("Initializing GameFoundation Success EventBinding"); },
                    e =>
                    {
                        Debug.LogError(
                            $"Error in EventBinding GameFoundation Initialization \n {e.Source} \t {e.Message}");
                    });

            _mainWeapon = InventoryManager.CreateItem(equipement.mainWeaponRef);
        }

        private void CombatEvent()
        {
            _kinematica = gameObject.GetComponent<Kinematica>();

            {
                //Movement Event
                movementEventBind
                    .ObserveEveryValueChanged(bindMovement => bindMovement.Value)
                    .Subscribe(isIdle =>
                    {
                        if (isIdle)
                            //Idle Event Call
                            //play idle audio
                            IdleAnimation();
                        else
                            //WalkEventCall
                            //play walk audio
                            WalkAnimation();
                    }).AddTo(_disposable);
            }

            {
                //Combat Event
                combatEventBind
                    .ObserveEveryValueChanged(bindCombat => bindCombat.Value)
                    .Subscribe(inRange =>
                    {
                        if (inRange)
                        {
                            StartAttack();
                            UpdateAttack();
                        }
                        else
                        {
                            EndAttack();
                        }
                    }).AddTo(_disposable);
            }
        }

        private void IdleAnimation()
        {
            if (combatEventBind.Value == false)
            {
                ref var motionSynthesizer = ref _kinematica.Synthesizer.Ref;

                motionSynthesizer
                    .Action()
                    .Push(motionSynthesizer.Query
                        .Where(Locomotion.Default)
                        .And(Idle.Default));
            }
        }

        private void WalkAnimation()
        {
            ref var motionSynthesizer = ref _kinematica.Synthesizer.Ref;

            motionSynthesizer
                .Action()
                .Push(motionSynthesizer.Query
                    .Where(Locomotion.Default)
                    .Except(Idle.Default));
        }


        private void StartAttack()
        {
            movementEventBind.Value = true;

            ref var motionSynthesizer = ref _kinematica.Synthesizer.Ref;

            motionSynthesizer
                .Action()
                .Push(motionSynthesizer.Query
                    .Where(Locomotion.Default)
                    .And(Combat.Default));
        }


        private void UpdateAttack()
        {
            //using both unity's new physics and original physics. 
            //not compatible with each other, which is handle by the conditional
            if (CastData.hit.collider == null)
                _targetAttribute = DstManager.GetComponentObject<RPGAttribute>(CastData.entity);
            else
                _targetAttribute = CastData.hit.transform.GetComponent<RPGAttribute>();

            float attackSpeed = 1;
            if (_mainWeapon.HasStat("attackSpeed")) attackSpeed = _mainWeapon.GetStat("attackSpeed");

            _attackEvent = Observable.Interval(TimeSpan.FromSeconds(attackSpeed))
                .Select(currentTick =>
                {
                    float damage = 0;
                    if (_mainWeapon.HasStat("damage")) damage = _mainWeapon.GetStat("damage");

                    return damage;
                }).Where(damage => _targetAttribute.TakeDamage(damage) == false)
                .Subscribe(damage =>
                {
//                    //when enemy dies
                    ref var motionSynthesizer = ref _kinematica.Synthesizer.Ref;

                    motionSynthesizer
                        .Action()
                        .Push(motionSynthesizer.Query
                            .Where(Locomotion.Default)
                            .And(Idle.Default));

                    //Dispose of this interval event since the enemy is dead
                    EndAttack();
                }).AddTo(_disposable);
        }

        private void EndAttack()
        {
            _attackEvent?.Dispose();
        }

        private void OnDestroy()
        {
            if (!_disposable.IsDisposed)
                _disposable.Dispose();
        }
    }
}