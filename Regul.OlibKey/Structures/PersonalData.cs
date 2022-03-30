using System;
using System.Runtime.Serialization;
using Onebeld.Extensions;

namespace Regul.OlibKey.Structures;

[DataContract]
public class PersonalData : ViewModelBase, ICloneable
{
    private string _fullname = null!;
    private string _number = null!;
    private string _placeOfIssue = null!;
    private string _socialSecurityNumber = null!;
    private string _tin = null!;
    private string _email = null!;
    private string _telephone = null!;
    private string _company = null!;
    private string _postcode = null!;
    private string _country = null!;
    private string _region = null!;
    private string _city = null!;
    private string _address = null!;

    [DataMember]
    public string Fullname
    {
        get => _fullname;
        set => RaiseAndSetIfChanged(ref _fullname, value);
    }

    [DataMember]
    public string Number
    {
        get => _number;
        set => RaiseAndSetIfChanged(ref _number, value);
    }

    [DataMember]
    public string PlaceOfIssue
    {
        get => _placeOfIssue;
        set => RaiseAndSetIfChanged(ref _placeOfIssue, value);
    }

    [DataMember]
    public string SocialSecurityNumber
    {
        get => _socialSecurityNumber;
        set => RaiseAndSetIfChanged(ref _socialSecurityNumber, value);
    }

    [DataMember]
    public string Tin
    {
        get => _tin;
        set => RaiseAndSetIfChanged(ref _tin, value);
    }

    [DataMember]
    public string Email
    {
        get => _email;
        set => RaiseAndSetIfChanged(ref _email, value);
    }

    [DataMember]
    public string Telephone
    {
        get => _telephone;
        set => RaiseAndSetIfChanged(ref _telephone, value);
    }

    [DataMember]
    public string Company
    {
        get => _company;
        set => RaiseAndSetIfChanged(ref _company, value);
    }

    [DataMember]
    public string Postcode
    {
        get => _postcode;
        set => RaiseAndSetIfChanged(ref _postcode, value);
    }

    [DataMember]
    public string Country
    {
        get => _country;
        set => RaiseAndSetIfChanged(ref _country, value);
    }

    [DataMember]
    public string Region
    {
        get => _region;
        set => RaiseAndSetIfChanged(ref _region, value);
    }

    [DataMember]
    public string City
    {
        get => _city;
        set => RaiseAndSetIfChanged(ref _city, value);
    }

    [DataMember]
    public string Address
    {
        get => _address;
        set => RaiseAndSetIfChanged(ref _address, value);
    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}