using BenchmarkDotNet.Attributes;
using Lineweights.PDF;
using Lineweights.PDF.From.Elements;

namespace Lineweights.Drawings.Benchmarks;

internal sealed class PdfBenchmarks : BenchmarksBase
{
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("PDF", "View")]
    public PdfView Pdf_Convert_View_Wireframe()
    {
        return new ViewToPdf().Convert(_view);
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("PDF", "Sheet")]
    public PdfSheet Pdf_Convert_Sheet_Wireframe()
    {
        return new SheetToPdf().Convert(_sheet);
    }
}
