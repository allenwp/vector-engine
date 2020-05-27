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
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            GameLoop.SceneInit = Flight.Scenes.Main.Init;
            Thread thread = new Thread(new ThreadStart(GameLoop.Loop));
            thread.Name = "Game Loop Thread";
            thread.IsBackground = true; // So that it aborts along with the application
            thread.Start();

            MIDI midi = new MIDI();
            midi.SetupWatchers();
            Thread.Sleep(1000);
            Task.Run(midi.SetupMidiPorts);
        }
    }
}
