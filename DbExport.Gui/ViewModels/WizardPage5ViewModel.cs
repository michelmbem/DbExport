using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DbExport.Gui.Models;
using DbExport.Schema;

namespace DbExport.Gui.ViewModels;

public partial class WizardPage5ViewModel : WizardPageViewModel
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Roots))]
    private Database? database;

    [ObservableProperty]
    private bool isBusy;

    public WizardPage5ViewModel()
    {
        Progress.IsIndeterminate = true;
        Progress.Message = "Loading database schema...";
    }

    public ObservableCollection<TreeNode> Roots { get; private set; } = [];

    public override bool CanMoveForward => !(IsBusy || Database is null);

    partial void OnDatabaseChanged(Database? value)
    {
        Roots = value is null ? [] : TreeNode.FromDatabase(value);
    }
}