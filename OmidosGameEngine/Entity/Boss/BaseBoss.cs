using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Graphics;
using OmidosGameEngine.Tween;
using OmidosGameEngine.Collision;
using OmidosGameEngine.World;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Entity.Player;
using OmidosGameEngine.Sounds;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Entity.OverLayer;

namespace OmidosGameEngine.Entity.Boss
{
    public class BaseBoss : BaseEntity
    {
        protected Dictionary<BossState, Image> images;
        protected Dictionary<BossState, Action> actions;

        private Image freezeImage;
        private Color MainColor;

        protected BossState status;

        protected float health;
        protected float maxHealth;

        protected bool isHit;
        protected Alarm hitAlarm;

        protected float acceleration;
        protected float speed;
        protected float maxSpeed;

        protected float damage;
        protected float damagePercentage;
        protected float slidingFactor;
        protected Color enemyColor;
        protected float freezeResistance;
        protected float freezeReviving;
        protected int score;

        protected bool enableReflection = true;

        public float SlowFactor
        {
            set;
            get;
        }

        public float Direction
        {
            set;
            get;
        }

        public Image CurrentImage
        {
            set;
            get;
        }
        
        public BaseBoss(Vector2 startingPosition)
        {
            this.Position = startingPosition;

            this.damagePercentage = 1f;
            this.MainColor = Color.White;
            this.freezeResistance = 0.8f;
            this.freezeReviving = 0.005f;
            this.slidingFactor = 1;
            this.Direction = OGE.Random.Next(360);
            this.score = 0;
            this.MainColor = Color.White;
            this.status = BossState.Enterance;
            this.isHit = false;
            this.SlowFactor = 1;
            this.hitAlarm = new Alarm(0.2f, TweenType.OneShot, () => { isHit = false; });
            AddTween(hitAlarm);

            this.images = new Dictionary<BossState, Image>();
            this.actions = new Dictionary<BossState, Action>();

            this.freezeImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Bosses\Ice"));
            this.freezeImage.CenterOrigin();

            this.EntityCollisionType = CollisionType.Boss;
        }

        public override void Intialize()
        {
            base.Intialize();

            HUDEntity.BossMaxHealth = (int)Math.Ceiling(maxHealth);
        }

        public virtual void BossHit(float damage, float speed, float direction, bool enableHitAlarm = false)
        {
            if (isHit && enableHitAlarm)
            {
                return;
            }

            isHit = true;
            hitAlarm.Start();
            health -= damage * (2 - SlowFactor);
            if (health <= 0)
            {
                health = 0;
                DestroyBoss();
                return;
            }

            if (speed * slidingFactor > 0)
            {
                Vector2 speedVector = OGE.GetProjection(this.speed, this.Direction) + OGE.GetProjection(speed * slidingFactor, direction);
                this.speed = speedVector.Length();

                this.Direction = OGE.GetAngle(Vector2.Zero, speedVector);
                CheckReflection();
            }
        }

        public virtual void DestroyBoss()
        {
            GlobalVariables.LevelScore += score;

            SoundManager.EmitterPosition = Position;
            SoundManager.PlaySFX("explosion");

            (OGE.CurrentWorld as GameplayWorld).ClearArea();
            OGE.CurrentWorld.RemoveEntity(this);
        }

        private void CheckReflection()
        {
            if (status == BossState.Enterance || !enableReflection)
            {
                return;
            }

            Vector2 speedVector = OGE.GetProjection(speed, Direction);

            if (Position.X + speedVector.X < 0 || Position.X + speedVector.X > OGE.CurrentWorld.Dimensions.X)
            {
                speedVector.X *= -1;
                Direction = OGE.GetAngle(Vector2.Zero, speedVector);
            }

            if (Position.Y + speedVector.Y < 0 || Position.Y + speedVector.Y > OGE.CurrentWorld.Dimensions.Y)
            {
                speedVector.Y *= -1;
                Direction = OGE.GetAngle(Vector2.Zero, speedVector);
            }
        }

        protected virtual void UpdatePhysics()
        {
            CheckReflection();

            speed += acceleration * SlowFactor * OGE.EnemySlowFactor;
            if (speed > maxSpeed * OGE.EnemySlowFactor * SlowFactor)
            {
                speed = maxSpeed * OGE.EnemySlowFactor * SlowFactor;
            }

            speed -= OGE.Friction * OGE.EnemySlowFactor;
            if (speed < 0)
            {
                speed = 0;
            }

            Vector2 speedVector = OGE.GetProjection(speed, Direction);
            Position.X += speedVector.X;
            Position.Y += speedVector.Y;

            CheckCollision();
        }

        protected virtual void DoEffectOnPlayer(PlayerEntity player)
        {
        }

        protected virtual void CheckCollision()
        {
            PlayerEntity player = Collide(CollisionType.Player, Position) as PlayerEntity;
            if (player != null)
            {
                player.PlayerHit(damage, damagePercentage * damage, OGE.GetAngle(Position, player.Position));
                DoEffectOnPlayer(player);
            }
        }

        public override void Update(GameTime gameTime)
        {
            CurrentImage = images[status];
            actions[status]();

            SlowFactor = (float)MathHelper.Clamp(SlowFactor + freezeReviving, freezeResistance, 1);

            UpdatePhysics();

            HUDEntity.BossCurrentHealth += (int)Math.Ceiling(health);

            base.Update(gameTime);
        }

        public override void Draw(Camera camera)
        {
            if(CurrentImage != null)
            {
                if (isHit)
                {
                    CurrentImage.TintColor = new Color(255, 150, 150);
                }
                else
                {
                    CurrentImage.TintColor = MainColor;
                }
                CurrentImage.Draw(Position, camera);
            }

            base.Draw(camera);

            this.freezeImage.TintColor = Color.White * (1 - SlowFactor);
            this.freezeImage.Draw(Position, camera);
        }
    }
}
