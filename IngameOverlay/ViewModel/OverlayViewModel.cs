using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace IngameOverlay.ViewModel
{
    public class OverlayViewModel
    {
        public ObservableCollection<string> inhibTimer { get; set; } = new ObservableCollection<string>() { "", "", "", "", "", "" };
        public ObservableCollection<bool> isTimerShowed { get; set; } = new ObservableCollection<bool>(){false, false, false, false};
    }
}
