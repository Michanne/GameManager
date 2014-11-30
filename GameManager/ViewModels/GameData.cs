using Nokia.Graphics.Imaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace GameManager.ViewModels
{
    public class GameData : INotifyPropertyChanged
    {

        public GameData()
        { 
        }

        public GameData(GameData data)
        {

            this.GameConsole = data.GameConsole;
            this.GameCover = data.GameCover;
            this.GameCover200Uri = data.GameCover200Uri;
            this.GameCover99Uri = data.GameCover99Uri;
            this.GameCoverCacheImage = data.GameCoverCacheImage;
            this.GameID = data.GameID;
            this.GameReleaseDate = data.GameReleaseDate;
            this.GameSerial = data.GameSerial;
            this.GameSummary = data.GameSummary;
            this.GameTitle = data.GameTitle;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {

            if (PropertyChanged != null)
            {

                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string gameTitle { get; set; }
        private int gameUPC { get; set; }
        private string gameSerial { get; set; }
        private string gameReleaseDate { get; set; }
        private string gameConsole { get; set; }
        private string gameSummary { get; set; }
        private string gameCover { get; set; }
        private string gameCover99Uri { get; set; }
        private string gameCover200Uri { get; set; }
        private BitmapImage gameCoverCacheImage { get; set; }
        private int gameID { get; set; }

        public string GameTitle
        {

            get
            {

                return gameTitle;
            }

            set
            {

                if (value != gameTitle)
                {

                    gameTitle = value;
                    NotifyPropertyChanged("GameTitle");
                }
            }
        }

        public string GameSerial
        {

            get
            {

                return gameSerial;
            }

            set
            {

                if (value != gameSerial)
                {

                    gameSerial = value;
                    NotifyPropertyChanged("GameTitle");
                }
            }
        }

        public string GameReleaseDate
        {

            get
            {

                return gameReleaseDate;
            }

            set
            {

                if (value != gameReleaseDate)
                {

                    gameReleaseDate = value;
                    NotifyPropertyChanged("GameTitle");
                }
            }
        }

        public string GameConsole
        {

            get
            {

                return gameConsole;
            }

            set
            {

                if (value != gameConsole)
                {

                    gameConsole = value;
                    NotifyPropertyChanged("GameTitle");
                }
            }
        }

        public string GameSummary
        {

            get
            {

                return gameSummary;
            }

            set
            {

                if (value != gameSummary)
                {

                    gameSummary = value;
                    NotifyPropertyChanged("GameTitle");
                }
            }
        }

        public string GameCover
        {

            get
            {

                return gameCover;
            }

            set
            {

                if (value != gameCover)
                {

                    gameCover = value;
                    NotifyPropertyChanged("GameTitle");
                }
            }
        }

        public string GameCover99Uri
        {

            get
            {

                return gameCover99Uri;
            }

            set
            {

                if (value != gameCover99Uri)
                {

                    gameCover99Uri = value;
                    NotifyPropertyChanged("GameTitle");
                }
            }
        }

        public string GameCover200Uri
        {

            get
            {

                return gameCover200Uri;
            }

            set
            {

                if (value != gameCover200Uri)
                {

                    gameCover200Uri = value;
                    NotifyPropertyChanged("GameTitle");
                }
            }
        }

        public int GameID
        {

            get
            {

                return gameID;
            }

            set
            {

                if (value != gameID)
                {

                    gameID = value;
                    NotifyPropertyChanged("GameTitle");
                }
            }
        }

        public BitmapImage GameCover99Image
        {

            get
            {

                BitmapImage image = new BitmapImage() { CreateOptions = BitmapCreateOptions.None };

                using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                {

                    using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(GameCover99Uri, FileMode.Open, FileAccess.Read, FileShare.Read, isoStore))
                    {

                        image.SetSource(isoStream);
                    }
                }

                return image;
            }
        }

        public BitmapImage GameCover200Image
        {

            get
            {

                BitmapImage image = new BitmapImage() { CreateOptions = BitmapCreateOptions.None };

                using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                {

                    using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(GameCover200Uri, FileMode.Open, FileAccess.Read, FileShare.Read, isoStore))
                    {

                        image.SetSource(isoStream);
                    }
                }

                return image;
            }
        }

        [XmlIgnore]
        public BitmapImage GameCoverCacheImage
        {

            get
            {

                return gameCoverCacheImage;
            }

            set
            {

                if (value != gameCoverCacheImage)
                {

                    gameCoverCacheImage = value;
                    NotifyPropertyChanged("GameCoverCacheImage");
                }
            }
        }
	}
}
