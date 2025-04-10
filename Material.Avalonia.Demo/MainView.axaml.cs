using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Material.Styles.Controls;
using Material.Styles.Models;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;

namespace Material.Avalonia.Demo;

public partial class MainView : UserControl {
    public MainView() {
        InitializeComponent();
        DrawerList.PointerReleased += DrawerSelectionChanged;
        DrawerList.KeyUp += DrawerList_KeyUp;
    }

    private void DrawerList_KeyUp(object? sender, KeyEventArgs e) {
        if (e.Key == Key.Space || e.Key == Key.Enter)
            DrawerSelectionChanged(sender, null);
    }

    public void DrawerSelectionChanged(object? sender, RoutedEventArgs? args) {
        if (sender is not ListBox listBox)
            return;

        if (!listBox.IsFocused && !listBox.IsKeyboardFocusWithin)
            return;
        try {
            PageCarousel.SelectedIndex = listBox.SelectedIndex;
            mainScroller.Offset = Vector.Zero;
            mainScroller.VerticalScrollBarVisibility =
                ((Control)PageCarousel.SelectedItem!).GetValue(ScrollViewer.VerticalScrollBarVisibilityProperty);
        }
        catch {
            // ignored
        }

        LeftDrawer.OptionalCloseLeftDrawer();
    }

    private void TemplatedControl_OnTemplateApplied(object? sender, TemplateAppliedEventArgs e) {
        SnackbarHost.Post("Welcome to demo of Material.Avalonia!", null, DispatcherPriority.Normal);
    }

    /// <summary>
    /// This method is used for showcase of snackbar.
    /// </summary>
    private void HelloButtonMenuItem_OnClick(object? sender, RoutedEventArgs e) {
        // According to guidelines of Material design, 'endless' snackbar is not recommended.
        // They should dismiss after 4 - 10 seconds.
        // https://m2.material.io/components/snackbars#behavior
        var helloSnackBar = new SnackbarModel("Hello, user!", TimeSpan.FromSeconds(5));
        SnackbarHost.Post(helloSnackBar, null, DispatcherPriority.Normal);
    }

    /// <summary>
    /// This method is used for showcase of snackbar.
    /// </summary>
    private void GoodbyeButtonMenuItem_OnClick(object? sender, RoutedEventArgs e) {
        SnackbarHost.Post("See ya next time, user!", null, DispatcherPriority.Normal);
    }

    /// <summary>
    /// This method is used for showcase of snackbar.
    /// </summary>
    private void ConnectToNetworkMenuItem_OnClick(object? sender, RoutedEventArgs e) {
        void Retry() {
            SnackbarHost.Post(
                new SnackbarModel("Connected to network.", TimeSpan.FromSeconds(5)),
                null, DispatcherPriority.Normal);
        }

        SnackbarHost.Post(
            new SnackbarModel("Unable to connect network. Please check everything is fine.",
                TimeSpan.FromSeconds(10),
                new SnackbarButtonModel {
                    Text = "Retry",
                    Action = Retry
                }), null, DispatcherPriority.Normal);
    }

    private void MaterialIcon_OnPointerPressed(object? sender, PointerPressedEventArgs e) {
        var materialTheme = Application.Current!.LocateMaterialTheme<MaterialTheme>();
        materialTheme.BaseTheme =
            materialTheme.BaseTheme == BaseThemeMode.Light ? BaseThemeMode.Dark : BaseThemeMode.Light;
    }
}