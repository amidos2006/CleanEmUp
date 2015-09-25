using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace OmidosGameEngine.Sounds
{
    public static class SoundManager
    {
        public const float MAX_VOLUME = 0.35f;
        public const float MIN_VOLUME = 0.1f;
        public const float MUTE_VOLUME = 0f;

        private static AudioEmitter emitter;
        private static AudioListener listner;
        private static Dictionary<string, SoundEffect> soundEffectsLibrary;
        private static Dictionary<string, SoundEffectProperties> soundEffectsProperties;
        private static List<SoundEffectInstance> playingSfx;

        public static bool MusicOn
        {
            set;
            get;
        }

        public static bool SoundOn
        {
            set;
            get;
        }

        public static Vector2 EmitterPosition
        {
            set
            {
                emitter.Position = new Vector3(value.X / 200, value.Y / 200, 0);
            }
        }
        public static Vector2 ListnerPosition
        {
            set
            {
                listner.Position = new Vector3(value.X / 200, value.Y / 200, HeightOfCamera);
            }
        }

        public static float HeightOfCamera
        {
            set;
            get;
        }

        private static Dictionary<string, Song> musicLibrary;

        public static string CurrentRunningMusic
        {
            set;
            get;
        }

        public static void LoadContent(Dictionary<string, Song> musicList, Dictionary<string, SoundEffect> soundList, Dictionary<string, SoundEffectProperties> propertiesList)
        {
            SoundManager.MusicOn = true;
            SoundManager.SoundOn = true;

            SoundManager.musicLibrary = musicList;
            SoundManager.CurrentRunningMusic = string.Empty;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 1;

            SoundManager.soundEffectsLibrary = soundList;
            SoundManager.soundEffectsProperties = propertiesList;
            SoundEffect.MasterVolume = 1;
            
            SoundManager.HeightOfCamera = 3; 
            SoundManager.emitter = new AudioEmitter();
            SoundManager.listner = new AudioListener();
            SoundManager.EmitterPosition = new Vector2();
            SoundManager.ListnerPosition = new Vector2();

            SoundManager.playingSfx = new List<SoundEffectInstance>();
        }

        public static SoundEffectInstance PlaySFX(string sfxName)
        {
            SoundEffectInstance soundCue = soundEffectsLibrary[sfxName].CreateInstance();
            soundCue.IsLooped = soundEffectsProperties[sfxName].Loop;
            soundCue.Volume = soundEffectsProperties[sfxName].Volume;
            soundCue.Pitch = soundEffectsProperties[sfxName].Pitch;

            Apply3D(soundCue);
            
            if (SoundOn)
            {
                soundCue.Play();
            }

            playingSfx.Add(soundCue);
            return soundCue;
        }

        public static void Apply3D(SoundEffectInstance soundCue)
        {
            soundCue.Apply3D(listner, emitter);
        }

        public static void StopSfx(SoundEffectInstance soundCue)
        {
            soundCue.Stop(true);
            soundCue.Dispose();
        }

        public static void PauseSFX()
        {
            foreach (SoundEffectInstance soundCue in playingSfx)
            {
                if (soundCue.State == SoundState.Playing)
                {
                    try
                    {
                        soundCue.Pause();
                    }
                    catch
                    {
                    }
                }
            }
        }

        public static void ClearSfx()
        {
            foreach (SoundEffectInstance soundCue in playingSfx)
            {
                if (soundCue.State != SoundState.Stopped)
                {
                    soundCue.Stop(true);
                    soundCue.Dispose();
                }
            }

            playingSfx.Clear();
        }

        public static void ResumeSFX()
        {
            foreach (SoundEffectInstance soundCue in playingSfx)
            {
                soundCue.Resume();
            }
        }

        public static void PlayMusic(string musicName, float volume = MAX_VOLUME)
        {
            ChangeMusicVolume(volume);

            if (musicName != CurrentRunningMusic)
            {
                StopMusic();
                CurrentRunningMusic = musicName;

                if (MusicOn)
                {
                    MediaPlayer.Play(musicLibrary[musicName]);
                }
            }
        }

        public static void StopMusic()
        {
            if (musicLibrary.ContainsKey(CurrentRunningMusic))
            {
                CurrentRunningMusic = string.Empty;
                MediaPlayer.Stop();
            }
        }

        public static float ChangeMusicVolume(float volume)
        {
            float oldVolume = MediaPlayer.Volume;

            MediaPlayer.Volume = volume;

            return oldVolume;
        }

        public static bool MusicRunning()
        {
            return musicLibrary.ContainsKey(CurrentRunningMusic) && MediaPlayer.Volume > 0;
        }

        public static void Update()
        {
            List<SoundEffectInstance> removeList = new List<SoundEffectInstance>();
            foreach (SoundEffectInstance soundCue in playingSfx)
            {
                if (soundCue.State == SoundState.Stopped)
                {
                    removeList.Add(soundCue);
                }
            }

            foreach (SoundEffectInstance soundCue in removeList)
            {
                soundCue.Dispose();
                playingSfx.Remove(soundCue);
            }
        }
    }
}
