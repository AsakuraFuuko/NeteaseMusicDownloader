using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseMusicDownloader.Utils
{
    public class DownloadUtils
    {
        public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;

        public event EventHandler<AsyncCompletedEventArgs> DownloadFileCompleted;

        // Download a file
        public async void Get(string url, string fileName)
        {
            Uri uri = new Uri(url);
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadProgressChanged += (sender, args) => OnProgressChanged(args);
                webClient.DownloadFileCompleted += (sender, args) => OnAsyncCompleted(args);
                await webClient.DownloadFileTaskAsync(uri, fileName);
            }
        }

        // Notify when progress changes
        private void OnProgressChanged(DownloadProgressChangedEventArgs e)
        {
            var handler = DownloadProgressChanged;
            if (handler != null)
                DownloadProgressChanged(this, e);
        }

        private void OnAsyncCompleted(AsyncCompletedEventArgs e)
        {
            var handler = DownloadFileCompleted;
            if (handler != null)
                DownloadFileCompleted(this, e);
        }
    }
}