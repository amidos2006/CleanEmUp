using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;

namespace OmidosGameEngine.Data
{
    public class DriveData
    {
        public const int MAX_DRIVE_NUMBER = 5;

        [XmlElement(ElementName="Drive")]
        public StoryData[] DrivesData;

        public static void SetDriveBackdrop(int driveNumber)
        {
            if (driveNumber <= MAX_DRIVE_NUMBER)
            {
                GlobalVariables.Background = new Backdrop(OGE.Content.Load<Texture2D>(@"Graphics\Backgrounds\Background" + driveNumber));
            }
        }
    }
}
