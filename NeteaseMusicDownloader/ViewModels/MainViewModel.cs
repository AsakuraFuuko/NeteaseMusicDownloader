using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeteaseMusicDownloader.Models;
using NeteaseMusicDownloader.Utils;
using System.IO;
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
        private string _musicId;
        private string _trackURL;
        private int _progress = 0;
        private long _bytesReceived = 0;
        private long _totalBytesToReceive = 0;

        private Song _song = new Song();
        public string Title { get; set; }

        public string MusicId
        {
            get { return _musicId; }
            set
            {
                _musicId = value;
                RaisePropertyChanged("MusicId");
            }
        }

        public string TrackURL
        {
            get { return _trackURL; }
            set
            {
                _trackURL = value;
                RaisePropertyChanged("TrackURL");
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

        public string SongFileName
        {
            get { return _song.HMucis != null ? string.Format("{0} - {1}.{2}", _song.Artist, _song.Title, _song.Extension) : "music.mp3"; }
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

        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                Title = "标题";
                MusicId = "http://music.163.com/#/song?id=29775130";
            }
            else
            {
                // Code runs "for real"
                Title = "标题";
                MusicId = "http://music.163.com/#/song?id=29775130";
                Progress = 0;
                GetTrackURLCommand = new RelayCommand(() =>
                {
                    _song.parseUrl(MusicId);
                    NeteaseUtil.GetSongDetail(ref _song);
                    TrackURL = NeteaseUtil.GetTrackURL(_song.HMucis.dfsId);
                    RaisePropertyChanged("SongTitle");
                    RaisePropertyChanged("SongArtist");
                    RaisePropertyChanged("SongBitRate");
                });

                DownloadCommand = new RelayCommand(() =>
                {
                    if (string.IsNullOrWhiteSpace(TrackURL))
                        return;
                    var downloader = Application.Current.Resources["MyDownload"] as DownloadUtils;
                    downloader.DownloadProgressChanged += (sender, args) =>
                    {
                        Progress = args.ProgressPercentage;
                        BytesReceived = args.BytesReceived;
                        TotalBytesToReceive = args.TotalBytesToReceive;
                    };
                    downloader.Get(TrackURL, Path.Combine("music", SongFileName));
                });
            }
        }
    }
}