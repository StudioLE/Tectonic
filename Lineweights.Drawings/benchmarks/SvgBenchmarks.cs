using BenchmarkDotNet.Attributes;
using Lineweights.SVG;
using Lineweights.SVG.From.Elements;

namespace Lineweights.Drawings.Benchmarks;

internal sealed class SvgBenchmarks : BenchmarksBase
{
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("SVG", "View")]
    public SvgElement Svg_Convert_View_Wireframe()
    {
        return new ViewToSvg().Convert(_view);
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("SVG", "Sheet")]
    public SvgElement Svg_Convert_Sheet_Wireframe()
    {
        return new SheetToSvg().Convert(_sheet);
    }
}
