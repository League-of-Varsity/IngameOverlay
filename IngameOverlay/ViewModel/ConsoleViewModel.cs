using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace IngameOverlay.ViewModel
{
    public class ConsoleViewModel
    {
        public ObservableCollection<string> consoleLog { get; set; } = new ObservableCollection<string>();
        public List<ObjectTimer> timer = new List<ObjectTimer>() { new ObjectTimer(0), new ObjectTimer(1), new ObjectTimer(2), new ObjectTimer(3), new ObjectTimer(4), new ObjectTimer(5) };
        public ConsoleViewModel()
        {

        }
    }
}