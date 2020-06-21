﻿using System;
using Unity.Kinematica;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using WorldOfECS.Animation.Annotation;

namespace WorldOfECS.Core
{
    public class RPGAttribute : MonoBehaviour
    {
        private NavMeshAgent _agent;

        private Kinematica _kinematica;

        private Collider col;

        [SerializeField] private Stat stat;

        private void Start()
        {
            _kinematica = gameObject.GetComponent<Kinematica>();

            //TODO move deinitialization to another suitable spot 
            col = gameObject.GetComponent<Collider>();
            _agent = gameObject.GetComponent<NavMeshAgent>();
        }

        /// <summary>
        /// </summary>
        /// <param name="damageAmt"></param>
        /// <returns>Return false if health is less the or equal zero</returns>
        public bool TakeDamage(float damageAmt)
        {
            ref var motionSynthesizer = ref _kinematica.Synthesizer.Ref;

            if (stat.healthPoint > 0)
            {
                stat.healthPoint = math.max(stat.healthPoint - damageAmt, 0);

//                //hurt not yet implemented in kinematica
//                motionSynthesizer
//                    .Action()
//                    .Push(motionSynthesizer.Query
//                        .Where(Locomotion.Default)
//                        .And(Hurt.Default));

                return true;
            }

            col.enabled = false;
            _agent.enabled = false;

            motionSynthesizer
                .Action()
                .Push(motionSynthesizer.Query
                    .Where(Locomotion.Default)
                    .And(Death.Default));

            _kinematica.applyRootMotion = true;

            return false;
        }
    }

    [Serializable]
    public struct Stat
    {
        public float healthPoint;
    }
}