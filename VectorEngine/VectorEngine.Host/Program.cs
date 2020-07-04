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
using VectorEngine.Host.Reflection;
using Xna = Microsoft.Xna.Framework;
using System.Reflection;
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

        static object selectedEntityComponent = null;
        static object draggedObject = null;

        public static Entity EditorCamera = null;

        /// <summary>
        /// When true, this the scene graph view should be scrolled to show the selectedEntityComponent.
        /// </summary>
        static bool scrollSceneGraphView = false;
        /// <summary>
        /// When true, this the entities view should be scrolled to show the selectedEntityComponent.
        /// </summary>
        static bool scrollEntitiesView = false;

#if DEBUG
        private static bool _showEditor = true;
#else
        private static bool _showEditor = false;
#endif

        private static MIDI midi = null;
        private static MidiState midiState = new MidiState();

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

                        // TODO: configure a button 8 to be EditorCamera.SelfEnabled
                        //if (((MidiNoteOnMessage)midiMessage).Note == 16) // speicial case TODO: Move this to just an automatic assignemnt at start
                        //{
                        //    EditorCamera.SelfEnabled = !EditorCamera.SelfEnabled;
                        //}
                    }

                    IMidiMessage midiMessage;
                    while (midi.MidiMessageQueue.TryDequeue(out midiMessage))
                    {
                        midiState.UpdateState(midiMessage);
                    }

                    // TODO: figure out why LastFrameTime makes ImGui run stupid fast... (for things like key repeats)
                    _controller.Update(GameTime.LastFrameTime / 10f, snapshot); // Feed the input events to our ImGui controller, which passes them through to ImGui.

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
            SubmitInspectorWindow(admin);
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

        private static unsafe void SubmitSceneGraphWindow(EntityAdmin admin)
        {
            ImGui.Begin("Scene Graph");

            var list = EntityAdmin.Instance.GetComponents<Transform>(true).Where(trans => trans.Parent == null).ToList();
            AddSceneGraphTransforms(admin, list);

            ImGui.End();

            scrollSceneGraphView = false;
        }

        private static unsafe void AddSceneGraphTransforms(EntityAdmin admin, List<Transform> list)
        {
            // The selectedTransform is either the selectedEntityComponent (if it's a Transform) or the Transform associated with the selected Entity/Component.
            Transform selectedTransform = selectedEntityComponent as Transform;
            if (selectedTransform == null)
            {
                Entity entity = selectedEntityComponent as Entity;
                if (entity == null)
                {
                    Component component = selectedEntityComponent as Component;
                    if (component != null)
                    {
                        entity = component.Entity;
                    }
                }

                if (entity != null)
                {
                    selectedTransform = entity.GetComponent<Transform>(true);
                }
            }

            foreach (var transform in list)
            {
                ImGuiTreeNodeFlags nodeFlags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick | ImGuiTreeNodeFlags.SpanAvailWidth; // OpenOnDoubleClick doesn't seem to work. Not sure why.
                if (transform.Children.Count == 0)
                {
                    nodeFlags |= ImGuiTreeNodeFlags.Leaf;
                }
                if (transform == selectedTransform)
                {
                    nodeFlags |= ImGuiTreeNodeFlags.Selected;
                    if (scrollSceneGraphView)
                    {
                        ImGui.SetScrollHereY();
                    }
                }

                bool parentOfSelected = false;
                Transform parent = selectedTransform as Transform;
                while (parent != null)
                {
                    parent = parent.Parent;
                    if (parent == transform)
                    {
                        parentOfSelected = true;
                        break;
                    }
                }
                if (parentOfSelected)
                {
                    ImGui.SetNextItemOpen(true);
                }
                if (!transform.IsActive)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1f));
                }
                bool expanded = ImGui.TreeNodeEx(transform.Guid.ToString(), nodeFlags, transform.EntityName);
                if (!transform.IsActive)
                {
                    ImGui.PopStyleColor();
                }

                if (ImGui.IsItemClicked())
                {
                    selectedEntityComponent = transform;
                    scrollEntitiesView = true;
                }
                if (ImGui.BeginDragDropSource())
                {
                    ImGui.SetDragDropPayload(typeof(Transform).FullName, IntPtr.Zero, 0); // Payload is needed to trigger BeginDragDropTarget()
                    draggedObject = transform;
                    ImGui.Text(transform.EntityName);
                    ImGui.EndDragDropSource();
                }
                if (ImGui.BeginDragDropTarget())
                {
                    var payload = ImGui.AcceptDragDropPayload(typeof(Transform).FullName);
                    if (payload.NativePtr != null) // Only when this is non-null does it mean that we've released the drag
                    {
                        var draggedTransform = draggedObject as Transform;
                        if (draggedTransform != null)
                        {
                            Transform newParent;
                            if (draggedTransform.Parent == transform)
                            {
                                newParent = null;
                            }
                            else
                            {
                                newParent = transform;
                            }
                            Transform.AssignParent(draggedTransform, newParent, admin, true);
                        }
                        draggedObject = null;
                    }
                    ImGui.EndDragDropTarget();
                }
                if (expanded)
                {
                    AddSceneGraphTransforms(admin, transform.Children.ToList());
                    ImGui.TreePop();
                }
            }
        }

        private static unsafe void SubmitEntitiesWindow(EntityAdmin admin)
        {
            ImGui.Begin("Entities and Components");

            if (ImGui.Button("Create Entity"))
            {
                var entity = admin.CreateEntity("Create Entity");
                selectedEntityComponent = entity;
                scrollEntitiesView = true;
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

            foreach (var entity in admin.Entities)
            {
                var components = entity.Components;

                ImGuiTreeNodeFlags nodeFlags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick | ImGuiTreeNodeFlags.SpanAvailWidth; // OpenOnDoubleClick doesn't seem to work. Not sure why.
                if (entity.Components.Count == 0)
                {
                    nodeFlags |= ImGuiTreeNodeFlags.Leaf;
                }
                if (entity == selectedEntityComponent)
                {
                    nodeFlags |= ImGuiTreeNodeFlags.Selected;
                    if (scrollEntitiesView)
                    {
                        ImGui.SetScrollHereY();
                    }
                }

                if (entity.GetComponent<Transform>(true) == null)
                {
                    ImGui.PushFont(ImGuiController.BoldFont);
                }
                if (!entity.IsActive)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1f));
                }
                for (int i = 0; i < components.Count; i++)
                {
                    if (components[i] == selectedEntityComponent)
                    {
                        ImGui.SetNextItemOpen(true);
                    }
                }
                bool expanded = ImGui.TreeNodeEx(entity.Guid.ToString(), nodeFlags, entity.Name);
                if (!entity.IsActive)
                {
                    ImGui.PopStyleColor();
                }
                if (entity.GetComponent<Transform>(true) == null)
                {
                    ImGui.PopFont();
                }

                if (ImGui.IsItemClicked())
                {
                    selectedEntityComponent = entity;
                    scrollSceneGraphView = true;
                }
                if (expanded)
                {
                    for (int i = 0; i < components.Count; i++)
                    {
                        nodeFlags = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.SpanAvailWidth | ImGuiTreeNodeFlags.NoTreePushOnOpen; // This last one means that you can't do aImGui.TreePop(); or things will be messed up. 
                        if (components[i] == selectedEntityComponent)
                        {
                            nodeFlags |= ImGuiTreeNodeFlags.Selected;
                            if (scrollEntitiesView)
                            {
                                ImGui.SetScrollHereY();
                            }
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
                            scrollSceneGraphView = true;
                        }
                    }
                    ImGui.TreePop();
                }
            }

            // Don't leak an object that's been destroyed
            bool foundSelected = false;
            foreach (var entity in admin.Entities)
            {
                if (selectedEntityComponent == entity)
                {
                    foundSelected = true;
                    break;
                }
                foundSelected = entity.Components.Contains(selectedEntityComponent);
                if (foundSelected)
                {
                    break;
                }
            }
            if (!foundSelected)
            {
                selectedEntityComponent = null;
            }

            ImGui.End();

            scrollEntitiesView = false;
        }

        private static unsafe void SubmitInspectorWindow(EntityAdmin admin)
        {
            ImGui.Begin("Inspector");

            ImGuiTreeNodeFlags collapsingHeaderFlags = ImGuiTreeNodeFlags.CollapsingHeader;
            collapsingHeaderFlags |= ImGuiTreeNodeFlags.DefaultOpen;

            if (selectedEntityComponent != null)
            {
                var entity = selectedEntityComponent as Entity;
                var component = selectedEntityComponent as Component;

                if (entity != null)
                {
                    ImGui.PushFont(ImGuiController.BoldFont);
                    ImGui.Text("Entity: " + entity.Name);
                    ImGui.PopFont();

                    // TOOD: Show a drop down of all components in the assemblies
                    if (ImGui.Button("Add Component"))
                    {
                        // TODO
                    }
                }

                if (component != null)
                {
                    ImGui.PushFont(ImGuiController.BoldFont);
                    ImGui.Text("Entity: " + component.Entity.Name);
                    ImGui.Text("Component: " + component.Name);
                    ImGui.PopFont();

                    if (ImGui.Button("Remove Component"))
                    {
                        admin.RemoveComponent(component);
                    }
                }

                ImGui.NewLine();

                var selectedType = selectedEntityComponent.GetType();

                if (ImGui.CollapsingHeader("Fields", collapsingHeaderFlags))
                {
                    var fields = selectedType.GetFields();
                    foreach (var info in fields)
                    {
                        SubmitFieldPropertyInspector(new FieldPropertyInfo(info));
                    }
                }

                var properties = selectedType.GetProperties();

                if (ImGui.CollapsingHeader("Properties", collapsingHeaderFlags))
                {
                    // When SetMethod is private, it will still be writable so long as it's class isn't inherited, so check to see if it's public too for the behaviour I want.
                    foreach (var info in properties.Where(prop => prop.CanRead && prop.CanWrite && prop.SetMethod.IsPublic))
                    {
                        SubmitFieldPropertyInspector(new FieldPropertyInfo(info));
                    }
                }

                if (ImGui.CollapsingHeader("Read-Only Properties", collapsingHeaderFlags))
                {
                    // When SetMethod is private, it will still be writable so long as it's class isn't inherited, so check to see if it's public too for the behaviour I want.
                    foreach (var info in properties.Where(prop => prop.CanRead && (!prop.CanWrite || !prop.SetMethod.IsPublic)))
                    {
                        var fieldPropertyInfo = new FieldPropertyInfo(info);
                        SubmitReadonlyFieldPropertyInspector(fieldPropertyInfo);
                        SubmitHelpMarker(fieldPropertyInfo);
                    }
                }
            }

            ImGui.End();
        }

        static void SubmitFieldPropertyInspector(FieldPropertyInfo info)
        {
            var infoType = info.FieldPropertyType;
            if (infoType == typeof(string))
            {
                string val = (string)info.GetValue(selectedEntityComponent);
                if (val == null)
                {
                    val = string.Empty;
                }
                if (ImGui.InputText(info.Name, ref val, 1000))
                {
                    info.SetValue(selectedEntityComponent, val);
                }
            }
            else if (infoType == typeof(bool))
            {
                SubmitMidiAssignment(selectedEntityComponent, info);

                bool val = (bool)info.GetValue(selectedEntityComponent);
                if (ImGui.Checkbox(info.Name, ref val))
                {
                    info.SetValue(selectedEntityComponent, val);
                }
            }
            else if (infoType == typeof(float))
            {
                SubmitMidiAssignment(selectedEntityComponent, info);

                float val = (float)info.GetValue(selectedEntityComponent);
                if (ImGui.DragFloat(info.Name, ref val))
                {
                    info.SetValue(selectedEntityComponent, val);
                }
            }
            else if (infoType == typeof(Vector2))
            {
                Vector2 val = (Vector2)info.GetValue(selectedEntityComponent);
                if (ImGui.DragFloat2(info.Name, ref val))
                {
                    info.SetValue(selectedEntityComponent, val);
                }
            }
            else if (infoType == typeof(Vector3))
            {
                Vector3 val = (Vector3)info.GetValue(selectedEntityComponent);
                if (ImGui.DragFloat3(info.Name, ref val))
                {
                    info.SetValue(selectedEntityComponent, val);
                }
            }
            else if (infoType == typeof(Vector4))
            {
                Vector4 val = (Vector4)info.GetValue(selectedEntityComponent);
                if (ImGui.DragFloat4(info.Name, ref val))
                {
                    info.SetValue(selectedEntityComponent, val);
                }
            }
            else if (infoType == typeof(Xna.Vector2))
            {
                Xna.Vector2 xnaVal = (Xna.Vector2)info.GetValue(selectedEntityComponent);
                Vector2 val = new Vector2(xnaVal.X, xnaVal.Y);
                if (ImGui.DragFloat2(info.Name, ref val))
                {
                    xnaVal.X = val.X;
                    xnaVal.Y = val.Y;
                    info.SetValue(selectedEntityComponent, xnaVal);
                }
            }
            else if (infoType == typeof(Xna.Vector3))
            {
                Xna.Vector3 xnaVal = (Xna.Vector3)info.GetValue(selectedEntityComponent);
                Vector3 val = new Vector3(xnaVal.X, xnaVal.Y, xnaVal.Z);
                if (ImGui.DragFloat3(info.Name, ref val))
                {
                    xnaVal.X = val.X;
                    xnaVal.Y = val.Y;
                    xnaVal.Z = val.Z;
                    info.SetValue(selectedEntityComponent, xnaVal);
                }
            }
            else if (infoType == typeof(Xna.Vector4))
            {
                Xna.Vector4 xnaVal = (Xna.Vector4)info.GetValue(selectedEntityComponent);
                Vector4 val = new Vector4(xnaVal.X, xnaVal.Y, xnaVal.Z, xnaVal.W);
                if (ImGui.DragFloat4(info.Name, ref val))
                {
                    xnaVal.X = val.X;
                    xnaVal.Y = val.Y;
                    xnaVal.Z = val.Z;
                    xnaVal.W = val.W;
                    info.SetValue(selectedEntityComponent, xnaVal);
                }
            }
            else if (infoType == typeof(int))
            {
                SubmitMidiAssignment(selectedEntityComponent, info);

                int val = (int)info.GetValue(selectedEntityComponent);
                if (ImGui.InputInt(info.Name, ref val))
                {
                    info.SetValue(selectedEntityComponent, val);
                }
            }
            else if (infoType == typeof(uint))
            {
                SubmitMidiAssignment(selectedEntityComponent, info);

                int val = (int)((uint)info.GetValue(selectedEntityComponent));
                if (ImGui.InputInt(info.Name, ref val))
                {
                    if (val < 0)
                    {
                        val = 0;
                    }
                    info.SetValue(selectedEntityComponent, (uint)val);
                }
            }
            else if (infoType.IsEnum)
            {
                var val = info.GetValue(selectedEntityComponent);
                var enumNames = infoType.GetEnumNames();
                int currentIndex = 0;
                for (int i = 0; i < enumNames.Length; i++)
                {
                    if (enumNames[i] == val.ToString())
                    {
                        currentIndex = i;
                    }
                }
                if (ImGui.Combo(info.Name, ref currentIndex, enumNames, enumNames.Length))
                {
                    info.SetValue(selectedEntityComponent, currentIndex);
                }
            }
            else
            {
                SubmitReadonlyFieldPropertyInspector(info);
            }

            SubmitHelpMarker(info);
        }

        static void SubmitReadonlyFieldPropertyInspector(FieldPropertyInfo info)
        {
            string valText;
            var value = info.GetValue(selectedEntityComponent);
            if (value != null)
            {
                valText = value.ToString();
            }
            else
            {
                valText = "null";
            }
            ImGui.Text(string.Format("{0}: {1}", info.Name, valText));
        }

        static void SubmitMidiAssignment(object selectedEntityComponent, FieldPropertyInfo info)
        {
            if (midiState.Assigning)
            {
                ImGui.PushID(selectedEntityComponent.ToString() + info.Name);
                if (ImGui.Button("A"))
                {
                    midiState.AssignControl(selectedEntityComponent, info);
                }
                ImGui.SameLine();
                ImGui.PopID();
            }
        }

        static void SubmitHelpMarker(FieldPropertyInfo info)
        {
            var helpAttribute = CustomAttributeExtensions.GetCustomAttribute<EditorHelper.HelpAttribute>(info.MemberInfo, true);
            if (helpAttribute != null)
            {
                ImGui.SameLine();
                HelpMarker(helpAttribute.HelpText);
            }
        }

        // Helper to display a little (?) mark which shows a tooltip when hovered.
        // In your own code you may want to display an actual icon if you are using a merged icon fonts (see docs/FONTS.txt)
        static void HelpMarker(string desc)
        {
            ImGui.TextDisabled("(?)");
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35.0f);
                ImGui.TextUnformatted(desc);
                ImGui.PopTextWrapPos();
                ImGui.EndTooltip();
            }
        }
    }
}
