using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using GameManager.ViewModels;
using System.IO.IsolatedStorage;

namespace GameManager
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            clearCacheButton.IsEnabled = true;
        }

        private async void Button_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {

            List<KeyValue<int, GameData>> Database = new List<KeyValue<int, GameData>>();

            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {

                await IsolatedStorageHelper.Save<List<KeyValue<int, GameData>>>(Database, "/GameLibrary/Database.xml");

                App.ViewModel.MainGameView.GamesList.Clear();

                if (isoStore.DirectoryExists("/GameImages/"))
                {

                    foreach (var file in isoStore.GetFileNames("/GameImages/"))
                    {

                        if (isoStore.FileExists("/GameImages/" + file))
                            isoStore.DeleteFile("/GameImages/" + file);
                    }

                    isoStore.DeleteDirectory("/GameImages/");
                }
            }

            clearCacheButton.IsEnabled = false;
        }
    }
}