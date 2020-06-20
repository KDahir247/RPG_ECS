using System;
using UniRx;
using UnityEngine;
using UnityEngine.GameFoundation.DataAccessLayers;
using UnityEngine.GameFoundation;
using WorldOfECS.ComponentSystem;

namespace WorldOfECS.Foundation
{
    public class PlayerCombat : MonoBehaviour
    {
        private  MemoryDataLayer _memoryDataLayer;
        private InventoryItem _mainWeapon;

        private IDisposable _attackEventCall;
        // Start is called before the first frame update

        private void Awake()
        {
            _memoryDataLayer = new MemoryDataLayer();
            
            if (!GameFoundation.IsInitialized)
            {
                GameFoundation.Initialize(_memoryDataLayer,
                    () => { Debug.Log("Initializing GameFoundation Success PlayerCombat"); },
                    e =>
                    {
                        Debug.LogError(
                            $"Error in PlayerCombat GameFoundation Initialization \n {e.Source} \t {e.Message}");
                    });
            }

            _mainWeapon = InventoryManager.CreateItem("unarmed");


            CombatJobSystem.startAttackEvent += StartAttack;
            CombatJobSystem.endAttackEvent += EndAttack;

        }


        /// <summary>
        /// Called event within ECS combat
        /// </summary>
        /// <param name="hit">target hit</param>
        void StartAttack(RaycastHit hit)
        {
            float attackSpeed = 1;

            if (_mainWeapon.HasStat("attackSpeed"))
            {
                attackSpeed = _mainWeapon.GetStat("attackSpeed");
            }

            _attackEventCall = Observable.Interval(TimeSpan.FromSeconds(attackSpeed))
                .Select(_ =>
                {
                    float damage = 0;

                    if (_mainWeapon.HasStat("damage"))
                    {
                        damage = _mainWeapon.GetStat("damage");
                    }

                    return damage;

                })
                .Subscribe(damage => { Debug.Log(damage); });

        }

        /// <summary>
        /// Called event within ECS combat
        /// </summary>
        void EndAttack()
        {
            _attackEventCall?.Dispose();
        }
    }
}