﻿using System;
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

            GameLoop.SceneInit = VectorEngine.DemoGame.SceneEditorTest.Init;
            Thread thread = new Thread(new ThreadStart(GameLoop.Loop));
            thread.Name = "Game Loop Thread";
            // this apartment state is required for the ASIOOutput.StartDriver method
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }
    }
}