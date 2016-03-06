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
    public class Song
    {
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
                if (HMucis != null)
                {
                    return HMucis.Extension;
                }
                else if (MMucis != null)
                {
                    return MMucis.Extension;
                }
                else if (LMucis != null)
                {
                    return LMucis.Extension;
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
                if (HMucis != null)
                {
                    return HMucis.dfsId;
                }
                else if (MMucis != null)
                {
                    return MMucis.dfsId;
                }
                else if (LMucis != null)
                {
                    return LMucis.dfsId;
                }
                else
                {
                    return "";
                }
            }
        }

        public string AlbumImage { get; set; }

        public Music HMucis { get; set; }

        public Music MMucis { get; set; }

        public Music LMucis { get; set; }

        public string SongFileName
        {
            get { return string.Format("{0} - {1}.{2}", Artist, Title, Extension); }
        }

        public RelayCommand PlaylistDownloadCommand
        {
            get;
            set;
        }
    }
}