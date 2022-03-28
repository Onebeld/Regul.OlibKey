using Onebeld.Extensions;
using PleasantUI.Controls.Custom;

namespace Regul.OlibKey.Views.Windows;

public class CreateDatabaseViewModel : ViewModelBase
{
    private string _masterPassword = string.Empty;
    private int _numberOfEncryptionProcedures = 1;
    private int _iteration = 10000;
    private bool _useCompress = true;
    private bool _useTrash = true;

    #region Properties

    public string MasterPassword
    {
        get => _masterPassword;
        set => RaiseAndSetIfChanged(ref _masterPassword, value);
    }

    public int NumberOfEncryptionProcedures
    {
        get => _numberOfEncryptionProcedures;
        set => RaiseAndSetIfChanged(ref _numberOfEncryptionProcedures, value);
    }

    public int Iteration
    {
        get => _iteration;
        set => RaiseAndSetIfChanged(ref _iteration, value);
    }

    public bool UseCompress
    {
        get => _useCompress;
        set => RaiseAndSetIfChanged(ref _useCompress, value);
    }

    public bool UseTrash
    {
        get => _useTrash;
        set => RaiseAndSetIfChanged(ref _useTrash, value);
    }

    #endregion

    private void CloseWindow(PleasantModalWindow window)
    {
        if (Iteration == 0)
            Iteration = 10000;
        if (NumberOfEncryptionProcedures == 0)
            NumberOfEncryptionProcedures = 1;
            
        if (string.IsNullOrEmpty(MasterPassword)) return;
            
        window.Close(true);
    }
}