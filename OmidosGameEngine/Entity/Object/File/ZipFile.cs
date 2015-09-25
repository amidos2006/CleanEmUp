using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Tween;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Collision;
using OmidosGameEngine.Sounds;
using OmidosGameEngine.World;

namespace OmidosGameEngine.Entity.Object.File
{
    public class ZipFile : BaseFile
    {
        private Image healthBarImage;
        private Image healthBarBGImage;

        private float health;
        private float maxHealth;

        private Alarm hitAlarm;
        private bool isHit;

        private Color hitColor;
        private Color normalColor;

        public ZipFile()
        {
            health = 200;
            maxHealth = 200;

            hitAlarm = new Alarm(0.3f, TweenType.OneShot, () => { isHit = false; });
            AddTween(hitAlarm);

            healthBarBGImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Objects\HealthBG"));
            healthBarImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Objects\Health"));

            healthBarBGImage.CenterOrigin();
            healthBarImage.CenterOrigin();
            healthBarImage.Scale = 0;
            healthBarBGImage.Scale = 0;

            normalImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Objects\ZipFile"));
            normalImage.CenterOrigin();
            normalImage.Scale = 0;

            CurrentImages.Add(new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Objects\ZipFile")));
            CurrentImages[0].CenterOrigin();

            hitColor = new Color(255, 80, 80);
            normalColor = Color.White;

            AddCollisionMask(new HitboxMask(normalImage.Width, normalImage.Height, normalImage.OriginX, normalImage.OriginY));
        }

        public override void Intialize()
        {
            base.Intialize();

            FileNotifierEntity fileNotifier = new FileNotifierEntity();
            fileNotifier.Position.X = Position.X;
            fileNotifier.Position.Y = Position.Y;

            OGE.CurrentWorld.AddEntity(fileNotifier);
        }

        protected override void DestroyFile()
        {
            base.DestroyFile();

            FileNotifierEntity fileNotifier = new FileNotifierEntity();
            fileNotifier.Position.X = Position.X;
            fileNotifier.Position.Y = Position.Y;

            OGE.CurrentWorld.AddEntity(fileNotifier);
        }

        protected override void EnemyCollide(Enemy.BaseEnemy e)
        {
            if (isHit)
            {
                return;
            }

            health -= e.Damage;
            isHit = true;
            hitAlarm.Start();
            e.EnemyHit(0, 10, OGE.GetAngle(Position, e.Position));

            SoundManager.EmitterPosition = Position;
            SoundManager.PlaySFX("file_collision");

            if (health < 0)
            {
                (OGE.CurrentWorld as GameplayWorld).NumberOfDestroyedZip += 1;
                DestroyFile();
            }
        }

        protected override void ExplosionCollision(Explosion.BaseExplosion e)
        {
            if (!e.FriendlyExplosion)
            {
                health -= e.GetDamageAccordingToPosition(Position);
                if (health < 0)
                {
                    (OGE.CurrentWorld as GameplayWorld).NumberOfDestroyedZip += 1;
                    DestroyFile();
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            healthBarImage.Scale = scale;
            healthBarBGImage.Scale = scale;
        }

        public override void Draw(Camera camera)
        {
            if (isHit)
            {
                normalImage.TintColor = hitColor;
            }
            else
            {
                normalImage.TintColor = normalColor;
            }

            base.Draw(camera);
            
            Vector2 healthBarPosition = new Vector2(Position.X - 1, Position.Y);
            healthBarPosition.Y += normalImage.Height / 2 + 8;

            healthBarBGImage.Draw(healthBarPosition, camera);
            healthBarImage.SourceRectangle = new Rectangle(0, 0, (int)((health / maxHealth) * healthBarBGImage.Width), 
                healthBarBGImage.Height);
            healthBarImage.Draw(healthBarPosition, camera);
        }
    }
}
