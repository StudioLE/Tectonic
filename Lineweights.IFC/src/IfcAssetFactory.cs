﻿using Lineweights.Core.Assets;
using Lineweights.Core.Storage;
using StudioLE.Core.Results;

namespace Lineweights.IFC;

public class IfcAssetFactory : ExternalAssetFactoryBase<Model>
{
    /// <inheritdoc />
    protected override IConverter<Model, Task<IResult<Uri>>> Converter { get; }

    /// <inheritdoc cref="IfcAssetFactory"/>
    public IfcAssetFactory(IStorageStrategy storageStrategy)
    {
        Converter = new ModelToIfcFile(storageStrategy, Asset.Id + ".ifc");
        Asset.Name = "IFC of Model";
        Asset.ContentType = "application/x-step";
    }
}
