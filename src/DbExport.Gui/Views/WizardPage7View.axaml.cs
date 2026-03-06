using Avalonia;
using Avalonia.Controls;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;

namespace DbExport.Gui.Views;

public partial class WizardPage7View : UserControl
{
    private TextMate.Installation? textMate;
    
    public WizardPage7View()
    {
        InitializeComponent();
        InitializeSqlEditor();
    }
    
    private static ThemeName EditorThemeName => App.IsDarkMode ? ThemeName.DarkPlus: ThemeName.LightPlus;

    private void InitializeSqlEditor()
    {
        SqlEditor.TextArea.TextView.Margin = new Thickness(10, 0);
        
        InstallTextMate();
        App.AddThemeListener((_, _) => InstallTextMate());
    }

    private void InstallTextMate()
    {
        textMate?.Dispose();
        
        var registryOptions = new RegistryOptions(EditorThemeName);
        textMate = SqlEditor.InstallTextMate(registryOptions);
        textMate.SetGrammar(registryOptions.GetScopeByLanguageId("sql"));
    }
}