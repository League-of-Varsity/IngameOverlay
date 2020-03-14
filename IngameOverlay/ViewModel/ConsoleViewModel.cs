using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace IngameOverlay.ViewModel
{
    public class ConsoleViewModel
    {
        public ObservableCollection<string> consoleLog { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<int> inhibTimer { get; set; } = new ObservableCollection<int>() { -1,-1,-1,-1,-1,-1 };
        public ConsoleViewModel()
        {
        }
    }
}