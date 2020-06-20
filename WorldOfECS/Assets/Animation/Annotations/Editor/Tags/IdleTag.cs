using System.Collections;
using System.Collections.Generic;
using Unity.Kinematica.Editor;
using UnityEngine;
using WorldOfECS.Animation.Annotation;

namespace WorldOfECS.Animation.Tag
{
    [System.Serializable]
    //color represent by hex value
    [Tag("Idle", "#34e8eb")]
    public struct IdleTag : Payload<Idle>
    {
        public Idle Build(PayloadBuilder builder)
        {
            return Idle.Default;
        }
    }
}