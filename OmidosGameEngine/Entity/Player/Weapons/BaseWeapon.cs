using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Tween;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Collision;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Audio;
using OmidosGameEngine.Sounds;
using OmidosGameEngine.Entity.Cursor;

namespace OmidosGameEngine.Entity.Player.Weapons
{
    public class BaseWeapon : IWeapon , ILogical
    {
        private SoundEffectInstance runningSound;

        protected Alarm refreshShotgun;
        protected bool shootBullet;
        protected int numberOfBullets;
        protected float bulletSpeed;
        protected float processorTime;
        protected float accuracy;
        protected float maxDistance;
        protected Texture2D texture;
        protected IMask baseMask;
        protected Random random;

        public Image TurnetImage
        {
            set;
            get;
        }

        private float processor;
        public float Processor
        {
            set
            {
                processor = value;
                if (processor > 100)
                {
                    processor = 100;
                }
                if (processor < 0)
                {
                    processor = 0;
                }
            }
            get
            {
                return processor;
            }
        }

        public bool CooldownProcessor
        {
            set;
            get;
        }

        public float ProcessorCoolDown
        {
            set;
            get;
        }

        public string GunName
        {
            set;
            get;
        }

        public BaseWeapon(float refreshTime)
        {
            random = new Random();

            refreshShotgun = new Alarm(refreshTime, TweenType.OneShot, new AlarmFinished(RefreshWeapon));
            numberOfBullets = 1;
            bulletSpeed = 5;
            processorTime = 5;
            accuracy = 5;
            maxDistance = OGE.WorldCamera.Width;
            shootBullet = true;

            this.ProcessorCoolDown = 0.5f;
            this.CooldownProcessor = false;
            this.Processor = 0;
        }

        public virtual void LoadContent()
        {
            
        }

        public virtual void PlaySound(string soundName, bool isLooping = false)
        {
            if (runningSound == null)
            {
                if (isLooping)
                {
                    runningSound = SoundManager.PlaySFX(soundName);
                }
                else
                {
                    SoundManager.PlaySFX(soundName);
                }
            }
            else
            {
                SoundManager.Apply3D(runningSound);
            }
        }

        public virtual void StopSound()
        {
            if (runningSound != null)
            {
                SoundManager.StopSfx(runningSound);
                runningSound = null;
            }
        }

        protected virtual void RefreshWeapon()
        {
            shootBullet = true;
        }

        public virtual bool GenerateBullet(Vector2 position, float direction, float bonusAccuracy)
        {
            if(shootBullet && !CooldownProcessor)
            {
                refreshShotgun.Start(true);
                Processor += processorTime;
                shootBullet = false;
                CursorEntity.IsShooting = true;

                return true;
            }

            if (CooldownProcessor)
            {
                StopSound();
            }
            
            CursorEntity.IsShooting = false;
            return false;
        }

        public void Update(GameTime gameTime)
        {
            refreshShotgun.Update(gameTime);
            refreshShotgun.SpeedFactor = OGE.PlayerSlowFactor;

            if (Processor >= 100)
            {
                CooldownProcessor = true;
            }

            if (CooldownProcessor && Processor == 0)
            {
                CooldownProcessor = false;
            }

            if (CooldownProcessor)
            {
                Processor -= 1.6f * ProcessorCoolDown;
            }
            else
            {
                Processor -= ProcessorCoolDown;
            }
        }
    }
}
