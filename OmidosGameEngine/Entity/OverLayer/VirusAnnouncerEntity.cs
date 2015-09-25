using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Data;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Entity.Enemy;
using OmidosGameEngine.Entity.Object.File;

namespace OmidosGameEngine.Entity.OverLayer
{
    public class VirusButtonAnnouncerEntity : AnnouncerEntity
    {
        private const int MAX_VIRUSES_DEMO = 5;

        private Image virusBackImage;
        private Dictionary<Type, EnemyData> viewedEnemies;
        private int selectedKey;
        private List<Type> keys;
        private Text nameWordText;
        private Text nameText;
        private Text descriptionWordText;
        private Text descriptionText;
        private Button startLevel;
        private Button nextVirus;
        private Button previousVirus;
        private BaseEntity enemy;

        public VirusButtonAnnouncerEntity(AnnouncerEnded endFunction, Color color, Dictionary<Type, EnemyData> viewedEnemies, string playText, ButtonPressed playPressed)
            : base(endFunction, 325)
        {
            this.text = new Text("Security Console", FontSize.Large);
            this.text.OriginX = this.text.Width / 2;

            this.virusBackImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\WindowGraphics\EnemyWindow"));
            this.virusBackImage.CenterOrigin();
            this.virusBackImage.TintColor = color;

            this.startLevel = new Button(color, playText, playPressed);
            this.nextVirus = new Button(color, "Next =>", GetNextEnemy);
            this.previousVirus = new Button(color, "<= Previous", GetPreviousEnemy);
            
            this.startLevel.TintColor = color;
            this.nextVirus.TintColor = color;
            this.previousVirus.TintColor = color;

            this.startLevel.Position.X = OGE.HUDCamera.Width / 2;
            this.startLevel.Position.Y = OGE.HUDCamera.Height / 2 + maxHeight / 2 - 40;

            this.previousVirus.Position.X = OGE.HUDCamera.Width / 2 - 350;
            this.previousVirus.Position.Y = OGE.HUDCamera.Height / 2 + maxHeight / 2 - 40;

            this.nextVirus.Position.X = OGE.HUDCamera.Width / 2 + 350;
            this.nextVirus.Position.Y = OGE.HUDCamera.Height / 2 + maxHeight / 2 - 40;

            this.viewedEnemies = viewedEnemies;
            this.selectedKey = 0;
            this.keys = this.viewedEnemies.Keys.ToList();
            if (this.viewedEnemies.Count > 0)
            {
                this.enemy = Activator.CreateInstance(keys[selectedKey], false) as BaseEntity;
            }

            this.nameWordText = new Text("Name", FontSize.Small);
            this.nameText = new Text("", FontSize.Small);
            this.descriptionWordText = new Text("Description", FontSize.Small);
            this.descriptionText = new Text("", FontSize.Small);

            this.nameText.TintColor = color;
            this.nameWordText.TintColor = color;
            this.descriptionWordText.TintColor = color;
            this.descriptionText.TintColor = color;

            this.nameWordText.Align(AlignType.Center);
            this.nameText.Align(AlignType.Center);
            this.descriptionText.Align(AlignType.Center);
            this.descriptionWordText.Align(AlignType.Center);

            this.TintColor = color;
        }

        private void GetNextEnemy()
        {
            selectedKey += 1;

            enemy = Activator.CreateInstance(keys[selectedKey], false) as BaseEntity;
        }

        private void GetPreviousEnemy()
        {
            selectedKey -= 1;
            
            enemy = Activator.CreateInstance(keys[selectedKey], false) as BaseEntity;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (status == AnnouncerStatus.Steady)
            {
                virusBackImage.Update(gameTime);

                previousVirus.Active = selectedKey > 0;
                nextVirus.Active = selectedKey < viewedEnemies.Count - 1;

                startLevel.Update(gameTime);
                previousVirus.Update(gameTime);
                nextVirus.Update(gameTime);

                if (viewedEnemies.Count > 0)
                {
                    if (viewedEnemies[keys[selectedKey]].Locked)
                    {
                        nameText.TextContext = "? ? ? ? ? ? ?";
                        descriptionText.TextContext = "? ? ? ? ? ? ? ? ? ? ?";
                    }
                    else
                    {
                        nameText.TextContext = viewedEnemies[keys[selectedKey]].Name;
                        descriptionText.TextContext = viewedEnemies[keys[selectedKey]].Description;
                    }

                    if (GlobalVariables.IsDemoVersion && selectedKey > MAX_VIRUSES_DEMO)
                    {
                        nameText.TextContext = "? ? ? ? ? ? ?";
                        descriptionText.TextContext = "? ? ? ? ? ? ? ? ? ? ?";
                    }

                    if (enemy != null)
                    {
                        if (!(enemy is BaseFile))
                        {
                            enemy.CurrentImages[0].Angle = 270;
                            enemy.CurrentImages[0].Scale = 1;
                        }
                    }
                }
            }
        }

        public override void Draw(Camera camera)
        {
            base.Draw(camera);

            if (status == AnnouncerStatus.Steady)
            {
                text.Draw(new Vector2(camera.Width / 2, Position.Y + camera.Height / 2 - maxHeight / 2 + 10), camera);

                virusBackImage.Draw(new Vector2(camera.Width / 2, camera.Height / 2 - maxHeight/2 + virusBackImage.Height / 2 + 
                    text.Height + 20), camera);
                if (enemy != null)
                {
                    if (viewedEnemies[keys[selectedKey]].Locked)
                    {
                        enemy.CurrentImages[0].TintColor = Color.Black;
                    }
                    else
                    {
                        enemy.CurrentImages[0].TintColor = Color.White;
                    }

                    if (GlobalVariables.IsDemoVersion && selectedKey > MAX_VIRUSES_DEMO)
                    {
                        enemy.CurrentImages[0].TintColor = Color.Black;
                    }

                    enemy.Position = new Vector2(camera.Width / 2, camera.Height / 2 - maxHeight / 2 + virusBackImage.Height / 2 +
                        text.Height + 20);
                    enemy.CurrentImages[0].Draw(enemy.Position, camera);
                }

                Vector2 tempPosition = new Vector2(camera.Width / 2, camera.Height / 2 - maxHeight / 2 + virusBackImage.Height +
                    text.Height + 30);
                nameWordText.Draw(tempPosition, camera);
                tempPosition += new Vector2(0, nameWordText.Height + 5);
                nameText.Draw(tempPosition, camera);
                tempPosition += new Vector2(0, nameText.Height + 10);
                descriptionWordText.Draw(tempPosition, camera);
                tempPosition += new Vector2(0, descriptionWordText.Height + 5);
                descriptionText.Draw(tempPosition, camera);

                startLevel.Draw(camera);
                nextVirus.Draw(camera);
                previousVirus.Draw(camera);
            }
        }
    }
}
