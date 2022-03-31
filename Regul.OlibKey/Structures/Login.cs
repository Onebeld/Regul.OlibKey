using System;
using System.Runtime.Serialization;
using Onebeld.Extensions;

namespace Regul.OlibKey.Structures;

[DataContract]
public class Login : ViewModelBase, ICloneable
{
    private string? _image;
    private string? _username;
    private string? _email;
    private string? _password;
    private string? _webSite;
    private string? _secretKey;
    private bool _isActivatedTotp;

    [DataMember]
    public string? Image
    {
        get => _image;
        set => RaiseAndSetIfChanged(ref _image, value);
    }
        
    [DataMember]
    public string? Username
    {
        get => _username;
        set => RaiseAndSetIfChanged(ref _username, value);
    }

    [DataMember]
    public string? Email
    {
        get => _email;
        set => RaiseAndSetIfChanged(ref _email, value);
    }

    [DataMember]
    public string? Password
    {
        get => _password;
        set => RaiseAndSetIfChanged(ref _password, value);
    }

    [DataMember]
    public string? WebSite
    {
        get => _webSite;
        set => RaiseAndSetIfChanged(ref _webSite, value);
    }

    [DataMember]
    public string? SecretKey
    {
        get => _secretKey;
        set => RaiseAndSetIfChanged(ref _secretKey, value);
    }

    [DataMember]
    public bool IsActivatedTotp
    {
        get => _isActivatedTotp;
        set => RaiseAndSetIfChanged(ref _isActivatedTotp, value);
    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}