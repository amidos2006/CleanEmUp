using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Collision;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace OmidosGameEngine.Entity.Boss
{
    public class HackintoshBossFan:BaseBoss
    {
        private float rotationSpeed = 20;

        public float Scale
        {
            get
            {
                return CurrentImage.ScaleX;
            }
        }

        public HackintoshBossFan(Vector2 position, float damage)
            :base(new Vector2(position.X, position.Y))
        {
            this.damage = damage;
            status = BossState.Rotate;
            images.Add(BossState.Rotate,new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Bosses\HackintoshBossFan")));
            foreach (Image image in this.images.Values)
            {
                image.CenterOrigin();
            }

            this.actions.Add(BossState.Rotate, Rotate);

            CurrentImage = images[status];

            AddCollisionMask(new HitboxMask(48, 48, 24, 24));
        }

        public override void BossHit(float damage, float speed, float direction, bool enableHitAlarm = false)
        {

        }

        private void Rotate() 
        {
            CurrentImage.Angle = (CurrentImage.Angle + rotationSpeed * OGE.EnemySlowFactor) % 360;
        }
    }
}
