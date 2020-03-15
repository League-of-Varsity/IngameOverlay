using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
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
            viewmodel.inhibTimer.CollectionChanged += InhibTimer_CollectionChanged;
            viewmodel.consoleLog.CollectionChanged += ConsoleLog_CollectionChanged;
            gameClientAPI.StartAsync();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            viewmodel = (ViewModel.ConsoleViewModel)this.DataContext;
            overlay = new OverlayWindow();
            overlay.Show();
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
            if (listview.Items.Count > 10)
            {
                listview.Items.MoveCurrentToLast();
                listview.ScrollIntoView(listview.Items.CurrentItem);
            }
        }

        private void InhibTimer_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var team = e.NewStartingIndex < 3 ? 0 : 1;
            overlay.viewmodel.isTimerShowed[team] = true;
            var collection = (ObservableCollection<int>)sender;
            var span = new TimeSpan(0, 0, collection[e.NewStartingIndex]);
            overlay.viewmodel.inhibTimer[e.NewStartingIndex] = collection[e.NewStartingIndex] >= 0 ? span.ToString(@"m\:ss") : "";
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
            viewmodel.inhibTimer[index] = 300;
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += (s, e) =>
            {
                viewmodel.inhibTimer[index]--;
                if (viewmodel.inhibTimer[index] < 0)
                {
                    dispatcherTimer.Stop();
                    var team = index < 3 ? 0 : 1;
                    if(team == 0)
                    {
                        var array = new int[3] { viewmodel.inhibTimer[0], viewmodel.inhibTimer[1], viewmodel.inhibTimer[2] };
                        overlay.viewmodel.isTimerShowed[team] = array.Sum() == -3 ? false : true;
                    }
                    else
                    {
                        var array = new int[3] { viewmodel.inhibTimer[3], viewmodel.inhibTimer[4], viewmodel.inhibTimer[5] };
                        overlay.viewmodel.isTimerShowed[team] = array.Sum() == -3 ? false : true;
                    }
                }
            };
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void onGameEnd()
        {
            gameClientAPI.isGameend = true;
            gameClientAPI.Events.Clear();
            viewmodel.consoleLog.Add("Game ended.");
        }
    }
}