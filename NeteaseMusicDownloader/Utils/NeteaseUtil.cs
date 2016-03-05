using NeteaseMusicDownloader.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseMusicDownloader.Utils
{
    public class NeteaseUtil
    {
        public static readonly string baseUrl = "http://m1.music.126.net/{0}/{1}.mp3";

        public static string GetTrackURL(string dfsid)
        {
            if (string.IsNullOrWhiteSpace(dfsid))
            {
                return "";
            }
            var byte1 = Encoding.Default.GetBytes("3go8&$8*3*3h0k(2)2");
            var byte2 = Encoding.Default.GetBytes(dfsid);
            var byte1_len = byte1.Length;
            for (int i = 0; i < byte2.Length; i++)
            {
                byte2[i] = (byte)(byte2[i] ^ byte1[i % byte1_len]);
            }
            //$raw_code = base64_encode(md5($dfsid, true));
            //$code = str_replace(array('/', '+'), array('_', '-'), $raw_code);
            MD5 md5 = new MD5CryptoServiceProvider();
            var raw_code = Convert.ToBase64String(md5.ComputeHash(byte2));
            var result = raw_code.Replace("/", "_").Replace("+", "-");
            return string.Format(baseUrl, result, dfsid);
        }

        public static void GetSongDetail(ref Song song)
        {
            string url = "http://music.163.com/api/song/detail?ids=[{0}]";
            using (WebClient client = new WebClient() { Encoding = Encoding.UTF8 })
            {
                string json = client.DownloadString(string.Format(url, song.MusicId));
                JObject jObject = JObject.Parse(json);

                song.Title = jObject.SelectToken("songs[0].name").ToString();
                song.Artist = jObject.SelectToken("songs[0].artists[0].name").ToString();
                if (jObject.SelectToken("songs[0].hMusic") != null)
                    song.HMucis = new Music()
                    {
                        Name = jObject.SelectToken("songs[0].hMusic.name").ToString(),
                        Extension = jObject.SelectToken("songs[0].hMusic.extension").ToString(),
                        BitRate = jObject.SelectToken("songs[0].hMusic.bitrate").ToObject<int>(),
                        dfsId = jObject.SelectToken("songs[0].hMusic.dfsId").ToString(),
                    };
                if (jObject.SelectToken("songs[0].mMusic") != null)
                    song.MMucis = new Music()
                    {
                        Name = jObject.SelectToken("songs[0].mMusic.name").ToString(),
                        Extension = jObject.SelectToken("songs[0].mMusic.extension").ToString(),
                        BitRate = jObject.SelectToken("songs[0].mMusic.bitrate").ToObject<int>(),
                        dfsId = jObject.SelectToken("songs[0].mMusic.dfsId").ToString(),
                    };
                if (jObject.SelectToken("songs[0].lMusic") != null)
                    song.LMucis = new Music()
                    {
                        Name = jObject.SelectToken("songs[0].lMusic.name").ToString(),
                        Extension = jObject.SelectToken("songs[0].lMusic.extension").ToString(),
                        BitRate = jObject.SelectToken("songs[0].lMusic.bitrate").ToObject<int>(),
                        dfsId = jObject.SelectToken("songs[0].lMusic.dfsId").ToString(),
                    };
            }
        }
    }
}