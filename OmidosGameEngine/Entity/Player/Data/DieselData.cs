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
    public class DieselData : PlayerData
    {
        public DieselData()
        {
            Speed = 1 * SPEED_UNIT;
            Accuracy = 0;
            Health = 200 * HEALTH_UNIT;
        }

        public override void LoadContent()
        {
            BodyImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Players\DieselBody"));
            BodyImage.OriginX = BodyImage.Width / 2;
            BodyImage.OriginY = BodyImage.Height;

            UpThrusterPosition.Add(new Vector2(-8, -35));
            UpThrusterPosition.Add(new Vector2(8, -35));

            LeftThrusterPosition.Add(new Vector2(-35, -7));
            RightThrusterPosition.Add(new Vector2(35, -7));
        }

        public override EffectArea GetOverclocking()
        {
            return new WaveEffectArea(Color.White, 10, 8);
        }

        public override PlayerData Clone()
        {
            DieselData d = new DieselData();
            return base.Clone(d);
        }
    }
}
