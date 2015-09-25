using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Tween;
using OmidosGameEngine.Graphics;

namespace OmidosGameEngine.Entity.OverLayer
{
    public class TimeAnnouncerEntity:AnnouncerEntity
    {
        private Alarm endAlarm;

        public TimeAnnouncerEntity(AnnouncerEnded endFunction, float height, float time, FontSize size)
            : base(endFunction, height)
        {
            endAlarm = new Alarm(time, TweenType.OneShot, new AlarmFinished(FinishAnnouncer));
            AddTween(endAlarm, true);

            text = new Text("", size);
        }
    }
}