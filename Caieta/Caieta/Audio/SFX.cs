using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Caieta.Audio
{
    public class SFX
    {
        public Dictionary<string, SoundEffect> SFXs;
        private Dictionary<string, SoundEffectInstance> _SoundInstance;

        //public int Count => _SoundInstance.Count;
        public int Volume { get; private set; }

        public SFX()
        {
            Volume = 100;

            SFXs = new Dictionary<string, SoundEffect>();
            _SoundInstance = new Dictionary<string, SoundEffectInstance>();
        }

        public void Load(string name)
        {
            SFXs.Add(name, Engine.Instance.Content.Load<SoundEffect>("Audio/SFX/" + name));
            _SoundInstance.Add(name, SFXs[name].CreateInstance());
        }

        public bool IsPlaying(string name)
        {
            return _SoundInstance[name].State == SoundState.Playing;
        }

        public void PlayNew(string name, bool loop = false, float volume = 1.0f, float pitch = 0.0f, float pan = 0.0f)
        {
            if (!SFXs.ContainsKey(name))
                Debug.ErrorLog("[SFX]: No SFX named '" + name + "' previous loaded.");
            else
            {
                SFXs[name].Play(volume, pitch, pan);
                //Debug.Log("[SFX]: Play '" + name + "' : looping " + loop + ".");
            }
        }

        public void Play(string name, bool loop = false, float volume = 1.0f, float pitch = 0.0f, float pan = 0.0f)
        {
            if (!_SoundInstance.ContainsKey(name))
                Debug.ErrorLog("[SFX]: No SFX named '" + name + "' previous loaded.");
            else
            {
                TrackVolume(name, volume);
                TrackPitch(name, pitch);
                TrackPan(name, pan);
                _SoundInstance[name].IsLooped = loop;
                _SoundInstance[name].Play();

                //Debug.Log("[SFX]: Play '" + name + "' : looping " + loop + ".");
            }
        }

        public void Play(string name, float volume, float pitch, float pan)
        {
            if (!SFXs.ContainsKey(name))
                Debug.ErrorLog("[SFX]: No SFX named '" + name + "' previous loaded.");
            else
            {
                SFXs[name].Play(volume, pitch, pan);

                //Debug.Log("[SFX]: Play *pure* SFX '" + name + "' : volume " + volume + " : pitch " + pitch + " : pan " + pan + ".");
            }
        }

        public void TrackVolume(string name, float value)
        {
            if (!_SoundInstance.ContainsKey(name))
                Debug.ErrorLog("[SFX]: No SFX named '" + name + "' previous loaded.");
            else
            {
                var volume = MathHelper.Clamp(value, 0, 1);
                _SoundInstance[name].Volume = volume;

                //Debug.Log("[SFX]: Changed '" + name + "' individual volume to " + volume + ".");
            }
        }

        public void TrackPitch(string name, float value)
        {
            if (!_SoundInstance.ContainsKey(name))
                Debug.ErrorLog("[SFX]: No SFX named '" + name + "' previous loaded.");
            else
            {
                var pitch = MathHelper.Clamp(value, -1, 1);
                _SoundInstance[name].Pitch = pitch;

                //Debug.Log("[SFX]: Changed '" + name + "' individual pitch to " + pitch + ".");
            }
        }

        public void TrackPan(string name, float value)
        {
            if (!_SoundInstance.ContainsKey(name))
                Debug.ErrorLog("[SFX]: No SFX named '" + name + "' previous loaded.");
            else
            {
                var pan = MathHelper.Clamp(value, -1, 1);
                _SoundInstance[name].Pan = pan;

                //Debug.Log("[SFX]: Changed '" + name + "' individual pan to " + pan + ".");
            }
        }

        public void Stop(string name)
        {
            if (!_SoundInstance.ContainsKey(name))
                Debug.ErrorLog("[SFX]: No SFX named '" + name + "' previous loaded.");
            else
            {
                _SoundInstance[name].Stop();

                //Debug.Log("[SFX]: Stop SFX '" + name + "'.");
            }
        }

        public void Pause(string name)
        {
            if (!_SoundInstance.ContainsKey(name))
                Debug.ErrorLog("[SFX]: No SFX named '" + name + "' previous loaded.");
            else
            {
                _SoundInstance[name].Pause();

                //Debug.Log("[SFX]: Pause SFX '" + name + "'.");
            }
        }

        public void Resume(string name)
        {
            if (!_SoundInstance.ContainsKey(name))
                Debug.ErrorLog("[SFX]: No SFX named '" + name + "' previous loaded.");
            else
            {
                _SoundInstance[name].Resume();

                //Debug.Log("[SFX]: Resume SFX '" + name + "'.");
            }
        }

        public void Mute()
        {
            SoundEffect.MasterVolume = 0.0f;
            Debug.Log("[SFX]: Mute.");
        }

        public void UnMute()
        {
            SoundEffect.MasterVolume = (Volume / 100f) * (AudioManager.MasterVolume / 100f);
            Debug.Log("[SFX]: UnMute.");
        }

        public void SetVolume(int volume)
        {
            Volume = MathHelper.Clamp(volume, 0, 100);
            SoundEffect.MasterVolume = (Volume / 100f) * (AudioManager.MasterVolume / 100f);
            //Debug.Log("[SFX]: Volume " + Volume + "% set to " + (Volume / 100f) + ". Master Volume at " + AudioManager.MasterVolume + ".");
        }

        public void Unload()
        {
            foreach (SoundEffectInstance sfx in _SoundInstance.Values)
                sfx.Stop();

            _SoundInstance.Clear();
            SFXs.Clear();
        }
    }
}
