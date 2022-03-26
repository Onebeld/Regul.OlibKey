using Avalonia;
using Onebeld.Extensions;
using PleasantUI.Controls.Custom;

namespace Regul.OlibKey.Views.Windows
{
    public class PasswordGeneratorViewModel : ViewModelBase
    {
        private readonly PleasantDialogWindow _modalWindow;

        private string _password;
        private bool _returnRequired;

        public string Password
        {
            get => _password;
            set => RaiseAndSetIfChanged(ref _password, value);
        }

        private bool ReturnRequired
        {
            get => _returnRequired;
            set => RaiseAndSetIfChanged(ref _returnRequired, value);
        }

        public PasswordGeneratorViewModel(PleasantDialogWindow modalWindow, bool returnRequired)
        {
            _modalWindow = modalWindow;
            ReturnRequired = returnRequired;
        }

        public void CopyPassword() => Application.Current?.Clipboard?.SetTextAsync(Password);

        public void GeneratePassword() => Password = General.PasswordGenerator.Generate();

        public void SavePassword() => _modalWindow?.Close(Password);
    }
}