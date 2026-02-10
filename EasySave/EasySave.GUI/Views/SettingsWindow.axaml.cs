using Avalonia.Controls;
using EasySave.GUI.ViewModels;
using System;

namespace EasySave.GUI.Views;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
    }

    public SettingsWindow(SettingsViewModel viewModel) : this()
    {
        DataContext = viewModel;
        viewModel.CloseRequested += (s, e) => Close();
    }
}
