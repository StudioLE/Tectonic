using BenchmarkDotNet.Attributes;
using Elements;

namespace Lineweights.Drawings.Benchmarks;

internal sealed class ViewBenchmarks : BenchmarksBase
{
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("View")]
    public GeometricElement[] View_Render_Wireframe_Default()
    {
        return _view
            .Render()
            .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
            .ToArray();
    }

    [Benchmark]
    [BenchmarkCategory("View")]
    public GeometricElement[] View_Render_Wireframe_WithDegreeOfParallelism_1()
    {
        return _view
            .Render()
            .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
            .WithDegreeOfParallelism(1)
            .ToArray();
    }
}
