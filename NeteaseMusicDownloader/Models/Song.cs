using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

        public Music HMucis { get; set; }

        public Music MMucis { get; set; }

        public Music LMucis { get; set; }
    }
}