using System.ComponentModel.DataAnnotations;
using System.Reflection;
using StudioLE.Extensions.System.Exceptions;

namespace Cascade.Workflows.CommandLine.Composition;

public class ObjectTreeProperty : IObjectTreeComponent
{
    /// <inheritdoc/>
    public Type Type { get; }

    /// <inheritdoc />
    public bool IsNullable { get; }

    /// <inheritdoc />
    public IReadOnlyCollection<ObjectTreeProperty> Properties { get; }

    /// <summary>
    /// The <see cref="PropertyInfo"/> of the property.
    /// </summary>
    public PropertyInfo Property { get; }

    /// <summary>
    /// The parent of the property.
    /// </summary>
    public IObjectTreeComponent Parent { get; }

    /// <summary>
    /// The key of the property.
    /// </summary>
    /// <remarks>
    /// The key is the name of the property.
    /// </remarks>
    public string Key { get; }

    /// <summary>
    /// The fully qualified key of the property.
    /// </summary>
    /// <remarks>
    /// The fully qualified key is the name of the property, prefixed by the fully qualified key of the parent.
    /// </remarks>
    public string FullKey { get; }

    /// <summary>
    /// The helper text of the property.
    /// </summary>
    /// <remarks>
    /// The helper text is the description of the property, or just the name if that's not set.
    /// </remarks>
    public string HelperText { get; private set; }

    internal ObjectTreeProperty(PropertyInfo property, IObjectTreeComponent parent)
    {
        Property = property;
        Parent = parent;
        Type? underlyingType = Nullable.GetUnderlyingType(property.PropertyType);
        IsNullable = underlyingType is not null;
        Type type = underlyingType ?? property.PropertyType;
        Type = type;
        Key = property.Name;
        FullKey = parent is ObjectTreeProperty parentProperty
            ? $"{parentProperty.FullKey}.{Key}"
            : Key;
        // TODO: Get the HelperText from DescriptionAttribute
        HelperText = property.Name;
        Properties = ObjectTreeComponentHelpers.CreateProperties(this);
    }

    private object GetParentInstance()
    {
        return Parent switch
        {
            ObjectTree tree => tree.Instance ?? throw new("Parent value isn't set."),
            ObjectTreeProperty parentProperty => parentProperty.GetValue(),
            _ => throw new TypeSwitchException<IObjectTreeComponent>(Parent)
        };
    }

    /// <summary>
    /// Can the property value be set?
    /// </summary>
    public bool CanSet()
    {
        return Property.SetMethod is not null;
    }

    /// <summary>
    /// Get the current value of the property.
    /// </summary>
    public object GetValue()
    {
        object parentInstance = GetParentInstance();
        return Property.GetValue(parentInstance) ?? throw new("Failed to get property value.");
    }

    /// <summary>
    /// Set the value of the property.
    /// </summary>
    /// <remarks>
    /// If the parent is a value type, the parent will be recursively set to the new value.
    /// </remarks>
    /// <param name="value">The value.</param>
    public void SetValue(object value)
    {
        if(Property.SetMethod is null)
            throw new("Property doesn't have a setter.");
        object parentInstance = GetParentInstance();
        Property.SetValue(parentInstance, value);
        if(!Parent.Type.IsValueType)
            return;
        if(Parent is ObjectTreeProperty parentProperty)
            parentProperty.SetValue(parentInstance);
        else if(Parent is ObjectTree parentTree)
            parentTree.Instance = parentInstance;
        else
            throw new TypeSwitchException<IObjectTreeComponent>(Parent);
    }

    public IReadOnlyCollection<string> ValidateValue()
    {
        object value = GetValue();
        ValidationContext context = new(value)
        {
            DisplayName = FullKey
        };
        List<ValidationResult> results = new();
        ValidationAttribute[] validationAttributes = Property
            .GetCustomAttributes<ValidationAttribute>()
            .ToArray();
        if (Validator.TryValidateValue(value, context, results, validationAttributes))
            return Array.Empty<string>();
        return results
            .Select(x => x.ErrorMessage)
            .OfType<string>()
            .ToArray();
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return FullKey;
    }
}
