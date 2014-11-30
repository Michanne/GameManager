using GameManager.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GameManager
{
    public static class IsolatedStorageHelper
    {

        public static async Task Save<T>(this List<KeyValue<int, GameData>> obj, string file)
        {

            await Task.Run(() =>
            {

                IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication();
                IsolatedStorageFileStream isoStream = null;

                try
                {

                    isoStream = isoFile.CreateFile(file);
                    XmlSerializer serializer = new XmlSerializer(typeof(List<KeyValue<int, GameData>>));
                    serializer.Serialize(isoStream, obj);
                }

                catch (Exception)
                {

                }

                finally
                {

                    if (isoStream != null)
                    {

                        isoStream.Close();
                        isoStream.Dispose();
                    }
                }
            }
            );
        }

        public static async Task<List<KeyValue<int, GameData>>> Load<T>(string file)
        {

            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
            List<KeyValue<int, GameData>> obj = (List<KeyValue<int, GameData>>)Activator.CreateInstance(typeof(List<KeyValue<int, GameData>>));

            if (isoStore.FileExists(file))
            {

                IsolatedStorageFileStream isoStream = null;

                try
                {

                    isoStream = isoStore.OpenFile(file, FileMode.Open);
                    XmlSerializer serializer = new XmlSerializer(typeof(List<KeyValue<int, GameData>>));
                    obj = (List<KeyValue<int, GameData>>)serializer.Deserialize(isoStream);
                }

                catch (Exception)
                {

                }

                finally
                {

                    if (isoStream != null)
                    {

                        isoStream.Close();
                        isoStream.Dispose();
                    }
                }

                return obj;
            }

            await Save<List<KeyValue<int, GameData>>>(obj, file);
            return obj;
        }
    }

    [DataContract]
    [XmlType(TypeName = "KeyValuePair")]
    public struct KeyValue<K, V>
    {

        public K Key
        { get; set; }

        public V Value
        { get; set; }

        public KeyValue(K key, V value)
            : this()
        {

            Key = key;
            Value = value;
        }
    }
}
