using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using IEPP.Models;
using System.IO;
using System.Collections.ObjectModel;
using CefSharp.DevTools.Debugger;

namespace IEPP.Utils
{
    public class JsonHelper
    {
        private string UserPath { get; set; }

        /// <summary>
        /// Number of items to skip from the start of the JSON. Defaults to 0.
        /// </summary>
        public int StartRange { get; set; }

        /// <summary>
        /// Number of items to read. Defaults to 50.
        /// </summary>
        public int ItemsNumber { get; set; }

        private bool noMore;
        public bool NoMoreHistoryItems
        {
            get => noMore;
            set
            {
                noMore = value;
                if (noMore)
                    Console.WriteLine("no more");
            }
        }

        public List<Bookmark> ReadAllBookmarks()
        {
            string filePath = UserPath + "/bookmarks.json";

            if (File.Exists(filePath))
                using (StreamReader file = File.OpenText(filePath))
                {
                    JsonSerializer ser = new JsonSerializer();
                    //Bookmarks = (ObservableCollection<BookmarkContainer>)ser.Deserialize(file, typeof(ObservableCollection<BookmarkContainer>));
                    return (List<Bookmark>)ser.Deserialize(file, typeof(List<Bookmark>));
                }

            return null;
        }

        public List<HistoryItem> ReadAllHistory()
        {
            string filePath = UserPath + "/history.json";

            if (File.Exists(filePath))
                using (StreamReader file = File.OpenText(filePath))
                {
                    JsonSerializer ser = new JsonSerializer();
                    return (List<HistoryItem>)ser.Deserialize(file, typeof(List<HistoryItem>));
                }

            return null;
        }

        public List<HistoryItem> ReadPartialHistory()
        {
            if (NoMoreHistoryItems)
                return null;

            string filePath = UserPath + "/history.json";
            List<HistoryItem> partialHistory = new List<HistoryItem>();

            using (JsonTextReader jsonReader = new JsonTextReader(new StreamReader(filePath)))
            {
                jsonReader.Read();
                if (jsonReader.TokenType == JsonToken.StartArray)
                {
                    // read and discard items until reaching the StartRange
                    int itemCount = 0;
                    while (jsonReader.Read() && jsonReader.TokenType != JsonToken.EndArray && itemCount < StartRange)
                    {
                        if (jsonReader.TokenType == JsonToken.StartObject)
                        {
                            ++itemCount;
                        }
                    }

                    itemCount = 0;
                    while (jsonReader.Read() && jsonReader.TokenType != JsonToken.EndArray && itemCount <= ItemsNumber)
                    {
                        if (jsonReader.TokenType == JsonToken.StartObject)
                        {
                            HistoryItem item = JsonSerializer.CreateDefault().Deserialize<HistoryItem>(jsonReader);
                            partialHistory.Add(item);
                            ++itemCount;
                        }
                    }                    
                }
            }

            if (partialHistory.Count < ItemsNumber)
                NoMoreHistoryItems = true;

            return partialHistory;
        }

        public void Save<T>(ObservableCollection<T> list, string type)
        {
            string filePath = UserPath + "/" + type + ".json";
            using (StreamWriter file = File.CreateText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, list);
            }
        }

        public void Save<T>(List<T> list, string type)
        {
            string filePath = UserPath + "/" + type + ".json";
            using (StreamWriter file = File.CreateText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, list);
            }
        }

        public JsonHelper(string userPath)
        {
            StartRange = 0;
            ItemsNumber = 50;
            UserPath = userPath;
            NoMoreHistoryItems = false;
        }
    }       
}
