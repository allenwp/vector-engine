using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using System.Threading;
using Windows.Devices.Midi;
using VectorEngine.Host.Midi;
using VectorEngine.Host.Util;
using System.Reflection;

namespace VectorEngine.Host
{
    class Program
    {
        private static Sdl2Window _window;
        private static GraphicsDevice _gd;
        private static CommandList _cl;
        private static ImGuiController _controller;

        public static readonly Type GameConfigType = typeof(VectorEngine.Calibration.GameConfig);
        public static readonly Assembly GameAssembly = Assembly.GetAssembly(GameConfigType);

        public static readonly Vector3 CLEAR_COLOR_PLAY = new Vector3(0.946f, 0.370f, 0.014f);
        public static readonly Vector3 CLEAR_COLOR_STOPPED = new Vector3(0.45f, 0.55f, 0.6f);
        public static Vector3 ClearColor = CLEAR_COLOR_STOPPED;

        public static Entity EditorCamera = null;

#if DEBUG
        private static bool _showEditor = true;
#else
        private static bool _showEditor = false;
#endif

        private static MIDI midi = null;
        public static MidiState MidiState { get; private set; } = new MidiState();

        [STAThread] // Needed for ASIOOutput.StartDriver method
        static void Main(string[] args)
        {
            // Create window, GraphicsDevice, and all resources necessary for the demo.
            VeldridStartup.CreateWindowAndGraphicsDevice(
                new WindowCreateInfo(50, 50, 3600, 2000, WindowState.Normal, "Vector Engine Editor"),
                new GraphicsDeviceOptions(true, null, true),
                out _window,
                out _gd);

            _gd.MainSwapchain.SyncToVerticalBlank = false;

            _window.Resized += () =>
            {
                _gd.MainSwapchain.Resize((uint)_window.Width, (uint)_window.Height);
                _controller.WindowResized(_window.Width, _window.Height);
            };
            _cl = _gd.ResourceFactory.CreateCommandList();
            _controller = new ImGuiController(_gd, _gd.MainSwapchain.Framebuffer.OutputDescription, _window.Width, _window.Height);

            string assetsPath = HostHelper.AssetsPath;
            FileLoader.Init(assetsPath);
            FileLoader.LoadAllComponentGroups();

            if (_showEditor)
            {
                HostHelper.StopGame(true);
            }
            else
            {
                HostHelper.PlayGame(true);
            }

            // Main application loop
            while (_window.Exists)
            {
                GameLoop.Tick();

                InputSnapshot snapshot = _window.PumpEvents();
                if (!_window.Exists) { break; }

                foreach (var keypress in snapshot.KeyEvents)
                {
                    if (keypress.Key == Key.E && keypress.Down && keypress.Modifiers == ModifierKeys.Control)
                    {
                        _showEditor = !_showEditor;
                    }
                    if (keypress.Key == Key.S && keypress.Down && keypress.Modifiers == ModifierKeys.Control)
                    {
                        HostHelper.SaveScene();
                    }
                }

                if (_showEditor)
                {
                    if (EditorCamera == null)
                    {
                        foreach (var component in EntityAdmin.Instance.Components)
                        {
                            if (component.Entity.Name == EmptyScene.EDITOR_CAM_ENTITY_NAME)
                            {
                                EditorCamera = component.Entity;
                                break;
                            }
                        }
                    }

                    if (midi == null)
                    {
                        midi = new MIDI();
                        midi.SetupWatchersAndPorts();
                        while (!midi.SetupComplete)
                        {
                            Thread.Sleep(1);
                        }
                    }

                    IMidiMessage midiMessage;
                    while (midi.MidiMessageQueue.TryDequeue(out midiMessage))
                    {
                        MidiState.UpdateState(midiMessage);
                    }

                    // TODO: figure out why LastFrameTime makes ImGui run stupid fast... (for things like key repeats)
                    _controller.Update(GameTime.LastFrameTime / 10f, snapshot); // Feed the input events to our ImGui controller, which passes them through to ImGui.

                    EditorUI.SubmitUI(EntityAdmin.Instance);

                    _cl.Begin();
                    _cl.SetFramebuffer(_gd.MainSwapchain.Framebuffer);
                    _cl.ClearColorTarget(0, new RgbaFloat(ClearColor.X, ClearColor.Y, ClearColor.Z, 1f));
                    _controller.Render(_gd, _cl);
                    _cl.End();
                    _gd.SubmitCommands(_cl);
                    _gd.SwapBuffers(_gd.MainSwapchain);
                }
            }

            if (!HostHelper.PlayingGame)
            {
                HostHelper.SaveScene();
            }

            // Clean up Veldrid resources
            _gd.WaitForIdle();
            _controller.Dispose();
            _cl.Dispose();
            _gd.Dispose();
        }
    }
}
