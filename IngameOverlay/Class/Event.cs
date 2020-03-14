using System;
using System.Collections.Generic;

namespace IngameOverlay
{
    public partial class GameClientAPI
    {
        public class EventList
        {
            public List<Event> Events { get; set; }
        }


        public class Event
        {
            public int EventID { get; set; }
            public string EventName { get; set; }
            public float EventTime { get; set; }
            public string KillerName { get; set; }
            public string TurretKilled { get; set; }
            public List<string> Assisters { get; set; }
            public string InhibKilled { get; set; }
            public string DragonType { get; set; }
            public string Stolen { get; set; }
            public string VictimName { get; set; }
            public int KillStreak { get; set; }
            public string Acer { get; set; }
            public string AcingTeam { get; set; }

            public override string ToString()
            {
                var span = new TimeSpan(0, 0, Convert.ToInt32(EventTime));
                return string.Format("[{0}][{1}] {2}", EventID.ToString().PadLeft(3, '0'), span.ToString(@"mm\:ss"), EventName);
            }
        }

        public class EventCompereer : IEqualityComparer<Event>
        {
            public bool Equals(Event x, Event y)
            {
                if (x.EventID == y.EventID)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public int GetHashCode(Event obj)
            {
                return obj.EventID.GetHashCode();
            }
        }
    }
}