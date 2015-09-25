using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OmidosGameEngine.Entity.OverLayer
{
    public class ArrowEntity:BaseEntity
    {
        private Image arrowImage;
        private List<Vector2> arrowPositions;
        private float projectionDistance;
        private bool playerExists;

        public ArrowEntity()
        {
            arrowPositions = new List<Vector2>();
            playerExists = true;
            projectionDistance = 100;

            arrowImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\HUD\FileArrow"));
            arrowImage.TintColor = Color.White * 0.5f;
            arrowImage.OriginY = arrowImage.Height / 2;

            EntityCollisionType = Collision.CollisionType.Shield;
        }

        public void UpdatePosition(List<Vector2> positions)
        {
            arrowPositions = new List<Vector2>(positions);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            playerExists = false;

            List<BaseEntity> player = OGE.CurrentWorld.GetCollisionEntitiesType(Collision.CollisionType.Player);
            if (player.Count > 0)
            {
                playerExists = true;
                Position.X = player[0].Position.X;
                Position.Y = player[0].Position.Y;
            }
        }

        public override void Draw(Camera camera)
        {
            base.Draw(camera);

            if (playerExists)
            {
                foreach (Vector2 position in arrowPositions)
                {
                    arrowImage.Angle = OGE.GetAngle(Position, position);

                    if (OGE.GetDistance(Position, position) >= projectionDistance + arrowImage.Width + 30)
                    {
                        arrowImage.Draw(Position + OGE.GetProjection(projectionDistance, arrowImage.Angle), camera);
                    }
                    else
                    {
                        arrowImage.Draw(position - OGE.GetProjection(arrowImage.Width + 30, arrowImage.Angle), camera);
                    }
                }
            }
        }
    }
}
