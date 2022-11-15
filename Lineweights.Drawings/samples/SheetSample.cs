using System.ComponentModel.DataAnnotations;
using System.IO;
using Lineweights.Core.Documents;
using Lineweights.Flex;
using Lineweights.PDF;
using Lineweights.PDF.From.Elements;
using Lineweights.SVG;
using Lineweights.SVG.From.Elements;
using Lineweights.Workflows.Samples;
using QuestPDF.Fluent;
using StudioLE.Core.System.IO;

namespace Lineweights.Drawings.Samples;

public static class SheetSample
{

    public class ViewInputs
    {
        [Required]
        [Range(0, 50)]
        public int Scale { get; set; } = 10;

        [Required]
        [Range(0, .5)]
        public double Padding { get; set; } = .25;
    }

    public class SheetInputs
    {
        [Required]
        [Range(0, 2)]
        public double Width { get; set; } = .841;

        [Required]
        [Range(0, 2)]
        public double Height { get; set; } = .594;

        [Required]
        [Range(0, 2)]
        public double Title { get; set; } = .075;
    }

    public class ArrangementInputs
    {
        [Required]
        public Justification MainJustification { get; set; } = Justification.SpaceEvenly;

        [Required]
        public Justification CrossJustification { get; set; } = Justification.SpaceEvenly;

        [Required]
        public Alignment CrossAlignment { get; set; } = Alignment.End;
    }

    public class Outputs
    {
        public Model Model { get; set; } = new();

        public List<Asset> Assets { get; } = new();
    }

    public static Outputs Execute(ViewInputs viewInputs, SheetInputs sheetInputs, ArrangementInputs arrangementInputs)
    {
        ViewDirection[] viewDirections =
        {
            ViewDirection.Top,
            ViewDirection.Bottom,
            ViewDirection.Left,
            ViewDirection.Right,
            ViewDirection.Front,
            ViewDirection.Back
        };

        Model model = Scenes.FromJson(Scenes.Name.Brickwork);

        ViewBuilder viewBuilder = new ViewBuilder()
            .ScopePadding(viewInputs.Padding, viewInputs.Padding, viewInputs.Padding)
            .Scale(1d / viewInputs.Scale)
            .ElementsInView(model.Elements.Values.ToArray());

        View[] views = viewDirections
            .Select(direction => viewBuilder
                .ViewDirection(direction)
                .Build())
            .ToArray();

        SequenceBuilder sequenceBuilder = new();
        DefaultViewArrangement viewArrangement = new();

        viewArrangement
            .MainJustification(arrangementInputs.MainJustification)
            .CrossJustification(arrangementInputs.CrossJustification)
            .CrossAlignment(arrangementInputs.CrossAlignment);

        ISheetBuilder builder = new SheetBuilder(sequenceBuilder, viewArrangement)
            .SheetSize(sheetInputs.Width, sheetInputs.Height)
            .VerticalTitleArea(sheetInputs.Title)
            .Views(views);
        Sheet sheet = (Sheet)builder.Build();

        // Prepare outputs
        Outputs outputs = new();
        outputs.Model.AddElements(sheet.Render());
        // outputs.Model.AddElements(model.Elements.Values);
        // outputs.Model.AddElements(views);
        // outputs.Model.AddElement(sheet);
        outputs.Assets.Add(CreateSvg(sheet));
        outputs.Assets.Add(CreatePdf(sheet));

        return outputs;
    }

    private static Asset CreatePdf(Sheet sheet)
    {
        SheetToPdf converter = new();
        FileInfo file = PathHelpers.GetTempFile(".pdf");
        PdfSheet pdfSheet = converter.Convert(sheet);
        pdfSheet.GeneratePdf(file.FullName);
        return new()
        {
            Info = new()
            {
                Name = "SVG of Sheet",
                Location = new(file.FullName)
            },
            ContentType = "application/pdf"
        };
    }

    private static Asset CreateSvg(Sheet sheet)
    {
        SheetToSvg converter = new();
        FileInfo file = PathHelpers.GetTempFile(".svg");
        SvgDocument svgDocument = converter.Convert(sheet);
        svgDocument.Save(file.FullName);
        return new()
        {
            Info = new()
            {
                Name = "SVG of Sheet",
                Location = new(file.FullName)
            },
            ContentType = "image/svg+xml"
        };
    }
}
