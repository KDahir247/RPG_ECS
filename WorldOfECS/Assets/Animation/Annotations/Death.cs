using Unity.Kinematica;

namespace WorldOfECS.Animation.Annotation
{
    [Trait]
    public struct Death
    {
        public static Death Default => new Death();
    }
}