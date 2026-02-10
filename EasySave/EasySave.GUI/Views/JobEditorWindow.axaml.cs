using Avalonia.Controls;
using EasySave.GUI.ViewModels;
using System;

namespace EasySave.GUI.Views;

public partial class JobEditorWindow : Window
{
    public JobEditorWindow()
    {
        InitializeComponent();
    }

    public JobEditorWindow(JobEditorViewModel viewModel) : this()
    {
        DataContext = viewModel;
        viewModel.CloseRequested += (s, e) => Close();
    }
}
