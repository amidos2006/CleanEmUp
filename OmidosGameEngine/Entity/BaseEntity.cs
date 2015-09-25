using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Collision;
using OmidosGameEngine.Graphics;
using OmidosGameEngine.Tween;

namespace OmidosGameEngine.Entity
{
    public class BaseEntity : ILogical
    {
        private List<ITween> tweens;

        public Vector2 Position;

        public List<IMask> CollisionMasks
        {
            set;
            get;
        }

        public CollisionType EntityCollisionType
        {
            set;
            get;
        }

        public List<Image> CurrentImages
        {
            set;
            get;
        }

        public bool InsideScreen
        {
            set;
            get;
        }
        
        public BaseEntity()
        {
            InsideScreen = false;
            Position = new Vector2();
            CollisionMasks = new List<IMask>();
            CurrentImages = new List<Image>();
            tweens = new List<ITween>();
        }

        public virtual void Intialize()
        {
        }

        public virtual void LoadContent()
        {
        }

        public void AddTween(ITween tween, bool start = false)
        {
            tweens.Add(tween);

            if (start)
            {
                tween.Start(true);
            }
        }

        public virtual void AddCollisionMask(IMask mask)
        {
            mask.Parent = this;
            CollisionMasks.Add(mask);
        }

        public void RemoveTween(ITween tween)
        {
            tweens.Remove(tween);
        }

        public virtual BaseEntity Collide(CollisionType collisionType, Vector2 position)
        {
            foreach (IMask collisionMask in CollisionMasks)
			{
			    List<BaseEntity> entities = OGE.CurrentWorld.GetCollisionEntitiesType(collisionType);
                for (int i = 0; i < entities.Count; i++)
                {
                    foreach (IMask collideObjectMask in entities[i].CollisionMasks)
                    {
                        BaseEntity collideObject = collisionMask.Collide(position, collideObjectMask);

                        if (collideObject != null)
                        {
                            return collideObject;
                        }
                    }
                }
			}

            return null;
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (ITween tween in tweens)
            {
                tween.Update(gameTime);
            }

            foreach (Image image in CurrentImages)
            {
                image.Update(gameTime);
            }
        }

        public virtual void Draw(Camera camera)
        {
            foreach (Image image in CurrentImages)
            {
                image.Draw(Position, camera);
            }
        }
    }
}
