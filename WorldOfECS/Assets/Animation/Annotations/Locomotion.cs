using Unity.Kinematica;

namespace WorldOfECS.Animation.Annotation
{
    //required by all Kinematica asset animation clip (default for player Kinematica)
    [Trait]
    public struct Locomotion
    {
        public static Locomotion Default => new Locomotion();
    }
}