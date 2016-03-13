using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeteaseMusicDownloader.Models;
using NeteaseMusicDownloader.Utils;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;

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
        private string _musicUrl;
        private string _playlistUrl;
        private string _trackUrl;
        private int _progress = 0;
        private long _bytesReceived = 0;
        private long _totalBytesToReceive = 0;
        private ObservableCollection<Song> _songCollection = new ObservableCollection<Song>();

        private Song _song = new Song();
        public string Title { get; set; }

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

        public long BytesReceived
        {
            get { return _bytesReceived; }
            private set
            {
                _bytesReceived = value;
                RaisePropertyChanged("BytesReceived");
            }
        }

        public long TotalBytesToReceive
        {
            get { return _totalBytesToReceive; }
            private set
            {
                _totalBytesToReceive = value;
                RaisePropertyChanged("TotalBytesToReceive");
            }
        }

        public Song CurrentDownload { get; set; }

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

        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                Title = "Netease Music Downloader";
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
                Title = "Netease Music Downloader";
                MusicUrl = "http://music.163.com/#/song?id=29775130";
                PlaylistUrl = "http://music.163.com/#/my/m/music/playlist?id=6435531";
                Progress = 0;
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
                        BytesReceived = args.BytesReceived;
                        TotalBytesToReceive = args.TotalBytesToReceive;
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
                        BytesReceived = args.BytesReceived;
                        TotalBytesToReceive = args.TotalBytesToReceive;
                    };
                    downloader.Get(trackUrl, Path.Combine("music", FileUtils.GetSafeFileName(song.SongFileName)));
                });
            }
        }
    }
}