using Avalonia.Controls;
using Avalonia.Input;

namespace DbExport.Gui.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnTitleBarPointerPressed(object sender, PointerPressedEventArgs e)
    {
        var currentPoint = e.GetCurrentPoint(this);

        if (!currentPoint.Properties.IsLeftButtonPressed) return;

        BeginMoveDrag(new PointerPressedEventArgs(this, currentPoint.Pointer, this, currentPoint.Position, default, default, default));
    }

    private void Grid_ActualThemeVariantChanged(object? sender, System.EventArgs e)
    {
    }
}