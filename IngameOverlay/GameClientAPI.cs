using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace IngameOverlay
{
    public partial class GameClientAPI
    {
        public List<Event> Events { get; set; } = new List<Event>();
        public bool isGameend = false;
        public event OnEventOccured onEventOccured;
        public delegate void OnEventOccured(EventsArgs events);
        public event OnMessageRecieved onMessageRecieved;
        public delegate void OnMessageRecieved(MessageEventArgs message);

        public async Task StartAsync()
        {
            while (true)
            {
                if ((Process.GetProcessesByName("League of Legends").Length > 0))
                {
                    if (!isGameend)
                    {
                        await GetEvents();
                        await Task.Delay(200);
                    }
                    else
                    {
                        onMessageRecieved(new MessageEventArgs(format("Waiting for LoL ends.")));
                        await Task.Delay(1000);
                    }
                }
                else
                {
                    if (this.Events.Count != 0)
                    {
                        this.Events.Clear();
                        onMessageRecieved(new MessageEventArgs(format("All events cleared.")));
                        isGameend = false;
                    }
                    else
                    {
                        onMessageRecieved(new MessageEventArgs(format("Waiting for LoL starts.")));
                        await Task.Delay(5000);
                    }
                }
            }
        }

        private async Task GetEvents()
        {
            var events = await GetEventDataAsync();
            if (events != null)
            {
                IEnumerable<Event> newEvents = events.Except(this.Events, new EventCompereer());
                foreach (var value in newEvents)
                {
                    this.Events.Add(value);
                    if (value.EventName.Equals("GameEnd"))
                    {
                        isGameend = true;
                    }
                    onEventOccured(new EventsArgs(value));
                }
            }
            else
            {
                onMessageRecieved(new MessageEventArgs(format("Now loading.")));
            }
        }

        private async Task<List<Event>> GetEventDataAsync()
        {
            return await Task.Run(() =>
             {
                 try
                 {
                     var request = WebRequest.Create("https://127.0.0.1:2999/liveclientdata/eventdata");
                     var response = request.GetResponse();
                     using (System.IO.Stream stream = response.GetResponseStream())
                     {
                         var sr = new System.IO.StreamReader(stream, System.Text.Encoding.GetEncoding("utf-8"));
                         return JsonSerializer.Deserialize<EventList>(sr.ReadToEnd()).Events;
                     }
                 }
                 catch (Exception)
                 {
                     return null;
                 }
             });
        }

        public class EventsArgs : EventArgs
        {
            public Event events { get; set; }
            public EventsArgs(Event e)
            {
                this.events = e;
            }
        }

        public class MessageEventArgs : EventArgs
        {
            public string message { get; set; }
            public MessageEventArgs(string message)
            {
                this.message = message;
            }
        }
        private string format(string message)
        {
            return "[Info] " + message;
        }
    }
}
