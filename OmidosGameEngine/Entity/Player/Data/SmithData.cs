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
    public class SmithData : PlayerData
    {
        public SmithData()
        {
            Speed = 2 * SPEED_UNIT;
            Accuracy = 0;
            Health = 100 * HEALTH_UNIT;
        }

        public override void LoadContent()
        {
            BodyImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Players\SmithBody"));
            BodyImage.OriginX = BodyImage.Width / 2;
            BodyImage.OriginY = BodyImage.Height;

            UpThrusterPosition.Add(new Vector2(-8, -35));
            UpThrusterPosition.Add(new Vector2(8, -35));

            LeftThrusterPosition.Add(new Vector2(-35, -7));
            RightThrusterPosition.Add(new Vector2(35, -7));
        }

        public override EffectArea GetOverclocking()
        {
            return new GasEffectArea(new Color(255, 50, 50), 20, 8);
        }

        public override PlayerData Clone()
        {
            SmithData d = new SmithData();
            return base.Clone(d);
        }
    }
}
