using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Tween;

namespace OmidosGameEngine.Entity.Tutorial
{
    public class TutorialEntity : BaseEntity
    {
        private Image controlsImage;
        private Image arrowLayoutImage;

        private Text wText;
        private Text aText;
        private Text sText;
        private Text dText;
        private Text moveText;

        private Text spaceText;
        private Text overclockingText;
        private Text overclockingDetailsText;
        private Text leftMouseText;

        private Image labelsImage;

        private Text healthText;
        private Text topProcessorText;
        private Text bottomProcessorText;
        private Text scoreText;
        private Text overclockingBarText;

        private Color color;
        private float alpha = 0;
        private float alphaSpeed = 0.04f;
        private Alarm finishingAlarm;
        private float maxAlpha = 0.6f;
        private Action removeEntityAction;

        public TutorialEntity(Color color, Action removeEntityAction = null)
        {
            this.removeEntityAction = removeEntityAction;

            GlobalVariables.IsFirstTime = false;
            this.color = color;

            controlsImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Tutorial\Controls"));
            arrowLayoutImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Tutorial\ArrowLayout"));
            controlsImage.TintColor = color;
            arrowLayoutImage.TintColor = color;

            Position = new Vector2(OGE.HUDCamera.Width / 2, OGE.HUDCamera.Height / 2) - new Vector2(controlsImage.Width / 2, controlsImage.Height / 2);

            wText = new Text("W", FontSize.Medium);
            aText = new Text("A", FontSize.Medium);
            sText = new Text("S", FontSize.Medium);
            dText = new Text("D", FontSize.Medium);
            
            wText.Align(AlignType.Center);
            aText.Align(AlignType.Center);
            sText.Align(AlignType.Center);
            dText.Align(AlignType.Center);

            moveText = new Text("Move Player", FontSize.Small);
            moveText.Align(AlignType.Right);
            moveText.TintColor = color;

            spaceText = new Text("Spacebar", FontSize.Medium);
            spaceText.Align(AlignType.Center);

            overclockingText = new Text("Start Overclocking", FontSize.Small);
            overclockingText.Align(AlignType.Center);
            overclockingText.TintColor = color;
            
            overclockingDetailsText = new Text("(It launches a random special ability from player abilities when the Overclocking meter is full)", FontSize.Small);
            overclockingDetailsText.Align(AlignType.Center);
            overclockingDetailsText.TintColor = color;

            leftMouseText = new Text("Fire", FontSize.Small);
            
            leftMouseText.TintColor = color;
            
            labelsImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Tutorial\TutorialLabels"));
            labelsImage.TintColor = color;

            healthText = new Text("Player Health (if depleted, player dies)", FontSize.Small);
            topProcessorText = new Text("Primary Turret Processor(if reach maximum, it must cooldown)",
                FontSize.Small);
            bottomProcessorText = new Text("Secondry Turret Processor(if reach maximum, it must cooldown)",
                FontSize.Small);
            scoreText = new Text("Player score in that level", FontSize.Small);
            overclockingBarText = new Text("Show percentage filled of Overclocking (Increase by killing enemies)", FontSize.Small);
            
            healthText.TintColor = color;
            topProcessorText.TintColor = color;
            bottomProcessorText.TintColor = color;
            scoreText.TintColor = color;
            overclockingBarText.TintColor = color;

            scoreText.Align(AlignType.Right);
            overclockingBarText.Align(AlignType.Center);

            this.finishingAlarm = new Alarm(10, TweenType.OneShot, FadeEntity);
            AddTween(finishingAlarm);

            AdjustAlpha();

            EntityCollisionType = Collision.CollisionType.Solid;
        }

        private void FadeEntity()
        {
            alphaSpeed *= -1;
        }

        public void FinishTutorial()
        {
            if (finishingAlarm.IsRunning())
            {
                finishingAlarm.Stop();
                FadeEntity();
            }
        }

        private void AdjustAlpha()
        {
            controlsImage.TintColor = color * alpha;
            arrowLayoutImage.TintColor = color * alpha;

            wText.TintColor = Color.White * alpha;
            aText.TintColor = Color.White * alpha;
            sText.TintColor = Color.White * alpha;
            dText.TintColor = Color.White * alpha;

            moveText.TintColor = color * alpha;

            spaceText.TintColor = Color.White * alpha;
            overclockingText.TintColor = color * alpha;
            overclockingDetailsText.TintColor = color * alpha;

            leftMouseText.TintColor = color * alpha;

            labelsImage.TintColor = color * alpha;
            healthText.TintColor = color * alpha;
            topProcessorText.TintColor = color * alpha;
            bottomProcessorText.TintColor = color * alpha;
            scoreText.TintColor = color * alpha;
            overclockingBarText.TintColor = color * alpha;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            alpha += alphaSpeed;
            if (alpha > maxAlpha)
            {
                alpha = maxAlpha;
            }

            if (alpha >= maxAlpha && !finishingAlarm.IsRunning())
            {
                finishingAlarm.Start();
            }

            AdjustAlpha();

            if (alpha <= 0 && alphaSpeed < 0)
            {
                if (removeEntityAction == null)
                {
                    OGE.CurrentWorld.RemoveEntity(this);
                }
                else
                {
                    removeEntityAction();
                }
            }
        }

        public override void Draw(Camera camera)
        {
            base.Draw(camera);

            controlsImage.Draw(Position, OGE.HUDCamera);

            if (GlobalVariables.Controls == Data.ControlType.Arrows)
            {
                arrowLayoutImage.Draw(Position, OGE.HUDCamera);
            }
            else
            {
                wText.Draw(Position + new Vector2(160, 53), OGE.HUDCamera);
                aText.Draw(Position + new Vector2(76, 140), OGE.HUDCamera);
                sText.Draw(Position + new Vector2(160, 140), OGE.HUDCamera);
                dText.Draw(Position + new Vector2(244, 140), OGE.HUDCamera);
            }

            moveText.Draw(Position + new Vector2(-8, 88), OGE.HUDCamera);

            spaceText.Draw(Position + new Vector2(controlsImage.Width / 2, 235), OGE.HUDCamera);
            overclockingText.Draw(Position + new Vector2(controlsImage.Width / 2, controlsImage.Height + 5), OGE.HUDCamera);
            overclockingDetailsText.Draw(Position + new Vector2(controlsImage.Width / 2,
                controlsImage.Height + overclockingText.Height + 10), OGE.HUDCamera);

            leftMouseText.Draw(Position + new Vector2(controlsImage.Width + 5, -3), OGE.HUDCamera);

            labelsImage.Draw(Vector2.Zero, OGE.HUDCamera);
            healthText.Draw(Vector2.Zero + new Vector2(372, 145), OGE.HUDCamera);
            topProcessorText.Draw(Vector2.Zero + new Vector2(372, 47), OGE.HUDCamera);
            bottomProcessorText.Draw(Vector2.Zero + new Vector2(372, 97), OGE.HUDCamera);
            scoreText.Draw(Vector2.Zero + new Vector2(1110, 25), OGE.HUDCamera);
            overclockingBarText.Draw(Vector2.Zero + new Vector2(637, 590), OGE.HUDCamera);
        }
    }
}
