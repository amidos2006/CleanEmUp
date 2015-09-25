using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace OmidosGameEngine.Entity.Cursor
{
    public static class CursorEntity
    {
        private static Image menuCursorImage;
        private static Image ingameCursorImage;
        private static float rotationSpeed;
        private static float scaleSpeed;
        private static CursorType cursorType;

        public static CursorType CursorView
        {
            set
            {
                cursorType = value;
            }
        }

        public static bool IsShooting
        {
            set;
            get;
        }

        public static void Intialize()
        {
            CursorEntity.cursorType = CursorType.Normal;
            CursorEntity.IsShooting = false;
            CursorEntity.rotationSpeed = 5f;
            CursorEntity.scaleSpeed = 0.1f;

            menuCursorImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Cursor\menuCursor"));
            ingameCursorImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Cursor\ingameCursor"));
            ingameCursorImage.CenterOrigin();
            ingameCursorImage.Scale = 0.5f;
        }

        public static void Update(GameTime gameTime)
        {
            if (cursorType == CursorType.Aim)
            {
                if (IsShooting)
                {
                    ingameCursorImage.Scale = MathHelper.Clamp(ingameCursorImage.ScaleX + scaleSpeed, 0.5f, 1);
                }

                ingameCursorImage.Angle = (ingameCursorImage.Angle + rotationSpeed) % 360;
                ingameCursorImage.Scale = MathHelper.Clamp(ingameCursorImage.ScaleX - scaleSpeed / 16, 0.5f, 1);
            }
        }

        public static void Draw(Vector2 position)
        {
            switch (cursorType)
            {
                case CursorType.Normal:
                    menuCursorImage.Draw(position, OGE.HUDCamera);
                    break;
                case CursorType.Aim:
                    ingameCursorImage.Draw(position, OGE.HUDCamera);
                    break;
            }
        }
    }
}
