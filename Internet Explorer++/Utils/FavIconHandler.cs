using IEPP.ViewModels;
using Nager.PublicSuffix;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace IEPP.Utils
{
    public class FavIconHandler : INotifyPropertyChanged
    {
        #region PropertyChanged declaration
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private string cacheDirPath;
        private string domainName;
        private string imagePath;

        private object _favIcon;

        /// <summary>
        /// For binding to System.Windows.Window.Icon.
        /// </summary>
        public object FavIcon
        {
            get { return _favIcon; }
            set { _favIcon = value; NotifyPropertyChanged("FavIcon"); }
        }

        private BitmapDecoder _decoder;
        private BitmapDecoder Decoder
        {
            get => _decoder;
            set
            {
                if (_decoder != null) _decoder.DownloadCompleted -= decoderDownloadCompleted;
                _decoder = value;
                if (_decoder != null) _decoder.DownloadCompleted += decoderDownloadCompleted;
            }
        }

        private void decoderDownloadCompleted(object sender, EventArgs e)
        {
            FavIcon = Decoder.Frames.OrderBy(f => f.Width).FirstOrDefault();

            SaveFavIconToCache(domainName + ".png");
            Decoder = null;
        }

        private void GetDomainName(string url)
        {
            domainName = new Uri(url).GetLeftPart(UriPartial.Authority)
                .Replace("/www.", "/")
                .Replace("http://", "")
                .Replace("https://", "")
                .ToString();
        }

        private void SaveFavIconToCache(string filename)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(Decoder.Frames.OrderBy(f => f.Width).FirstOrDefault());

            imagePath = cacheDirPath + '/' + filename;

            if (!File.Exists(imagePath))
                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
        }
        public void DownloadFavIcon(Uri url)
        {
            string favIconUrl = "http://www.google.com/s2/favicons?domain=";
            var baseUrl = url.GetLeftPart(UriPartial.Authority);

            Decoder = BitmapDecoder.Create(new Uri(favIconUrl + baseUrl + "&sz=32"), BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.None);
        }

        public void GetFavIcon(string url)
        {
            GetDomainName(url);

            var favIconPath = cacheDirPath + '/' + domainName + ".png";

            if (File.Exists(favIconPath))
                FavIcon = new BitmapImage(new Uri(favIconPath));
            else
                DownloadFavIcon(new Uri(url));
        }

        public void GetFavIcon(string url, string domain)
        {
            var favIconPath = cacheDirPath + '/' + domain + ".png";
            domainName = domain;

            if (File.Exists(favIconPath))
                FavIcon = new BitmapImage(new Uri(favIconPath));
            else
                DownloadFavIcon(new Uri(url));
        }

        public FavIconHandler()
        {
            App.Current.Dispatcher.Invoke(() => cacheDirPath = (App.Current.MainWindow.DataContext as MainVM).CacheDir);
        }
    }
}
