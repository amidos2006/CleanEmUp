using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Graphics.Particles;
using OmidosGameEngine.Entity;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Collision;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Graphics.Lighting;
using BloomPostprocess;
using OmidosGameEngine.Graphics;
using OmidosGameEngine.Tween;
using OmidosGameEngine.Sounds;
using OmidosGameEngine.Entity.Cursor;
using OmidosGameEngine.Entity.OverLayer;

namespace OmidosGameEngine.World
{
    public class BaseWorld : ILogical
    {
        protected LightingSystem lightingSystem;
        protected List<Backdrop> backdrops;
        protected ParticleEffectSystem trailEffectSystem;
        protected Dictionary<CollisionType, List<BaseEntity>> collisionEntities;
        protected List<BaseEntity> removedEntities;
        protected List<BaseEntity> addedEntities;
        protected List<BaseEntity> addedOverlayerEntities;
        protected List<BaseEntity> overScreenLayer;
        protected ParticleEffectSystem explosionEffectSystem;
        protected BloomComponent bloomPostProcess;
        protected AdditiveWhiteImage additiveWhiteLayer;
        protected RedHitEffectImage redHitEffectLayer;
        protected HackedNoiseEffect noiseEffect;
        protected Alarm slowDownAlarm;
        protected float musicVolume;
        protected bool pauseGame;
        private bool transitionEnd = false;
        private float alpha = 1;
        private float scale = 1;
        private float transitionSpeed = 0.03f;

        public Vector2 Dimensions
        {
            set;
            get;
        }

        public ParticleEffectSystem TrailEffectSystem
        {
            get
            {
                return trailEffectSystem;
            }
        }

        public ParticleEffectSystem ExplosionEffectSystem
        {
            get
            {
                return explosionEffectSystem;
            }
        }

        public LightingSystem LightingEffectSystem
        {
            get
            {
                return lightingSystem;
            }
        }

        public AdditiveWhiteImage AdditiveWhite
        {
            get
            {
                return additiveWhiteLayer;
            }
        }

        public RedHitEffectImage RedHitEffect
        {
            get
            {
                return redHitEffectLayer;
            }
        }

        public HackedNoiseEffect NoiseEffect
        {
            get
            {
                return noiseEffect;
            }
        }

        public Texture2D Transition
        {
            set;
            get;
        }

        public BaseEntity PauseEntity
        {
            set;
            get;
        }

        public BaseWorld(Vector2 dimensions, BloomComponent bloomComponent)
        {
            this.Dimensions = dimensions;

            this.Transition = new Texture2D(OGE.GraphicDevice, OGE.HUDCamera.Width, OGE.HUDCamera.Height);

            this.bloomPostProcess = bloomComponent;
        }

        public virtual void Intialize()
        {
            GlobalVariables.TrailParticleEffectSystem.RemoveAllParticles();
            GlobalVariables.ExplosionParticleEffectSystem.RemoveAllParticles();

            trailEffectSystem = GlobalVariables.TrailParticleEffectSystem;
            explosionEffectSystem = GlobalVariables.ExplosionParticleEffectSystem;
            lightingSystem = GlobalVariables.LightingSystemLayer;
            lightingSystem.AmbientLight = 0.3f;
            additiveWhiteLayer = GlobalVariables.AdditiveWhiteLightLayer;
            redHitEffectLayer = new RedHitEffectImage(-0.015f);
            noiseEffect = new HackedNoiseEffect(-0.015f);
            bloomPostProcess.Initialize();

            backdrops = new List<Backdrop>();
            removedEntities = new List<BaseEntity>();
            addedEntities = new List<BaseEntity>();
            addedOverlayerEntities = new List<BaseEntity>();
            overScreenLayer = new List<BaseEntity>();

            collisionEntities = new Dictionary<CollisionType, List<BaseEntity>>();
            Array array = Enum.GetValues(typeof(CollisionType));
            foreach (CollisionType collisionType in array)
            {
                collisionEntities.Add(collisionType, new List<BaseEntity>());
            }

            slowDownAlarm = new Alarm(0, TweenType.OneShot, new AlarmFinished(ReturnNormal));

            OGE.WorldCamera.WorldDimensions = Dimensions;
            SoundManager.ClearSfx();

            this.pauseGame = false;
            this.musicVolume = SoundManager.MAX_VOLUME;
        }

        public virtual void LoadContent()
        {

        }

        public virtual void PauseGame()
        {
            pauseGame = true;
            musicVolume = SoundManager.ChangeMusicVolume(SoundManager.MIN_VOLUME);
            SoundManager.PauseSFX();
        }

        public virtual void UnPauseGame()
        {
            pauseGame = false;
            SoundManager.ChangeMusicVolume(musicVolume);
            SoundManager.ResumeSFX();
        }

        public void SlowMode(double time)
        {
            OGE.PlayerSlowFactor = 0.75f;
            OGE.EnemySlowFactor = 0.5f;

            SoundManager.ChangeMusicVolume(SoundManager.MIN_VOLUME);

            slowDownAlarm.Reset(time);
            slowDownAlarm.Start();
        }

        protected void ReturnNormal()
        {
            OGE.PlayerSlowFactor = 1;
            OGE.EnemySlowFactor = 1;

            SoundManager.ChangeMusicVolume(SoundManager.MAX_VOLUME);
        }

        public void AddBackground(Backdrop backdrop)
        {
            backdrops.Add(backdrop);
        }

        public void RemoveBackground(Backdrop backdrop)
        {
            backdrops.Remove(backdrop);
        }

        public void AddOverLayer(BaseEntity entity)
        {
            entity.Intialize();
            entity.LoadContent();

            addedOverlayerEntities.Add(entity);
        }

        public void RemoveOverLayer(BaseEntity entity)
        {
            removedEntities.Add(entity);
        }

        public void AddEntity(BaseEntity entity)
        {
            entity.Intialize();
            entity.LoadContent();

            addedEntities.Add(entity);
        }

        public void RemoveEntity(BaseEntity entity)
        {
            removedEntities.Add(entity);
        }

        public void RemoveAllEntities()
        {
            foreach (KeyValuePair<CollisionType, List<BaseEntity>> list in collisionEntities)
            {
                list.Value.Clear();
            }
        }

        public List<BaseEntity> GetCollisionEntitiesType(CollisionType collisionType)
        {
            List<BaseEntity> entities = new List<BaseEntity>(collisionEntities[collisionType]);

            foreach (BaseEntity entity in removedEntities)
            {
                entities.Remove(entity);
            }

            return entities;
        }

        private void CleanUpWorld()
        {
            foreach (KeyValuePair<CollisionType, List<BaseEntity>> list in collisionEntities)
            {
                foreach (BaseEntity entity in removedEntities)
                {
                    list.Value.Remove(entity);
                }
            }

            foreach (BaseEntity entity in removedEntities)
            {
                overScreenLayer.Remove(entity);
            }

            removedEntities.Clear();
        }

        private void AddToWorld()
        {
            foreach (BaseEntity entity in addedEntities)
            {
                collisionEntities[entity.EntityCollisionType].Add(entity);
            }

            foreach (BaseEntity entity in addedOverlayerEntities)
            {
                overScreenLayer.Add(entity);
            }

            addedEntities.Clear();
            addedOverlayerEntities.Clear();
        }

        public virtual void Update(GameTime gameTime)
        {
            if (Transition != null && !transitionEnd)
            {
                alpha -= transitionSpeed;
                scale += transitionSpeed;
                if (alpha <= 0)
                {
                    alpha = 0;
                    transitionEnd = true;
                }
            }

            if (pauseGame)
            {
                if (PauseEntity != null)
                {
                    PauseEntity.Update(gameTime);
                }
                return;
            }

            slowDownAlarm.Update(gameTime);

            foreach (Backdrop backdrop in backdrops)
            {
                backdrop.Update(gameTime);
            }

            foreach (KeyValuePair<CollisionType,List<BaseEntity>> list in collisionEntities)
            {
                foreach (BaseEntity entity in list.Value)
                {
                    if(!removedEntities.Contains(entity))
                    {
                        entity.Update(gameTime);
                    }
                }
            }

            foreach (BaseEntity overLayer in overScreenLayer)
            {
                if (!removedEntities.Contains(overLayer))
                {
                    overLayer.Update(gameTime);
                }
            }

            CleanUpWorld();
            AddToWorld();

            trailEffectSystem.Update(gameTime);
            explosionEffectSystem.Update(gameTime);
            lightingSystem.Update(gameTime);
            additiveWhiteLayer.Update(gameTime);
            redHitEffectLayer.Update(gameTime);
            noiseEffect.Update(gameTime);
            bloomPostProcess.Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime)
        {
            explosionEffectSystem.BeginDraw(OGE.WorldCamera);
            trailEffectSystem.BeginDraw(OGE.WorldCamera);
            lightingSystem.BeginDraw(OGE.WorldCamera);
            
            bloomPostProcess.BeginDraw();
            OGE.GraphicDevice.Clear(Color.Black);

            foreach (Backdrop backdrop in backdrops)
            {
                backdrop.Draw(new Vector2(0, 0), OGE.WorldCamera);
            }

            lightingSystem.Draw();
            trailEffectSystem.Draw();

            foreach (KeyValuePair<CollisionType, List<BaseEntity>> list in collisionEntities)
            {
                foreach (BaseEntity entity in list.Value)
                {
                    if (!removedEntities.Contains(entity))
                    {
                        entity.Draw(OGE.WorldCamera);
                    }
                }
            }

            explosionEffectSystem.Draw();

            redHitEffectLayer.Draw(new Vector2(),OGE.HUDCamera);
            noiseEffect.Draw(new Vector2(), OGE.HUDCamera);
            additiveWhiteLayer.Draw(new Vector2(), OGE.HUDCamera);

            foreach (BaseEntity overLayer in overScreenLayer)
            {
                overLayer.Draw(OGE.HUDCamera);
            }

            if (PauseEntity != null && pauseGame)
            {
                PauseEntity.Draw(OGE.HUDCamera);
            }

            if (Transition != null && !transitionEnd)
            {
                OGE.SpriteBatch.Begin(OGE.SpriteSortMode, OGE.BlendState);
                OGE.SpriteBatch.Draw(Transition, new Vector2(Transition.Width / 2, Transition.Height / 2), null, Color.White * alpha, 0, 
                    new Vector2(Transition.Width / 2, Transition.Height / 2), scale, SpriteEffects.None, 0);
                OGE.SpriteBatch.End();
            }

            bloomPostProcess.Draw(gameTime);
        }
    }
}
