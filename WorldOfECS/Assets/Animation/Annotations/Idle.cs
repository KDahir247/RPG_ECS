using System.Collections;
using System.Collections.Generic;
using Unity.Kinematica;
using UnityEngine;


namespace WorldOfECS.Animation.Annotation
{
    [Trait]
    public struct Idle
    {
        public static Idle Default = new Idle();
    }
}