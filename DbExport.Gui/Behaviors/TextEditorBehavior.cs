using Avalonia;
using Avalonia.Data;
using AvaloniaEdit;

namespace DbExport.Gui.Behaviors;

public class TextEditorBehavior : AvaloniaObject
{
    public static readonly AttachedProperty<string?> TextProperty =
        AvaloniaProperty.RegisterAttached<TextEditorBehavior, TextEditor, string?>(
            "Text", string.Empty, false, BindingMode.TwoWay);

    static TextEditorBehavior()
    {
        TextProperty.Changed.AddClassHandler<TextEditor>(OnTextChanged);
    }
    
    public static string? GetText(TextEditor editor) => editor.GetValue(TextProperty);
    
    public static void SetText(TextEditor editor, string? value) => editor.SetValue(TextProperty, value);

    private static void OnTextChanged(TextEditor editor, AvaloniaPropertyChangedEventArgs e)
    {
        editor.Text = (string?)e.NewValue ?? string.Empty;
    }
}