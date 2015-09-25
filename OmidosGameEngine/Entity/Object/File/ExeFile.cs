using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Entity.Enemy;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Collision;
using OmidosGameEngine.Tween;
using OmidosGameEngine.Sounds;

namespace OmidosGameEngine.Entity.Object.File
{
    public class ExeFile : BaseFile
    {
        private const float REPLICATE_TIME = 6;
        private const float MAX_FLICKER_TIME = 0.5f;

        private Image virusInfectedImage;
        private Image trojanInfectedImage;

        private float infectedDamage;
        private float health;
        private float maxHealth;
        private bool flicked;
        private int rescuedFileScore;

        private Type infectedType;
        private Alarm flickerAlarm;
        private Alarm replicateAlarm;

        public bool IsInfected
        {
            get
            {
                return status == FileStatus.Infected;
            }
        }

        public ExeFile()
        {
            infectedDamage = 25;
            infectedType = null;

            maxHealth = 50;
            health = maxHealth;

            rescuedFileScore = 100;

            flicked = false;

            normalImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Objects\ExeFile"));
            virusInfectedImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Objects\InfectedFile"));
            trojanInfectedImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Objects\InfectedFileTrojan"));

            CurrentImages.Add(new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Objects\ExeFile")));
            CurrentImages[0].CenterOrigin();

            normalImage.CenterOrigin();
            virusInfectedImage.CenterOrigin();
            trojanInfectedImage.CenterOrigin();

            normalImage.Scale = 0;
            virusInfectedImage.Scale = 0;
            trojanInfectedImage.Scale = 0;

            AddCollisionMask(new HitboxMask(normalImage.Width, normalImage.Height, normalImage.OriginX, normalImage.OriginY));

            replicateAlarm = new Alarm(REPLICATE_TIME, TweenType.OneShot, ReplicateAndDestroy);
            flickerAlarm = new Alarm(MAX_FLICKER_TIME, TweenType.OneShot, FlickColor);

            AddTween(replicateAlarm);
            AddTween(flickerAlarm);
        }

        public void AddScore()
        {
            GlobalVariables.LevelScore += rescuedFileScore;
        }

        protected override void EnemyCollide(BaseEnemy e)
        {
            if (status == FileStatus.Infected)
            {
                return;
            }

            if (e is VirusEnemy || e is TroyEnemy)
            {
                GetInfected(e.GetType());
            }
        }

        private void GetInfected(Type enemyType)
        {
            GenerateExplosion();
            infectedType = enemyType;

            if (infectedType == typeof(TroyEnemy))
            {
                infectedImage = trojanInfectedImage;
            }
            else
            {
                infectedImage = virusInfectedImage;
            }

            status = FileStatus.Infected;

            flickerAlarm.Start(true);
            replicateAlarm.Start(true);

            flicked = false;

            health = maxHealth;
        }

        private void ReplicateAndDestroy()
        {
            BaseEnemy enemy = Activator.CreateInstance(infectedType) as BaseEnemy;
            enemy.Position = new Vector2(Position.X, Position.Y);

            OGE.CurrentWorld.AddEntity(enemy);

            DestroyFile();
        }

        private void FlickColor()
        {
            if (flicked)
            {
                infectedImage.TintColor = Color.White;
            }
            else
            {
                infectedImage.TintColor = new Color(150, 150, 150);
            }

            flicked = !flicked;
            flickerAlarm.Reset(MAX_FLICKER_TIME * (1 - replicateAlarm.PercentComplete()) + MAX_FLICKER_TIME / 10);
            flickerAlarm.Start();
        }

        private void ReturnNormal()
        {
            GenerateExplosion();
            flickerAlarm.Stop();
            replicateAlarm.Stop();
            status = FileStatus.Normal;
        }

        public void LevelEnded()
        {
            if (infectedImage != null)
            {
                infectedImage.TintColor = Color.White;
            }
            replicateAlarm.Stop();
            flickerAlarm.Stop();
        }

        protected override void PlayerCollide(Player.PlayerEntity p)
        {
            if (status == FileStatus.Infected)
            {
                p.PlayerHit(infectedDamage, 0.8f * infectedDamage, OGE.GetAngle(Position, p.Position));
            }
        }

        protected override void BulletCollide(Player.Bullet.PlayerBullet b)
        {
            if (status == FileStatus.Infected)
            {
                b.DestroyBulletCollision(this);

                SoundManager.EmitterPosition = Position;
                SoundManager.PlaySFX("bullet_collision");

                health -= b.Damage;
                if (health < 0)
                {
                    ReturnNormal();
                }
            }
        }

        protected override void ExplosionCollision(Explosion.BaseExplosion e)
        {
            if (status == FileStatus.Infected)
            {
                health -= e.GetDamageAccordingToPosition(Position);
                if (health < 0)
                {
                    ReturnNormal();
                }
            }
        }
    }
}
