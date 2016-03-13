using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeteaseMusicDownloader.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace NeteaseMusicDownloader.Models
{
    public class Song : ObservableObject
    {
        private int _progress = 0;
        private PlayStatus _playStatus = PlayStatus.Play;

        public void parseUrl(string url)
        {
            //preg_match('#http://music.163.com/\#/song\?id=(\d+)#i', $url, $matches)

            var reg = new Regex(@"id=(\d*)").Match(url);
            if (reg.Success)
            {
                MusicId = reg.Groups[1].Value;
            }
        }

        public Song()
        {
        }

        public string MusicId { get; set; }

        public string Title { get; set; }

        public string Artist { get; set; }

        public string Extension
        {
            get
            {
                if (HMusic != null)
                {
                    return HMusic.Extension;
                }
                else if (MMusic != null)
                {
                    return MMusic.Extension;
                }
                else if (LMusic != null)
                {
                    return LMusic.Extension;
                }
                else
                {
                    return "mp3";
                }
            }
        }

        public string DfsId
        {
            get
            {
                if (HMusic != null)
                {
                    return HMusic.dfsId;
                }
                else if (MMusic != null)
                {
                    return MMusic.dfsId;
                }
                else if (LMusic != null)
                {
                    return LMusic.dfsId;
                }
                else
                {
                    return "";
                }
            }
        }

        public string AlbumImage { get; set; }

        public Music HMusic { get; set; }

        public Music MMusic { get; set; }

        public Music LMusic { get; set; }

        public int Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                RaisePropertyChanged("Progress");
            }
        }

        public string SongFileName
        {
            get { return string.Format("{0} - {1}.{2}", Artist, Title, Extension); }
        }

        public PlayStatus PlayStatus
        {
            get { return _playStatus; }
            set
            {
                _playStatus = value;
                RaisePropertyChanged("PlayStatus");
            }
        }
    }

    public enum PlayStatus { Play, Stop }
}