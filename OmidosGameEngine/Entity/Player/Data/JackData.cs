using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Entity.Player.Bullet;
using OmidosGameEngine.Entity.Player.OverClocking;

namespace OmidosGameEngine.Entity.Player.Data
{
    public class JackData : PlayerData
    {
        public JackData()
        {
            Speed = 2 * SPEED_UNIT;
            Accuracy = 0;
            Health = 90 * HEALTH_UNIT;
        }

        public override void LoadContent()
        {
            BodyImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Players\JackBody"));
            BodyImage.OriginX = BodyImage.Width / 2;
            BodyImage.OriginY = BodyImage.Height;

            UpThrusterPosition.Add(new Vector2(-8, -35));
            UpThrusterPosition.Add(new Vector2(8, -35));

            LeftThrusterPosition.Add(new Vector2(-35, -7));
            RightThrusterPosition.Add(new Vector2(35, -7));
        }

        public override EffectArea GetOverclocking()
        {
            return new ExplosionEffectArea(new Color(20, 80, 140), 1, 100, 8);
        }

        public override PlayerData Clone()
        {
            JackData d = new JackData();
            return base.Clone(d);
        }
    }
}
