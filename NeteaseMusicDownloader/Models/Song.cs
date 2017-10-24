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
        private int _downProgress = 0;
        private double _playProgress = 0;
        private PlayStatus _playStatus = PlayStatus.Play;
        private Music _musicNew;

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

        public Title Title { get; set; }

        public Artists Artists { get; set; }

        public Album Album { get; set; }

        public string Extension
        {
            get
            {
                if (MusicNew != null)
                {
                    return MusicNew.Extension;
                }
                else if (HMusic != null)
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

        public string BitRate
        {
            get
            {
                if (MusicNew != null)
                {
                    return MusicNew.BitRate / 1000 + "k";
                }
                else if (HMusic != null)
                {
                    return HMusic.BitRate / 1000 + "k";
                }
                else if (MMusic != null)
                {
                    return MMusic.BitRate / 1000 + "k";
                }
                else if (LMusic != null)
                {
                    return LMusic.BitRate / 1000 + "k";
                }
                else
                {
                    return "0k";
                }
            }
        }

        public string SongUrl
        {
            get
            {
                return MusicNew.Url;
            }
        }

        public Music MusicNew
        {
            get
            {
                if (_musicNew == null)
                {
                    _musicNew = NeteaseUtil.GetTrackDetialNew(MusicId);
                }
                return _musicNew;
            }
        }

        public Music HMusic { get; set; }

        public Music MMusic { get; set; }

        public Music LMusic { get; set; }

        public int DownProgress
        {
            get { return _downProgress; }
            set
            {
                _downProgress = value;
                RaisePropertyChanged("DownProgress");
            }
        }

        public double PlayProgress
        {
            get { return _playProgress; }
            set
            {
                _playProgress = value;
                RaisePropertyChanged("PlayProgress");
            }
        }

        public string SongFileName
        {
            get { return string.Format("{0} - {1}.{2}", Artists, Title, Extension); }
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