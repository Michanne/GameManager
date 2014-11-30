using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Net.Http;
using GameManager.Resources;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using GameManager.ViewModels;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Diagnostics;

namespace GameManager
{
    public partial class AddNewGame : PhoneApplicationPage
    {
        private OpticalReaderLib.OpticalReaderTask _opReadTask;
        private OpticalReaderLib.OpticalReaderResult _opReadResult;
        HttpClient client;
        private bool _opButtonContext;
        private bool addGameButtonContext;
        private Dictionary<string, HttpResponseMessage> cachedSearches = App.ViewModel.CachedSearches_SearchResults;
        SolidColorBrush whiteBrush = new SolidColorBrush(Colors.White);
        SolidColorBrush blackBrush = new SolidColorBrush(Colors.Black);

        ProgressIndicator indicator = new ProgressIndicator();

        //progress indicator enums
        enum ProgressStatus
        {

            Searching
        }

        //Event Resources
        ApplicationBarIconButton ScanButton;
        ApplicationBarIconButton AddGameButton;

        // Constructor
        public AddNewGame()
        {

            InitializeComponent();

            CreateOpticalReaderTask_Singleton();
            CreateHttpClient_Singleton();

            _opReadResult = null;

            _opReadTask.Completed += OpticalReaderTask_Completed;

            _opButtonContext = false;
            addGameButtonContext = false;

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;

            pickBackgroundColor();

            BuildLocalizedApplicationBar();
        }

        private void SetProgressIndicator(bool val, ProgressStatus status)
        {

            indicator.IsIndeterminate = val;
            indicator.IsVisible = val;
            indicator.Text = status.ToString();
            SystemTray.SetProgressIndicator(this, indicator);
        }

        public void pickBackgroundColor()
        {

            Visibility v = (Visibility)Resources["PhoneLightThemeVisibility"];

            if (v == System.Windows.Visibility.Visible)
                LayoutRoot.Background = whiteBrush;

            else
                LayoutRoot.Background = blackBrush;
        }

        private void CreateOpticalReaderTask_Singleton()
        {

            _opReadTask = App.ViewModel.OpticalReaderTask_MainPage as OpticalReaderLib.OpticalReaderTask;
        }

        private void CreateHttpClient_Singleton()
        {

            client = App.ViewModel.Client_Global as HttpClient;

            if (client == null)
                client = new HttpClient();
        }

        private void BuildLocalizedApplicationBar()
        {

            ApplicationBar = new ApplicationBar();

            ScanButton = new ApplicationBarIconButton(new Uri("/Images/map.centerme.png", UriKind.RelativeOrAbsolute));
            AddGameButton = new ApplicationBarIconButton(new Uri("/Images/edittext.png", UriKind.RelativeOrAbsolute));

            ScanButton.Click += AppBarButton_Click;
            AddGameButton.Click += AddGameButton_Click;

            ScanButton.Text = AppResources.AppBarButtonText;
            AddGameButton.Text = AppResources.AppBarAddGameText;
            ApplicationBar.Buttons.Add(ScanButton);
            ApplicationBar.Buttons.Add(AddGameButton);

            ApplicationBar.BackgroundColor = (Color)Resources["PhoneAccentColor"];
            ApplicationBar.ForegroundColor = Colors.White;
        }

        // Load data for the ViewModel Items
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {

            if (!App.ViewModel.IsDataLoaded)
            {

                App.ViewModel.LoadData();
            }

            base.OnNavigatedTo(e);

            if (_opReadResult != null && _opReadResult.TaskResult == Microsoft.Phone.Tasks.TaskResult.OK)
            {

                SetProgressIndicator(true, ProgressStatus.Searching);
                await FetchHTMLdata();
                SetProgressIndicator(false, ProgressStatus.Searching);
            }

            _opReadResult = null;
            _opButtonContext = false;
            addGameButtonContext = false;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {

            base.OnNavigatedFrom(e);

            client.CancelPendingRequests();

            //RemoveEvents();

            if (cachedSearches.Count > 10)
            {

                cachedSearches.Clear();
            }
        }

        private string CutHTML(string HTMLdata)
        {
            //This should remove all irrelevant data including HTML from the upcdatabase source text

            string returnvalue = null;

            //The following regex will remove everything in between an opening and closing chevron
            //and replace it with a space. <td></td> tags however will be replaced by line 
            //dividers, "|". The regex then searches for the game name using the Description keyword
            //and saves it to a string

            if (HTMLdata != null || HTMLdata != "null")
            {

                returnvalue = Regex.Replace(HTMLdata, "</?td>", "^", RegexOptions.IgnoreCase);
                returnvalue = Regex.Replace(returnvalue, "<[^>]*>", "", RegexOptions.IgnoreCase);
                
                Match rvMatch = Regex.Match(returnvalue, "Description[^/<]*", RegexOptions.IgnoreCase);

                if (rvMatch.Success)
                {

                    returnvalue = rvMatch.Value;
                    Match name = Regex.Match(returnvalue, @"(?<=Description\^\^\^\^)([^\^]*)(?=\^)", RegexOptions.IgnoreCase);

                    if (name.Success)
                    {
                        returnvalue = name.Value;
                    }
                }
  
                else
                    returnvalue = "null";
            }

            else
                returnvalue = "null";

            return returnvalue;
        }

        private async Task FetchHTMLdata()
        {

            /*When barcode is scanned and valid, perform a lookup using upcdatabase.com/item/*
                 Find the name of the game using the Description and perform another search using
                 the gamesdb Api and parse the XML. Provide the list of games to the user and allow
                 them to choose the game that best seems like their copy*/

            string siteAddress = "http://www.upcdatabase.com/item/" + _opReadResult.Text;
            string gameName = null;
            string HTMLData = null;
            HttpResponseMessage response = null;

            //We use GetAsync() so that we can cancel the operation if the user navigates away from the page early
            try
            {
                response = await client.GetAsync(siteAddress);
            }

            catch(TaskCanceledException e)
            {

                if (!e.CancellationToken.IsCancellationRequested)
                { 
                    //Request timed out
                    return;
                }
            }

            if (response.IsSuccessStatusCode)
            {

                HTMLData = await response.Content.ReadAsStringAsync();

                if (HTMLData == null)
                {

                    //Do stuff here
                    return;
                }

                gameName = CutHTML(HTMLData);

                await FetchGameInfo(gameName);
            }
        }

        private async Task FetchGameInfo(string gameName)
        {

            string gameAddressFromDB = null;
            HttpResponseMessage response = null;

            gameAddressFromDB = "http://thegamesdb.net/api/GetGamesList.php?name=" + gameName;

            if (cachedSearches.ContainsKey(gameAddressFromDB))
            {

                if ((cachedSearches[gameAddressFromDB] as HttpResponseMessage) != null)
                {

                    response = cachedSearches[gameAddressFromDB];
                }

                else
                {

                    try
                    {

                        response = await client.GetAsync(gameAddressFromDB);
                        if (response == null)
                        {

                            //filler to wait for response message to complete
                        }
                    }

                    catch (TaskCanceledException e)
                    {

                        if (!e.CancellationToken.IsCancellationRequested)
                        {
                            //Request timed out
                            //return;
                            Console.Write("Request timed out \t");
                        }
                    }

                    cachedSearches[gameAddressFromDB] = response;
                }
            }

            else
            {

                try
                {

                    response = await client.GetAsync(gameAddressFromDB);
                    if (response == null)
                    {

                        //filler to wait for response message to complete
                    }
                }

                catch (TaskCanceledException e)
                {

                    if (!e.CancellationToken.IsCancellationRequested)
                    {
                        //Request timed out
                        //return;
                        Console.Write("Request timed out \t");
                    }
                }

                cachedSearches.Add(gameAddressFromDB, response);
            }

            if (response != null && response.IsSuccessStatusCode)
            {

                //Send the XMLData over to the Search Results page to form the page data
                ImageWebHandler.XMLData = await response.Content.ReadAsStringAsync();


                SetProgressIndicator(false, ProgressStatus.Searching);
                NavigationService.Navigate(new Uri("/SearchResultsPage.xaml", UriKind.RelativeOrAbsolute));
            }

            else
            {

                SetProgressIndicator(false, ProgressStatus.Searching);
                Debug.WriteLine("Response was null");
            }
        }

        private void OpticalReaderTask_Completed(object sender, OpticalReaderLib.OpticalReaderResult e)
        {

            _opReadResult = e;

        }

        private void box_gameSearch_GotFocus(object sender, RoutedEventArgs e)
        {

            if (box_gameSearch.Text == "Enter game title here...")
                box_gameSearch.Text = "";
        }

        private void box_gameSearch_LostFocus(object sender, RoutedEventArgs e)
        {

            if (box_gameSearch.Text == "")
                box_gameSearch.Text = "Enter game title here...";
        }

        private void btn_gameSearch_Click(object sender, RoutedEventArgs e)
        {

            SetProgressIndicator(true, ProgressStatus.Searching);
            FetchGameInfo(box_gameSearch.Text);
        }

        void AppBarButton_Click(object sender, EventArgs e)
        {

            if (!_opButtonContext)
            {

                _opButtonContext = true;
                _opReadTask.Show();
            }
        }

        void AddGameButton_Click(object sender, EventArgs e)
        {

            if (!addGameButtonContext)
            {

                addGameButtonContext = true;
                NavigationService.Navigate(new Uri("/AddNewGameManually.xaml", UriKind.RelativeOrAbsolute));
            }
        }
    }
}