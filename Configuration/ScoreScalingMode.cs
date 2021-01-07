using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace SliceVisualizer.Configuration
{
    public enum ScoreScalingMode
    {
        Linear,
        Sqrt,
        Log,
    }
}