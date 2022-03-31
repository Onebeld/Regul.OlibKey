using System.Linq;
using Avalonia.Collections;
using Avalonia.Controls.Notifications;
using Onebeld.Extensions;
using Regul.Base;
using Regul.OlibKey.Enums;
using Regul.OlibKey.General;
using Regul.OlibKey.Structures;

namespace Regul.OlibKey.Views.Windows;

public class PasswordCheckerViewModel : ViewModelBase
{
    private readonly Database _database;
    private readonly PasswordManagerViewModel _targetViewModel;
    
    #region Properties

    public AvaloniaList<Data> SelectedData { get; } = new();

    public AvaloniaList<Data> DataListWithBadPasswords { get; } = new();
    
    public double OverallComplexity { get; set; }

    #endregion
    
    public PasswordCheckerViewModel(PasswordManagerViewModel targetViewModel, Database database)
    {
        _targetViewModel = targetViewModel;
        _database = database;
        
        CheckOverallComplexity();
    }

    public void CheckOverallComplexity()
    {
        SelectedData.Clear();
        DataListWithBadPasswords.Clear();
        
        double complexitySum = 0;

        foreach (Data data in _database.DataList)
        {
            if (data.TypeId != DataType.Login) continue;

            double complexity = PasswordUtils.GetPasswordComplexity(data.Login!.Password);
            complexitySum += complexity > 300 ? 300 : complexity;

            if (complexity < 150) 
                DataListWithBadPasswords.Add(data);
        }

        int count = _database.DataList.Count;
        
        OverallComplexity = count == 0 ? 0 : complexitySum / count;
        
        RaisePropertyChanged(nameof(OverallComplexity));
    }

    private void SelectAll()
    {
        SelectedData.Clear();
        SelectedData.AddRange(DataListWithBadPasswords);
    }

    private void FixBadPasswords()
    {
        foreach (Data data in SelectedData.Where(data => data.Login is not null))
            data.Login!.Password = PasswordGenerator.Generate();

        _targetViewModel.IsEdited = true;
        
        WindowsManager.ShowNotification(App.GetResource<string>("PasswordCheckerSuccessChangedMessage"));

        CheckOverallComplexity();
    }
}