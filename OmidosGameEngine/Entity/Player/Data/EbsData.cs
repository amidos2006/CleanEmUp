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
    public class EbsData : PlayerData
    {
        public EbsData()
        {
            Speed = 3f * SPEED_UNIT;
            Accuracy = 0;
            Health = 90 * HEALTH_UNIT;
        }

        public override void LoadContent()
        {
            BodyImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Players\EbsBody"));
            BodyImage.OriginX = BodyImage.Width / 2;
            BodyImage.OriginY = BodyImage.Height;

            UpThrusterPosition.Add(new Vector2(-8, -35));
            UpThrusterPosition.Add(new Vector2(8, -35));

            LeftThrusterPosition.Add(new Vector2(-35, -7));
            RightThrusterPosition.Add(new Vector2(35, -7));
        }

        public override EffectArea GetOverclocking()
        {
            return new PPEffectArea(new Color(255, 220, 130), 8);
        }

        public override PlayerData Clone()
        {
            EbsData d = new EbsData();
            return base.Clone(d);
        }
    }
}
