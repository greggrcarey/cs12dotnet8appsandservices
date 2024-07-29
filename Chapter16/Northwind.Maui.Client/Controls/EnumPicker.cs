using System.Reflection;

namespace Northwind.Maui.Client.Controls;

internal class EnumPicker : Picker
{
    public Type EnumType
    {
        get => (Type)GetValue(EnumTypeProperty);
        set => SetValue(EnumTypeProperty, value);
    }

    public static readonly BindableProperty EnumTypeProperty =
        BindableProperty.Create(
            propertyName: nameof(EnumType),
            returnType: typeof(Type),
            declaringType: typeof(EnumPicker),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                EnumPicker picker = (EnumPicker)bindable;
                if (oldValue is not null)
                {
                    picker.ItemsSource = null;
                }
                if (newValue is not null)
                {
                    if (!((Type)newValue).GetTypeInfo().IsEnum)
                        throw new ArgumentException(
                            "EnumPicker: EnumType property must be enumeration type");
                    picker.ItemsSource = Enum.GetValues((Type)newValue);
                }
            });

}
