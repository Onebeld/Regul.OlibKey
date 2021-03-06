using System;
using System.Runtime.Serialization;
using Onebeld.Extensions;

namespace Regul.OlibKey.Structures;

[DataContract]
public class BankCard : ViewModelBase, ICloneable
{
    private string? _typeBankCard;
    private string? _cardNumber;
    private string? _dateCard;
    private string? _securityCode;

    [DataMember]
    public string? TypeBankCard
    {
        get => _typeBankCard;
        set => RaiseAndSetIfChanged(ref _typeBankCard, value);
    }

    [DataMember]
    public string? CardNumber
    {
        get => _cardNumber;
        set => RaiseAndSetIfChanged(ref _cardNumber, value);
    }

    [DataMember]
    public string? DateCard
    {
        get => _dateCard;
        set => RaiseAndSetIfChanged(ref _dateCard, value);
    }

    [DataMember]
    public string? SecurityCode
    {
        get => _securityCode;
        set => RaiseAndSetIfChanged(ref _securityCode, value);
    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}