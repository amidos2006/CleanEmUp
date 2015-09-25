using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace OmidosGameEngine.Entity.OverLayer
{
    public class OverclockingBarFilledEntity:BaseEntity
    {
        private Image barFilledImage;
        private float scaleSpeed;
        private float scale;
        private float alphaSpeed;
        private float alpha;
        private Color normalColor;

        public OverclockingBarFilledEntity()
        {
            alpha = 1;
            scale = 1;
            alphaSpeed = 0.05f;
            scaleSpeed = 0.025f;
            normalColor = new Color(150, 255, 130);

            barFilledImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\HUD\Rage"));
            barFilledImage.TintColor = normalColor;
            barFilledImage.CenterOrigin();
            barFilledImage.OriginY += 1; 

            Position.X = OGE.HUDCamera.Width / 2;
            Position.Y = OGE.HUDCamera.Height - barFilledImage.OriginY - 5;

            CurrentImages.Add(barFilledImage);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            alpha -= alphaSpeed;
            if (alpha <= 0)
            {
                OGE.CurrentWorld.RemoveOverLayer(this);
            }

            CurrentImages[0].TintColor = normalColor * alpha;

            scale += scaleSpeed;
            CurrentImages[0].Scale = scale;
        }
    }
}
