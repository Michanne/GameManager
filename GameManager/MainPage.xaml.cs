using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using GameManager.ViewModels;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.Collections.ObjectModel;
using GameManager.Resources;
using System.Xml.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GameManager
{
    public partial class GamesViewPage : PhoneApplicationPage
    {

        private BitmapImage listViewImage = new BitmapImage(new Uri("/Images/gotoslide.png", UriKind.RelativeOrAbsolute));
        private BitmapImage gridViewImage = new BitmapImage(new Uri("/Images/outline.squares.png", UriKind.RelativeOrAbsolute));
        LongListSelector ListView;
        Panorama GridView;

        public GamesViewPage()
        {
            InitializeComponent();

            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }

            ListView = new LongListSelector() { ItemsSource = App.ViewModel.MainGameView.GamesList, FontWeight = FontWeights.Thin };
            ListView.ItemTemplate = (DataTemplate)Resources["ListTemplate"];

            GridView = new Panorama() { ItemsSource = App.ViewModel.MainGameView.GamesList, FontWeight = FontWeights.Thin };
            GridView.ItemTemplate = (DataTemplate)Resources["GridTemplate"];
            GridView.HeaderTemplate = (DataTemplate)Resources["PanoramaHeaderTemplate"];

            BuildContentPanel();

            pickBackgroundColor();

            BuildLocalizedApplicationBar();
        }

        private void BuildLocalizedApplicationBar()
        {

            ApplicationBar = new ApplicationBar();

            ApplicationBarMenuItem SettingsOption = new ApplicationBarMenuItem(AppResources.Settings);

            SettingsOption.Click += (sender, e) =>
                {

                    NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.RelativeOrAbsolute));
                };

            ApplicationBar.MenuItems.Add(SettingsOption);
            ApplicationBar.BackgroundColor = (Color)Resources["PhoneAccentColor"];
            ApplicationBar.ForegroundColor = Colors.White;
        }

        private void BuildContentPanel()
        {

            if (App.ViewModel.MainPageListOrGridView)
            {

                if (Presenter.Content != ListView)
                {

                    
                    Presenter.Content = ListView;
                }
            }

            else
            {

                if (Presenter.Content != GridView)
                {

                    Presenter.Content = GridView;
                }   
            }
        }

        private void CreateLoggingFile()
        {

            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {

                if (!isoStore.FileExists("/ErrorLogging/log.txt"))
                    isoStore.CreateFile("/ErrorLogging/log.txt");

                //will log here in the future...
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            this.DataContext = App.ViewModel;

            CreateGameDatabase();

            while (NavigationService.BackStack.Any())
                NavigationService.RemoveBackEntry();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {

            base.OnNavigatedFrom(e);

            App.ViewModel.MainGameView.GamesList.Clear();
        }

        public void pickBackgroundColor()
        {

            Visibility v = (Visibility)Resources["PhoneLightThemeVisibility"];

            if (v == System.Windows.Visibility.Visible)
                LayoutRoot.Background = new SolidColorBrush(Colors.White);

            else
                LayoutRoot.Background = new SolidColorBrush(Colors.Black);
        }

        public void CreateGameDatabase()
        {

            Deployment.Current.Dispatcher.BeginInvoke(async () =>
                {
                    List<KeyValue<int, GameData>> Database = null;

                    using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                    {

                        if (!isoStore.DirectoryExists("/GameLibrary/"))
                            isoStore.CreateDirectory("/GameLibrary/");

                        if (isoStore.FileExists("/GameLibrary/Database.xml"))
                        {

                            //isoStore.DeleteFile("/GameLibrary/Database.xml");
                            Database = await IsolatedStorageHelper.Load<List<KeyValue<int, GameData>>>("/GameLibrary/Database.xml");
                        }
                    }

                    if (Database != null)
                    {

                        foreach (KeyValue<int, GameData> key in Database)
                        {

                            (DataContext as GameModel).MainGameView.GamesList.Add(key.Value);
                        }
                    }

                    Database = null;
                });
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.MainPageListOrGridView)
            {

                viewButton.Source = listViewImage;
                App.ViewModel.MainPageListOrGridView = true;
                BuildContentPanel();
            }

            else
            {

                viewButton.Source = gridViewImage;
                App.ViewModel.MainPageListOrGridView = false;
                BuildContentPanel();
            }
                
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

            NavigationService.Navigate(new Uri("/AddNewGame.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Image_Unloaded_1(object sender, RoutedEventArgs e)
        {
            if (NavigationService.Source != NavigationService.CurrentSource)
            {
                var image = sender as Image;

                if (image != null)
                {

                    var bm = image.Source as BitmapImage;

                    if (bm != null)
                    {

                        bm.UriSource = null;
                    }
                }
            }
        }

        private void Image_Loaded_1(object sender, RoutedEventArgs e)
        {

            var image = sender as Image;
            //image source is still null, even though the data was reloaded and game cover is not null anymore
        }
    }
}