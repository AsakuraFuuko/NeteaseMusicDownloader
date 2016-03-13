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
        private string _musicUrl;
        private string _playlistUrl;
        private string _trackUrl;
        private int _progress = 0;
        private string _bytesReceived = "0";
        private string _totalBytesToReceive = "0";
        private Song _currentPlaySong = null;
        private AudioPlayback audioPlayback;
        private string _nowPlaying = "";
        private ObservableCollection<Song> _songCollection = new ObservableCollection<Song>();
        private BASSTimer timer = new BASSTimer(1000);

        private Song _song = new Song();
        public string Title { get; set; }

        public bool DownloadNext { get; set; }

        public string PlaylistUrl
        {
            get { return _playlistUrl; }
            set
            {
                _playlistUrl = value;
                RaisePropertyChanged("PlaylistUrl");
            }
        }

        public string MusicUrl
        {
            get { return _musicUrl; }
            set
            {
                _musicUrl = value;
                RaisePropertyChanged("MusicUrl");
            }
        }

        public string TrackUrl
        {
            get { return _trackUrl; }
            set
            {
                _trackUrl = value;
                RaisePropertyChanged("TrackURL");
            }
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
            get { return _song.Title; }
        }

        public string SongArtist
        {
            get { return _song.Artist; }
        }

        public string SongBitRate
        {
            get { return _song.HMucis != null ? _song.HMucis.BitRate / 1000 + "k" : ""; }
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

        public RelayCommand GetTrackURLCommand
        {
            get;
            private set;
        }

        public RelayCommand DownloadCommand
        {
            get;
            private set;
        }

        public RelayCommand GetPlaylistCommand
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
                MusicUrl = "http://music.163.com/#/song?id=29775130";
                PlaylistUrl = "http://music.163.com/#/my/m/music/playlist?id=6435531";
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
                MusicUrl = "http://music.163.com/#/song?id=29775130";
                PlaylistUrl = "http://music.163.com/#/my/m/music/playlist?id=6435531";
                Progress = 0;
                audioPlayback = new AudioPlayback();

                timer.Tick += (sender, args) =>
                {
                    Title = string.Format("{0} {1}/{2}", _title, audioPlayback.CurrentLength, audioPlayback.TotalLength);
                    RaisePropertyChanged("Title");
                };

                GetTrackURLCommand = new RelayCommand(() =>
                {
                    _song.parseUrl(MusicUrl);
                    NeteaseUtil.GetSongDetail(ref _song);
                    TrackUrl = NeteaseUtil.GetTrackURL(_song.DfsId);
                    RaisePropertyChanged("SongTitle");
                    RaisePropertyChanged("SongArtist");
                    RaisePropertyChanged("SongBitRate");
                });

                DownloadCommand = new RelayCommand(() =>
                {
                    if (string.IsNullOrWhiteSpace(TrackUrl))
                        return;
                    var downloader = Application.Current.Resources["MyDownload"] as DownloadUtils;
                    downloader.DownloadProgressChanged += (sender, args) =>
                    {
                        Progress = args.ProgressPercentage;
                        BytesReceived = args.BytesReceived.ToString();
                        TotalBytesToReceive = args.TotalBytesToReceive.ToString();
                    };
                    downloader.DownloadFileCompleted += (sender, args) =>
                    {
                        Progress = 0;
                        BytesReceived = "0";
                        TotalBytesToReceive = "0";
                    };
                    downloader.Get(TrackUrl, Path.Combine("music", FileUtils.GetSafeFileName(_song.SongFileName)));
                });

                GetPlaylistCommand = new RelayCommand(async () =>
                {
                    if (string.IsNullOrWhiteSpace(PlaylistUrl))
                        return;
                    var playlistId = "";
                    var reg = new Regex(@"id=(\d*)").Match(PlaylistUrl);
                    if (reg.Success)
                    {
                        playlistId = reg.Groups[1].Value;
                        SongCollection.Clear();
                        foreach (var song in await NeteaseUtil.GetSongsFromPlaylist(playlistId))
                        {
                            SongCollection.Add(song);
                        }
                    }
                });

                PlaylistDownloadCommand = new RelayCommand<Song>((song) =>
                {
                    string trackUrl = NeteaseUtil.GetTrackURL(song.DfsId);
                    if (string.IsNullOrWhiteSpace(trackUrl))
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
                    downloader.Get(trackUrl, Path.Combine("music", FileUtils.GetSafeFileName(song.SongFileName)));
                });

                ListenCommand = new RelayCommand<Song>((song) =>
                {
                    string trackUrl = NeteaseUtil.GetTrackURL(song.DfsId);
                    if (string.IsNullOrWhiteSpace(trackUrl))
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

                        audioPlayback.Load(trackUrl);
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