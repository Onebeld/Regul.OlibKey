using Onebeld.Extensions;

namespace Regul.OlibKey.Structures;

public class DatabaseSettings : ViewModelBase
{
    private int _iterations;
    private int _numberOfEncryptionProcedures;
    private bool _useCompress;
    private bool _useTrash;

    public int Iterations
    {
        get => _iterations;
        set => RaiseAndSetIfChanged(ref _iterations, value);
    }

    public int NumberOfEncryptionProcedures
    {
        get => _numberOfEncryptionProcedures;
        set => RaiseAndSetIfChanged(ref _numberOfEncryptionProcedures, value);
    }

    public bool UseCompress
    {
        get => _useCompress;
        set => RaiseAndSetIfChanged(ref _useCompress, value);
    }

    public bool UseTrashcan
    {
        get => _useTrash;
        set => RaiseAndSetIfChanged(ref _useTrash, value);
    }
}