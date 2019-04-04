using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace Caieta.Audio
{
    public static class AudioManager
    {
        //public const string _sfxExtension = ".wav";
        //public const string _ostExtension = ".ogg";

        public static SFX SFX;
        public static Music Music;

        public static int MasterVolume { get; private set; }

        public static void Initialize()
        {
            MasterVolume = 100;

            Music = new Music();
            SFX = new SFX();
        }

        public static void SetMasterVolume(int volume)
        {
            MasterVolume = MathHelper.Clamp(volume, 0, 100);
        }

        public static void Unload()
        {
            Music.Unload();
            SFX.Unload();
        }

        public static void Mute()
        {
            Music.Mute();
            SFX.Mute();
        }

        public static void UnMute()
        {
            Music.UnMute();
            SFX.UnMute();
        }
    }
}