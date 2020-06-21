using System;
using Unity.Kinematica.Editor;
using WorldOfECS.Animation.Annotation;

namespace WorldOfECS.Animation.Tag
{
    [Serializable]
    [Tag("Hurt", "#fffb87")]
    public struct HurtTag : Payload<Hurt>
    {
        public Hurt Build(PayloadBuilder builder)
        {
            return Hurt.Default;
        }
    }
}