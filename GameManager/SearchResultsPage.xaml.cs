using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Xml;
using System.IO;
using GameManager.ViewModels;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.ComponentModel;

namespace GameManager
{
    public partial class SearchResultsPage : PhoneApplicationPage
    {

        public static HttpClient client = null;
        public Storyboard board = null;
        public Canvas canvas = null;
        public TextBlock blk_GameTitle = null;
        public List<TextBlock> gameTitles = App.ViewModel.GameTitles_SearchResults;
        public Dictionary<TextBlock, Storyboard> boards = App.ViewModel.Boards_SearchResults;
        public HttpResponseMessage response = null;
        public BackgroundWorker worker = new BackgroundWorker();

        public SearchResultsPage()
        {

            InitializeComponent();

            CreateHttpClient_Singleton();

            DataContext = App.ViewModel;

            pickBackgroundColor();
        }

        public void pickBackgroundColor()
        {

            Visibility v = (Visibility)Resources["PhoneLightThemeVisibility"];

            if (v == System.Windows.Visibility.Visible)
                LayoutRoot.Background = new SolidColorBrush(Colors.White);

            else
                LayoutRoot.Background = new SolidColorBrush(Colors.Black);
        }

        private void CreateHttpClient_Singleton()
        {

            client = App.ViewModel.Client_Global as HttpClient;

            if (client == null)
                client = new HttpClient();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            base.OnNavigatedTo(e);

            //StartMarquee();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {

            gameTitles.Clear();
            boards.Clear();

            if (NavigationService.Source.ToString() == "/AddNewGameManually.xaml")
            {

                AddNewGameManually.FromSearchPage = true;
            }

            App.ViewModel.SearchView.GamesList.Clear();
            client.CancelPendingRequests();

            base.OnNavigatedFrom(e);
        }

        private void storyboard_Completed(object sender, EventArgs e)
        {

            CompleteAnim();

        }

        private bool CheckIfTextOutOfBounds(TextBlock blk_text)
        {

            return blk_text.ActualWidth > ContentPanel.ActualWidth;
        }

        private void StartMarquee()
        {
            foreach (TextBlock gameTitle in gameTitles)
            {
                if (CheckIfTextOutOfBounds(gameTitle))
                {

                    DoAnim(gameTitle, canvas);
                }
            }
        }

        private void DoAnim(TextBlock blk_text, Canvas canvas)
        {

            Storyboard storyboard = new Storyboard();
            TranslateTransform transform = new TranslateTransform() { X = 5, Y = 1 };
            blk_text.RenderTransformOrigin = new Point(0.5, 0.5);
            blk_text.RenderTransform = transform;

            DoubleAnimation animation = new DoubleAnimation();
            double length = -(blk_text.ActualWidth) - ((canvas.ActualWidth - blk_text.ActualWidth) / 2);
            animation.From = 0;
            animation.To = length;
            animation.Duration = TimeSpan.FromSeconds(-length / blk_text.ActualWidth);

            Storyboard.SetTarget(animation, blk_text);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));

            storyboard.Completed += storyboard_Completed;
            storyboard.Children.Add(animation);
            storyboard.FillBehavior = FillBehavior.Stop;
            storyboard.RepeatBehavior = RepeatBehavior.Forever;
            storyboard.SpeedRatio = 0.2;

            boards.Add(blk_text, storyboard);

            storyboard.Begin();
        }

        private void CompleteAnim()
        {

        }

        private void StopAnim()
        {
            foreach (Storyboard sBoard in boards.Values)
            {

                if (sBoard != null)
                    sBoard.Stop();
            }
        }

        private void blk_GameTitle_Loaded_1(object sender, RoutedEventArgs e)
        {

            gameTitles.Add(sender as TextBlock);
        }

        private void Marquee_Loaded_1(object sender, RoutedEventArgs e)
        {

            canvas = sender as Canvas;
        }

        private void Marquee_Unloaded_1(object sender, RoutedEventArgs e)
        {

            StopAnim();
        }

        //Retrieve the selected item and send the data over to the AddNewGameManually page
        private void SearchList_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

            if (SearchList.SelectedItem == null)
                return;

            client.CancelPendingRequests();

            var data = SearchList.SelectedItem as GameData;
            ImageWebHandler.FetchImageResponse(data);

            AddNewGameManually.sent_GameData = new GameData(data);

            NavigationService.Navigate(new Uri("/AddNewGameManually.xaml", UriKind.RelativeOrAbsolute));

            SearchList.SelectedItem = null;
            data = null;
        }

        private void img_GameCover_Unloaded_1(object sender, RoutedEventArgs e)
        {

            var image = sender as Image;

            if (image != null)
            {

                var bmImage = image.Source as BitmapImage;

                if (bmImage != null && !bmImage.Equals(AddNewGameManually.sent_GameData.GameCoverCacheImage))
                {
                    if (AddNewGameManually.sent_GameData != null)
                    {

                        if (!bmImage.Equals(AddNewGameManually.sent_GameData.GameCoverCacheImage))
                        {

                            bmImage.UriSource = null;
                            image.Source = null;
                        }
                    }

                    else
                    {

                        bmImage.UriSource = null;
                        image.Source = null;
                    }
                }
            }
        }

        private void SearchList_ItemRealized_1(object sender, ItemRealizationEventArgs e)
        {

            //if item realized is #9 in the searchList then load the next 10 items in the list
        }

        private void SearchList_ItemUnrealized_1(object sender, ItemRealizationEventArgs e)
        {

        }

        private async void SearchList_Loaded_1(object sender, RoutedEventArgs e)
        {

            ImageWebHandler.FormSearchResultsList();
            await ImageWebHandler.FetchImageResponse(App.ViewModel.SearchView.GamesList);
        }

        private void SearchList_Unloaded_1(object sender, RoutedEventArgs e)
        {

            ImageWebHandler.queue.Clear();
            GC.Collect();
        }
    }
}