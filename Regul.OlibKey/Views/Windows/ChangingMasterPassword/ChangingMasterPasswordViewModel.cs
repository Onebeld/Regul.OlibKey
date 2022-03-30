using Avalonia.Controls.Notifications;
using Onebeld.Extensions;
using PleasantUI.Controls.Custom;
using Regul.Base;

namespace Regul.OlibKey.Views.Windows;

public class ChangingMasterPasswordViewModel : ViewModelBase
{
    private string _oldMasterPassword = string.Empty;
    private string _newMasterPassword = string.Empty;
    private string _masterPassword;

    #region Properties

    public string OldMasterPassword
    {
        get => _oldMasterPassword;
        set => RaiseAndSetIfChanged(ref _oldMasterPassword, value);
    }

    public string NewMasterPassword
    {
        get => _newMasterPassword;
        set => RaiseAndSetIfChanged(ref _newMasterPassword, value);
    }

    #endregion

    public ChangingMasterPasswordViewModel(string masterPassword)
    {
        _masterPassword = masterPassword;
    }
    
    private void CloseWindow(PleasantModalWindow window)
    {
        if (OldMasterPassword != _masterPassword)
        {
            WindowsManager.ShowNotification(App.GetResource<string>("IncorrectMasterPasswordMessage"), NotificationType.Error);
            
            return;
        }
            
        window.Close(true);
    }
}