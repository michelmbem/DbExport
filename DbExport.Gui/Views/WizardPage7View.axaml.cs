using System;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using AvaloniaEdit.TextMate;
using DbExport.Gui.ViewModels;
using TextMateSharp.Grammars;

namespace DbExport.Gui.Views;

public partial class WizardPage7View : UserControl
{
    public WizardPage7View()
    {
        InitializeComponent();
    }

    private WizardPage7ViewModel? ViewModel => (WizardPage7ViewModel?)DataContext;

    private void InitializeSqlEditor()
    {
        var registryOptions = new RegistryOptions(ThemeName.DarkPlus);
        var textMate = SqlEditor.InstallTextMate(registryOptions);
        textMate.SetGrammar(registryOptions.GetScopeByLanguageId("sql"));
        
        var textView = SqlEditor.TextArea.TextView;
        textView.Margin = new Thickness(10, 0);
        
        SqlEditor.Text = ViewModel?.SqlScript ?? string.Empty;
    }

    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        InitializeSqlEditor();

        foreach (var menuItem in SqlEditor.ContextMenu!.Items.Where(i => i is MenuItem).Cast<MenuItem>())
            menuItem.CommandParameter = SqlEditor;

        ViewModel?.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        ViewModel?.PropertyChanged -= OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        var sqlScript = ViewModel?.SqlScript;
        if (sqlScript == null) return;
        
        SqlEditor.Text = sqlScript;
    }

    private void OnSqlEditorTextChanged(object? sender, EventArgs e)
    {
        ViewModel?.SqlScript = SqlEditor.Text;
    }
}