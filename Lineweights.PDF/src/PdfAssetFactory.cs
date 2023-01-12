﻿using Lineweights.Core.Documents;
using Lineweights.Drawings;
using Lineweights.PDF.From.Elements;
using StudioLE.Core.Results;

namespace Lineweights.PDF;

public class PdfAssetFactory<T> : ExternalAssetFactoryBase<T> where T : Canvas
{
    /// <inheritdoc />
    protected override IConverter<T, Task<IResult<Uri>>> Converter { get; }

    /// <inheritdoc cref="PdfAssetFactory{T}"/>
    public PdfAssetFactory(IStorageStrategy storageStrategy)
    {
        Converter = new CanvasToPdfFile(storageStrategy, Asset.Id + ".pdf");
        Asset.ContentType = "application/pdf";
    }

    /// <inheritdoc />
    protected override void AfterSetup(T source)
    {
        Asset.Name = source.Name;
    }
}
