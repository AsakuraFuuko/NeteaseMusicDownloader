using NeteaseMusicDownloader.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NeteaseMusicDownloader.Utils
{
    public class NeteaseUtil
    {
        #region old
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

        public async static Task<IEnumerable<Song>> GetSongDetail(string id)
        {
            var list = new List<Song>();
            string url = "http://music.163.com/api/song/detail?ids=[{0}]";
            using (WebClient client = new WebClient() { Encoding = Encoding.UTF8 })
            {
                string json = await client.DownloadStringTaskAsync(string.Format(url, id));
                JObject jObject = JObject.Parse(json);

                var song = new Song()
                {
                    Title = jObject.SelectToken("songs[0].name").ToString(),
                    Artist = jObject.SelectToken("songs[0].artists[0].name").ToString(),
                    MusicId = jObject.SelectToken("songs[0].id").ToString(),
                    AlbumImage = jObject.SelectToken("songs[0].album.picUrl").ToString(),
                };

                if (jObject.SelectToken("songs[0].hMusic") != null)
                    song.HMusic = new Music()
                    {
                        Name = jObject.SelectToken("songs[0].hMusic.name").ToString(),
                        Extension = jObject.SelectToken("songs[0].hMusic.extension").ToString(),
                        BitRate = jObject.SelectToken("songs[0].hMusic.bitrate").ToObject<int>(),
                        dfsId = jObject.SelectToken("songs[0].hMusic.dfsId").ToString(),
                    };
                if (jObject.SelectToken("songs[0].mMusic") != null)
                    song.MMusic = new Music()
                    {
                        Name = jObject.SelectToken("songs[0].mMusic.name").ToString(),
                        Extension = jObject.SelectToken("songs[0].mMusic.extension").ToString(),
                        BitRate = jObject.SelectToken("songs[0].mMusic.bitrate").ToObject<int>(),
                        dfsId = jObject.SelectToken("songs[0].mMusic.dfsId").ToString(),
                    };
                if (jObject.SelectToken("songs[0].lMusic") != null)
                    song.LMusic = new Music()
                    {
                        Name = jObject.SelectToken("songs[0].lMusic.name").ToString(),
                        Extension = jObject.SelectToken("songs[0].lMusic.extension").ToString(),
                        BitRate = jObject.SelectToken("songs[0].lMusic.bitrate").ToObject<int>(),
                        dfsId = jObject.SelectToken("songs[0].lMusic.dfsId").ToString(),
                    };
                list.Add(song);
            }
            return list;
        }

        public async static Task<IEnumerable<Song>> GetSongsFromPlaylist(string id)
        {
            var list = new List<Song>();
            string url = "http://music.163.com/api/playlist/detail?id={0}";
            using (WebClient client = new WebClient() { Encoding = Encoding.UTF8 })
            {
                string json = await client.DownloadStringTaskAsync(string.Format(url, id));
                JObject jObject = JObject.Parse(json);
                var result = jObject["result"];
                if (result != null)
                {
                    foreach (var item in result["tracks"])
                    {
                        var song = new Song()
                        {
                            Title = item.SelectToken("name").ToString(),
                            Artist = string.Join(" ", item.SelectToken("artists").Select(p => p.SelectToken("name")).Values<string>()),
                            MusicId = item.SelectToken("id").ToString(),
                            AlbumImage = item.SelectToken("album.picUrl").ToString(),
                        };
                        if (item.SelectToken("hMusic.name") != null)
                            song.HMusic = new Music()
                            {
                                Name = item.SelectToken("hMusic.name").ToString(),
                                Extension = item.SelectToken("hMusic.extension").ToString(),
                                BitRate = item.SelectToken("hMusic.bitrate").ToObject<int>(),
                                dfsId = item.SelectToken("hMusic.dfsId").ToString(),
                            };
                        if (item.SelectToken("mMusic.name") != null)
                            song.MMusic = new Music()
                            {
                                Name = item.SelectToken("mMusic.name").ToString(),
                                Extension = item.SelectToken("mMusic.extension").ToString(),
                                BitRate = item.SelectToken("mMusic.bitrate").ToObject<int>(),
                                dfsId = item.SelectToken("mMusic.dfsId").ToString(),
                            };
                        if (item.SelectToken("lMusic.name") != null)
                            song.LMusic = new Music()
                            {
                                Name = item.SelectToken("lMusic.name").ToString(),
                                Extension = item.SelectToken("lMusic.extension").ToString(),
                                BitRate = item.SelectToken("lMusic.bitrate").ToObject<int>(),
                                dfsId = item.SelectToken("lMusic.dfsId").ToString(),
                            };
                        list.Add(song);
                    }
                }
                return list;
            }
        }

        public async static Task<IEnumerable<Song>> GetSongsFromAlbum(string id)
        {
            var list = new List<Song>();
            string url = "http://music.163.com/api/album/{0}";
            using (WebClient client = new WebClient() { Encoding = Encoding.UTF8, Headers = new WebHeaderCollection() { { HttpRequestHeader.Referer, "http://music.163.com/" }, { HttpRequestHeader.Cookie, "appver=1.5.0.75771;" } } })
            {
                string json = await client.DownloadStringTaskAsync(string.Format(url, id));
                JObject jObject = JObject.Parse(json);
                var result = jObject["album"];
                if (result != null)
                {
                    foreach (var item in result["songs"])
                    {
                        var song = new Song()
                        {
                            Title = item.SelectToken("name").ToString(),
                            Artist = string.Join(" ", item.SelectToken("artists").Select(p => p.SelectToken("name")).Values<string>()),
                            MusicId = item.SelectToken("id").ToString(),
                            AlbumImage = item.SelectToken("album.picUrl").ToString(),
                        };
                        if (item.SelectToken("hMusic.name") != null)
                            song.HMusic = new Music()
                            {
                                Name = item.SelectToken("hMusic.name").ToString(),
                                Extension = item.SelectToken("hMusic.extension").ToString(),
                                BitRate = item.SelectToken("hMusic.bitrate").ToObject<int>(),
                                dfsId = item.SelectToken("hMusic.dfsId").ToString(),
                            };
                        if (item.SelectToken("mMusic.name") != null)
                            song.MMusic = new Music()
                            {
                                Name = item.SelectToken("mMusic.name").ToString(),
                                Extension = item.SelectToken("mMusic.extension").ToString(),
                                BitRate = item.SelectToken("mMusic.bitrate").ToObject<int>(),
                                dfsId = item.SelectToken("mMusic.dfsId").ToString(),
                            };
                        if (item.SelectToken("lMusic.name") != null)
                            song.LMusic = new Music()
                            {
                                Name = item.SelectToken("lMusic.name").ToString(),
                                Extension = item.SelectToken("lMusic.extension").ToString(),
                                BitRate = item.SelectToken("lMusic.bitrate").ToObject<int>(),
                                dfsId = item.SelectToken("lMusic.dfsId").ToString(),
                            };
                        list.Add(song);
                    }
                }
                return list;
            }
        }

        public async static Task<IEnumerable<Song>> GetSongsFromArtist(string id)
        {
            var list = new List<Song>();
            string url = "http://music.163.com/api/artist/{0}?&limit=9999";
            using (WebClient client = new WebClient() { Encoding = Encoding.UTF8, Headers = new WebHeaderCollection() { { HttpRequestHeader.Referer, "http://music.163.com/" }, { HttpRequestHeader.Cookie, "appver=1.5.0.75771;" } } })
            {
                string json = await client.DownloadStringTaskAsync(string.Format(url, id));
                JObject jObject = JObject.Parse(json);
                var result = jObject["hotSongs"];
                if (result != null)
                {
                    foreach (var item in jObject["hotSongs"])
                    {
                        var song = new Song()
                        {
                            Title = item.SelectToken("name").ToString(),
                            Artist = string.Join(" ", item.SelectToken("artists").Select(p => p.SelectToken("name")).Values<string>()),
                            MusicId = item.SelectToken("id").ToString(),
                            AlbumImage = item.SelectToken("album.picUrl").ToString(),
                        };
                        if (item.SelectToken("hMusic.name") != null)
                            song.HMusic = new Music()
                            {
                                Name = item.SelectToken("hMusic.name").ToString(),
                                Extension = item.SelectToken("hMusic.extension").ToString(),
                                BitRate = item.SelectToken("hMusic.bitrate").ToObject<int>(),
                                dfsId = item.SelectToken("hMusic.dfsId").ToString(),
                            };
                        if (item.SelectToken("mMusic.name") != null)
                            song.MMusic = new Music()
                            {
                                Name = item.SelectToken("mMusic.name").ToString(),
                                Extension = item.SelectToken("mMusic.extension").ToString(),
                                BitRate = item.SelectToken("mMusic.bitrate").ToObject<int>(),
                                dfsId = item.SelectToken("mMusic.dfsId").ToString(),
                            };
                        if (item.SelectToken("lMusic.name") != null)
                            song.LMusic = new Music()
                            {
                                Name = item.SelectToken("lMusic.name").ToString(),
                                Extension = item.SelectToken("lMusic.extension").ToString(),
                                BitRate = item.SelectToken("lMusic.bitrate").ToObject<int>(),
                                dfsId = item.SelectToken("lMusic.dfsId").ToString(),
                            };
                        list.Add(song);
                    }
                }
                return list;
            }
        }

        #endregion

        #region new 
        private static readonly string MODULES = "00e0b509f6259df8642dbc35662901477df22677ec152b5ff68ace615bb7b725152b3ab17a876aea8a5aa76d2e417629ec4ee341f56135fccf695280104e0312ecbda92557c93870114af6c9d05c4f7f0c3685b7a46bee255932575cce10b424d813cfe4875d3e82047b97ddef52741d546b8e289dc6935b3ece0462db0a22b8e7";
        private static readonly string NONCE = "0CoJUm6Qyw8W8jud";
        private static readonly string PUB_KEY = "010001";

        private static String CreateSecretKey(Int16 length)
        {
            String secretKey = String.Empty;
            String pool = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();

            for (Int16 i = 0; i < length; i++)
            {
                Int16 index = (Int16)Math.Floor(random.NextDouble() * pool.Length);
                secretKey += pool[index];
            }

            return secretKey;
        }

        private static String AESEncrypt(String str, String key)
        {
            AesManaged aesManaged = new AesManaged()
            {
                Key = Encoding.UTF8.GetBytes(key),
                IV = Encoding.UTF8.GetBytes("0102030405060708"),
                Mode = CipherMode.CBC
            };
            ICryptoTransform cryptoTransform = aesManaged.CreateEncryptor();
            Byte[] data = Encoding.UTF8.GetBytes(str);
            byte[] encryptedData = null;
            encryptedData = cryptoTransform.TransformFinalBlock(data, 0, data.Length);

            return Convert.ToBase64String(encryptedData);
        }

        private static string RSAEncrypt(string text, string pubKey, string modules)
        {
            try
            {
                return RSA.encrypt(text, pubKey, modules);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private static NeteaseParams Encrypt(JObject jobj)
        {
            var secKey = CreateSecretKey(16);
            var json = jobj.ToString(Newtonsoft.Json.Formatting.None);
            var encText = AESEncrypt(AESEncrypt(json, NONCE), secKey);
            var encSecKey = RSAEncrypt(secKey, PUB_KEY, MODULES);
            return new NeteaseParams(encText, encSecKey);
        }

        private static WebClient CreateClient()
        {
            return new WebClient()
            {
                Encoding = Encoding.UTF8,
                Headers = new WebHeaderCollection
                {
                    { "Referer", "http://music.163.com" },
                    { "Cookie", "appver=2.0.2; os=pc;" },
                    { "Content-Type", "application/x-www-form-urlencoded" },
                    { "X-Real-IP", "220.181.57.217" }
                },
                BaseAddress = "http://music.163.com"
            };
        }

        private class NeteaseParams
        {
            string _params;
            string encSecKey;

            public NeteaseParams(string _params, string encSecKey)
            {
                this._params = _params;
                this.encSecKey = encSecKey;
            }

            public override string ToString()
            {
                return string.Format("params={0}&encSecKey={1}", HttpUtility.UrlEncode(this._params), this.encSecKey);
            }

            public NameValueCollection ToNVC()
            {
                return new NameValueCollection()
                {
                    { "params", this._params },
                    { "encSecKey", this.encSecKey }
                };
            }

            public byte[] ToBytes()
            {
                return Encoding.UTF8.GetBytes(this.ToString());
            }
        }

        public async static Task<IEnumerable<Song>> GetSongDetailNew(string id)
        {
            var list = new List<Song>();
            string url = "/weapi/v3/song/detail";
            using (WebClient client = CreateClient())
            {
                JObject postData = new JObject
                {
                    { "c", "[{\"id\":" + id + "}]" },
                    { "csrf_token", "" }
                };
                var postEncData = Encrypt(postData);
                var json = await client.UploadStringTaskAsync(url, "POST", postEncData.ToString());
                Debug.Write(json);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    JObject jObject = JObject.Parse(json.ToString());
                    var playlist = jObject["playlist"];
                    if (playlist != null)
                    {
                        foreach (var item in playlist["tracks"])
                        {
                            var song = new Song()
                            {
                                Title = item.SelectToken("name").ToString(),
                                Artist = string.Join(" ", item.SelectToken("ar").Select(p => p.SelectToken("name")).Values<string>()),
                                MusicId = item.SelectToken("id").ToString(),
                                AlbumImage = item.SelectToken("al.picUrl").ToString(),
                            };
                            if (item.SelectToken("h.size") != null)
                                song.HMusic = new Music()
                                {
                                    BitRate = item.SelectToken("h.br").ToObject<int>(),
                                    Size = item.SelectToken("h.size").ToObject<long>(),
                                };
                            if (item.SelectToken("m.size") != null)
                                song.MMusic = new Music()
                                {
                                    BitRate = item.SelectToken("m.br").ToObject<int>(),
                                    Size = item.SelectToken("m.size").ToObject<long>(),
                                };
                            if (item.SelectToken("l.name") != null)
                                song.LMusic = new Music()
                                {
                                    BitRate = item.SelectToken("l.br").ToObject<int>(),
                                    Size = item.SelectToken("l.size").ToObject<long>(),
                                };
                            list.Add(song);
                        }
                    }
                }
                return list;
            }
        }

        public async static Task<IEnumerable<Song>> GetSongsFromPlaylistNew(string id)
        {
            var list = new List<Song>();
            string url = "/weapi/v3/playlist/detail";
            using (WebClient client = CreateClient())
            {
                JObject postData = new JObject
                {
                    { "id", id },
                    { "offset", 0 },
                    { "total", true },
                    { "limit", 1000 },
                    { "n", 1000 },
                    { "csrf_token", "" }
                };
                var postEncData = Encrypt(postData);
                var json = await client.UploadStringTaskAsync(url, "POST", postEncData.ToString());
                Debug.Write(json);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    JObject jObject = JObject.Parse(json.ToString());
                    var playlist = jObject["playlist"];
                    if (playlist != null)
                    {
                        foreach (var item in playlist["tracks"])
                        {
                            var song = new Song()
                            {
                                Title = item.SelectToken("name").ToString(),
                                Artist = string.Join(" ", item.SelectToken("ar").Select(p => p.SelectToken("name")).Values<string>()),
                                MusicId = item.SelectToken("id").ToString(),
                                AlbumImage = item.SelectToken("al.picUrl").ToString(),
                            };
                            if (item.SelectToken("h.size") != null)
                                song.HMusic = new Music()
                                {
                                    BitRate = item.SelectToken("h.br").ToObject<int>(),
                                    Size = item.SelectToken("h.size").ToObject<long>(),
                                };
                            if (item.SelectToken("m.size") != null)
                                song.MMusic = new Music()
                                {
                                    BitRate = item.SelectToken("m.br").ToObject<int>(),
                                    Size = item.SelectToken("m.size").ToObject<long>(),
                                };
                            if (item.SelectToken("l.name") != null)
                                song.LMusic = new Music()
                                {
                                    BitRate = item.SelectToken("l.br").ToObject<int>(),
                                    Size = item.SelectToken("l.size").ToObject<long>(),
                                };
                            list.Add(song);
                        }
                    }
                }
                return list;
            }
        }

        public static string GetTrackURLNew(string id, int br = 999000)
        {
            string link = null;
            string url = "/weapi/song/enhance/player/url";
            using (WebClient client = CreateClient())
            {
                JObject postData = new JObject
                {
                    { "ids", "[" + id + "]" },
                    { "br", br },
                    { "csrf_token", "" }
                };
                var postEncData = Encrypt(postData);
                var json = client.UploadString(url, "POST", postEncData.ToString());
                Debug.Write(json);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    JObject jObject = JObject.Parse(json.ToString());
                    var data = jObject["data"];
                    if (data.Count() > 0)
                    {
                        link = data[0]["url"].ToString();
                    }
                }
            }
            return link;
        }
        #endregion
    }
}