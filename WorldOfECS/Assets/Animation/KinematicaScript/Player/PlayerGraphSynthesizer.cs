using System.Collections;
using System.Collections.Generic;
using Unity.Kinematica;
using UnityEngine;
using WorldOfECS.Animation.Annotation;


[RequireComponent(typeof(Kinematica))]
public class PlayerGraphSynthesizer : MonoBehaviour
{
    private Kinematica _kinematica;

    // Start is called before the first frame update
    void Start()
    {
        _kinematica = GetComponent<Kinematica>();


        ref MotionSynthesizer synthesizer = ref _kinematica.Synthesizer.Ref;

        synthesizer
            .Action()
            .Push(synthesizer
                .Query
                .Where(Locomotion.Default)
                .And(Idle.Default));


    }

}
