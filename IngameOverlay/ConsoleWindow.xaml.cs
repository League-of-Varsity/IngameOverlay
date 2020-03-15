using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace IngameOverlay
{
    /// <summary>
    /// ConsoleWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ConsoleWindow : Window
    {
        private OverlayWindow overlay;
        private ViewModel.ConsoleViewModel viewmodel;
        public GameClientAPI gameClientAPI { get; set; } = new GameClientAPI();
        public ConsoleWindow()
        {
            InitializeComponent();

            var uri = new Uri("./Resource/riotgames.cer", UriKind.Relative);
            var info = Application.GetResourceStream(uri);
            var memoryStream = new MemoryStream();
            info.Stream.CopyTo(memoryStream);
            Certification.Install(memoryStream.ToArray());
            gameClientAPI.onEventOccured += GameClientAPI_onEventOccured;
            gameClientAPI.onMessageRecieved += GameClientAPI_onMessageRecieved;
            viewmodel.consoleLog.CollectionChanged += ConsoleLog_CollectionChanged;
            gameClientAPI.StartAsync();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            viewmodel = (ViewModel.ConsoleViewModel)this.DataContext;
            overlay = new OverlayWindow();
            overlay.Show();
            viewmodel.timer.ForEach((element) =>
            {
                element.onTicked += ConsoleWindow_onTicked;
                element.onTimerEnd += ConsoleWindow_onTimerEnd;
            });
            viewmodel.consoleLog.Add("Program started successfully!");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            overlay.Close();
        }

        private void GameClientAPI_onMessageRecieved(GameClientAPI.MessageEventArgs message)
        {
            if (viewmodel.consoleLog.Last() != message.message)
            {
                viewmodel.consoleLog.Add(message.message);
            }
        }

        private void ConsoleLog_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var listview = this.ConsoleBox;
            if (listview.Items.Count > 5)
            {
                ListBoxAutomationPeer svAutomation = (ListBoxAutomationPeer)ScrollViewerAutomationPeer.CreatePeerForElement(listview);
                IScrollProvider scrollInterface = (IScrollProvider)svAutomation.GetPattern(PatternInterface.Scroll);
                System.Windows.Automation.ScrollAmount scrollVertical = System.Windows.Automation.ScrollAmount.LargeIncrement;
                System.Windows.Automation.ScrollAmount scrollHorizontal = System.Windows.Automation.ScrollAmount.NoAmount;
                if (scrollInterface.VerticallyScrollable)
                    scrollInterface.Scroll(scrollHorizontal, scrollVertical);
            }
        }

        private void GameClientAPI_onEventOccured(GameClientAPI.EventsArgs events)
        {
            viewmodel.consoleLog.Add(events.events.ToString());
            switch (events.events.EventName)
            {
                case "InhibKilled":
                    onInhibitorKilled(events.events);
                    break;
                case "GameEnd":
                    onGameEnd();
                    break;
                default:
                    break;
            }
        }

        private void onInhibitorKilled(GameClientAPI.Event e)
        {
            var index = 0;
            switch (e.InhibKilled)
            {
                case "Barracks_T1_L1":
                    index = 0;
                    break;
                case "Barracks_T1_C1":
                    index = 1;
                    break;
                case "Barracks_T1_R1":
                    index = 2;
                    break;
                case "Barracks_T2_L1":
                    index = 3;
                    break;
                case "Barracks_T2_C1":
                    index = 4;
                    break;
                case "Barracks_T2_R1":
                    index = 5;
                    break;
                default:
                    break;
            }
            viewmodel.timer[index].Start();
        }

        private void onGameEnd()
        {
            foreach (var item in viewmodel.timer)
                item.Stop();
            for (int i = 0; i < overlay.viewmodel.inhibTimer.Count; i++)
                overlay.viewmodel.inhibTimer[i] = "";
            for (int i = 0; i < overlay.viewmodel.isTimerShowed.Count; i++)
                overlay.viewmodel.isTimerShowed[i] = false;
        }

        private void ConsoleWindow_onTicked(ObjectTimer sender, EventArgs args)
        {
            var team = sender.id < 3 ? 0 : 1;
            overlay.viewmodel.isTimerShowed[team] = true;
            var span = new TimeSpan(0, 0, sender.leftTime);
            overlay.viewmodel.inhibTimer[sender.id] = sender.leftTime >= 0 ? span.ToString(@"m\:ss") : "";
        }

        private void ConsoleWindow_onTimerEnd(ObjectTimer sender, EventArgs args)
        {
            sender.Stop();
            var team = sender.id < 3 ? 0 : 1;
            if (team == 0)
            {
                var array = new int[3] { viewmodel.timer[0].leftTime, viewmodel.timer[1].leftTime, viewmodel.timer[2].leftTime };
                overlay.viewmodel.isTimerShowed[team] = array.Sum() == -3 ? false : true;
            }
            else
            {
                var array = new int[3] { viewmodel.timer[3].leftTime, viewmodel.timer[4].leftTime, viewmodel.timer[5].leftTime };
                overlay.viewmodel.isTimerShowed[team] = array.Sum() == -3 ? false : true;
            }
        }
    }
}