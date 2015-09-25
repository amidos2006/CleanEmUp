using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Entity.Player.Weapons;
using OmidosGameEngine.Entity.Player.Bullet;

namespace OmidosGameEngine.Entity.Player.Data
{
    public class PlayerData
    {
        public const float SPEED_UNIT = 2;
        public const float HEALTH_UNIT = 1;

        public float Health
        {
            set;
            get;
        }

        public float Speed
        {
            set;
            get;
        }

        public float Accuracy
        {
            set;
            get;
        }

        public BaseWeapon Weapon
        {
            set;
            get;
        }

        public Image BodyImage
        {
            set;
            get;
        }

        public Image TurnetImage
        {
            set
            {
                Weapon.TurnetImage = value;
            }
            get
            {
                return Weapon.TurnetImage;
            }
        }

        public List<Vector2> UpThrusterPosition
        {
            set;
            get;
        }

        public List<Vector2> RightThrusterPosition
        {
            set;
            get;
        }

        public List<Vector2> LeftThrusterPosition
        {
            set;
            get;
        }

        public PlayerData()
        {
            UpThrusterPosition = new List<Vector2>();
            RightThrusterPosition = new List<Vector2>();
            LeftThrusterPosition = new List<Vector2>();
        }

        public virtual void LoadContent()
        {
        }

        protected PlayerData Clone(PlayerData p)
        {
            p.Accuracy = this.Accuracy;
            p.Health = this.Health;
            p.Speed = this.Speed;
            p.Weapon = this.Weapon;

            p.LeftThrusterPosition = new List<Vector2>();
            foreach (Vector2 vector in this.LeftThrusterPosition)
            {
                p.LeftThrusterPosition.Add(new Vector2(vector.X, vector.Y));
            }

            p.RightThrusterPosition = new List<Vector2>();
            foreach (Vector2 vector in this.RightThrusterPosition)
            {
                p.RightThrusterPosition.Add(new Vector2(vector.X, vector.Y));
            }

            p.UpThrusterPosition = new List<Vector2>();
            foreach (Vector2 vector in this.UpThrusterPosition)
            {
                p.UpThrusterPosition.Add(new Vector2(vector.X, vector.Y));
            }

            p.LoadContent();
            p.Weapon.LoadContent();

            return p;
        }

        public virtual PlayerData Clone()
        {
            return null;
        }

        public virtual EffectArea GetOverclocking()
        {
            return null;
        }

        public void FireWeapon(Vector2 position, float direction)
        {
            Weapon.GenerateBullet(position, direction, Accuracy);
        }
    }
}
