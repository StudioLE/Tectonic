using BenchmarkDotNet.Attributes;
using Elements;

namespace Lineweights.Drawings.Benchmarks;

internal sealed class SheetBenchmarks : BenchmarksBase
{
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Sheet")]
    public GeometricElement[] Sheet_Render_Wireframe_Default()
    {
        return _sheet
            .Render()
            .ToArray();
    }
}
