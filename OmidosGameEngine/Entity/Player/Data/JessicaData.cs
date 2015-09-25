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
    public class JessicaData : PlayerData
    {
        public JessicaData()
        {
            Speed = 4 * SPEED_UNIT;
            Accuracy = 0;
            Health = 60 * HEALTH_UNIT;
        }

        public override void LoadContent()
        {
            BodyImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Players\JessicaBody"));
            BodyImage.OriginX = BodyImage.Width / 2;
            BodyImage.OriginY = BodyImage.Height;

            UpThrusterPosition.Add(new Vector2(-8, -35));
            UpThrusterPosition.Add(new Vector2(8, -35));

            LeftThrusterPosition.Add(new Vector2(-35, -7));
            RightThrusterPosition.Add(new Vector2(35, -7));
        }

        public override EffectArea GetOverclocking()
        {
            return new FreezeEffectArea(new Color(50, 150, 255), 0.2f, 8);
        }

        public override PlayerData Clone()
        {
            JessicaData d = new JessicaData();
            return base.Clone(d);
        }
    }
}
