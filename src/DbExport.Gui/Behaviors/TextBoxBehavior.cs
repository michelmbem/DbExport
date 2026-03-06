using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace DbExport.Gui.Behaviors;

public class TextBoxBehavior : AvaloniaObject
{
    public static readonly AttachedProperty<bool> NumericOnlyProperty =
        AvaloniaProperty.RegisterAttached<TextBoxBehavior, TextBox, bool>(
            "NumericOnly", false, false, BindingMode.TwoWay);

    static TextBoxBehavior()
    {
        NumericOnlyProperty.Changed.AddClassHandler<TextBox>(OnNumericOnlyChanged);
    }
    
    public static bool GetNumericOnly(TextBox tb) => tb.GetValue(NumericOnlyProperty);
    
    public static void SetNumericOnly(TextBox tb, bool value) => tb.SetValue(NumericOnlyProperty, value);

    private static void OnNumericOnlyChanged(TextBox tb, AvaloniaPropertyChangedEventArgs e)
    {
        if ((bool)e.NewValue!)
            tb.AddHandler(InputElement.TextInputEvent, OnTextInput, RoutingStrategies.Tunnel);
        else
            tb.RemoveHandler(InputElement.TextInputEvent, OnTextInput);

        static void OnTextInput(object? sender, TextInputEventArgs e)
        {
            if (e.Text?.All(char.IsDigit) == false)
                e.Handled = true;
        }
    }
}