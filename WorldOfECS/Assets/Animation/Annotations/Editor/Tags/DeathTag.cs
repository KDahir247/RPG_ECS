using Unity.Kinematica.Editor;
using WorldOfECS.Animation.Annotation;

namespace WorldOfECS.Animation.Tag
{
    [Tag("Death", "#750000")]
    public struct DeathTag : Payload<Death>
    {
        public Death Build(PayloadBuilder builder)
        {
            return Death.Default;
        }
    }
}