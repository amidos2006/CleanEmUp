using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Graphics.Particles;
using OmidosGameEngine.Entity.ParticleGenerator;
using Microsoft.Xna.Framework.Input;
using OmidosGameEngine.Graphics.Lighting;
using OmidosGameEngine.Collision;
using OmidosGameEngine.Entity.Player.Data;
using OmidosGameEngine.Sounds;
using OmidosGameEngine.Tween;
using OmidosGameEngine.Graphics;
using OmidosGameEngine.Entity.OverLayer;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Entity.Player.Bullet;

namespace OmidosGameEngine.Entity.Player
{
    public class PlayerEntity : BaseEntity
    {
        private const float FRICTION_FACTOR = 2f;

        private const float MIN_RAGE = 0f;
        private const float MAX_RAGE = 100f;
        private const float RAGE_DECREMENT = 0f;

        private TrailParticleGenerator trailParticleGenerator;
        private FractionalParticleGenerator fractionalGenerator;

        private PlayerData topPlayer;
        private PlayerData downPlayer;

        private float health;
        private bool isHit;
        private Alarm hitCoolDown;

        private float speedFactor;
        private float speedResistance;
        private float minSpeedFactor;
        private float maxSpeedFactor;

        private float acceleration;
        private float hspeed;
        private float vspeed;
        private float maxSpeed;

        private Keys leftKey;
        private Keys rightKey;
        private Keys upKey;
        private Keys downKey;
        private Keys mainLeftKey;
        private Keys mainRightKey;
        private Keys mainUpKey;
        private Keys mainDownKey;
        private Alarm invertAlarm;

        private EffectArea runningOverclocking;
        private bool rageReady;
        private float rage;
        private Alarm rageFullEffectAlarm;

        private Alarm shieldAlarm;
        private ShieldImageEntity shieldImageEntity;
        private bool shieldOn;

        public ShootingType ShootingMethod
        {
            set;
            get;
        }

        public float Rage
        {
            set
            {
                if (!rageReady)
                {
                    rage = value;
                }

                if (rage > MAX_RAGE)
                {
                    rage = MAX_RAGE;
                    rageReady = true;
                }
                if (rage < MIN_RAGE)
                {
                    rage = MIN_RAGE;
                }
            }
            get
            {
                return rage;
            }
        }

        public float SpeedFactor
        {
            set
            {
                if (speedFactor > value && shieldOn)
                {
                    return;
                }

                speedFactor = value;
                if (speedFactor > maxSpeedFactor)
                {
                    speedFactor = maxSpeedFactor;
                }
                else if (speedFactor < minSpeedFactor)
                {
                    speedFactor = minSpeedFactor;
                }
            }
            get
            {
                return speedFactor;
            }
        }

        public Rectangle HitBox
        {
            set;
            get;
        }

        public float TotalHealth
        {
            get
            {
                return topPlayer.Health + downPlayer.Health;
            }
        }

        private void IntializeParticleGenerators()
        {
            Particle particlePrototype = new Particle();
            particlePrototype.ParticleColor = new Color(40, 72, 127);
            particlePrototype.DeltaScale = -0.06f;
            particlePrototype.DeltaAlpha = -0.1f;

            this.trailParticleGenerator = new TrailParticleGenerator(OGE.CurrentWorld.TrailEffectSystem, particlePrototype);
            this.trailParticleGenerator.AngleDisplacement = 15;
            this.trailParticleGenerator.Speed = 2;
            this.trailParticleGenerator.Scale = 0.5f;

            Particle prototype = new Particle();
            prototype.DeltaScale = -0.03f;
            prototype.DeltaSpeed = -0.02f;
            prototype.DeltaAngle = 5f;

            this.fractionalGenerator = new FractionalParticleGenerator(OGE.CurrentWorld.ExplosionEffectSystem, prototype, 6);
            this.fractionalGenerator.Speed = 5;
            this.fractionalGenerator.NumberOfCircles = 3;
        }

        private void AdjustDownPlayer()
        {
            downPlayer.BodyImage.Angle = 180;

            Vector2 temp;

            for (int i = 0; i < downPlayer.LeftThrusterPosition.Count; i++)
            {
                temp = downPlayer.LeftThrusterPosition[i];
                temp.Y *= -1;
                downPlayer.LeftThrusterPosition[i] = temp;
            }

            for (int i = 0; i < downPlayer.RightThrusterPosition.Count; i++)
            {
                temp = downPlayer.RightThrusterPosition[i];
                temp.Y *= -1;
                downPlayer.RightThrusterPosition[i] = temp;
            }

            for (int i = 0; i < downPlayer.UpThrusterPosition.Count; i++)
            {
                temp = downPlayer.UpThrusterPosition[i];
                temp.Y *= -1;
                downPlayer.UpThrusterPosition[i] = temp;
            }
        }

        public PlayerEntity(Vector2 startPosition, PlayerData topPlayer, PlayerData downPlayer)
        {
            this.Position.X = startPosition.X;
            this.Position.Y = startPosition.Y;

            this.topPlayer = topPlayer.Clone();
            this.downPlayer = downPlayer.Clone();

            this.topPlayer.Weapon.Processor = 0;
            this.downPlayer.Weapon.Processor = 0;

            AdjustDownPlayer();

            HitBox = new Rectangle((int)(-topPlayer.BodyImage.OriginX),(int)(-topPlayer.BodyImage.OriginY),
                (int)(Math.Max(topPlayer.BodyImage.Width, downPlayer.BodyImage.Width)),
                (int)(topPlayer.BodyImage.Height + downPlayer.BodyImage.Height));

            this.ShootingMethod = ShootingType.Parallel;

            this.CurrentImages.Add(this.topPlayer.BodyImage);
            this.CurrentImages.Add(this.downPlayer.BodyImage);

            this.health = topPlayer.Health + downPlayer.Health;
            this.acceleration = 0.6f;
            this.hspeed = 0;
            this.vspeed = 0;
            this.maxSpeed = topPlayer.Speed / 2 + downPlayer.Speed / 2;

            IntializeParticleGenerators();

            IntializeMainKeys();

            InvertEnded();
            this.invertAlarm = new Alarm(1f, TweenType.OneShot, new AlarmFinished(InvertEnded));
            AddTween(invertAlarm);

            ShieldEnded();
            this.shieldImageEntity = new ShieldImageEntity();
            this.shieldAlarm = new Alarm(0f, TweenType.OneShot, new AlarmFinished(ShieldEnded));
            AddTween(shieldAlarm);

            this.rageFullEffectAlarm = new Alarm(1.5f, TweenType.Looping, GenerateRageFullEffect);
            AddTween(this.rageFullEffectAlarm, true);

            this.Rage = MIN_RAGE;
            this.rageReady = false;
            this.runningOverclocking = null;

            this.isHit = false;
            this.hitCoolDown = new Alarm(0.3f, TweenType.OneShot, new AlarmFinished(HitEnded));
            AddTween(hitCoolDown);

            AddCollisionMask(new HitboxMask(HitBox.Width - 10, HitBox.Height - 10, (HitBox.Width - 10) / 2, (HitBox.Height - 10) / 2));

            this.speedResistance = 0.005f;
            this.minSpeedFactor = 0.5f;
            this.maxSpeedFactor = 1f;
            this.speedFactor = 1;

            this.EntityCollisionType = CollisionType.Player;
        }

        public override void Intialize()
        {
            base.Intialize();
            OGE.CurrentWorld.AddEntity(shieldImageEntity);
        }

        private void IntializeMainKeys()
        {
            if (GlobalVariables.Controls == OmidosGameEngine.Data.ControlType.WASD)
            {
                mainLeftKey = Keys.A;
                mainRightKey = Keys.D;
                mainUpKey = Keys.W;
                mainDownKey = Keys.S;
            }

            if (GlobalVariables.Controls == OmidosGameEngine.Data.ControlType.Arrows)
            {
                mainLeftKey = Keys.Left;
                mainRightKey = Keys.Right;
                mainUpKey = Keys.Up;
                mainDownKey = Keys.Down;
            }
        }

        private void OverclockingEnds()
        {
            if (runningOverclocking != null)
            {
                OGE.CurrentWorld.RemoveEntity(runningOverclocking);
            }

            runningOverclocking = null;
            rageReady = false;
            Rage = MIN_RAGE;
        }

        private void HitEnded()
        {
            isHit = false;
        }

        private void InvertEnded()
        {
            this.leftKey = this.mainLeftKey;
            this.rightKey = this.mainRightKey;
            this.upKey = this.mainUpKey;
            this.downKey = this.mainDownKey;
        }

        private void GenerateRageFullEffect()
        {
            if (rageReady && runningOverclocking == null)
            {
                OGE.CurrentWorld.AddOverLayer(new OverclockingBarFilledEntity());
            }
        }

        private void ShieldEnded()
        {
            shieldOn = false;
        }

        public void InvertControls(float amountOfTime)
        {
            if (shieldOn)
            {
                return;
            }

            this.leftKey = this.mainRightKey;
            this.rightKey = this.mainLeftKey;
            this.upKey = this.mainDownKey;
            this.downKey = this.mainUpKey;

            invertAlarm.Reset(amountOfTime);
            invertAlarm.Start();
        }

        public void OverHeatWeapons()
        {
            if (shieldOn)
            {
                return;
            }

            topPlayer.Weapon.CooldownProcessor = true;
            topPlayer.Weapon.Processor = 100;

            downPlayer.Weapon.CooldownProcessor = true;
            downPlayer.Weapon.Processor = 100;
        }

        public void DoubleDamage()
        {
            //topPlayer.Weapon.
        }

        public void CoolWeapons()
        {
            topPlayer.Weapon.CooldownProcessor = false;
            topPlayer.Weapon.Processor = 0;

            downPlayer.Weapon.CooldownProcessor = false;
            downPlayer.Weapon.Processor = 0;
        }

        public void ActivateShield(float time)
        {
            shieldOn = true;
            shieldAlarm.Reset(time);
            shieldAlarm.Start();
        }

        public void IncreaseHealth(float health)
        {
            this.health += health;
            if (this.health > TotalHealth)
            {
                this.health = TotalHealth;
            }
        }

        public void PlayerDestroy()
        {
            HUDEntity.PlayerHealth = 0;
            HUDEntity.RageMeter = 0;
            HUDEntity.RageActive = false;

            fractionalGenerator.FractionTexture = topPlayer.BodyImage.Texture;
            fractionalGenerator.GenerateParticles(Position + new Vector2(0, topPlayer.BodyImage.Texture.Height / 2));

            fractionalGenerator.FractionTexture = downPlayer.BodyImage.Texture;
            fractionalGenerator.GenerateParticles(Position - new Vector2(0, downPlayer.BodyImage.Texture.Height / 2));

            topPlayer.Weapon.StopSound();
            downPlayer.Weapon.StopSound();

            OverclockingEnds();

            OGE.CurrentWorld.RemoveEntity(shieldImageEntity);

            OGE.CurrentWorld.AdditiveWhite.Alpha += 0.3f;
            OGE.CurrentWorld.LightingEffectSystem.GenerateLightSource(Position, new Vector2(200, 200), new Color(50, 150, 255), -0.005f);

            OGE.CurrentWorld.RemoveEntity(this);
        }

        public void PlayerHit(float damage, float speed, float direction)
        {
            if (isHit || shieldOn)
            {
                return;
            }

            if (damage > 0)
            {
                OGE.CurrentWorld.NoiseEffect.Alpha += 0.5f;
                SoundManager.PlaySFX("player_hit");

                isHit = true;
                hitCoolDown.Start();
                health -= damage;
            }

            if (health <= 0)
            {
                health = 0;
                PlayerDestroy();
                return;
            }
            Vector2 speedVector = OGE.GetProjection(speed, direction);
            hspeed += speedVector.X;
            vspeed += speedVector.Y;
        }

        private void CheckMouse()
        {
            Vector2 mousePosition = Input.GetMousePosition(OGE.WorldCamera);
            Vector2 firePosition;
            float mouseAngle = OGE.GetAngle(Position, mousePosition);

            if (Input.GetRightStick().Length() > 0)
            {
                mouseAngle = OGE.GetAngle(Vector2.Zero, Input.GetRightStick());
            }

            topPlayer.TurnetImage.Angle = mouseAngle;
            downPlayer.TurnetImage.Angle = mouseAngle + 180;

            if (!GlobalVariables.EnableControls)
            {
                if (Input.CheckLeftMouseButton() == GameButtonState.Up)
                {
                    GlobalVariables.EnableControls = true;
                }
                else
                {
                    return;
                }
            }

            switch (ShootingMethod)
            {
                case ShootingType.Single:
                    if (Input.CheckRightMouseButton() == GameButtonState.Down)
                    {
                        SoundManager.EmitterPosition = Position;
                        firePosition = Position + OGE.GetProjection(topPlayer.TurnetImage.Width, mouseAngle + 180);
                        downPlayer.FireWeapon(firePosition, mouseAngle + 180);
                    }
                    if (Input.CheckLeftMouseButton() == GameButtonState.Down)
                    {
                        SoundManager.EmitterPosition = Position;
                        firePosition = Position + OGE.GetProjection(topPlayer.TurnetImage.Width, mouseAngle);
                        topPlayer.FireWeapon(firePosition, mouseAngle);
                    }

                    if (Input.CheckLeftMouseButton() == GameButtonState.Up)
                    {
                        topPlayer.Weapon.StopSound();
                    }
                    if (Input.CheckRightMouseButton() == GameButtonState.Up)
                    {
                        downPlayer.Weapon.StopSound();
                    }
                    break;
                case ShootingType.Parallel:
                    if (Input.CheckLeftMouseButton() == GameButtonState.Down)
                    {
                        SoundManager.EmitterPosition = Position;
                        firePosition = Position + OGE.GetProjection(topPlayer.TurnetImage.Width, mouseAngle + 180);
                        downPlayer.FireWeapon(firePosition, mouseAngle + 180);

                        firePosition = Position + OGE.GetProjection(topPlayer.TurnetImage.Width, mouseAngle);
                        topPlayer.FireWeapon(firePosition, mouseAngle);
                    }

                    if (Input.CheckLeftMouseButton() == GameButtonState.Up)
                    {
                        topPlayer.Weapon.StopSound();
                        downPlayer.Weapon.StopSound();
                    }
                    break;
            }
        }

        private void CheckKeyboard()
        {
            if (Input.CheckKeyboardButton(leftKey) == GameButtonState.Down)
            {
                hspeed -= acceleration * OGE.PlayerSlowFactor;

                foreach (Vector2 position in topPlayer.RightThrusterPosition)
                {
                    trailParticleGenerator.Angle = 0;
                    trailParticleGenerator.GenerateParticles(Position + position);
                }

                foreach (Vector2 position in downPlayer.RightThrusterPosition)
                {
                    trailParticleGenerator.Angle = 0;
                    trailParticleGenerator.GenerateParticles(Position + position);
                }
            }
            
            if (Input.CheckKeyboardButton(rightKey) == GameButtonState.Down)
            {
                hspeed += acceleration * OGE.PlayerSlowFactor;

                foreach (Vector2 position in topPlayer.LeftThrusterPosition)
                {
                    trailParticleGenerator.Angle = 180;
                    trailParticleGenerator.GenerateParticles(Position + position);
                }

                foreach (Vector2 position in downPlayer.LeftThrusterPosition)
                {
                    trailParticleGenerator.Angle = 180;
                    trailParticleGenerator.GenerateParticles(Position + position);
                }
            }

            if (Input.CheckKeyboardButton(upKey) == GameButtonState.Down)
            {
                vspeed -= acceleration * OGE.PlayerSlowFactor;

                foreach (Vector2 position in downPlayer.UpThrusterPosition)
                {
                    trailParticleGenerator.Angle = 90;
                    trailParticleGenerator.GenerateParticles(Position + position);
                }
            }
            
            if (Input.CheckKeyboardButton(downKey) == GameButtonState.Down)
            {
                vspeed += acceleration * OGE.PlayerSlowFactor;

                foreach (Vector2 position in topPlayer.UpThrusterPosition)
                {
                    trailParticleGenerator.Angle = 270;
                    trailParticleGenerator.GenerateParticles(Position + position);
                }
            }

            if (Input.CheckKeyboardButton(Keys.Space) == GameButtonState.Pressed || 
                Input.CheckRightMouseButton() == GameButtonState.Pressed)
            {
                LaunchOverclocking();
            }
        }

        private void UpdatePhysics()
        {
            if (hspeed != 0)
            {
                if (Math.Abs(hspeed) > OGE.Friction * OGE.PlayerSlowFactor * speedFactor * FRICTION_FACTOR)
                {
                    hspeed = Math.Sign(hspeed) * (Math.Abs(hspeed) - OGE.Friction * speedFactor * OGE.PlayerSlowFactor * FRICTION_FACTOR);
                }
                else
                {
                    hspeed = 0;
                }

                if (Math.Abs(hspeed) > maxSpeed * OGE.PlayerSlowFactor * speedFactor)
                {
                    hspeed = Math.Sign(hspeed) * maxSpeed * OGE.PlayerSlowFactor * speedFactor;
                }
            }

            if (vspeed != 0)
            {
                if (Math.Abs(vspeed) > OGE.Friction * OGE.PlayerSlowFactor * speedFactor * FRICTION_FACTOR)
                {
                    vspeed = Math.Sign(vspeed) * (Math.Abs(vspeed) - OGE.Friction * OGE.PlayerSlowFactor * speedFactor * FRICTION_FACTOR);
                }
                else
                {
                    vspeed = 0;
                }

                if (Math.Abs(vspeed) > maxSpeed * OGE.PlayerSlowFactor * speedFactor)
                {
                    vspeed = Math.Sign(vspeed) * maxSpeed * OGE.PlayerSlowFactor * speedFactor;
                }
            }

            Rectangle worldDimensions = new Rectangle(0, 0, (int)OGE.CurrentWorld.Dimensions.X, (int)OGE.CurrentWorld.Dimensions.Y);
            
            Rectangle nextPositionRectanlge = new Rectangle((int)(Position.X + hspeed + HitBox.X), (int)(Position.Y + HitBox.Y), HitBox.Width, HitBox.Height);
            if (worldDimensions.Contains(nextPositionRectanlge))
            {
                Position.X += hspeed;
            }
            else
            {
                hspeed = 0;
            }

            nextPositionRectanlge = new Rectangle((int)(Position.X + HitBox.X), (int)(Position.Y + vspeed + HitBox.Y), HitBox.Width, HitBox.Height);
            if (worldDimensions.Contains(nextPositionRectanlge))
            {
                Position.Y += vspeed;
            }
            else
            {
                vspeed = 0;
            }
        }

        private void UpdateGraphics()
        {
            HUDEntity.TotalPlayerHealth = TotalHealth;
            HUDEntity.PlayerHealth = health;

            HUDEntity.RageMeter = Rage;
            HUDEntity.RageActive = runningOverclocking != null;

            HUDEntity.TopGunName = topPlayer.Weapon.GunName;
            HUDEntity.TopOverHeat = topPlayer.Weapon.CooldownProcessor;
            HUDEntity.TopProcessor = topPlayer.Weapon.Processor;

            HUDEntity.DownGunName = downPlayer.Weapon.GunName;
            HUDEntity.DownOverHeat = downPlayer.Weapon.CooldownProcessor;
            HUDEntity.DownProcessor = downPlayer.Weapon.Processor;

            switch (ShootingMethod)
            {
                case ShootingType.Single:
                    HUDEntity.ShootingMode = "Single";
                    break;
                case ShootingType.Parallel:
                    HUDEntity.ShootingMode = "Parallel";
                    break;
            }

            OGE.CurrentWorld.NoiseEffect.UpdateHealth(health, TotalHealth);
        }

        private void UpdateCamera()
        {
            Vector2 mousePosition = Input.GetMousePosition(OGE.WorldCamera);

            Vector2 centerOfCamera = Position + (mousePosition - Position) / 3;

            OGE.WorldCamera.X = (int)(centerOfCamera.X - OGE.WorldCamera.Width / 2);
            OGE.WorldCamera.Y = (int)(centerOfCamera.Y - OGE.WorldCamera.Height / 2);

            SoundManager.ListnerPosition = centerOfCamera;
        }

        private void UpdateOverclocking()
        {
            Rage -= RAGE_DECREMENT;

            if (runningOverclocking != null)
            {
                runningOverclocking.Position = Position;

                rage = (float)runningOverclocking.GetRemainingPercent * 100;
            }
        }

        private void LaunchOverclocking()
        {
            if (rageReady && runningOverclocking == null)
            {
                int random = OGE.Random.Next(2);

                switch (random)
                {
                    case 0:
                        runningOverclocking = topPlayer.GetOverclocking();
                        break;
                    case 1:
                        runningOverclocking = downPlayer.GetOverclocking();
                        break;
                }

                runningOverclocking.EndAction = OverclockingEnds;
                runningOverclocking.Position = Position;

                OGE.CurrentWorld.AddEntity(runningOverclocking);
            }
            else
            {
                //TODO play sound that tells the player that the rage is not full yet
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            topPlayer.Weapon.Update(gameTime);
            downPlayer.Weapon.Update(gameTime);

            SpeedFactor += speedResistance;

            CheckKeyboard();
            UpdatePhysics();
            UpdateGraphics();
            UpdateCamera();
            UpdateOverclocking();
            CheckMouse();

            shieldAlarm.SpeedFactor = OGE.PlayerSlowFactor;

            shieldImageEntity.Position.X = Position.X;
            shieldImageEntity.Position.Y = Position.Y;
            if (shieldOn)
            {
                shieldImageEntity.TintColor = Color.White * (float)(1 - shieldAlarm.PercentComplete());
            }
            else
            {
                shieldImageEntity.TintColor = Color.White * 0;
            }
        }

        public override void Draw(Camera camera)
        {
            foreach (Image image in CurrentImages)
            {
                if (invertAlarm.IsRunning() || speedFactor < 1)
                {
                    image.TintColor = new Color(255, 100, 100);
                    topPlayer.TurnetImage.TintColor = new Color(255, 100, 100);
                    downPlayer.TurnetImage.TintColor = new Color(255, 100, 100);
                }
                else
                {
                    image.TintColor = Color.White;
                    topPlayer.TurnetImage.TintColor = Color.White;
                    downPlayer.TurnetImage.TintColor = Color.White;
                }
            }

            base.Draw(camera);

            topPlayer.TurnetImage.Draw(Position, camera);
            downPlayer.TurnetImage.Draw(Position, camera);
        }
    }
}
