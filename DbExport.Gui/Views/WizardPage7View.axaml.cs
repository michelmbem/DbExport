using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using AvaloniaEdit.TextMate;
using DbExport.Gui.ViewModels;
using TextMateSharp.Grammars;

namespace DbExport.Gui.Views;

public partial class WizardPage7View : UserControl
{
    private TextMate.Installation? textMate;
    
    public WizardPage7View()
    {
        InitializeComponent();
    }
    
    private static ThemeName EditorThemeName =>
        App.IsDarkMode ? ThemeName.DarkPlus: ThemeName.LightPlus;

    private WizardPage7ViewModel? ViewModel => (WizardPage7ViewModel?)DataContext;

    private void InitializeSqlEditor()
    {
        InstallTextMate();
        
        var textView = SqlEditor.TextArea.TextView;
        textView.Margin = new Thickness(10, 0);
        
        SqlEditor.Text = ViewModel?.SqlScript ?? string.Empty;
        
        Application.Current!.ActualThemeVariantChanged += OnThemeChanged;
    }

    private void InstallTextMate()
    {
        textMate?.Dispose();
        
        var registryOptions = new RegistryOptions(EditorThemeName);
        textMate = SqlEditor.InstallTextMate(registryOptions);
        textMate.SetGrammar(registryOptions.GetScopeByLanguageId("sql"));
    }

    private void OnThemeChanged(object? sender, EventArgs e)
    {
        InstallTextMate();
    }

    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        InitializeSqlEditor();
        ViewModel!.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        ViewModel!.PropertyChanged -= OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        var sqlScript = ViewModel?.SqlScript;
        if (sqlScript == null) return;
        
        SqlEditor.Text = sqlScript;
    }

    private void OnSqlEditorTextChanged(object? sender, EventArgs e)
    {
        ViewModel!.SqlScript = SqlEditor.Text;
    }
}