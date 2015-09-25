using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Entity.Enemy;
using OmidosGameEngine.Tween;
using OmidosGameEngine.Collision;
using OmidosGameEngine.Entity.ParticleGenerator;
using OmidosGameEngine.Graphics.Particles;
using OmidosGameEngine.Sounds;
using OmidosGameEngine.Entity.OverLayer;

namespace OmidosGameEngine.Entity.Boss
{
    public class DosBoss:BaseBoss
    {
        protected TrailParticleGenerator trailGenerator;
        protected FractionalParticleGenerator fractionalGenerator;

        private Image fanImage;
        private float fanRotationSpeed = 10;
        private DosBossController controller;

        private void IntializeParticleGenerators()
        {
            Particle particlePrototype = new Particle();
            particlePrototype.ParticleColor = enemyColor;
            particlePrototype.DeltaScale = -0.1f;
            particlePrototype.DeltaAlpha = -0.12f;

            this.trailGenerator = new TrailParticleGenerator(OGE.CurrentWorld.TrailEffectSystem, particlePrototype);
            this.trailGenerator.AngleDisplacement = 15;
            this.trailGenerator.Speed = 1.5f;
            this.trailGenerator.Scale = 0.6f;

            Particle prototype = new Particle();
            prototype.DeltaScale = -0.03f;
            prototype.DeltaSpeed = -0.02f;
            prototype.DeltaAngle = 5f;

            this.fractionalGenerator = new FractionalParticleGenerator(OGE.CurrentWorld.ExplosionEffectSystem, prototype, 4);
            this.fractionalGenerator.Speed = 5;
            this.fractionalGenerator.NumberOfCircles = 1;
        }

        public DosBoss(DosBossController bossController)
            : base(new Vector2(OGE.CurrentWorld.Dimensions.X / 2, -100))
        {
            this.controller = bossController;
            
            Texture2D texture = OGE.Content.Load<Texture2D>(@"Graphics\Entities\Bosses\DosBoss");
            this.images.Add(BossState.Move, new Image(texture));

            foreach (Image image in this.images.Values)
            {
                image.CenterOrigin();
            }

            this.actions.Add(BossState.Move, MoveUpdate);

            this.maxHealth = 750;
            this.health = this.maxHealth;

            this.acceleration = 0.2f;
            this.Direction = 90;
            this.maxSpeed = 10 * BaseEnemy.SPEED_UNIT;
            this.speed = 0;
            this.slidingFactor = 0f;
            this.freezeResistance = 1f;

            this.status = BossState.Move;
            this.enableReflection = false;

            this.damage = 60;
            this.enemyColor = new Color(255, 120, 255);
            this.score = 5000;
            this.CurrentImage = images[status];
            
            this.fanImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Bosses\DosBossFan"));
            this.fanImage.CenterOrigin();
            this.enableReflection = false;

            IntializeParticleGenerators();

            AddCollisionMask(new HitboxMask(70, 70, 35, 35));
        }

        public override void Intialize()
        {
            HUDEntity.BossMaxHealth += (int)Math.Ceiling(maxHealth);
        }

        public override void DestroyBoss()
        {
            GlobalVariables.LevelScore += score;

            fractionalGenerator.FractionTexture = CurrentImage.Texture;
            fractionalGenerator.GenerateParticles(Position);

            SoundManager.EmitterPosition = Position;
            SoundManager.PlaySFX("enemy_destroy");

            OGE.CurrentWorld.AdditiveWhite.Alpha += 0.2f;

            controller.RemoveBoss(this);
        }

        private void MoveUpdate()
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            fanImage.Angle = (fanImage.Angle + fanRotationSpeed * OGE.EnemySlowFactor) % 360;

            foreach (Image image in images.Values)
            {
                image.Angle = Direction;
            }

            trailGenerator.Angle = Direction + 180;
            trailGenerator.GenerateParticles(Position + OGE.GetProjection(50, Direction + 180));
        }

        public override void Draw(Camera camera)
        {
            fanImage.Draw(Position, camera);
            base.Draw(camera);
        }
    }
}
