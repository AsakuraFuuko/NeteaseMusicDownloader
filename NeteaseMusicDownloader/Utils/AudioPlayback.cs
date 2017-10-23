using BASS.NET.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Un4seen.Bass;

namespace NeteaseMusicDownloader.Utils
{
    internal class AudioPlayback : IDisposable
    {
        private int fileStream;
        private float _volume = 0.7f;

        public AudioPlayback()
        {
            Stop();
            CloseFile();
            Init();
        }

        public void Load(string fileName)
        {
            OpenFile(fileName);
        }

        private void Init()
        {
            if (!BassHelper.IsInitialized)
            {
                try
                {
                    //BassHelper.Registration("xxx", "xxx");// register
                    BassHelper.Initialize();
                    //Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_GVOL_STREAM, (int)(_volume * 10000));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Application.Current.Shutdown();
                }
            }
        }

        private void CloseFile()
        {
            if (fileStream != 0)
            {
                BassHelper.Free();
                fileStream = 0;
            }
        }

        private void OpenFile(string fileName)
        {
            if (fileName.EndsWith(".mp3"))
            {
                fileStream = Bass.BASS_StreamCreateURL(fileName, 0, BASSFlag.BASS_SAMPLE_FLOAT, null, IntPtr.Zero);
            }
            else
            {
                throw new InvalidOperationException("Unsupported extension");
            }
        }

        public void Play()
        {
            if (BassHelper.IsInitialized && fileStream != 0 && Bass.BASS_ChannelIsActive(fileStream) != BASSActive.BASS_ACTIVE_PLAYING)
            {
                if (EndCallback != null)
                {
                    Bass.BASS_ChannelSetSync(fileStream, BASSSync.BASS_SYNC_END | BASSSync.BASS_SYNC_MIXTIME, 0, EndCallback, IntPtr.Zero);
                }
                Bass.BASS_ChannelSetAttribute(fileStream, BASSAttribute.BASS_ATTRIB_VOL, Volume);
                Bass.BASS_ChannelPlay(fileStream, Bass.BASS_ChannelIsActive(fileStream) != BASSActive.BASS_ACTIVE_PAUSED);
            }
        }

        public void Pause()
        {
            if (BassHelper.IsInitialized)
            {
                Bass.BASS_ChannelPause(fileStream);
            }
        }

        public void Stop()
        {
            if (BassHelper.IsInitialized)
            {
                Bass.BASS_ChannelStop(fileStream);
            }
        }

        public void Dispose()
        {
            Stop();
            CloseFile();
            if (BassHelper.IsInitialized)
            {
                BassHelper.Free();
            }
        }

        public string TotalLength
        {
            get
            {
                if (BassHelper.IsInitialized && fileStream != 0)
                {
                    return new TimeSpan(0, 0, (int)(Bass.BASS_ChannelBytes2Seconds(fileStream, Bass.BASS_ChannelGetLength(fileStream, BASSMode.BASS_POS_BYTES)))).ToString();
                }
                else
                {
                    return "00:00:00";
                }
            }
        }

        public string CurrentLength
        {
            get
            {
                if (BassHelper.IsInitialized && fileStream != 0)
                {
                    return new TimeSpan(0, 0, (int)Bass.BASS_ChannelBytes2Seconds(fileStream, Bass.BASS_ChannelGetPosition(fileStream, BASSMode.BASS_POS_BYTES))).ToString();
                }
                else
                {
                    return "00:00:00";
                }
            }
        }

        public double Progress
        {
            get
            {
                if (BassHelper.IsInitialized && fileStream != 0)
                {
                    return (Bass.BASS_ChannelBytes2Seconds(fileStream, Bass.BASS_ChannelGetPosition(fileStream, BASSMode.BASS_POS_BYTES)) / (Bass.BASS_ChannelBytes2Seconds(fileStream, Bass.BASS_ChannelGetLength(fileStream, BASSMode.BASS_POS_BYTES))) * 100.0);
                }
                else
                {
                    return 0;
                }
            }
        }

        public float Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                _volume = value;
                if (BassHelper.IsInitialized && fileStream != 0)
                {
                    //Bass.BASS_SetVolume(value);
                    Bass.BASS_ChannelSetAttribute(fileStream, BASSAttribute.BASS_ATTRIB_VOL, _volume);
                }
            }
        }

        public SYNCPROC EndCallback { get; set; }
    }
}