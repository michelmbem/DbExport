using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Data;
using AvaloniaEdit;

namespace DbExport.Gui.Behaviors;

public class TextEditorBehavior : AvaloniaObject
{
    public static readonly AttachedProperty<string?> TextProperty =
        AvaloniaProperty.RegisterAttached<TextEditorBehavior, TextEditor, string?>(
            "Text", string.Empty, false, BindingMode.TwoWay);

    private static readonly HashSet<TextEditor> AttachedEditors = [];
    private static bool isUpdating;

    static TextEditorBehavior()
    {
        TextProperty.Changed.AddClassHandler<TextEditor>(OnTextChanged);
    }
    
    public static string? GetText(TextEditor editor) => editor.GetValue(TextProperty);
    
    public static void SetText(TextEditor editor, string? value) => editor.SetValue(TextProperty, value);

    private static void OnTextChanged(TextEditor editor, AvaloniaPropertyChangedEventArgs e)
    {
        if (isUpdating) return;

        if (AttachedEditors.Add(editor))
            editor.TextChanged += OnEditorTextChanged;

        editor.Text = (string?)e.NewValue;

        static void OnEditorTextChanged(object? sender, EventArgs e)
        {
            var editor = (TextEditor)sender!;

            isUpdating = true;
            SetText(editor, editor.Text);
            isUpdating = false;
        }
    }
}