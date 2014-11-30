using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace GameManager.ViewModels
{
    public class GameModel
    {

        public GameView MainGameView { get; set; }
        public GameView SearchView { get; set; }
        public GameView AddGameManuallyView { get; set; }

        public bool IsDataLoaded { get; set; }

        public OpticalReaderLib.OpticalReaderTask OpticalReaderTask_MainPage { get; set; }
        public HttpClient Client_Global { get; set; }

        public List<String> GamePlatforms { get; set; }

        public Dictionary<String, HttpResponseMessage> CachedSearches_SearchResults { get; set; }
        public List<String> CachedSearchResults { get; set; }
        public Dictionary<int, HttpResponseMessage> CachedImageURLs_SearchResults { get; set; }
        public Dictionary<TextBlock, Storyboard> Boards_SearchResults { get; set; }
        public List<TextBlock> GameTitles_SearchResults { get; set; }

        public bool MainPageListOrGridView { get; set; }

        public void LoadData()
        {
            MainGameView = new GameView();
            SearchView = new GameView();
            AddGameManuallyView = new GameView();

            OpticalReaderTask_MainPage = new OpticalReaderLib.OpticalReaderTask();

            Client_Global = new HttpClient();
            Client_Global.Timeout = TimeSpan.FromSeconds(1D);

            CachedSearches_SearchResults = new Dictionary<string, HttpResponseMessage>();
            CachedSearchResults = CachedSearches_SearchResults.Keys.ToList();
            CachedImageURLs_SearchResults = new Dictionary<int, HttpResponseMessage>();
            Boards_SearchResults = new Dictionary<TextBlock, Storyboard>();
            GameTitles_SearchResults = new List<TextBlock>();

            MainPageListOrGridView = true;

            MultiAdd<string>
            (
                
                AddGameManuallyView.StringList,
                "Sony Playstation 4",
                "Sony Playstation 3",
                "Sony Playstation 2",
                "Sony Playstation 1",
                "Sony PSP",
                "Sony Playstation Vita",
                "Nintendo Wii",
                "Nintendo Wii U",
                "Nintendo GameCube",
                "Nintendo 64",
                "Nintendo Game Boy Advance",
                "Nintendo Game Boy Color",
                "Nintendo Game Boy",
                "Nintendo Super Game Boy",
                "Xbox",
                "Xbox 360",
                "Xbox One",
                "PC",
                "Android",
                "iOS",
                "Nintendo 3DS",
                "Nintendo DS",
                "Auto"
            );

            IsDataLoaded = true;
        }

        public void MultiAdd<T>(ICollection<T> collection, params T[] strings)
        { 
        
            foreach(T thing in strings)
            {

                collection.Add(thing);
            }
        }
    }
}
