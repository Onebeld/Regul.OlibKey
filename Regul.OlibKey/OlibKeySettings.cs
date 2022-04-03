using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Onebeld.Extensions;

namespace Regul.OlibKey;

[DataContract]
public class OlibKeySettings : ViewModelBase
{
    private int _generationCount = 10;
    private bool _generatorAllowLowercase = true;
    private bool _generatorAllowNumber = true;
    private bool _generatorAllowOther;
    private bool _generatorAllowSpecial;
    private bool _generatorAllowUnderscore;
    private bool _generatorAllowUppercase = true;
    private string _generatorTextOther = string.Empty;

    private bool _lockStorage = true;
    private int _lockoutTime = 30;
    private bool _clearClipboard;
    private int _clearingClipboardTime = 10;
    private bool _cleanTrashcan = true;
    private int _clearingTrashcanTime = 30;
        
    public static OlibKeySettings Instance = null!;

    public static void Load()
    {
        if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Settings"))
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Settings");
            
        try
        {
            using FileStream fileStream = File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + "Settings/" + "olibKey.xml");
            Instance = (OlibKeySettings)new XmlSerializer(typeof(OlibKeySettings)).Deserialize(fileStream);
        }
        catch
        {
            Instance = new OlibKeySettings();
        }
    }
        
    public static void Save()
    {
        using FileStream fileStream = File.Create(AppDomain.CurrentDomain.BaseDirectory + "Settings/" + "olibKey.xml");
        new XmlSerializer(typeof(OlibKeySettings)).Serialize(fileStream, Instance);
    }

    [DataMember]
    public int GenerationCount
    {
        get => _generationCount;
        set => RaiseAndSetIfChanged(ref _generationCount, value);
    }

    [DataMember]
    public bool GeneratorAllowLowercase
    {
        get => _generatorAllowLowercase;
        set => RaiseAndSetIfChanged(ref _generatorAllowLowercase, value);
    }

    [DataMember]
    public bool GeneratorAllowNumber
    {
        get => _generatorAllowNumber;
        set => RaiseAndSetIfChanged(ref _generatorAllowNumber, value);
    }

    [DataMember]
    public bool GeneratorAllowOther
    {
        get => _generatorAllowOther;
        set => RaiseAndSetIfChanged(ref _generatorAllowOther, value);
    }

    [DataMember]
    public bool GeneratorAllowSpecial
    {
        get => _generatorAllowSpecial;
        set => RaiseAndSetIfChanged(ref _generatorAllowSpecial, value);
    }

    [DataMember]
    public bool GeneratorAllowUnderscore
    {
        get => _generatorAllowUnderscore;
        set => RaiseAndSetIfChanged(ref _generatorAllowUnderscore, value);
    }

    [DataMember]
    public bool GeneratorAllowUppercase
    {
        get => _generatorAllowUppercase;
        set => RaiseAndSetIfChanged(ref _generatorAllowUppercase, value);
    }

    [DataMember]
    public string GeneratorTextOther
    {
        get => _generatorTextOther;
        set => RaiseAndSetIfChanged(ref _generatorTextOther, value);
    }

    [DataMember]
    public bool LockStorage
    {
        get => _lockStorage;
        set => RaiseAndSetIfChanged(ref _lockStorage, value);
    }

    [DataMember]
    public int LockoutTime
    {
        get => _lockoutTime;
        set => RaiseAndSetIfChanged(ref _lockoutTime, value);
    }

    [DataMember]
    public bool ClearClipboard
    {
        get => _clearClipboard;
        set => RaiseAndSetIfChanged(ref _clearClipboard, value);
    }

    [DataMember]
    public int ClearingTime
    {
        get => _clearingClipboardTime;
        set => RaiseAndSetIfChanged(ref _clearingClipboardTime, value);
    }

    [DataMember]
    public bool CleanTrashcan
    {
        get => _cleanTrashcan;
        set => RaiseAndSetIfChanged(ref _cleanTrashcan, value);
    }

    [DataMember]
    public int ClearingTrashcanTime
    {
        get => _clearingTrashcanTime;
        set => RaiseAndSetIfChanged(ref _clearingTrashcanTime, value);
    }
}