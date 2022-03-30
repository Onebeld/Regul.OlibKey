using Avalonia;
using Avalonia.Input;
using Onebeld.Extensions;
using PleasantUI.Controls.Custom;
using Regul.OlibKey.General;

namespace Regul.OlibKey.Views.Windows;

public class PasswordGeneratorViewModel : ViewModelBase
{
    private readonly PleasantDialogWindow? _modalWindow;
    private string _password = string.Empty;
    private bool _returnRequired;

    #region Properties

    public string Password
    {
        get => _password;
        set => RaiseAndSetIfChanged(ref _password, value);
    }

    public bool ReturnRequired
    {
        get => _returnRequired;
        set => RaiseAndSetIfChanged(ref _returnRequired, value);
    }

    #endregion

    public PasswordGeneratorViewModel(PleasantDialogWindow? modalWindow, bool returnRequired)
    {
        _modalWindow = modalWindow;
        ReturnRequired = returnRequired;

        _modalWindow?.KeyBindings.Add(new KeyBinding
        {
            Command = Command.Create(SavePassword),
            Gesture = KeyGesture.Parse("Enter")
        });
    }

    public void CopyPassword() => Application.Current?.Clipboard?.SetTextAsync(Password);

    public void GeneratePassword() => Password = PasswordGenerator.Generate();

    public void SavePassword()
    {
        if (string.IsNullOrEmpty(Password))
            Password = PasswordGenerator.Generate();
        
        _modalWindow?.Close(Password);
    }
}