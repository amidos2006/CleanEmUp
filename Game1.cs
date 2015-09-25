using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using OmidosGameEngine.Graphics.Particles;
using OmidosGameEngine;
using OmidosGameEngine.Graphics;
using BloomPostprocess;
using OmidosGameEngine.Graphics.Lighting;
using OmidosGameEngine.Entity.ParticleGenerator;
using OmidosGameEngine.Entity.Player;
using OmidosGameEngine.World;
using OmidosGameEngine.Entity;
using OmidosGameEngine.Entity.Player.Weapons;
using OmidosGameEngine.Entity.Player.Data;
using OmidosGameEngine.Sounds;
using OmidosGameEngine.Entity.Enemy;
using OmidosGameEngine.Entity.Explosion;
using OmidosGameEngine.Entity.OverLayer;
using OmidosGameEngine.Entity.Object;
using OmidosGameEngine.Entity.Cursor;
using OmidosGameEngine.Entity.Generator;
using OmidosGameEngine.Data;
using Steamworks;
using System.Xml.Serialization;
using System.IO;

namespace CleanEmUp
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        BloomComponent bloomPostProcess;
        bool Pause = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = (int)OGE.ScreenResolution.X;
            graphics.PreferredBackBufferHeight = (int)OGE.ScreenResolution.Y;
            graphics.IsFullScreen = Properties.Settings.Default.FullScreen;

            graphics.ApplyChanges();
            
            bloomPostProcess = new BloomComponent(this);
            bloomPostProcess.Settings = BloomSettings.PresetSettings[3];

            this.Window.Title = "Clean'Em Up";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            OGE.Intialize(GraphicsDevice, Content);
            OGE.CleanEmUpApplication = this;
            GlobalVariables.Intialize();

            GlobalVariables.Controls = Properties.Settings.Default.Controls ? ControlType.WASD : ControlType.Arrows;
            GlobalVariables.IsFullScreen = Properties.Settings.Default.FullScreen;
            GlobalVariables.SettingsChanged = SettingsChanged;
            GlobalVariables.RevertSettings = RevertSettings;

            GlobalVariables.Background = new Backdrop(Content.Load<Texture2D>(@"Graphics\Backgrounds\Background" 
                + (OGE.Random.Next(DriveData.MAX_DRIVE_NUMBER) + 1)));

            GlobalVariables.IsReviewVersion = false;
            GlobalVariables.IsDemoVersion = false;

            if (GlobalVariables.IsReviewVersion)
            {
                OGE.NextWorld = new ReviewWorld(bloomPostProcess);
            }
            else
            {
                OGE.NextWorld = new MainMenuWorld(bloomPostProcess);
            }

            SteamManager.Initialize();

            base.Initialize();
        }

        protected override void OnDeactivated(object sender, EventArgs args)
        {
            Pause = true;
            OGE.PauseGame();
            base.OnDeactivated(sender, args);
        }

        protected override void OnActivated(object sender, EventArgs args)
        {
            Pause = false;
            OGE.UnPauseGame();
            base.OnActivated(sender, args);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            #region SoundManager

            string enginePath = @"Content\SoundEffects\CleanEmUpSoundEffects.xgs";
            string soundBankPath = @"Content\SoundEffects\Sound Bank.xsb";
            string waveBankPath = @"Content\SoundEffects\Wave Bank.xwb";

            Dictionary<string, Song> musicList = new Dictionary<string, Song>();
            musicList.Add("menu", Content.Load<Song>(@"Music\0_Menu"));
            musicList.Add("boss", Content.Load<Song>(@"Music\0_Boss"));
            musicList.Add("ingame11", Content.Load<Song>(@"Music\1_Ingame"));
            musicList.Add("ingame12", Content.Load<Song>(@"Music\1_Storage"));
            musicList.Add("ingame13", Content.Load<Song>(@"Music\1_3_Trojannosaurus"));
            musicList.Add("ingame21", Content.Load<Song>(@"Music\2_Diediedie"));
            musicList.Add("ingame22", Content.Load<Song>(@"Music\2_Forward"));
            musicList.Add("ingame23", Content.Load<Song>(@"Music\2_4_Ingame_2"));
            musicList.Add("ingame31", Content.Load<Song>(@"Music\3_ChillKill"));
            musicList.Add("ingame32", Content.Load<Song>(@"Music\3_Kill"));
            musicList.Add("ingame33", Content.Load<Song>(@"Music\1_3_Trojannosaurus"));
            musicList.Add("ingame41", Content.Load<Song>(@"Music\4_Vacuum"));
            musicList.Add("ingame42", Content.Load<Song>(@"Music\4_Virosaur"));
            musicList.Add("ingame43", Content.Load<Song>(@"Music\2_4_Ingame_2"));
            musicList.Add("ingame5", Content.Load<Song>(@"Music\5_JamSpam"));

            Dictionary<string, SoundEffect> soundList = new Dictionary<string, SoundEffect>();
            soundList.Add("bullet_collision", Content.Load<SoundEffect>(@"SoundEffects\bullet_collision"));
            soundList.Add("enemy_destroy", Content.Load<SoundEffect>(@"SoundEffects\enemy_destroy"));
            soundList.Add("error", Content.Load<SoundEffect>(@"SoundEffects\error"));
            soundList.Add("explosion", Content.Load<SoundEffect>(@"SoundEffects\explosion"));
            soundList.Add("file_collision", Content.Load<SoundEffect>(@"SoundEffects\file_collision"));
            soundList.Add("file_pickup", Content.Load<SoundEffect>(@"SoundEffects\file_pickup"));
            soundList.Add("flame_thrower", Content.Load<SoundEffect>(@"SoundEffects\flame_thrower"));
            soundList.Add("freezer", Content.Load<SoundEffect>(@"SoundEffects\freezer"));
            soundList.Add("grenade_launch", Content.Load<SoundEffect>(@"SoundEffects\grenade_launch"));
            soundList.Add("minigun", Content.Load<SoundEffect>(@"SoundEffects\minigun"));
            soundList.Add("pickup", Content.Load<SoundEffect>(@"SoundEffects\pickup"));
            soundList.Add("player_hit", Content.Load<SoundEffect>(@"SoundEffects\player_hit"));
            soundList.Add("rifle", Content.Load<SoundEffect>(@"SoundEffects\rifle"));
            soundList.Add("rocket_launch", Content.Load<SoundEffect>(@"SoundEffects\rocket_launch"));
            soundList.Add("shotgun", Content.Load<SoundEffect>(@"SoundEffects\shotgun"));
            soundList.Add("hellgun", Content.Load<SoundEffect>(@"SoundEffects\hellgun"));
            soundList.Add("tommygun", Content.Load<SoundEffect>(@"SoundEffects\tommygun"));
            soundList.Add("uzi", Content.Load<SoundEffect>(@"SoundEffects\uzi"));
            soundList.Add("pistol", Content.Load<SoundEffect>(@"SoundEffects\pistol"));

            Dictionary<string, SoundEffectProperties> propertiesList = new Dictionary<string, SoundEffectProperties>();
            XmlSerializer xml = new XmlSerializer(typeof(GameSoundEffects));
            StreamReader reader = new StreamReader(@"Content\SoundEffects\SoundEffectsProperties.xml");
            GameSoundEffects tempList = (GameSoundEffects)xml.Deserialize(reader);
            reader.Close();

            for (int i = 0; i < tempList.properties.Length; i++)
            {
                propertiesList.Add(tempList.properties[i].Name, tempList.properties[i]);
            }

            SoundManager.LoadContent(musicList, soundList, propertiesList);

            SoundManager.MusicOn = Properties.Settings.Default.Music;
            SoundManager.SoundOn = Properties.Settings.Default.Sound;

            #endregion
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        private void SettingsChanged()
        {
            if (Properties.Settings.Default.Music != SoundManager.MusicOn)
            {
                SoundManager.StopMusic();
            }

            Properties.Settings.Default.FullScreen = GlobalVariables.IsFullScreen;
            Properties.Settings.Default.Music = SoundManager.MusicOn;
            Properties.Settings.Default.Sound = SoundManager.SoundOn;
            Properties.Settings.Default.Controls = GlobalVariables.Controls == ControlType.WASD;

            if (graphics.IsFullScreen != GlobalVariables.IsFullScreen)
            {
                graphics.IsFullScreen = GlobalVariables.IsFullScreen;
                graphics.ApplyChanges();
            }

            Properties.Settings.Default.Save();
        }

        private void RevertSettings()
        {
            GlobalVariables.IsFullScreen = Properties.Settings.Default.FullScreen;
            SoundManager.MusicOn = Properties.Settings.Default.Music;
            SoundManager.SoundOn = Properties.Settings.Default.Sound;
            GlobalVariables.Controls = (Properties.Settings.Default.Controls ? ControlType.WASD : ControlType.Arrows);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Pause)
            {
                return;
            }

            //if (Input.CheckKeyboardButton(Keys.F11) == GameButtonState.Pressed)
            //{
            //    graphics.IsFullScreen = !graphics.IsFullScreen;
            //    graphics.ApplyChanges();
            //}

            OGE.Update(gameTime);
            
            SteamManager.Update();

            base.Update(gameTime);

            //Console.WriteLine(1 / gameTime.ElapsedGameTime.TotalSeconds);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            OGE.DrawFrameRate = (int)(1 / gameTime.ElapsedGameTime.TotalSeconds);
            OGE.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
