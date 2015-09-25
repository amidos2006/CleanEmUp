using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Graphics;

namespace OmidosGameEngine.Collision
{
    public static class Collision
    {
        public static float AlphaThreshold
        {
            set;
            get;
        }

        public static bool HitBoxCollision(Rectangle hitboxA, Rectangle hitboxB, Vector2 positionA, Vector2 positionB)
        {
            hitboxA.X += (int)positionA.X;
            hitboxA.Y += (int)positionA.Y;

            hitboxB.X += (int)positionB.X;
            hitboxB.Y += (int)positionB.Y;

            return hitboxA.Intersects(hitboxB);
        }

        public static bool PerPixelCollision(Image imageA, Image imageB, Vector2 positionA, Vector2 positionB)
        {
            // Data for each pixel.  
            Color[] imageAData = new Color[imageA.Texture.Width * imageA.Texture.Height];
            imageA.Texture.GetData(imageAData);

            Color[] imageBData = new Color[imageB.Texture.Width * imageB.Texture.Height];
            imageB.Texture.GetData(imageBData);

            // Transform A.  
            Matrix imageAMatrix = Matrix.CreateTranslation(new Vector3(-new Vector2(imageA.OriginX, imageA.OriginY), 0.0f)) *
                Matrix.CreateScale(new Vector3(imageA.ScaleX,imageA.ScaleY,0)) *
                Matrix.CreateRotationZ(imageA.Angle) *
                Matrix.CreateTranslation(new Vector3(positionA, 0.0f));

            // Transform B.  
            Matrix imageBMatrix = Matrix.CreateTranslation(new Vector3(-new Vector2(imageB.OriginX, imageB.OriginY), 0.0f)) *
                Matrix.CreateScale(new Vector3(imageB.ScaleX, imageB.ScaleY, 0)) *
                Matrix.CreateRotationZ(imageB.Angle) *
                Matrix.CreateTranslation(new Vector3(positionB, 0.0f));

            Matrix transformAtoB = imageAMatrix * Matrix.Invert(imageBMatrix);

            for (int yA = 0; yA < imageA.Texture.Height; yA++)
            {
                for (int xA = 0; xA < imageB.Texture.Width; xA++)
                {
                    Vector2 positionInB = Vector2.Transform(new Vector2(xA, yA), transformAtoB);

                    int xB = (int)Math.Round(positionInB.X);
                    int yB = (int)Math.Round(positionInB.Y);

                    if (0 <= xB && xB < imageB.Texture.Width &&
                        0 <= yB && yB < imageB.Texture.Height)
                    {
                        // Get the colors of the overlapping pixels  
                        Color colorA = imageAData[xA + yA * imageA.Texture.Width];
                        Color colorB = imageBData[xB + yB * imageB.Texture.Width];

                        if (colorA.A > AlphaThreshold && colorB.A > AlphaThreshold)
                        {
                            // Collision occured.  
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
