using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Entity.Player.OverClocking;
using OmidosGameEngine.Entity.Player.Bullet;

namespace OmidosGameEngine.Entity.Player.Data
{
    public class OmarData : PlayerData
    {
        public OmarData()
        {
            Speed = 2.5f * SPEED_UNIT;
            Accuracy = 0;
            Health = 70 * HEALTH_UNIT;
        }

        public override void LoadContent()
        {
            BodyImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Players\OmarBody"));
            BodyImage.OriginX = BodyImage.Width / 2;
            BodyImage.OriginY = BodyImage.Height;

            UpThrusterPosition.Add(new Vector2(-8, -35));
            UpThrusterPosition.Add(new Vector2(8, -35));

            LeftThrusterPosition.Add(new Vector2(-35, -7));
            RightThrusterPosition.Add(new Vector2(35, -7));
        }

        public override EffectArea GetOverclocking()
        {
            return new HealEffectArea(new Color(150, 255, 130), 0.5f, 8);
        }

        public override PlayerData Clone()
        {
            OmarData d = new OmarData();
            return base.Clone(d);
        }
    }
}
