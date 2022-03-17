using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Regul.Base;
using Regul.ModuleSystem;
using Regul.ModuleSystem.Models;
using Regul.OlibKey.Views;

namespace Regul.OlibKey
{
    public class OlibKeyModule : IModule
    {
        public void Execute()
        {
            OlibKeySettings.Load();
            
            Application.Current?.Styles.Add(new StyleInclude(new Uri("resm:Style?assembly=Regul"))
            {
                Source = new Uri("avares://Regul.OlibKey/Styles.axaml")
            });
            
            ModuleManager.Editors.Add(
                new Editor
                {
                    Id = OlibKeyId,
                    Name = "OlibKey",
                    CreatingAnEditor = () => new PasswordManagerView(),
                    Icon = App.GetResource<PathGeometry>("KeyIcon"),
                    DialogFilters = new List<FileDialogFilter>
                    {
                        new FileDialogFilter {Name = $"Olib-{App.GetResource<string>("Files")}", Extensions = {"olib"}}
                    }
                });
            
            App.ActionsWhenCompleting.Add(Release);
        }

        private static void Release()
        {
            OlibKeySettings.Save();
        }

        private const string OlibKeyId = "Onebeld_Editor_OlibKey_8F3a64jU57D";

        public IImage Icon { get; set; } = new Bitmap(AvaloniaLocator.Current.GetService<IAssetLoader>()
            .Open(new Uri("avares://Regul.OlibKey/icon.png")));
        public string Name { get; } = "OlibKey";
        public string Creator { get; } = "Onebeld";
        public string Description { get; } = "Password manager";
        public string Version { get; } = "1.0.0.0";
        public bool CorrectlyInitialized { get; set; }
        public bool ThereIsAnUpdate { get; set; }

        public Language[] Languages { get; } =
        {
            new Language("English", "en"),
            new Language("Russian", "ru")
        };
        public IStyle LanguageStyle { get; set; }
        public string PathToLocalization { get; } = "Regul.OlibKey/Localization/";

        public string LinkForCheckUpdates { get; } =
            "https://raw.githubusercontent.com/Onebeld/Regul.OlibKey/main/module.txt";
    }
}