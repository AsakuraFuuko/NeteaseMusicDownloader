using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeteaseMusicDownloader.Models;
using NeteaseMusicDownloader.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using Un4seen.Bass;

namespace NeteaseMusicDownloader.ViewModels
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        readonly private string _title = "Netease Music Downloader";
        private string _neteaseUrl;
        private int _progress = 0;
        private string _bytesReceived = "0";
        private string _totalBytesToReceive = "0";
        private Song _currentPlaySong = null;
        private Song _selectedSong;
        private AudioPlayback audioPlayback;
        private string _nowPlaying = "";
        private ObservableCollection<Song> _songCollection = new ObservableCollection<Song>();
        private BASSTimer timer = new BASSTimer(1000);

        public string Title { get; set; }

        public bool DownloadNext { get; set; }

        public string NeteaseUrl
        {
            get { return _neteaseUrl; }
            set
            {
                _neteaseUrl = value;
                RaisePropertyChanged("NeteaseUrl");
            }
        }

        public string SongTrackUrl
        {
            get { return NeteaseUtil.GetTrackURL(SelectedSong?.DfsId); }
        }

        public ObservableCollection<Song> SongCollection
        {
            get { return _songCollection; }
            set
            {
                _songCollection = value;
                RaisePropertyChanged("SongCollection");
            }
        }

        public string SongTitle
        {
            get { return SelectedSong?.Title; }
        }

        public string SongArtist
        {
            get { return SelectedSong?.Artist; }
        }

        public string SongBitRate
        {
            get { return SelectedSong?.BitRate; }
        }

        public int Progress
        {
            get { return _progress; }
            private set
            {
                _progress = value;
                RaisePropertyChanged("Progress");
            }
        }

        public string BytesReceived
        {
            get { return _bytesReceived; }
            private set
            {
                _bytesReceived = value;
                RaisePropertyChanged("BytesReceived");
            }
        }

        public string TotalBytesToReceive
        {
            get { return _totalBytesToReceive; }
            private set
            {
                _totalBytesToReceive = value;
                RaisePropertyChanged("TotalBytesToReceive");
            }
        }

        public string NowPlaying
        {
            get { return _nowPlaying; }
            private set
            {
                _nowPlaying = value;
                RaisePropertyChanged("NowPlaying");
            }
        }

        public Song CurrentPlaySong
        {
            get { return _currentPlaySong; }
            private set
            {
                if (_currentPlaySong != value)
                {
                    _currentPlaySong = value;
                    RaisePropertyChanged("CurrentPlaySong");
                }
            }
        }

        public Song SelectedSong
        {
            get
            {
                return _selectedSong;
            }
            set
            {
                _selectedSong = value;
                RaisePropertyChanged("SongTrackUrl");
                RaisePropertyChanged("SongTitle");
                RaisePropertyChanged("SongArtist");
                RaisePropertyChanged("SongBitRate");
                RaisePropertyChanged("SelectedSong");
            }
        }

        public RelayCommand GetSongsCommand
        {
            get;
            private set;
        }

        public RelayCommand<Song> PlaylistDownloadCommand
        {
            get;
            private set;
        }

        public RelayCommand<Song> ListenCommand
        {
            get;
            private set;
        }

        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                Title = _title;
                NeteaseUrl = "http://music.163.com/#/my/m/music/playlist?id=6435531";
                SongCollection.Add(new Song()
                {
                    Title = "Clover Heart's",
                    Artist = "榊原ゆい",
                    AlbumImage = "http://p4.music.126.net/n189nEFRefNaucKD8akNQw==/7886796906449604.jpg",
                });
                SongCollection.Add(new Song()
                {
                    Title = "サクラ サクラ (Instrumental With 尺八・三味线) - instrumental",
                    Artist = "榊原ゆい",
                    AlbumImage = "http://p4.music.126.net/n189nEFRefNaucKD8akNQw==/7886796906449604.jpg",
                });
                SongCollection.Add(new Song()
                {
                    Title = "キミの隣りで...(絶体絶命都市3 ─壊れゆく街と彼女の歌─)",
                    Artist = "榊原ゆい",
                    AlbumImage = "http://p4.music.126.net/n189nEFRefNaucKD8akNQw==/7886796906449604.jpg",
                });
            }
            else
            {
                // Code runs "for real"
                Title = _title;
                NeteaseUrl = "http://music.163.com/#/my/m/music/playlist?id=6435531";
                Progress = 0;
                audioPlayback = new AudioPlayback();

                timer.Tick += (sender, args) =>
                {
                    Title = string.Format("{0} {1}/{2}", _title, audioPlayback.CurrentLength, audioPlayback.TotalLength);
                    RaisePropertyChanged("Title");
                };

                GetSongsCommand = new RelayCommand(async () =>
                {
                    if (string.IsNullOrWhiteSpace(NeteaseUrl))
                        return;
                    var id = "";
                    var urlType = "";
                    var reg = new Regex(@".*/(.*?)\?id=(\d*)").Match(NeteaseUrl);
                    if (reg.Success)
                    {
                        urlType = reg.Groups[1].Value;
                        id = reg.Groups[2].Value;
                        SongCollection.Clear();
                        switch (urlType)
                        {
                            case "album":
                                foreach (var song in await NeteaseUtil.GetSongsFromAlbum(id))
                                {
                                    SongCollection.Add(song);
                                }
                                break;

                            case "artist":
                                foreach (var song in await NeteaseUtil.GetSongsFromArtist(id))
                                {
                                    SongCollection.Add(song);
                                }
                                break;

                            case "playlist":
                                foreach (var song in await NeteaseUtil.GetSongsFromPlaylist(id))
                                {
                                    SongCollection.Add(song);
                                }
                                break;

                            case "song":
                                foreach (var song in await NeteaseUtil.GetSongDetail(id))
                                {
                                    SongCollection.Add(song);
                                }
                                break;
                        }
                    }
                });

                PlaylistDownloadCommand = new RelayCommand<Song>((song) =>
                {
                    SelectedSong = song;
                    if (string.IsNullOrWhiteSpace(SongTrackUrl))
                        return;
                    var downloader = new DownloadUtils();
                    downloader.DownloadProgressChanged += (sender, args) =>
                    {
                        song.Progress = args.ProgressPercentage;
                        BytesReceived = args.BytesReceived.ToString();
                        TotalBytesToReceive = args.TotalBytesToReceive.ToString();
                    };
                    downloader.DownloadFileCompleted += (sender, args) =>
                    {
                        if (DownloadNext)
                        {
                            int index = -1;
                            if ((index = SongCollection.IndexOf(song)) != -1 && index + 1 < SongCollection.Count)
                            {
                                PlaylistDownloadCommand.Execute(SongCollection.ElementAt(index + 1));
                            }
                        }
                        else
                        {
                            BytesReceived = "0";
                            TotalBytesToReceive = "0";
                        }
                    };
                    downloader.Get(SongTrackUrl, Path.Combine("music", FileUtils.GetSafeFileName(song.SongFileName)));
                });

                ListenCommand = new RelayCommand<Song>((song) =>
                {
                    SelectedSong = song;
                    if (string.IsNullOrWhiteSpace(SongTrackUrl))
                        return;
                    if (song.PlayStatus == PlayStatus.Play)
                    {
                        if (CurrentPlaySong != null && CurrentPlaySong != song)
                        {
                            audioPlayback.Stop();
                            CurrentPlaySong.PlayStatus = PlayStatus.Play;
                        }

                        audioPlayback.EndCallback += (handle, channel, data, user) =>
                        {
                            int index = -1;
                            if ((index = SongCollection.IndexOf(song)) != -1)
                            {
                                ListenCommand.Execute(SongCollection.ElementAt((index + 1) % SongCollection.Count));
                            }
                            else
                            {
                                audioPlayback.Stop();
                                NowPlaying = "";
                                song.PlayStatus = PlayStatus.Play;
                                timer.Enabled = false;
                                timer.Stop();
                            }
                        };

                        audioPlayback.Load(SongTrackUrl);
                        audioPlayback.Play();

                        CurrentPlaySong = song;
                        NowPlaying = string.Format("Now Playing {0} - {1}", song.Artist, song.Title);
                        timer.Enabled = true;
                        timer.Start();
                        song.PlayStatus = PlayStatus.Stop;
                    }
                    else
                    {
                        audioPlayback.Stop();
                        NowPlaying = "";
                        song.PlayStatus = PlayStatus.Play;
                        timer.Enabled = false;
                        timer.Stop();
                    }
                });
            }
        }
    }
}