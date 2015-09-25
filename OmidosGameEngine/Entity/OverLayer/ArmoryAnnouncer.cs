using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Data;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Graphics;
using OmidosGameEngine.Entity.Player.Data;
using OmidosGameEngine.Entity.Player.Weapons;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Sounds;

namespace OmidosGameEngine.Entity.OverLayer
{
    public class ArmoryAnnouncer : AnnouncerEntity
    {
        private const int MAX_CHARACTERS_DEMO = 2;
        private const int MAX_WEAPONS_DEMO = 5;

        private Button playButton;
        private Button virusDatabaseButton;
        private Button backButton;

        private PlayerData topPlayer;
        private PlayerData bottomPlayer;
        private BaseWeapon topWeapon;
        private BaseWeapon bottomWeapon;

        private Image armoryImage;
        private Image playerCombinationImage;
        private Text bodyWordText;
        private Text turretWordText;
        private Text playerPositionWordText;
        private Text playerPoints;
        private Text nameWordText;
        private Text nameText;
        private Text descriptionWordText;
        private Text descriptionText;
        private Text overclockingWordText;
        private Text overclockingText;
        private Text unlockPointsWordText;
        private Text unlockPointsText;
        private int unlockNumber;
        private UnlockButtonType unlockType;

        private Vector2 armoryTopPosition;
        private Vector2 armoryBottomPosition;

        private List<UnlockButton> topCharacterButtons;
        private List<UnlockButton> topWeaponButtons;
        private List<UnlockButton> bottomCharacterButtons;
        private List<UnlockButton> bottomWeaponButtons;

        private Rectangle GetSourceRectangle(int number, int length)
        {
            return new Rectangle(0, number * length, length, length);
        }

        private void IntializeCharacterAndWeapons()
        {
            Color unlockColor = new Color(150, 255, 130);
            Color affordableColor = new Color(255, 255, 130);
            Color lockColor = new Color(255, 180, 180);

            Texture2D texture = OGE.Content.Load<Texture2D>(@"Graphics\Entities\WindowGraphics\Characters");
            Image image;
            UnlockButton temp;
            Vector2 topCorner = new Vector2(armoryTopPosition.X + 20 + texture.Width / 2, armoryTopPosition.Y + 71);
            Vector2 bottomCorner = new Vector2(armoryBottomPosition.X + 20 + texture.Width / 2, armoryBottomPosition.Y + 71);

            for (int i = 0; i < GlobalVariables.Data.Characters.Length; i++)
			{
                image = new Image(texture, GetSourceRectangle(i, texture.Width));

                temp = new UnlockButton(unlockColor, affordableColor, lockColor, image, ButtonPressed, ButtonUnlock, ButtonOver);
                temp.Locked = GlobalVariables.Data.Characters[i].Locked;
                temp.ButtonNumber = i;
                temp.ButtonType = UnlockButtonType.TopCharacter;
                temp.Position.X = topCorner.X + i * (texture.Width + 14);
                temp.Position.Y = topCorner.Y;
                topCharacterButtons.Add(temp);

                image = new Image(texture, GetSourceRectangle(i, texture.Width));

                temp = new UnlockButton(unlockColor, affordableColor, lockColor, image, ButtonPressed, ButtonUnlock, ButtonOver);
                temp.Locked = GlobalVariables.Data.Characters[i].Locked;
                temp.ButtonNumber = i;
                temp.ButtonType = UnlockButtonType.BottomCharacter;
                temp.Position.X = bottomCorner.X + i * (texture.Width + 14);
                temp.Position.Y = bottomCorner.Y;
                bottomCharacterButtons.Add(temp);
			}

            texture = OGE.Content.Load<Texture2D>(@"Graphics\Entities\WindowGraphics\Weapons");
            topCorner = new Vector2(topCorner.X, topCorner.Y + 103);
            bottomCorner = new Vector2(bottomCorner.X, bottomCorner.Y + 103);
            
            for (int i = 0; i < GlobalVariables.Data.Weapons.Length; i++)
            {
                image = new Image(texture, GetSourceRectangle(i, texture.Width));

                temp = new UnlockButton(unlockColor, affordableColor, lockColor, image, ButtonPressed, ButtonUnlock, ButtonOver);
                temp.Locked = GlobalVariables.Data.Weapons[i].Locked;
                temp.ButtonNumber = i;
                temp.ButtonType = UnlockButtonType.TopWeapon;
                temp.Position.X = topCorner.X + (i % 6) * (texture.Width + 14);
                temp.Position.Y = topCorner.Y + (i / 6) * (texture.Width + 14);
                topWeaponButtons.Add(temp);

                image = new Image(texture, GetSourceRectangle(i, texture.Width));

                temp = new UnlockButton(unlockColor, affordableColor, lockColor, image, ButtonPressed, ButtonUnlock, ButtonOver);
                temp.Locked = GlobalVariables.Data.Weapons[i].Locked;
                temp.ButtonNumber = i;
                temp.ButtonType = UnlockButtonType.BottomWeapon;
                temp.Position.X = bottomCorner.X + (i % 6) * (texture.Width + 14);
                temp.Position.Y = bottomCorner.Y + (i / 6) * (texture.Width + 14);
                bottomWeaponButtons.Add(temp);
            }

            if (GlobalVariables.IsDemoVersion)
            {
                for (int i = MAX_CHARACTERS_DEMO; i < GlobalVariables.Data.Characters.Length; i++)
                {
                    topCharacterButtons[i].Active = false;
                    bottomCharacterButtons[i].Active = false;
                }

                for (int i = MAX_WEAPONS_DEMO; i < GlobalVariables.Data.Weapons.Length; i++)
                {
                    topWeaponButtons[i].Active = false;
                    bottomWeaponButtons[i].Active = false;
                }

                if (GlobalVariables.TopPlayerIndex >= MAX_CHARACTERS_DEMO)
                {
                    GlobalVariables.TopPlayerIndex = MAX_CHARACTERS_DEMO - 1;
                }

                if (GlobalVariables.BottomPlayerIndex >= MAX_CHARACTERS_DEMO)
                {
                    GlobalVariables.BottomPlayerIndex = MAX_CHARACTERS_DEMO - 1;
                }

                if (GlobalVariables.TopWeaponIndex >= MAX_WEAPONS_DEMO)
                {
                    GlobalVariables.TopWeaponIndex = MAX_WEAPONS_DEMO - 1;
                }

                if (GlobalVariables.BottomWeaponIndex >= MAX_WEAPONS_DEMO)
                {
                    GlobalVariables.BottomWeaponIndex = MAX_WEAPONS_DEMO - 1;
                }
            }

            topCharacterButtons[GlobalVariables.TopPlayerIndex].Selected = true;
            topWeaponButtons[GlobalVariables.TopWeaponIndex].Selected = true;
            bottomCharacterButtons[GlobalVariables.BottomPlayerIndex].Selected = true;
            bottomWeaponButtons[GlobalVariables.BottomWeaponIndex].Selected = true;

            topPlayer = GlobalVariables.GetCorrectPlayerData(GlobalVariables.TopPlayerIndex);
            topWeapon = GlobalVariables.GetCorrectPrimaryWeaponData(GlobalVariables.TopWeaponIndex);
            bottomPlayer = GlobalVariables.GetCorrectPlayerData(GlobalVariables.BottomPlayerIndex);
            bottomWeapon = GlobalVariables.GetCorrectSecondaryWeaponData(GlobalVariables.BottomWeaponIndex);
        }

        public ArmoryAnnouncer(AnnouncerEnded endFunction, Color color, ButtonPressed playPressed, ButtonPressed backPressed, ButtonPressed virusDatabasePressed)
            : base(endFunction, 630)
        {
            text = new Text("Armory Console", FontSize.Large);
            text.Align(AlignType.Center);
            this.TintColor = color;

            topCharacterButtons = new List<UnlockButton>();
            topWeaponButtons = new List<UnlockButton>();
            bottomCharacterButtons = new List<UnlockButton>();
            bottomWeaponButtons = new List<UnlockButton>();

            armoryTopPosition = new Vector2(OGE.HUDCamera.Width / 2 - 580, OGE.HUDCamera.Height / 2 - 220);
            armoryBottomPosition = new Vector2(OGE.HUDCamera.Width / 2 + 180, OGE.HUDCamera.Height / 2 - 220);

            IntializeCharacterAndWeapons();

            armoryImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\WindowGraphics\ArmoryBG"));
            armoryImage.TintColor = color;
            playerCombinationImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\WindowGraphics\PlayerCombinationBG"));
            playerCombinationImage.TintColor = color;
            playerCombinationImage.CenterOrigin();
            bodyWordText = new Text("Body", FontSize.Small);
            turretWordText = new Text("Turret", FontSize.Small);
            playerPositionWordText = new Text("", FontSize.Medium);
            playerPoints = new Text("", FontSize.Medium);
            nameWordText = new Text("Name", FontSize.Small);
            nameText = new Text("", FontSize.Small);
            descriptionWordText = new Text("Description", FontSize.Small);
            descriptionText = new Text("", FontSize.Small);
            overclockingWordText = new Text("Overclocking", FontSize.Small);
            overclockingText = new Text("", FontSize.Small);
            unlockPointsWordText = new Text("Requirements", FontSize.Small);
            unlockPointsText = new Text("", FontSize.Small);
            bodyWordText.Align(AlignType.Center);
            turretWordText.Align(AlignType.Center);
            playerPositionWordText.Align(AlignType.Center);
            playerPoints.Align(AlignType.Center);
            nameWordText.Align(AlignType.Center);
            nameText.Align(AlignType.Center);
            descriptionWordText.Align(AlignType.Center);
            descriptionText.Align(AlignType.Center);
            overclockingWordText.Align(AlignType.Center);
            overclockingText.Align(AlignType.Center);
            unlockPointsWordText.Align(AlignType.Center);
            unlockPointsText.Align(AlignType.Center);
            bodyWordText.TintColor = color;
            turretWordText.TintColor = color;
            playerPositionWordText.TintColor = color;
            playerPoints.TintColor = color;
            nameWordText.TintColor = color;
            nameText.TintColor = color;
            descriptionWordText.TintColor = color;
            descriptionText.TintColor = color;
            overclockingWordText.TintColor = color;
            overclockingText.TintColor = color;
            unlockPointsWordText.TintColor = color;
            unlockPointsText.TintColor = color;

            playButton = new Button(color, "Start Sector", playPressed);
            playButton.AddFunction(SetPlayerInGlobalData);
            playButton.Position.X = OGE.HUDCamera.Width / 2;
            playButton.Position.Y = OGE.HUDCamera.Height / 2 + maxHeight / 2 - 35;

            if (GlobalVariables.SurvivalMode == -1)
            {
                backButton = new Button(color, "Return to Sector Console", backPressed);
            }
            else
            {
                backButton = new Button(color, "Return to Survival Console", backPressed);
            }
            backButton.Position.X = playButton.Position.X + 350;
            backButton.Position.Y = playButton.Position.Y;

            virusDatabaseButton = new Button(color, "Go to Security Console", virusDatabasePressed);
            virusDatabaseButton.Position.X = playButton.Position.X - 350;
            virusDatabaseButton.Position.Y = playButton.Position.Y;
        }

        private void SetPlayerInGlobalData()
        {
            topPlayer.Weapon = topWeapon;
            bottomPlayer.Weapon = bottomWeapon;
            GlobalVariables.TopPlayer = topPlayer.Clone();
            GlobalVariables.BottomPlayer = bottomPlayer.Clone();
            GlobalVariables.TopPlayer.Weapon = topWeapon;
            GlobalVariables.BottomPlayer.Weapon = bottomWeapon;
        }

        private void Unlock()
        {
            if (unlockType == UnlockButtonType.BottomCharacter || unlockType == UnlockButtonType.TopCharacter)
            {
                GlobalVariables.PlayerScore -= GlobalVariables.Data.Characters[unlockNumber].UnlockPoints;
                GlobalVariables.Data.Characters[unlockNumber].Locked = false;
                topCharacterButtons[unlockNumber].Locked = false;
                bottomCharacterButtons[unlockNumber].Locked = false;

                ButtonPressed(unlockNumber, unlockType);
            }
            else if (unlockType == UnlockButtonType.BottomWeapon || unlockType == UnlockButtonType.TopWeapon)
            {
                GlobalVariables.PlayerScore -= GlobalVariables.Data.Weapons[unlockNumber].UnlockPoints;
                GlobalVariables.Data.Weapons[unlockNumber].Locked = false;
                topWeaponButtons[unlockNumber].Locked = false;
                bottomWeaponButtons[unlockNumber].Locked = false;

                ButtonPressed(unlockNumber, unlockType);
            }

            OGE.CurrentWorld.UnPauseGame();
        }

        private void ButtonUnlock(int buttonNumber, UnlockButtonType type)
        {
            if (type == UnlockButtonType.BottomCharacter || type == UnlockButtonType.TopCharacter)
            {
                if (GlobalVariables.PlayerScore >= GlobalVariables.Data.Characters[buttonNumber].UnlockPoints)
                {
                    unlockNumber = buttonNumber;
                    unlockType = type;

                    List<Button> buttons = new List<Button>();
                    buttons.Add(new Button(TintColor, "Yes", Unlock));
                    buttons.Add(new Button(TintColor, "No", OGE.CurrentWorld.UnPauseGame));

                    OGE.CurrentWorld.PauseEntity = new TitleButtonAnnouncerEntity(OGE.CurrentWorld.UnPauseGame, 
                        TintColor, "Unlock " + GlobalVariables.Data.Characters[buttonNumber].Name + " ?", buttons);
                    OGE.CurrentWorld.PauseGame();
                }
                else
                {
                    SoundManager.PlaySFX("error");
                }
            }
            else if (type == UnlockButtonType.BottomWeapon || type == UnlockButtonType.TopWeapon)
            {
                if (GlobalVariables.PlayerScore >= GlobalVariables.Data.Weapons[buttonNumber].UnlockPoints 
                    && GlobalVariables.ClearedLevels >= GlobalVariables.Data.Weapons[buttonNumber].ClearedLevels)
                {
                    unlockNumber = buttonNumber;
                    unlockType = type;

                    List<Button> buttons = new List<Button>();
                    buttons.Add(new Button(TintColor, "Yes", Unlock));
                    buttons.Add(new Button(TintColor, "No", OGE.CurrentWorld.UnPauseGame));

                    OGE.CurrentWorld.PauseEntity = new TitleButtonAnnouncerEntity(OGE.CurrentWorld.UnPauseGame,
                        TintColor, "Unlock " + GlobalVariables.Data.Weapons[buttonNumber].Name + " ?", buttons);
                    OGE.CurrentWorld.PauseGame();
                }
                else
                {
                    SoundManager.PlaySFX("error");
                }
            }
        }

        private void ButtonOver(int buttonNumber, UnlockButtonType type)
        {
            nameWordText.TextContext = "Name";
            descriptionWordText.TextContext = "Description";
            overclockingWordText.TextContext = "Overclocking";

            if (type == UnlockButtonType.BottomCharacter || type == UnlockButtonType.TopCharacter)
            {
                nameText.TextContext = GlobalVariables.Data.Characters[buttonNumber].Name;
                descriptionText.TextContext = GlobalVariables.Data.Characters[buttonNumber].Description;
                overclockingText.TextContext = GlobalVariables.Data.Characters[buttonNumber].Overclocking;

                if (GlobalVariables.Data.Characters[buttonNumber].Locked)
                {
                    unlockPointsWordText.TextContext = "Requirements";
                    unlockPointsText.TextContext = GlobalVariables.Data.Characters[buttonNumber].UnlockPoints + " pts";
                }
            }

            if (type == UnlockButtonType.BottomWeapon || type == UnlockButtonType.TopWeapon)
            {
                nameText.TextContext = GlobalVariables.Data.Weapons[buttonNumber].Name;
                descriptionText.TextContext = GlobalVariables.Data.Weapons[buttonNumber].Description;

                if (GlobalVariables.Data.Weapons[buttonNumber].Locked)
                {
                    unlockPointsWordText.TextContext = "Requirements";

                    string requirements = GlobalVariables.Data.Weapons[buttonNumber].UnlockPoints + " pts";
                    if (GlobalVariables.ClearedLevels < GlobalVariables.Data.Weapons[buttonNumber].ClearedLevels)
                    {
                        requirements += " + " + GlobalVariables.Data.Weapons[buttonNumber].ClearedLevels + " secured sectors (" 
                            + (GlobalVariables.Data.Weapons[buttonNumber].ClearedLevels - GlobalVariables.ClearedLevels) 
                            + " sectors remaining)";
                    }

                    unlockPointsText.TextContext = requirements;
                }
            }
        }

        private void ButtonPressed(int buttonNumber, UnlockButtonType type)
        {
            switch (type)
            {
                case UnlockButtonType.TopCharacter:
                    ClearTopCharacters();
                    topCharacterButtons[buttonNumber].Selected = true;
                    topPlayer = GlobalVariables.GetCorrectPlayerData(buttonNumber);
                    GlobalVariables.TopPlayerIndex = buttonNumber;
                    break;
                case UnlockButtonType.TopWeapon:
                    ClearTopWeapons();
                    topWeaponButtons[buttonNumber].Selected = true;
                    topWeapon = GlobalVariables.GetCorrectPrimaryWeaponData(buttonNumber);
                    GlobalVariables.TopWeaponIndex = buttonNumber;
                    break;
                case UnlockButtonType.BottomCharacter:
                    ClearBottomCharacters();
                    bottomCharacterButtons[buttonNumber].Selected = true;
                    bottomPlayer = GlobalVariables.GetCorrectPlayerData(buttonNumber);
                    GlobalVariables.BottomPlayerIndex = buttonNumber;
                    break;
                case UnlockButtonType.BottomWeapon:
                    ClearBottomWeapons();
                    bottomWeaponButtons[buttonNumber].Selected = true;
                    bottomWeapon = GlobalVariables.GetCorrectSecondaryWeaponData(buttonNumber);
                    GlobalVariables.BottomWeaponIndex = buttonNumber;
                    break;
            }
        }

        private void ClearTopCharacters()
        {
            for (int i = 0; i < topCharacterButtons.Count; i++)
            {
                topCharacterButtons[i].Selected = false;
            }
        }

        private void ClearTopWeapons()
        {
            for (int i = 0; i < topWeaponButtons.Count; i++)
            {
                topWeaponButtons[i].Selected = false;
            }
        }

        private void ClearBottomCharacters()
        {
            for (int i = 0; i < bottomCharacterButtons.Count; i++)
            {
                bottomCharacterButtons[i].Selected = false;
            }
        }

        private void ClearBottomWeapons()
        {
            for (int i = 0; i < bottomWeaponButtons.Count; i++)
            {
                bottomWeaponButtons[i].Selected = false;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (status == AnnouncerStatus.Steady)
            {
                nameWordText.TextContext = "";
                nameText.TextContext = "";
                descriptionWordText.TextContext = "";
                descriptionText.TextContext = "";
                overclockingWordText.TextContext = "";
                overclockingText.TextContext = "";
                unlockPointsWordText.TextContext = "";
                unlockPointsText.TextContext = "";

                for (int i = 0; i < topCharacterButtons.Count; i++)
                {
                    topCharacterButtons[i].Affordable = GlobalVariables.PlayerScore >= GlobalVariables.Data.Characters[i].UnlockPoints;
                    bottomCharacterButtons[i].Affordable = GlobalVariables.PlayerScore >= GlobalVariables.Data.Characters[i].UnlockPoints;

                    topCharacterButtons[i].Update(gameTime);
                    bottomCharacterButtons[i].Update(gameTime);
                }

                for (int i = 0; i < topWeaponButtons.Count; i++)
                {
                    topWeaponButtons[i].Affordable = GlobalVariables.PlayerScore >= GlobalVariables.Data.Weapons[i].UnlockPoints &&
                        GlobalVariables.ClearedLevels >= GlobalVariables.Data.Weapons[i].ClearedLevels;
                    bottomWeaponButtons[i].Affordable = GlobalVariables.PlayerScore >= GlobalVariables.Data.Weapons[i].UnlockPoints &&
                        GlobalVariables.ClearedLevels >= GlobalVariables.Data.Weapons[i].ClearedLevels;

                    topWeaponButtons[i].Update(gameTime);
                    bottomWeaponButtons[i].Update(gameTime);
                }

                playerPoints.TextContext = "[ Points: " + GlobalVariables.PlayerScore + " pts ]";

                playButton.Update(gameTime);
                virusDatabaseButton.Update(gameTime);
                backButton.Update(gameTime);
            }
        }

        public override void Draw(Camera camera)
        {
            base.Draw(camera);

            if (status == AnnouncerStatus.Steady)
            {
                text.Draw(new Vector2(camera.Width / 2, camera.Height / 2 - height / 2 + 5), camera);
                playerPoints.Draw(new Vector2(camera.Width / 2, camera.Height / 2 - height / 2 + text.Height + 10), camera);

                armoryImage.Draw(armoryTopPosition, camera);
                armoryImage.Draw(armoryBottomPosition, camera);
                
                playerPositionWordText.TextContext = "Primary Parts";
                playerPositionWordText.Draw(armoryTopPosition + new Vector2(armoryImage.Width / 2, -playerPositionWordText.Height - 5), 
                    camera);

                playerPositionWordText.TextContext = "Secondary Parts";
                playerPositionWordText.Draw(armoryBottomPosition + new Vector2(armoryImage.Width / 2, -playerPositionWordText.Height - 5), 
                    camera);

                bodyWordText.Draw(armoryTopPosition + new Vector2(armoryImage.Width / 2, 15), camera);
                bodyWordText.Draw(armoryBottomPosition + new Vector2(armoryImage.Width / 2, 15), camera);

                turretWordText.Draw(armoryTopPosition + new Vector2(armoryImage.Width / 2, 115), camera);
                turretWordText.Draw(armoryBottomPosition + new Vector2(armoryImage.Width / 2, 115), camera);

                for (int i = 0; i < topCharacterButtons.Count; i++)
                {
                    topCharacterButtons[i].Draw(camera);
                    bottomCharacterButtons[i].Draw(camera);
                }

                for (int i = 0; i < topWeaponButtons.Count; i++)
                {
                    topWeaponButtons[i].Draw(camera);
                    bottomWeaponButtons[i].Draw(camera);
                }

                playerCombinationImage.Draw(new Vector2(camera.Width / 2 - 1, camera.Height / 2 - 50), camera);
                nameWordText.Draw(new Vector2(camera.Width / 2, camera.Height / 2 + 85), camera);
                nameText.Draw(new Vector2(camera.Width / 2, camera.Height / 2 + nameWordText.Height + 85), camera);
                descriptionWordText.Draw(new Vector2(camera.Width / 2, camera.Height / 2 + nameWordText.Height + nameText.Height + 95), 
                    camera);
                descriptionText.Draw(new Vector2(camera.Width / 2, camera.Height / 2 + nameWordText.Height + 
                    nameText.Height + descriptionWordText.Height + 95), camera);

                float unlockYPosition = camera.Height / 2 + nameWordText.Height + nameText.Height +
                        descriptionWordText.Height + descriptionText.Height + 105;
                if (overclockingText.TextContext != "")
                {
                    overclockingWordText.Draw(new Vector2(camera.Width / 2, unlockYPosition), camera);
                    overclockingText.Draw(new Vector2(camera.Width / 2, unlockYPosition + overclockingWordText.Height), camera);

                    unlockYPosition += overclockingWordText.Height + overclockingText.Height + 10;
                }
                
                unlockPointsWordText.Draw(new Vector2(camera.Width / 2, unlockYPosition), camera);
                unlockPointsText.Draw(new Vector2(camera.Width / 2, unlockYPosition + unlockPointsWordText.Height), camera);

                topPlayer.BodyImage.TintColor = Color.White;
                topPlayer.BodyImage.Draw(new Vector2(camera.Width / 2, camera.Height / 2 - 50), camera);
                bottomPlayer.BodyImage.TintColor = Color.White;
                bottomPlayer.BodyImage.Angle = 180;
                bottomPlayer.BodyImage.Draw(new Vector2(camera.Width / 2, camera.Height / 2 - 50), camera);
                bottomPlayer.BodyImage.Angle = 0;

                topWeapon.TurnetImage.TintColor = Color.White;
                topWeapon.TurnetImage.Angle = 270;
                topWeapon.TurnetImage.Draw(new Vector2(camera.Width / 2, camera.Height / 2 - 50), camera);
                topWeapon.TurnetImage.Angle = 0;
                bottomWeapon.TurnetImage.TintColor = Color.White;
                bottomWeapon.TurnetImage.Angle = 90;
                bottomWeapon.TurnetImage.Draw(new Vector2(camera.Width / 2, camera.Height / 2 - 50), camera);
                bottomWeapon.TurnetImage.Angle = 0;

                playButton.Draw(camera);
                virusDatabaseButton.Draw(camera);
                backButton.Draw(camera);
            }
        }
    }
}
