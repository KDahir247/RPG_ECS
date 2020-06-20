using Unity.Kinematica.Editor;
using WorldOfECS.Animation.Annotation;

namespace WorldOfECS.Animation.Tag
{
    [System.Serializable]
    [Tag("Combat", "#e00909")]
    public struct CombatTag : Payload<Combat>
    {
        public Combat Build(PayloadBuilder builder)
        {
            return Combat.Default;
        }
    }
}