using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Collision;
using OmidosGameEngine.World;
using OmidosGameEngine.Entity.Enemy;
using OmidosGameEngine.Entity.Player;
using OmidosGameEngine.Sounds;
using OmidosGameEngine.Entity.OverLayer;

namespace OmidosGameEngine.Entity.Object.File
{
    public class DocumentFile : BaseFile
    {
        private const int SAFE_RANGE = 200;

        public DocumentFile()
        {
            normalImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Objects\DocumentFile"));
            normalImage.CenterOrigin();
            normalImage.Scale = 0;

            CurrentImages.Add(new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Objects\DocumentFile")));
            CurrentImages[0].CenterOrigin();

            AddCollisionMask(new HitboxMask(normalImage.Width, normalImage.Height, normalImage.OriginX, normalImage.OriginY));
        }

        private bool CheckNearHackintosh(BaseEntity e, List<HackintoshEnemy> hackList)
        {
            foreach (HackintoshEnemy hack in hackList)
            {
                if (OGE.GetDistance(e.Position, hack.Position) < SAFE_RANGE)
                {
                    return true;
                }
            }

            return false;
        }

        private void ModifyPosition(BaseEntity e)
        {
            Random random = OGE.Random;

            List<BaseEntity> list = OGE.CurrentWorld.GetCollisionEntitiesType(CollisionType.Player);
            List<BaseEntity> enemyList = OGE.CurrentWorld.GetCollisionEntitiesType(CollisionType.Enemy);
            List<HackintoshEnemy> hackintoshList = new List<HackintoshEnemy>();

            foreach (BaseEntity enemy in enemyList)
            {
                if (enemy is HackintoshEnemy)
                {
                    hackintoshList.Add(enemy as HackintoshEnemy);
                }
            }

            if (list.Count > 0)
            {
                PlayerEntity p = list[0] as PlayerEntity;
                do
                {
                    e.Position.X = random.Next((int)OGE.CurrentWorld.Dimensions.X - SAFE_RANGE) + SAFE_RANGE / 2;
                    e.Position.Y = random.Next((int)OGE.CurrentWorld.Dimensions.Y - SAFE_RANGE) + SAFE_RANGE / 2;
                } while (OGE.GetDistance(e.Position, p.Position) < SAFE_RANGE || CheckNearHackintosh(e, hackintoshList));
            }
            else
            {
                e.Position.X = random.Next((int)OGE.CurrentWorld.Dimensions.X - SAFE_RANGE) + SAFE_RANGE / 2;
                e.Position.Y = random.Next((int)OGE.CurrentWorld.Dimensions.Y - SAFE_RANGE) + SAFE_RANGE / 2;
            }
        }

        protected override void PlayerCollide(Player.PlayerEntity p)
        {
            GlobalVariables.Achievements[this.GetType()].CurrentNumber += 1;

            GameplayWorld gameplayWorld = (OGE.CurrentWorld as GameplayWorld);
            if (gameplayWorld != null)
            {
                gameplayWorld.NumberOfDocumentFiles += 1;
                if (gameplayWorld.NumberOfDocumentFiles < gameplayWorld.TotalNumberOfDocumentFiles)
                {
                    TextNotifierEntity text = new TextNotifierEntity((gameplayWorld.TotalNumberOfDocumentFiles - 
                        gameplayWorld.NumberOfDocumentFiles).ToString());
                    OGE.CurrentWorld.AddOverLayer(text);

                    DocumentFile file = new DocumentFile();
                    ModifyPosition(file);

                    OGE.CurrentWorld.AddEntity(file);
                }
            }

            SurvivalGameplayWorld survivalGameplayWorld = (OGE.CurrentWorld as SurvivalGameplayWorld);
            if (survivalGameplayWorld != null)
            {
                survivalGameplayWorld.NumberOfDocumentFiles += 1;

                DocumentFile file = new DocumentFile();
                ModifyPosition(file);

                OGE.CurrentWorld.AddEntity(file);
            }

            SoundManager.PlaySFX("file_pickup");
            OGE.CurrentWorld.RemoveEntity(this);
        }
    }
}
