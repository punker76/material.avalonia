﻿using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Material.Avalonia.Demo.Pages;
using Material.Dialog;
using Material.Dialog.Enums;
using Material.Dialog.Icons;

namespace Material.Avalonia.Demo.ViewModels;

public class DialogDemoViewModel : ViewModelBase {
    private readonly MainWindow? _window;
    private DateTime _previousDatePickerResult = DateTime.Now;
    private TimeSpan _previousTimePickerResult = TimeSpan.Zero;

    public DialogDemoViewModel() {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime app) {
            if (app.MainWindow is not MainWindow w)
                return;

            _window = w;
            IsDialogsAvailable = true;
        }

        StandaloneDialogItems = [
            new DialogDemoItemViewModel("Simple Dialog", Dialog1),
            new DialogDemoItemViewModel("Dialog with confirmation", Dialog2),
            new DialogDemoItemViewModel("Dialog with confirmation (content-only)", Dialog3),
            new DialogDemoItemViewModel("Dialog with bitmap icon", Dialog4),
            new DialogDemoItemViewModel("Login dialog", LoginDialog),
            new DialogDemoItemViewModel("Folder rename dialog", FolderNameDialog),
            // TODO Fix DatePicker TimePicker dialogs https://github.com/AvaloniaCommunity/Material.Avalonia/issues/470
            // new DialogDemoItemViewModel("Time picker", TimePickerDialog),
            // new DialogDemoItemViewModel("Date picker", DatePickerDialog),
            new DialogDemoItemViewModel("Custom dialog", CustomDialog)
        ];
    }

    public bool IsDialogsAvailable { get; }

    public DialogDemoItemViewModel[] StandaloneDialogItems { get; } = Array.Empty<DialogDemoItemViewModel>();

    private async IAsyncEnumerable<string> Dialog1() {
        if (_window == null) yield break;
        var dialog = DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams {
            ContentHeader = "Welcome to use Material.Avalonia",
            SupportingText = "Enjoy Material Design in AvaloniaUI!",
            StartupLocation = WindowStartupLocation.CenterOwner
        });
        var result = await dialog.ShowDialog(_window);
        yield return $"Result: {result.GetResult}";
    }

    private async IAsyncEnumerable<string> Dialog2() {
        if (_window == null) yield break;
        var result = await DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams {
            ContentHeader = "Confirm action",
            SupportingText = "Are you sure to DELETE 20 FILES?",
            StartupLocation = WindowStartupLocation.CenterOwner,
            NegativeResult = new DialogResult("cancel"),
            DialogHeaderIcon = DialogIconKind.Help,
            DialogButtons = new[] {
                new DialogButton {
                    Content = "CANCEL",
                    Result = "cancel"
                },
                new DialogButton {
                    Content = "DELETE",
                    Result = "delete"
                }
            }
        }).ShowDialog(_window);
        yield return $"Result: {result.GetResult}";
    }

    private async IAsyncEnumerable<string> Dialog3() {
        if (_window == null) yield break;
        var result = await DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams {
            ContentHeader = "Confirm action",
            SupportingText = "Are you sure to DELETE 20 FILES?",
            DialogHeaderIcon = DialogIconKind.Help,
            StartupLocation = WindowStartupLocation.CenterOwner,
            NegativeResult = new DialogResult("cancel"),
            Borderless = true,
            DialogButtons = new[] {
                new DialogButton {
                    Content = "CANCEL",
                    Result = "cancel"
                },
                new DialogButton {
                    Content = "DELETE",
                    Result = "delete"
                }
            }
        }).ShowDialog(_window);

        yield return $"Result: {result.GetResult}";

        if (result.GetResult == "delete") {
            await DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams {
                ContentHeader = "Result",
                SupportingText = "20 files has deleted.",
                StartupLocation = WindowStartupLocation.CenterOwner,
                DialogHeaderIcon = DialogIconKind.Success,
                Borderless = true,
            }).ShowDialog(_window);
        }
    }

    private async IAsyncEnumerable<string> Dialog4() {
        if (_window == null) yield break;
        // Open asset stream using assets.Open method.
        await using var icon = AssetLoader.Open(new Uri("avares://Material.Avalonia.Demo/Assets/avalonia-logo.png"));

        var dialog = DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams {
            ContentHeader = "Welcome to use Material.Avalonia",
            SupportingText = "Enjoy Material Design in AvaloniaUI!",
            StartupLocation = WindowStartupLocation.CenterOwner,
            Borderless = true,
            // Create Image control
            DialogIcon = new Bitmap(icon),
            NeutralDialogButtons = new[] {
                new DialogButton {
                    Content = "READ MORE...",
                    Result = "read_more"
                }
            }
        });
        var result = await dialog.ShowDialog(_window);

        yield return $"Result: {result.GetResult}";
    }

    private async IAsyncEnumerable<string> LoginDialog() {
        if (_window == null) yield break;
        var result = await DialogHelper.CreateTextFieldDialog(new TextFieldDialogBuilderParams {
            ContentHeader = "Authentication required.",
            SupportingText = "Please login before any action.",
            StartupLocation = WindowStartupLocation.CenterOwner,
            DialogHeaderIcon = DialogIconKind.Blocked,
            Borderless = true,
            Width = 400,
            TextFields = new[] {
                new TextFieldBuilderParams {
                    HelperText = "* Required",
                    Classes = "outline",
                    Label = "Account",
                    MaxCountChars = 24,
                    Validater = ValidateAccount,
                },
                new TextFieldBuilderParams {
                    HelperText = "* Required",
                    Classes = "outline",
                    Label = "Password",
                    MaxCountChars = 64,
                    FieldKind = TextFieldKind.Masked,
                    Validater = ValidatePassword
                }
            },
            DialogButtons = new[] {
                new DialogButton {
                    Content = "CANCEL",
                    Result = "cancel",
                    IsNegative = true
                },
                new DialogButton {
                    Content = "LOGIN",
                    Result = "login",
                    IsPositive = true
                }
            }
        }).ShowDialog(_window);

        yield return $"Result: {result.GetResult}";

        if (result.GetResult != "login")
            yield break;

        yield return $"Account: {result.GetFieldsResult()[0].Text}";
        yield return $"Password: {result.GetFieldsResult()[1].Text}";
    }

    private Tuple<bool, string> ValidateAccount(string text) {
        var result = text.Length > 5;
        return new Tuple<bool, string>(result, result ? "" : "Too few account name.");
    }

    private Tuple<bool, string> ValidatePassword(string text) {
        var result = text.Length >= 1;
        return new Tuple<bool, string>(result, result ? "" : "Field should be filled.");
    }


    private async IAsyncEnumerable<string> FolderNameDialog() {
        if (_window == null) yield break;
        var result = await DialogHelper.CreateTextFieldDialog(new TextFieldDialogBuilderParams {
            ContentHeader = "Rename folder",
            StartupLocation = WindowStartupLocation.CenterOwner,
            Borderless = true,
            Width = 400,
            TextFields = new TextFieldBuilderParams[] {
                new() {
                    Label = "Folder name",
                    MaxCountChars = 256,
                    Validater = ValidatePassword,
                    DefaultText = "Folder1",
                    HelperText = "* Required"
                }
            },
            DialogButtons = new[] {
                new DialogButton {
                    Content = "CANCEL",
                    Result = "cancel",
                    IsNegative = true
                },
                new DialogButton {
                    Content = "RENAME",
                    Result = "rename",
                    IsPositive = true
                }
            },
        }).ShowDialog(_window);

        yield return $"Result: {result.GetResult}";

        if (result.GetResult == "rename") {
            yield return $"Folder name: {result.GetFieldsResult()[0].Text}";
        }
    }

    private async IAsyncEnumerable<string> TimePickerDialog() {
        if (_window == null) yield break;
        var result = await DialogHelper.CreateTimePicker(new TimePickerDialogBuilderParams {
            Borderless = true,
            StartupLocation = WindowStartupLocation.CenterOwner,
            ImplicitValue = _previousTimePickerResult,
            DialogButtons = new[] {
                new DialogButton {
                    Content = "CONFIRM",
                    Result = "confirm",
                    IsPositive = true
                }
            }
        }).ShowDialog(_window);

        yield return $"Result: {result.GetResult}";

        if (result.GetResult != "confirm")
            yield break;

        var r = result.GetTimeSpan();
        yield return $"TimeSpan: {r.ToString()}";
        _previousTimePickerResult = r;
    }

    private async IAsyncEnumerable<string> DatePickerDialog() {
        if (_window == null) yield break;
        var result = await DialogHelper.CreateDatePicker(new DatePickerDialogBuilderParams {
            Borderless = true,
            StartupLocation = WindowStartupLocation.CenterOwner,
            ImplicitValue = _previousDatePickerResult,
            DialogButtons = new[] {
                new DialogButton {
                    Content = "CONFIRM",
                    Result = "confirm",
                    IsPositive = true
                }
            }
        }).ShowDialog(_window);

        yield return $"Result: {result.GetResult}";

        if (result.GetResult != "confirm")
            yield break;

        var r = result.GetDate();
        // ReSharper disable once SimplifyStringInterpolation
        yield return $"TimeSpan: {r.ToString("d")}";
        _previousDatePickerResult = r;
    }

    private async IAsyncEnumerable<string> CustomDialog() {
        if (_window == null) yield break;
        // Open asset stream using assets.Open method.
        await using var icon = AssetLoader.Open(new Uri("avares://Material.Avalonia.Demo/Assets/avalonia-logo.png"));

        var dialog = DialogHelper.CreateCustomDialog(new CustomDialogBuilderParams() {
            Content = new CustomDialogContentDemo(),
            StartupLocation = WindowStartupLocation.CenterOwner,
            Borderless = false,
        });

        var result = await dialog.ShowDialog(_window);

        yield return $"Result: {result.GetResult}";
    }
}