using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using ImGuiNET;
using System.Runtime.InteropServices;

namespace VectorEngine.Host
{
    class Program
    {
        private static Sdl2Window _window;
        private static GraphicsDevice _gd;
        private static CommandList _cl;
        private static ImGuiController _controller;

        private static Vector3 _clearColor = new Vector3(0.45f, 0.55f, 0.6f);

        static object selectedEntityComponent = null;

#if DEBUG
        private static bool _showEditor = true;
#else
        private static bool _showEditor = false;
#endif

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
                    _controller.Update(GameTime.LastFrameTime, snapshot); // Feed the input events to our ImGui controller, which passes them through to ImGui.

                    SubmitUI(EntityAdmin.Instance);

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

        private static unsafe void SubmitUI(EntityAdmin admin)
        {
            SubmitSystemsWindow(admin);
            SubmitSceneGraphWindow(admin);
            SubmitEntitiesWindow(admin);
        }

        private static unsafe void SubmitSystemsWindow(EntityAdmin admin)
        {
            ImGui.Begin("Systems Order");
            foreach (var system in admin.Systems)
            {
                ImGui.Text(system.GetType().ToString());
            }
            ImGui.End();
        }

        /// <returns>GUID of selected Transform or Guid.Empty if none selected</returns>
        private static unsafe void SubmitSceneGraphWindow(EntityAdmin admin)
        {
            ImGui.Begin("Scene Graph");
            // TODO
            ImGui.End();
        }

        /// <returns>Selected Entity or component or null if none selected</returns>
        private static unsafe void SubmitEntitiesWindow(EntityAdmin admin)
        {
            ImGui.Begin("Entities and Components");

            if (ImGui.Button("New Entity"))
            {
                var entity = admin.CreateEntity("New Entity");
                selectedEntityComponent = entity;
            }

            ImGui.SameLine();

            Entity entityToDestroy = selectedEntityComponent as Entity;
            bool disabled = entityToDestroy == null;
            if (disabled)
            {
                Util.ImGuiUtil.BeginDisable();
            }
            if (ImGui.Button("Destroy Entity") && !disabled)
            {
                admin.DestroyEntity(entityToDestroy);
            }
            if (disabled)
            {
                Util.ImGuiUtil.EndDisable();
            }

            bool foundSelected = false;

            foreach (var entity in admin.Entities)
            {
                ImGuiTreeNodeFlags nodeFlags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick | ImGuiTreeNodeFlags.SpanAvailWidth; // OpenOnDoubleClick doesn't seem to work. Not sure why.
                if (entity.Components.Count == 0)
                {
                    nodeFlags |= ImGuiTreeNodeFlags.Leaf;
                }
                if (entity == selectedEntityComponent)
                {
                    nodeFlags |= ImGuiTreeNodeFlags.Selected;
                    foundSelected = true;
                }
                if (!entity.IsActive)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1f));
                }
                bool expanded = ImGui.TreeNodeEx(entity.Guid.ToString(), nodeFlags, entity.Name);
                if (!entity.IsActive)
                {
                    ImGui.PopStyleColor();
                }
                if (ImGui.IsItemClicked())
                {
                    selectedEntityComponent = entity;
                    foundSelected = true;
                }
                if (expanded)
                {
                    var components = entity.Components;
                    for (int i = 0; i < components.Count; i++)
                    {
                        nodeFlags = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.SpanAvailWidth | ImGuiTreeNodeFlags.NoTreePushOnOpen; // This last one means that you can't do aImGui.TreePop(); or things will be messed up. 
                        if (components[i] == selectedEntityComponent)
                        {
                            nodeFlags |= ImGuiTreeNodeFlags.Selected;
                            foundSelected = true;
                        }            
                        if (!components[i].IsActive)
                        {
                            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1f));
                        }
                        ImGui.TreeNodeEx(components[i].Guid.ToString(), nodeFlags, components[i].Name);
                        if (!components[i].IsActive)
                        {
                            ImGui.PopStyleColor();
                        }
                        if (ImGui.IsItemClicked())
                        {
                            selectedEntityComponent = components[i];
                            foundSelected = true;
                        }
                    }
                    ImGui.TreePop();
                }
            }

            // Don't leak an object that's been destroyed
            if (!foundSelected)
            {
                selectedEntityComponent = null;
            }

            ImGui.End();
        }
    }
}
