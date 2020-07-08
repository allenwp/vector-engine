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

namespace VectorEngine.Host
{
    class Program
    {
        private static Sdl2Window _window;
        private static GraphicsDevice _gd;
        private static CommandList _cl;
        private static ImGuiController _controller;

        private static Vector3 _clearColor = new Vector3(0.45f, 0.55f, 0.6f);

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

            GameLoop.Init(Flight.Scenes.Main.Init);

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
                }

                if (_showEditor)
                {
                    if (EditorCamera == null)
                    {
                        EditorCamera = VectorEngine.Extras.Util.EditorUtil.CreateSceneViewCamera();
                    }

                    if (midi == null)
                    {
                        midi = new MIDI();
                        midi.SetupWatchersAndPorts();
                        while (!midi.SetupComplete)
                        {
                            Thread.Sleep(1);
                        }

                        var gameTimes = EntityAdmin.Instance.GetComponents<GameTimeSingleton>();
                        if (gameTimes.Count() > 0)
                        {
                            MidiState.AssignControl(gameTimes.First(), "Paused", 16);
                        }
                        MidiState.AssignControl(EditorCamera, "SelfEnabled", 17);
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
                    _cl.ClearColorTarget(0, new RgbaFloat(_clearColor.X, _clearColor.Y, _clearColor.Z, 1f));
                    _controller.Render(_gd, _cl);
                    _cl.End();
                    _gd.SubmitCommands(_cl);
                    _gd.SwapBuffers(_gd.MainSwapchain);
                }
            }

            // Clean up Veldrid resources
            _gd.WaitForIdle();
            _controller.Dispose();
            _cl.Dispose();
            _gd.Dispose();
        }
    }
}
