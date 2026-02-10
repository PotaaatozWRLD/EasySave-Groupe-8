using Avalonia.Controls;
using EasySave.GUI.ViewModels;
using System.Collections.Specialized;

namespace EasySave.GUI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // Subscribe to ListBox selection changes to update ViewModel
        var listBox = this.FindControl<ListBox>("JobsListBox");
        if (listBox != null)
        {
            listBox.SelectionChanged += (s, e) =>
            {
                if (DataContext is MainViewModel viewModel && listBox.SelectedItems != null)
                {
                    viewModel.SelectedJobs.Clear();
                    foreach (var item in listBox.SelectedItems)
                    {
                        if (item is EasySave.Shared.BackupJob job)
                        {
                            viewModel.SelectedJobs.Add(job);
                        }
                    }
                }
            };
        }
    }
}