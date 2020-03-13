using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IngameOverlay
{
    public partial class GameClientAPI
    {
        private List<Event> before { get; set; } = new List<Event>();
        public List<Event> Events { get; set; } = new List<Event>();
        public event OnEventOccured onEventOccured;
        public delegate void OnEventOccured(EventsArgs events);

        public GameClientAPI()
        {
            GetEvents();
        }
        private async void GetEvents()
        {
            var isgameend = true;
            while (isgameend)
            {
                try
                {
                    if (Process.GetProcessesByName("League of Legends").Length > 0)
                    {
                        var events = await GetEventsAsync();
                        IEnumerable<Event> results = events.Except(before, new EventCompereer());
                        foreach (var value in results)
                        {
                            //Console.WriteLine(value.ToString());
                            if (value.EventName.Equals("GameEnd"))
                            {
                                isgameend = false;
                            }
                        }
                        before = events;
                        if (results.Count() != 0)
                        {
                            onEventOccured(new EventsArgs((List<Event>)results));
                        }
                    }
                    else
                    {
                        Console.WriteLine(format("waiting for process"));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(format(e.ToString()));
                }

                System.Threading.Thread.Sleep(250);
            }
            before.Clear();
            Console.WriteLine(format("Game has been finished."));
        }

        private async Task<List<Event>> GetEventsAsync()
        {
            return await Task<List<Event>>.Run(() =>
            {
                var request = WebRequest.Create("https://127.0.0.1:2999/liveclientdata/eventdata");
                var response = request.GetResponse();
                using (System.IO.Stream stream = response.GetResponseStream())
                {
                    var sr = new System.IO.StreamReader(stream, System.Text.Encoding.GetEncoding("utf-8"));
                    return JsonSerializer.Deserialize<EventList>(sr.ReadToEnd()).Events;
                }
            });
        }

        private string format(string message)
        {
            return "[" + System.DateTime.Now.ToString("HH:mm:ss") + "] " + message;
        }

        public class EventsArgs : EventArgs
        {
            public List<Event> events { get; set; }
            public EventsArgs(List<Event> e)
            {
                this.events = e;
            }
        }
    }
}
