using Unity.Kinematica.Editor;
using WorldOfECS.Animation.Annotation;

namespace WorldOfECS.Animation.Tag
{
    [System.Serializable]
    //color represent by hex value
    [Tag("LocomotionTag", "#3471eb")]
    public struct LocomotionTag : Payload<Locomotion>
    {
        public Locomotion Build(PayloadBuilder builder)
        {
            return Locomotion.Default;
        }
    }
}