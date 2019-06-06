using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace Caieta.Audio
{
    public class Music
    {
        private Dictionary<string, Song> _Musics;
        public Song CurrMusic { get; private set; }

        public int Volume { get; private set; }

        public Action<string> OnStart;
        public Action<string> OnEnd;

        public Music()
        {
            _Musics = new Dictionary<string, Song>();

            Volume = 100;

            OnStart = null;
            OnEnd = null;

            MediaPlayer.MediaStateChanged += OnChange;
        }

        public void Load(string name)
        {
            _Musics.Add(name, Engine.Instance.Content.Load<Song>("Audio/Music/" + name));
        }

        public void Unload(string name)
        {
            if (!_Musics.ContainsKey(name))
                Debug.ErrorLog("[AudioManager]: No Music named '" + name + "' previous loaded.");
            else
                _Musics.Remove(name);
        }

        public void Unload()
        {
            Stop();
            _Musics.Clear();
        }

        private void OnChange(object sender, System.EventArgs e)
        {
            if (CurrMusic != null && MediaPlayer.State == MediaState.Playing)
            {
                OnStart?.Invoke(CurrMusic.Name);

                Debug.Log("[Music]: On Start '" + CurrMusic.Name + "' trigger.");
            }
            else if (CurrMusic != null && MediaPlayer.State == MediaState.Stopped)
            {
                OnEnd?.Invoke(CurrMusic.Name);

                Debug.Log("[Music]: On Stop '" + CurrMusic.Name + "' trigger.");
            }
        }

        public bool IsPlaying()
        {
            if (MediaPlayer.State == MediaState.Playing)
                return true;

            return false;
        }

        public void Play(string song_name, bool loop = true)
        {
            if (!_Musics.ContainsKey(song_name))
                Debug.ErrorLog("[Music]: No Music named '" + song_name + "' previous loaded.");
            else
            {
                CurrMusic = _Musics[song_name];

                MediaPlayer.Play(CurrMusic);

                MediaPlayer.IsRepeating = loop;

                Debug.Log("[Music]: Play(" + song_name + ") : looping(" + loop + ")");
            }
        }

        public void Stop()
        {
            CurrMusic = null;
            MediaPlayer.Stop();
            Debug.Log("[Music]: Stop.");
        }

        public void Pause()
        {
            MediaPlayer.Pause();
            Debug.Log("[Music]: Pause.");
        }

        public void Resume()
        {
            MediaPlayer.Resume();
            Debug.Log("[Music]: Resume.");
        }

        public void Mute()
        {
            MediaPlayer.Volume = 0.0f;
            Debug.Log("[Music]: Mute.");
        }

        public void UnMute()
        {
            MediaPlayer.Volume = (Volume / 100f) * (AudioManager.MasterVolume / 100f);
            Debug.Log("[Music]: UnMute.");
        }

        public void SetVolume(int volume)
        {
            Volume = MathHelper.Clamp(volume, 0, 100);
            MediaPlayer.Volume = (Volume / 100f) * (AudioManager.MasterVolume / 100f);
            Debug.Log("[Music]: Volume " + Volume + "% set to " + (Volume / 100f) + ". Master Volume at " + AudioManager.MasterVolume + "%.");
        }
    }
}
