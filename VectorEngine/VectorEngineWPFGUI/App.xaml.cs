using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using VectorEngine;

namespace VectorEngineWPFGUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Entity EditorCamera;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Thread thread = new Thread(new ThreadStart(Loop));
            thread.Name = "Game Loop Thread";
            thread.IsBackground = true; // So that it aborts along with the application
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            MIDI midi = new MIDI();
            midi.SetupWatchers();
            Thread.Sleep(1000);
            Task.Run(midi.SetupMidiPorts);
        }

        public static void Loop()
        {
            GameLoop.Init(Flight.Scenes.Main.Init);
            EditorCamera = VectorEngine.Extras.Util.EditorUtil.CreateSceneViewCamera();
            while (true)
            {
                GameLoop.Tick();
            }
        }
    }
}
