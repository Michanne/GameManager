using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using GameManager.ViewModels;
using System.IO.IsolatedStorage;
using System.IO;
using GameManager.Resources;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows.Media.Imaging;
using System.Runtime.Serialization;
using Nokia.Graphics.Imaging;
using System.Diagnostics;

namespace GameManager
{
    public partial class AddNewGameManually : PhoneApplicationPage
    {

        public static GameData sent_GameData = new GameData();
        public static bool FromSearchPage = false;

        //Resources
        ApplicationBarIconButton SaveButton;

        public AddNewGameManually()
        {

            InitializeComponent();

            DataContext = App.ViewModel;

            pickBackgroundColor();

            BuildLocalizedApplicationBar();
        }

        private void RemoveEvents()
        {

           SaveButton.Click -= SaveGameToDatabase;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            if (FromSearchPage)
            {


                SetUIElements();
                NavigationService.RemoveBackEntry();
                FromSearchPage = false;
            }

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {

            if (NavigationService.Source != gamePlatform.PickerPageUri && gameCover != null)
            {

                gameCover.Source = null;
            }

            sent_GameData = null;

            //RemoveEvents();

            base.OnNavigatedFrom(e);
        }

        private void SetUIElements()
        {

            gameCover.Source = sent_GameData.GameCoverCacheImage;

            if (gameCover.Source == null)
            {

                ImageWebHandler.FetchImageResponse(sent_GameData);
                gameCover.Source = sent_GameData.GameCoverCacheImage;
            }

            gameID.Text = string.Format("{0}", sent_GameData.GameID);
            gameTitle.Text = sent_GameData.GameTitle;
            gamePlatform.SelectedItem = "Auto";
        }

        private void BuildLocalizedApplicationBar()
        {

            ApplicationBar = new ApplicationBar();

            SaveButton = new ApplicationBarIconButton(new Uri("", UriKind.RelativeOrAbsolute));

            SaveButton.Click += SaveGameToDatabase;
            SaveButton.Text = AppResources.AppBarSaveText;

            ApplicationBar.Buttons.Add(SaveButton);
            ApplicationBar.BackgroundColor = (Color)Resources["PhoneAccentColor"];
            ApplicationBar.ForegroundColor = Colors.White;
        }

        public void pickBackgroundColor()
        {

            Visibility v = (Visibility)Resources["PhoneLightThemeVisibility"];

            if (v == System.Windows.Visibility.Visible)
                LayoutRoot.Background = new SolidColorBrush(Colors.White);

            else
                LayoutRoot.Background = new SolidColorBrush(Colors.Black);
        }


        //Retrieve data from XAML elements, deserialize the database
        //Store the data into the dictionary and serialize the database

        //--still need to dispose the gamecover image--
        //--need to provide a way to save the images to a cache with size 99x99 and size 200 x 200--
        private async void SaveGameToDatabase(object sender, EventArgs e)
        {

            List<KeyValue<int, GameData>> Database;

            GameData data = null;

            if (sent_GameData != null)
            {

                data = sent_GameData;
            }

            else
            {

                Debug.WriteLine("Game was null");
                return;
            }

            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {

                if (!isoStore.DirectoryExists("/GameLibrary/"))
                    isoStore.CreateDirectory("/GameLibrary/");

                if (isoStore.FileExists("/GameLibrary/Database.xml"))
                {

                    Database = await IsolatedStorageHelper.Load<List<KeyValue<int, GameData>>>("/GameLibrary/Database.xml");
                }

                else
                {

                    Database = new List<KeyValue<int, GameData>>();
                }

                if (data.GameCoverCacheImage != null && data.GameCoverCacheImage.UriSource != null)
                {

                    SaveImageToPhone(data);
                }

                data.GameCover99Uri = "/GameImages/" + data.GameID + "-" + data.GameTitle + "-cover-image-99.jpg";
                data.GameCover200Uri = "/GameImages/" + data.GameID + "-" + data.GameTitle + "-cover-image-200.jpg";
                data.GameCover = null;

                Database.Add(new KeyValue<int,GameData>(data.GameID, data));

                await IsolatedStorageHelper.Save<List<KeyValue<int, GameData>>>(Database, "/GameLibrary/Database.xml");

                Database.Clear();
            }

            if (gameCover != null)
            {

                var bitmap = gameCover.Source as BitmapImage;

                if (bitmap != null)
                {

                    bitmap.UriSource = null;
                    gameCover.Source = null;
                }
            }

            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
        }

        //saves a 99x99 and 200x200 image to the phone
        private void SaveImageToPhone(GameData data)
        {

            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {

                if (!isoStore.DirectoryExists("/GameImages/"))
                    isoStore.CreateDirectory("/GameImages/");

                IsolatedStorageFileStream isoStream = isoStore.CreateFile("/GameImages/" + data.GameID + "-" + data.GameTitle + "-cover-image-99.jpg");

                BitmapImage image = data.GameCoverCacheImage;
                image.DecodePixelHeight = 99;
                image.DecodePixelWidth = 99;

                var saveImage = new WriteableBitmap(image);

                saveImage.SaveJpeg(isoStream, 99, 99, 0, 100);

                isoStream.Close();

                try
                {

                    isoStream = isoStore.CreateFile("/GameImages/" + data.GameID + "-" + data.GameTitle + "-cover-image-200.jpg");
                }
                catch (Exception)
                {

                    //for some reason this crashes when trying to add certain games....
                    //gotta figure that out...
                    //Metro: Last Light - PS3

                    return;
                }

                image = data.GameCoverCacheImage;
                image.DecodePixelHeight = 200;
                image.DecodePixelWidth = 200;

                saveImage = new WriteableBitmap(image);
                saveImage.SaveJpeg(isoStream, 200, 200, 0, 100);

                saveImage = null;
                image = null;
                data.GameCoverCacheImage = null;

                isoStream.Close();
                isoStream.Dispose();
            }
        }
    }
}