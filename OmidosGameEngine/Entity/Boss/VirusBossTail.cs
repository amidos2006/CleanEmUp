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
    public class VirusBossTail:BaseBoss
    {
        private VirusBoss boss;

        public bool IsHit
        {
            set
            {
                isHit = value;
            }
        }

        public float Scale
        {
            get
            {
                return CurrentImage.ScaleX;
            }
        }

        public VirusBossTail(VirusBoss boss, int index, int length)
            :base(new Vector2(boss.Position.X, boss.Position.Y))
        {
            this.boss = boss;
            this.damage = boss.Damage / 2;
            status = BossState.Move;
            images.Add(BossState.Move,new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Bosses\VirusBossTail")));
            foreach (Image image in this.images.Values)
            {
                image.CenterOrigin();
                image.Scale = 0.2f + 0.8f * (1 - index * 1.0f / length);
            }

            this.actions.Add(BossState.Move, DoNothing);

            CurrentImage = images[status];

            AddCollisionMask(new HitboxMask((int)(CurrentImage.ScaleX * 38), (int)(CurrentImage.ScaleY * 38), 
                (int)(CurrentImage.ScaleX * 38 / 2), (int)(CurrentImage.ScaleY * 38 / 2)));
        }

        public override void BossHit(float damage, float speed, float direction, bool enableHitAlarm = false)
        {
            boss.BossHit(damage, speed, direction, enableHitAlarm);
        }

        private void DoNothing() 
        {  
        }
    }
}
