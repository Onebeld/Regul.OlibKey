using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Avalonia.Collections;
using Onebeld.Extensions;
using Regul.OlibKey.General;

namespace Regul.OlibKey.Structures;

[DataContract]
public class Database : ViewModelBase
{
    private AvaloniaList<Data> _dataList = new();
    private AvaloniaList<Folder> _folders = new();
    private Trashcan _trashcan = new ();
    private DatabaseSettings _settings = null!;

    [DataMember]
    public AvaloniaList<Data> DataList
    {
        get => _dataList;
        set => RaiseAndSetIfChanged(ref _dataList, value);
    }

    [DataMember]
    public AvaloniaList<Folder> Folders
    {
        get => _folders;
        set => RaiseAndSetIfChanged(ref _folders, value);
    }

    [XmlElement(IsNullable = false)]
    [DataMember]
    public Trashcan Trashcan
    {
        get => _trashcan;
        set => RaiseAndSetIfChanged(ref _trashcan, value);
    }

    [XmlIgnore]
    public DatabaseSettings Settings
    {
        get => _settings;
        set => RaiseAndSetIfChanged(ref _settings, value);
    }
        
    public static Database Load(string path, string masterPassword)
    {
        string file = File.ReadAllText(path);

        string[] split = file.Split(':');

        int iterations = int.Parse(split[0]);
        int numberOfEncryptionProcedures = int.Parse(split[1]);
        string encryptString = split[2];

        bool useCompress = false;
        bool UseTrashcan = false;

        if (split.Length > 3)
        {
            useCompress = bool.Parse(split[3]);

            if (useCompress)
                file = Compressing.Decompress(Encryptor.DecryptString(encryptString, masterPassword, iterations,
                    numberOfEncryptionProcedures));
        }
        else
        {
            file = Encryptor.DecryptString(encryptString, masterPassword, iterations, numberOfEncryptionProcedures);
        }

        if (split.Length > 4)
            UseTrashcan = bool.Parse(split[4]);

        Database database = FromXml(file);
        DatabaseSettings settings = new()
        {
            Iterations = iterations,
            NumberOfEncryptionProcedures = numberOfEncryptionProcedures,
            UseCompress = useCompress,
            UseTrashcan = UseTrashcan
        };
        database.Settings = settings;

        return database;
    }

    public static Database FromXml(string xml)
    {
        return (Database)new XmlSerializer(typeof(Database)).Deserialize(new StringReader(xml));
    }
}