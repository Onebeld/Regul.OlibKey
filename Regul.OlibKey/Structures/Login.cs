using System;
using System.Runtime.Serialization;
using Onebeld.Extensions;

namespace Regul.OlibKey.Structures;

[DataContract]
public class Login : ViewModelBase, ICloneable
{
    private string? _image;
    private string _username = null!;
    private string _email = null!;
    private string _password = null!;
    private string _webSite = null!;
    private string _secretKey = null!;

    [DataMember]
    public string? Image
    {
        get => _image;
        set => RaiseAndSetIfChanged(ref _image, value);
    }
        
    [DataMember]
    public string Username
    {
        get => _username;
        set => RaiseAndSetIfChanged(ref _username, value);
    }

    [DataMember]
    public string Email
    {
        get => _email;
        set => RaiseAndSetIfChanged(ref _email, value);
    }

    [DataMember]
    public string Password
    {
        get => _password;
        set => RaiseAndSetIfChanged(ref _password, value);
    }

    [DataMember]
    public string WebSite
    {
        get => _webSite;
        set => RaiseAndSetIfChanged(ref _webSite, value);
    }

    [DataMember]
    public string SecretKey
    {
        get => _secretKey;
        set => RaiseAndSetIfChanged(ref _secretKey, value);
    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}