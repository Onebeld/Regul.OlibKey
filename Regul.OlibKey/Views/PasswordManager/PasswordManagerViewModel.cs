using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Onebeld.Extensions;
using PleasantUI.Controls.Custom;
using Regul.Base;
using Regul.Base.Views.Windows;
using Regul.ModuleSystem;
using Regul.OlibKey.Enums;
using Regul.OlibKey.General;
using Regul.OlibKey.Structures;
using Regul.OlibKey.Views.Pages;
using Regul.OlibKey.Views.PasswordManager.Content;
using Regul.OlibKey.Views.Windows;

namespace Regul.OlibKey.Views
{
	public class PasswordManagerViewModel : ViewModelBase
	{
		private Database _database;
		private IPage _page;
		private Control _content;
		private Data _selectedData;

		private bool _isUnlocked;
		private bool _isNotCreatedDatabase;
		private string _masterPassword;

		#region Properties

		public bool IsEdited
		{
			get => PleasantTabItem.IsEditedIndicator;
			set
			{
				PleasantTabItem.IsEditedIndicator = value;
				RaisePropertyChanged(nameof(IsEdited));
			}
		}

		public Database Database
		{
			get => _database;
			set
			{
				RaiseAndSetIfChanged(ref _database, value);
				IsEdited = true;
			}
		}

		public Data SelectedData
		{
			get => _selectedData;
			set
			{
				RaiseAndSetIfChanged(ref _selectedData, value);

				if (SelectedData == null)
					Page = new StartPage();
				else
				{
					Page = new DataPage(DataInformation.View, this);
				}
			}
		}

		public bool IsUnlocked
		{
			get => _isUnlocked;
			set => RaiseAndSetIfChanged(ref _isUnlocked, value);
		}

		public IPage Page
		{
			get => _page;
			set => RaiseAndSetIfChanged(ref _page, value);
		}

		public Control Content
		{
			get => _content;
			set => RaiseAndSetIfChanged(ref _content, value);
		}

		public bool IsNotCreatedDatabase
		{
			get => _isNotCreatedDatabase;
			set => RaiseAndSetIfChanged(ref _isNotCreatedDatabase, value);
		}

		public string MasterPassword
		{
			get => _masterPassword;
			set => RaiseAndSetIfChanged(ref _masterPassword, value);
		}

		public string PathToFile;

		#endregion

		private PleasantTabItem PleasantTabItem { get; }
		private PasswordManagerView PasswordManagerView { get; }

		public PasswordManagerViewModel()
		{
			Page = new StartPage();
		}
		
		public PasswordManagerViewModel(
			PleasantTabItem pleasantTabItem, 
			Editor editor, 
			PasswordManagerView passwordManagerView,
			string path = null) : this()
		{
			PleasantTabItem = pleasantTabItem;
			PasswordManagerView = passwordManagerView;
			PasswordManagerView.CurrentEditor = editor;

			PathToFile = path;

			if (string.IsNullOrEmpty(path))
				IsNotCreatedDatabase = true;

			Content = new CreateUnblockDatabaseContent();

			IsEdited = false;
		}

		private void AddData()
		{
			SelectedData = null;
			
			Page = new DataPage(DataInformation.Create, this);
		}

		private async void CreateDatabase()
		{
			CreateDatabase createDatabase = new CreateDatabase();

			if (!await createDatabase.Show<bool>(WindowsManager.MainWindow)) return;
			
			CreateDatabaseViewModel viewModel =
				createDatabase.GetDataContext<CreateDatabaseViewModel>();

			MasterPassword = viewModel.MasterPassword;
			
			Database = new Database
			{
				Settings = new DatabaseSettings
				{
					Iterations = viewModel.Iteration,
					NumberOfEncryptionProcedures = viewModel.NumberOfEncryptionProcedures,
					UseCompress = viewModel.UseCompress,
					UseTrash = viewModel.UseTrash
				}
			};

			IsNotCreatedDatabase = false;
			Content = new MainContent();
		}

		private void OpenDatabase()
		{
			if (!File.Exists(PathToFile)) return;
			
			try
			{
				Database = Database.Load(PathToFile, MasterPassword);
			}
			catch (Exception e)
			{
				MainViewModel viewModel = WindowsManager.MainWindow.GetDataContext<MainViewModel>();
				viewModel.NotificationManager.Show(new Notification(App.GetResource<string>("Error"), App.GetResource<string>("IncorrectMasterPasswordMessage"), NotificationType.Error));
				
				return;
			}

			IsUnlocked = true;

			Content = new MainContent();
		}

		public bool Save(string path)
		{
			try
			{
				PathToFile = path;
				
				Database.Save(PathToFile, MasterPassword);

				IsEdited = false;

				return true;
			}
			catch
			{
				return false;
			}
		}

		public void Release()
		{
			
		}
	}
}
