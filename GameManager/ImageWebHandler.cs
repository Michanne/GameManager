using GameManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml;

namespace GameManager
{
    public static class ImageWebHandler
    {

        public static Dictionary<int, HttpResponseMessage> imageURLcollection = App.ViewModel.CachedImageURLs_SearchResults;
        public static HttpClient client = App.ViewModel.Client_Global;
        public static string XMLData = null;
        public static BackgroundWorker worker = new BackgroundWorker();
        public static Queue<GameData> queue = new Queue<GameData>();

        public static void FormSearchResultsList()
        {

            //Using the XMLData, form a list of 10 game data on the page
            using (XmlReader reader = XmlReader.Create(new StringReader(XMLData)))
            {

                string title = String.Empty;
                int gameID = 0;
                string imageURL = String.Empty;
                string releaseDate = String.Empty;
                string console = String.Empty;
                string imageXML = String.Empty;

                for (int i = 0; i < 20; ++i)
                {

                    if (reader.ReadToFollowing("id"))
                    {

                        reader.Read();

                        try
                        {

                            gameID = int.Parse(reader.Value);
                        }

                        catch (Exception)
                        {

                            //do something here later
                            gameID = 0;
                            continue;
                        }
                    }

                    else
                    {

                        break;
                    }


                    reader.ReadToFollowing("GameTitle");
                    reader.Read();
                    title = reader.Value;

                    reader.ReadToFollowing("ReleaseDate");
                    reader.Read();
                    releaseDate = reader.Value;

                    reader.ReadToFollowing("Platform");
                    reader.Read();
                    console = reader.Value;

                    App.ViewModel.SearchView.GamesList.Add(new GameData()
                    {

                        GameTitle = title,
                        GameConsole = console,
                        GameReleaseDate = releaseDate,
                        GameCover = imageURL,
                        GameID = gameID
                    });

                    title = String.Empty;
                    gameID = 0;
                    imageURL = String.Empty;
                    releaseDate = String.Empty;
                    console = String.Empty;
                }
            }
        }

        public static async Task FetchImageResponse(ObservableCollection<GameData> gamelist)
        {

            HttpResponseMessage response = null;

            foreach (var game in gamelist)
            {

                queue.Enqueue(game);
            }

            while (queue.Count != 0 ) 
            {

                var game = queue.Dequeue();

                if (game.GameCoverCacheImage == null || game.GameCoverCacheImage.UriSource == null)
                {

                    if (imageURLcollection.ContainsKey(game.GameID)
                        && imageURLcollection[game.GameID] as HttpResponseMessage != null)
                    {

                        response = imageURLcollection[game.GameID];
                    }

                    else
                    {

                        try
                        {

                            response = await client.GetAsync("http://thegamesdb.net/api/GetArt.php?id=" + game.GameID, HttpCompletionOption.ResponseHeadersRead);
                        }

                        catch (TaskCanceledException e)
                        {

                            if (!e.CancellationToken.IsCancellationRequested)
                            {

                                //request timed out
                            }
                        }

                        if (imageURLcollection.ContainsKey(game.GameID))
                        {

                            imageURLcollection[game.GameID] = response;
                        }

                        else
                        {

                            imageURLcollection.Add(game.GameID, response);
                        }
                    }



                    if (response != null && response.IsSuccessStatusCode)
                    {

                        //Read through image page

                        string imageXML = await response.Content.ReadAsStringAsync();

                        using (XmlReader imageReader = XmlReader.Create(new StringReader(imageXML)))
                        {

                            
                            imageReader.ReadToFollowing("baseImgUrl");
                            imageReader.Read();
                            string imageURL = imageReader.Value;

                            imageReader.ReadToFollowing("boxart");
                            imageReader.MoveToFirstAttribute();
                            imageReader.ReadAttributeValue();

                            if (imageReader.Value == "front")
                            {

                                imageReader.Read();
                                imageURL += imageReader.Value;

                                var bitmap = new BitmapImage() { CreateOptions = BitmapCreateOptions.None };
                                bitmap.DecodePixelHeight = 99;
                                bitmap.DecodePixelWidth = 99;
                                bitmap.UriSource = new Uri(imageURL);

                                game.GameCoverCacheImage = bitmap;

                                bitmap = null;
                            }

                            else
                            {

                                if (imageReader.ReadToFollowing("boxart"))
                                {

                                    imageReader.MoveToFirstAttribute();
                                    imageReader.ReadAttributeValue();
                                    imageReader.Read();
                                    imageURL += imageReader.Value;

                                    var bitmap = new BitmapImage() { CreateOptions = BitmapCreateOptions.None };
                                    bitmap.DecodePixelHeight = 99;
                                    bitmap.DecodePixelWidth = 99;
                                    bitmap.UriSource = new Uri(imageURL);

                                    game.GameCoverCacheImage = bitmap;

                                    bitmap = null;
                                }

                                else
                                {

                                    imageURL = String.Empty;
                                }
                            }

                            imageURL = String.Empty;
                        }
                    }
                }
            }

            client.CancelPendingRequests();
        }

        public static async Task FetchImageResponse(GameData game)
        {

            HttpResponseMessage response = null;

            if (game.GameCoverCacheImage == null || game.GameCoverCacheImage.UriSource == null)
            {

                if (imageURLcollection.ContainsKey(game.GameID)
                    && imageURLcollection[game.GameID] as HttpResponseMessage != null)
                {

                    response = imageURLcollection[game.GameID];
                }

                else
                {

                    try
                    {

                        response = await client.GetAsync("http://thegamesdb.net/api/GetArt.php?id=" + game.GameID, HttpCompletionOption.ResponseHeadersRead);
                    }

                    catch (TaskCanceledException e)
                    {

                        if (!e.CancellationToken.IsCancellationRequested)
                        {

                            //request timed out
                        }
                    }

                    if (imageURLcollection.ContainsKey(game.GameID))
                    {

                        imageURLcollection[game.GameID] = response;
                    }

                    else
                    {

                        imageURLcollection.Add(game.GameID, response);
                    }
                }



                if (response != null && response.IsSuccessStatusCode)
                {

                    //Read through image page

                    string imageXML = await response.Content.ReadAsStringAsync();

                    using (XmlReader imageReader = XmlReader.Create(new StringReader(imageXML)))
                    {


                        imageReader.ReadToFollowing("baseImgUrl");
                        imageReader.Read();
                        string imageURL = imageReader.Value;

                        imageReader.ReadToFollowing("boxart");
                        imageReader.MoveToFirstAttribute();
                        imageReader.ReadAttributeValue();

                        if (imageReader.Value == "front")
                        {

                            imageReader.Read();
                            imageURL += imageReader.Value;

                            var bitmap = new BitmapImage() { CreateOptions = BitmapCreateOptions.None };
                            bitmap.DecodePixelHeight = 99;
                            bitmap.DecodePixelWidth = 99;
                            bitmap.UriSource = new Uri(imageURL);

                            game.GameCoverCacheImage = bitmap;

                            bitmap = null;
                        }

                        else
                        {

                            if (imageReader.ReadToFollowing("boxart"))
                            {

                                imageReader.MoveToFirstAttribute();
                                imageReader.ReadAttributeValue();
                                imageReader.Read();
                                imageURL += imageReader.Value;

                                var bitmap = new BitmapImage() { CreateOptions = BitmapCreateOptions.None };
                                bitmap.DecodePixelHeight = 99;
                                bitmap.DecodePixelWidth = 99;
                                bitmap.UriSource = new Uri(imageURL);

                                game.GameCoverCacheImage = bitmap;

                                bitmap = null;
                            }

                            else
                            {

                                imageURL = String.Empty;
                            }
                        }

                        imageURL = String.Empty;
                    }
                }
            }

            client.CancelPendingRequests();
        }
    }
}
