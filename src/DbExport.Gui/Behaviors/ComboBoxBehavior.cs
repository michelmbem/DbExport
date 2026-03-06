using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace DbExport.Gui.Behaviors;

public class ComboBoxBehavior : AvaloniaObject
{
    public static readonly AttachedProperty<ICommand?> DropDownProperty =
        AvaloniaProperty.RegisterAttached<ComboBoxBehavior, ComboBox, ICommand?>(
            "DropDown", null, false, BindingMode.TwoWay);

    static ComboBoxBehavior()
    {
        DropDownProperty.Changed.AddClassHandler<ComboBox>(OnDropDownChanged);
    }
    
    public static ICommand? GetDropDown(ComboBox cb) => cb.GetValue(DropDownProperty);
    
    public static void SetDropDown(ComboBox cb, ICommand? value) => cb.SetValue(DropDownProperty, value);

    private static void OnDropDownChanged(ComboBox cb, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is ICommand)
            cb.DropDownOpened += OnDropDownOpened;
        else
            cb.DropDownOpened -= OnDropDownOpened;

        void OnDropDownOpened(object? sender, EventArgs e1)
        {
            if (e.NewValue is ICommand command && command.CanExecute(null))
                command.Execute(null);
        }
    }
}