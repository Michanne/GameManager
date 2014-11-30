using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GameManager.ViewModels
{
    public class GameView : INotifyPropertyChanged
    {
        public GameView()
        {

            GamesList = new ObservableCollection<GameData>();
            StringList = new ObservableCollection<string>();
            ViewMode = false;
        }

        private ObservableCollection<GameData> gameDataList = new ObservableCollection<GameData>();
        private ObservableCollection<string> stringList = new ObservableCollection<string>();
        public bool ViewMode { get; set; }  //true for Grid view, false for List view

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {

            if (PropertyChanged != null)
            {

                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ObservableCollection<GameData> GamesList
        {

            get
            {

                return this.gameDataList;
            }

            set
            {

                if (value != this.gameDataList)
                {

                    this.gameDataList = value;
                    NotifyPropertyChanged("GamesList");
                }
            }
        }

        public ObservableCollection<string> StringList
        {

            get
            {

                return this.stringList;
            }

            set
            {

                if (value != this.stringList)
                {

                    this.stringList = value;
                    NotifyPropertyChanged("StringList");
                }
            }
        }
    }
}
