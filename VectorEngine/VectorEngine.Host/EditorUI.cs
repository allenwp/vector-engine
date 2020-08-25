using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using VectorEngine.Host.Reflection;
using Xna = Microsoft.Xna.Framework;
using System.Reflection;
using VectorEngine.Host.Midi;
using VectorEngine.Host.Util;
using System.IO;

namespace VectorEngine.Host
{
    public class EditorUI
    {
        public static object SelectedEntityComponent { get; set; }
        static object draggedObject = null;

        /// <summary>
        /// When true, this the scene graph view should be scrolled to show the selectedEntityComponent.
        /// </summary>
        static bool scrollSceneGraphView = false;
        /// <summary>
        /// When true, this the entities view should be scrolled to show the selectedEntityComponent.
        /// </summary>
        static bool scrollEntitiesView = false;

        static string invokeFilter = string.Empty;
        static int invokeSelectedIndex = 0;
        
        static Dictionary<string, Assembly> assemblies;
        static int selectedComponentIndex = 0;

        static int selectedComponentGroupFileIndex = 0;

        public static unsafe void SubmitUI(EntityAdmin admin)
        {
            if (assemblies == null)
            {
                assemblies = new Dictionary<string, Assembly>();
                assemblies["Game Assembly"] = Program.GameAssembly;
                assemblies["VectorEngine"] = Assembly.GetAssembly(typeof(VectorEngine.EntityAdmin));
                assemblies["VectorEngine.Extras"] = Assembly.GetAssembly(typeof(VectorEngine.Extras.Util.EditorUtil));
                assemblies["Host Assembly"] = Assembly.GetExecutingAssembly();
            }

            var entityToComponentGroups = EntityAdminUtil.GetEntityToComponentGroupsMapping(admin, out ComponentGroup[] componentGroups);

            SubmitMainMenu();
            SubmitSystemsWindow(admin);
            SubmitSceneGraphWindow(admin);
            SubmitEntitiesWindow(admin, entityToComponentGroups);
            SubmitInspectorWindow(admin);
            SubmitMidiWindow();
            SubmitInvokeWindow();
            SubmitComponentGroupFilesWindow(admin);
            SubmitComponentGroupsWindow(admin, componentGroups);

            CleanUp(admin);
        }

        private static unsafe void SubmitMainMenu()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("Menu"))
                {
                    // TODO?
                    ImGui.EndMenu();
                }
                ImGui.EndMainMenuBar();
            }
        }

        private static unsafe void SubmitSystemsWindow(EntityAdmin admin)
        {
            ImGui.Begin("Vector Engine");

            string playLabel = HostHelper.PlayingGame ? "Stop" : "Play";
            if (ImGui.Button(playLabel))
            {
                HostHelper.TogglePlayGame();
            }
            ImGui.SameLine();
            ImGui.Checkbox("Tick Systems", ref GameLoop.TickSystems);

            ImGui.Separator();
            ImGui.PushFont(ImGuiController.BoldFont);
            ImGui.Text("Systems Order:");
            ImGui.PopFont();
            ImGui.BeginChild("scrolling", Vector2.Zero, false, ImGuiWindowFlags.HorizontalScrollbar);
            foreach (var system in admin.Systems)
            {
                ImGui.Text(system.GetType().ToString());
            }
            ImGui.EndChild();

            ImGui.End();
        }

        private static unsafe void SubmitSceneGraphWindow(EntityAdmin admin)
        {
            ImGui.Begin("Scene Graph");

            var list = admin.GetComponents<Transform>(true).Where(trans => trans.Parent == null).ToList();
            AddSceneGraphTransforms(admin, list);

            ImGui.End();

            scrollSceneGraphView = false;
        }

        private static unsafe void AddSceneGraphTransforms(EntityAdmin admin, List<Transform> list)
        {
            // The selectedTransform is either the selectedEntityComponent (if it's a Transform) or the Transform associated with the selected Entity/Component.
            Transform selectedTransform = SelectedEntityComponent as Transform;
            if (selectedTransform == null)
            {
                Entity entity = SelectedEntityComponent as Entity;
                if (entity == null)
                {
                    Component component = SelectedEntityComponent as Component;
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
                    SelectedEntityComponent = transform;
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

        private static unsafe void SubmitEntitiesWindow(EntityAdmin admin, Dictionary<Entity, List<int>> entityToComponentGroups)
        {
            ImGui.Begin("Entities and Components");

            bool createEntity = SubmitAddComponent("Create Entity", out Type componentType);

            Entity entityToDestroy = SelectedEntityComponent as Entity;
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

            ImGui.Separator();
            ImGui.BeginChild("scrolling", Vector2.Zero, false, ImGuiWindowFlags.HorizontalScrollbar);

            var entities = EntityAdminUtil.GetEntities(admin);

            foreach (var entity in entities)
            {
                var components = entity.Components;

                ImGuiTreeNodeFlags nodeFlags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick | ImGuiTreeNodeFlags.SpanAvailWidth; // OpenOnDoubleClick doesn't seem to work. Not sure why.
                if (entity.Components.Count == 0)
                {
                    nodeFlags |= ImGuiTreeNodeFlags.Leaf;
                }
                if (entity == SelectedEntityComponent)
                {
                    nodeFlags |= ImGuiTreeNodeFlags.Selected;
                    if (scrollEntitiesView)
                    {
                        ImGui.SetScrollHereY();
                    }
                }

                if (entity.HasComponent<DontDestroyOnClear>(true))
                {
                    ImGui.PushFont(ImGuiController.BoldFont);
                }
                if (!entity.IsActive)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1f));
                }
                bool errorInEntity = false;
                for (int i = 0; i < components.Count; i++)
                {
                    if (!RequiresSystem.HasECSSystemForType(components[i].GetType(), HostHelper.GameSystems, out _))
                    {
                        errorInEntity = true;
                    }
                    if (components[i] == SelectedEntityComponent)
                    {
                        ImGui.SetNextItemOpen(true);
                    }
                }
                string componentGroupsText = string.Empty;
                if (entityToComponentGroups.ContainsKey(entity))
                {
                    foreach (int groupNum in entityToComponentGroups[entity])
                    {
                        componentGroupsText = $"({groupNum}){componentGroupsText}";
                    }
                    if (entityToComponentGroups[entity].Count() > 1)
                    {
                        errorInEntity = true;
                    }
                }
                if (errorInEntity)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 0f, 0f, 1f));
                }
                string entityTransformText = !entity.HasComponent<Transform>(true) ? "~ " : "";
                string entityLabelText = $"{componentGroupsText}{entityTransformText}{entity.Name}";
                bool expanded = ImGui.TreeNodeEx(entity.Guid.ToString(), nodeFlags, entityLabelText);
                if (errorInEntity)
                {
                    ImGui.PopStyleColor();
                }
                if (!entity.IsActive)
                {
                    ImGui.PopStyleColor();
                }
                if (entity.HasComponent<DontDestroyOnClear>(true))
                {
                    ImGui.PopFont();
                }

                if (ImGui.IsItemClicked())
                {
                    SelectedEntityComponent = entity;
                    scrollSceneGraphView = true;
                }
                if (expanded)
                {
                    for (int i = 0; i < components.Count; i++)
                    {
                        nodeFlags = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.SpanAvailWidth | ImGuiTreeNodeFlags.NoTreePushOnOpen; // This last one means that you can't do aImGui.TreePop(); or things will be messed up. 
                        if (components[i] == SelectedEntityComponent)
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
                        bool hasRequiredSystems = RequiresSystem.HasECSSystemForType(components[i].GetType(), HostHelper.GameSystems, out _);
                        if (!hasRequiredSystems)
                        {
                            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 0f, 0f, 1f));
                        }
                        ImGui.TreeNodeEx(components[i].Guid.ToString(), nodeFlags, components[i].Name);
                        if (!hasRequiredSystems)
                        {
                            ImGui.PopStyleColor();
                        }
                        if (!components[i].IsActive)
                        {
                            ImGui.PopStyleColor();
                        }
                        if (ImGui.IsItemClicked())
                        {
                            SelectedEntityComponent = components[i];
                            scrollSceneGraphView = true;
                        }
                    }
                    ImGui.TreePop();
                }
            }

            scrollEntitiesView = false;
            if (createEntity)
            {
                var entity = admin.CreateEntity($"{componentType.Name}'s Entity");
                SelectedEntityComponent = admin.AddComponent(entity, componentType);
                scrollEntitiesView = true; // For some reason this doesn't work half the time -_- Not sure why...
            }

            ImGui.End();

        }

        private static unsafe void SubmitInspectorWindow(EntityAdmin admin)
        {
            ImGui.Begin("Inspector");

            ImGuiTreeNodeFlags collapsingHeaderFlags = ImGuiTreeNodeFlags.CollapsingHeader;
            collapsingHeaderFlags |= ImGuiTreeNodeFlags.DefaultOpen;

            if (SelectedEntityComponent != null)
            {
                var entity = SelectedEntityComponent as Entity;
                var component = SelectedEntityComponent as Component;

                if (entity != null)
                {
                    ImGui.PushFont(ImGuiController.BoldFont);
                    ImGui.Text("Entity: " + entity.Name);
                    ImGui.PopFont();

                    ImGui.Separator();
                    Type componentType;
                    if (SubmitAddComponent("Add Component", out componentType))
                    {
                        SelectedEntityComponent = admin.AddComponent(entity, componentType);
                        entity = null;
                        component = SelectedEntityComponent as Component;
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

                    Type missingSystem;
                    bool hasRequiredSystems = RequiresSystem.HasECSSystemForType(component.GetType(), HostHelper.GameSystems, out missingSystem);
                    if (!hasRequiredSystems)
                    {
                        ImGui.Separator();
                        ImGui.NewLine();
                        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 0f, 0f, 1f));
                        ImGui.PushFont(ImGuiController.BoldFont);
                        ImGui.Text("ERROR: MISSING REQUIRED SYSTEM " + missingSystem);
                        ImGui.PopFont();
                        ImGui.PopStyleColor();
                        ImGui.NewLine();
                    }
                }

                ImGui.Separator();
                ImGui.BeginChild("scrolling", Vector2.Zero, false, ImGuiWindowFlags.HorizontalScrollbar);

                var selectedType = SelectedEntityComponent.GetType();

                if (ImGui.CollapsingHeader("Fields", collapsingHeaderFlags))
                {
                    var fields = selectedType.GetFields();
                    foreach (var info in fields.Where(field => !field.IsLiteral && !field.IsInitOnly))
                    {
                        SubmitFieldPropertyInspector(new FieldPropertyInfo(info), SelectedEntityComponent);
                    }
                }

                var properties = selectedType.GetProperties();

                if (ImGui.CollapsingHeader("Properties", collapsingHeaderFlags))
                {
                    // When SetMethod is private, it will still be writable so long as it's class isn't inherited, so check to see if it's public too for the behaviour I want.
                    foreach (var info in properties.Where(prop => prop.CanRead && prop.CanWrite && prop.SetMethod.IsPublic))
                    {
                        SubmitFieldPropertyInspector(new FieldPropertyInfo(info), SelectedEntityComponent);
                    }
                }

                if (ImGui.CollapsingHeader("Read-Only Properties", collapsingHeaderFlags))
                {
                    // When SetMethod is private, it will still be writable so long as it's class isn't inherited, so check to see if it's public too for the behaviour I want.
                    foreach (var info in properties.Where(prop => prop.CanRead && (!prop.CanWrite || !prop.SetMethod.IsPublic)))
                    {
                        var fieldPropertyInfo = new FieldPropertyInfo(info);
                        SubmitReadonlyFieldPropertyInspector(fieldPropertyInfo, SelectedEntityComponent);
                        SubmitHelpMarker(fieldPropertyInfo);
                    }
                }

                ComponentGroup compGroup = SelectedEntityComponent as ComponentGroup;
                if (compGroup != null)
                {
                    ImGui.NewLine();
                    ImGui.Separator();

                    ImGui.PushFont(ImGuiController.BoldFont);
                    ImGui.Text($"Component Group: {compGroup.FileName}");
                    ImGui.PopFont();
                    if (!string.IsNullOrWhiteSpace(compGroup.FileName))
                    {
                        if (ImGui.Button("Save"))
                        {
                            SaveClearComponentGroup(admin, compGroup, true, false);
                        }
                        if (ImGui.Button("Save & Clear from Scene"))
                        {
                            SaveClearComponentGroup(admin, compGroup, true, true);
                        }
                        if (ImGui.Button("Clear Without Saving"))
                        {
                            SaveClearComponentGroup(admin, compGroup, false, true);
                        }
                    }
                    else
                    {
                        ImGui.Text("(Please set file name to reveal saving functionality.)");
                    }
                }
            }

            ImGui.End();
        }

        static void SaveClearComponentGroup(EntityAdmin admin, ComponentGroup compGroup, bool save, bool clear)
        {
            List<Component> components = clear ? new List<Component>() : null;
            var json = Serialization.SerializationHelper.Serialize(compGroup, components);
            if (save)
            {
                Directory.CreateDirectory(ComponentGroup.ROOT_PATH);
                FileLoader.SaveTextFile(compGroup.ComponentGroupPath, json);
            }
            if (clear)
            {
                foreach (var component in components)
                {
                    admin.RemoveComponent(component);
                }
            }
        }

        static string GetIdString(FieldPropertyInfo info, object entityComponent)
        {
            string objectID = entityComponent.ToString();
            if (entityComponent as Component != null)
            {
                objectID = (entityComponent as Component).Guid.ToString();
            }
            else if (entityComponent as Entity != null)
            {
                objectID = (entityComponent as Entity).Guid.ToString();
            }
            return objectID + info.Name;
        }

        static bool SubmitAddComponent(string buttonLabel, out Type component)
        {
            List<Type> components = new List<Type>();
            foreach (var assembly in assemblies.Values)
            {
                components.AddRange(assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(Component)) && !type.IsAbstract));
            }
            string[] componentNames = new string[components.Count];
            for (int i = 0; i < components.Count; i++)
            {
                componentNames[i] = components[i].FullName;
            }
            ImGui.Combo("", ref selectedComponentIndex, componentNames, componentNames.Length, Xna.MathHelper.Clamp(componentNames.Length, 0, 50));
            ImGui.SameLine();

            component = components[selectedComponentIndex];
            return ImGui.Button(buttonLabel);
        }

        static void SubmitFieldPropertyInspector(FieldPropertyInfo info, object entityComponent, bool showMidi = true)
        {
            ImGui.PushID(GetIdString(info, entityComponent));

            var rangeAttribute = CustomAttributeExtensions.GetCustomAttribute<EditorHelper.RangeAttribute>(info.MemberInfo, true);

            var infoType = info.FieldPropertyType;
            if (infoType == typeof(string))
            {
                string val = (string)info.GetValue(entityComponent);
                if (val == null)
                {
                    val = string.Empty;
                }
                if (ImGui.InputText(info.Name, ref val, 1000))
                {
                    info.SetValue(entityComponent, val);
                }
            }
            else if (infoType == typeof(bool))
            {
                if (showMidi) SubmitMidiAssignment(entityComponent, info, MidiState.MidiControlDescriptionType.Button);

                bool val = (bool)info.GetValue(entityComponent);
                if (ImGui.Checkbox(info.Name, ref val))
                {
                    info.SetValue(entityComponent, val);
                }
            }
            else if (infoType == typeof(float))
            {
                if (showMidi) SubmitMidiAssignment(entityComponent, info, MidiState.MidiControlDescriptionType.Knob);

                float val = (float)info.GetValue(entityComponent);
                bool result;
                if (rangeAttribute != null
                    && (rangeAttribute.RangeType == EditorHelper.RangeAttribute.RangeTypeEnum.Float
                        || rangeAttribute.RangeType == EditorHelper.RangeAttribute.RangeTypeEnum.Int))
                {
                    if (rangeAttribute.RangeType == EditorHelper.RangeAttribute.RangeTypeEnum.Float)
                    {
                        result = ImGui.SliderFloat(info.Name, ref val, rangeAttribute.MinFloat, rangeAttribute.MaxFloat);
                    }
                    else
                    {
                        result = ImGui.SliderFloat(info.Name, ref val, rangeAttribute.MinInt, rangeAttribute.MaxInt);
                    }
                }
                else
                {
                    result = ImGui.DragFloat(info.Name, ref val, 0.1f);
                }
                if (result)
                {
                    info.SetValue(entityComponent, val);
                }
            }
            else if (infoType == typeof(Vector2))
            {
                if (showMidi) SubmitMidiAssignment(entityComponent, info, MidiState.MidiControlDescriptionType.Knob);

                Vector2 val = (Vector2)info.GetValue(entityComponent);
                if (ImGui.DragFloat2(info.Name, ref val))
                {
                    info.SetValue(entityComponent, val);
                }
            }
            else if (infoType == typeof(Vector3))
            {
                if (showMidi) SubmitMidiAssignment(entityComponent, info, MidiState.MidiControlDescriptionType.Knob);

                Vector3 val = (Vector3)info.GetValue(entityComponent);
                if (ImGui.DragFloat3(info.Name, ref val))
                {
                    info.SetValue(entityComponent, val);
                }
            }
            else if (infoType == typeof(Vector4))
            {
                if (showMidi) SubmitMidiAssignment(entityComponent, info, MidiState.MidiControlDescriptionType.Knob);

                Vector4 val = (Vector4)info.GetValue(entityComponent);
                if (ImGui.DragFloat4(info.Name, ref val))
                {
                    info.SetValue(entityComponent, val);
                }
            }
            else if (infoType == typeof(Xna.Vector2))
            {
                if (showMidi) SubmitMidiAssignment(entityComponent, info, MidiState.MidiControlDescriptionType.Knob);

                Xna.Vector2 xnaVal = (Xna.Vector2)info.GetValue(entityComponent);
                Vector2 val = new Vector2(xnaVal.X, xnaVal.Y);
                if (ImGui.DragFloat2(info.Name, ref val))
                {
                    xnaVal.X = val.X;
                    xnaVal.Y = val.Y;
                    info.SetValue(entityComponent, xnaVal);
                }
            }
            else if (infoType == typeof(Xna.Vector3))
            {
                if (showMidi) SubmitMidiAssignment(entityComponent, info, MidiState.MidiControlDescriptionType.Knob);

                Xna.Vector3 xnaVal = (Xna.Vector3)info.GetValue(entityComponent);
                Vector3 val = new Vector3(xnaVal.X, xnaVal.Y, xnaVal.Z);
                if (ImGui.DragFloat3(info.Name, ref val))
                {
                    xnaVal.X = val.X;
                    xnaVal.Y = val.Y;
                    xnaVal.Z = val.Z;
                    info.SetValue(entityComponent, xnaVal);
                }
            }
            else if (infoType == typeof(Xna.Vector4))
            {
                if (showMidi) SubmitMidiAssignment(entityComponent, info, MidiState.MidiControlDescriptionType.Knob);

                Xna.Vector4 xnaVal = (Xna.Vector4)info.GetValue(entityComponent);
                Vector4 val = new Vector4(xnaVal.X, xnaVal.Y, xnaVal.Z, xnaVal.W);
                if (ImGui.DragFloat4(info.Name, ref val))
                {
                    xnaVal.X = val.X;
                    xnaVal.Y = val.Y;
                    xnaVal.Z = val.Z;
                    xnaVal.W = val.W;
                    info.SetValue(entityComponent, xnaVal);
                }
            }
            else if (infoType == typeof(int))
            {
                if (showMidi) SubmitMidiAssignment(entityComponent, info, MidiState.MidiControlDescriptionType.Knob);

                int val = (int)info.GetValue(entityComponent);
                bool result;
                if (rangeAttribute != null && rangeAttribute.RangeType == EditorHelper.RangeAttribute.RangeTypeEnum.Int)
                {
                    result = ImGui.SliderInt(info.Name, ref val, rangeAttribute.MinInt, rangeAttribute.MaxInt);
                }
                else
                {
                    result = ImGui.InputInt(info.Name, ref val);
                }
                if (result)
                {
                    info.SetValue(entityComponent, val);
                }
            }
            else if (infoType == typeof(uint))
            {
                if (showMidi) SubmitMidiAssignment(entityComponent, info, MidiState.MidiControlDescriptionType.Knob);

                int val = (int)((uint)info.GetValue(entityComponent));
                bool result;
                if (rangeAttribute != null && rangeAttribute.RangeType == EditorHelper.RangeAttribute.RangeTypeEnum.Int)
                {
                    result = ImGui.SliderInt(info.Name, ref val, rangeAttribute.MinInt, rangeAttribute.MaxInt);
                }
                else
                {
                    result = ImGui.InputInt(info.Name, ref val);
                }
                if (result)
                {
                    if (val < 0)
                    {
                        val = 0;
                    }
                    info.SetValue(entityComponent, (uint)val);
                }
            }
            else if (infoType.IsEnum)
            {
                if (showMidi) SubmitMidiAssignment(entityComponent, info, MidiState.MidiControlDescriptionType.Button);

                var val = info.GetValue(entityComponent);
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
                    info.SetValue(entityComponent, infoType.GetEnumValues().GetValue(currentIndex));
                }
            }
            else
            {
                SubmitReadonlyFieldPropertyInspector(info, entityComponent);
            }
            ImGui.PopID();

            SubmitHelpMarker(info);
        }

        static void SubmitReadonlyFieldPropertyInspector(FieldPropertyInfo info, object entityComponent)
        {
            string valText;
            var value = info.GetValue(entityComponent);
            if (value != null)
            {
                valText = value.ToString();
            }
            else
            {
                valText = "null";
            }
            ImGui.Text($"{info.Name}: {valText}");
        }

        static void SubmitMidiAssignment(object entityComponent, FieldPropertyInfo info, MidiState.MidiControlDescriptionType type)
        {
            if (Program.MidiState.Assigning && Program.MidiState.LastAssignmentType == type)
            {
                ImGui.PushID(GetIdString(info, entityComponent));
                if (ImGui.Button("A"))
                {
                    Program.MidiState.AssignControl(entityComponent, info);
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

        private static unsafe void SubmitMidiWindow()
        {
            ImGui.Begin("MIDI Controller");

            ImGui.Text("Knob float step: " + Program.MidiState.KnobControlStep);

            if (Program.MidiState.Assigning)
            {
                ImGui.PushFont(ImGuiController.BoldFont);
                ImGui.TextColored(new Vector4(1, 0, 0, 1), "Assigning " + Program.MidiState.LastAssignmentControlString);
                ImGui.PopFont();
            }
            else
            {
                ImGui.NewLine();
            }

            ImGui.Separator();
            ImGui.BeginChild("scrolling", Vector2.Zero, false, ImGuiWindowFlags.HorizontalScrollbar);

            foreach (var controlStatePair in Program.MidiState.ControlStates)
            {
                var controlState = controlStatePair.Value;
                if (controlState.ControlledObject != null)
                {
                    var description = Program.MidiState.AssignToControlMapping[controlStatePair.Key];
                    string objectName = controlState.ControlledObject.ToString();
                    if (controlState.ControlledObject as Component != null)
                    {
                        objectName = $"{(controlState.ControlledObject as Component).EntityName}: {objectName}";
                    }

                    string vectorField = string.Empty;
                    if (controlState.IsVector)
                    {
                        switch (controlState.VectorIndex)
                        {
                            case 0:
                                vectorField = "X";
                                break;
                            case 1:
                                vectorField = "Y";
                                break;
                            case 2:
                                vectorField = "Z";
                                break;
                            case 3:
                                vectorField = "W";
                                break;
                        }

                        vectorField = $" ({vectorField})";
                    }
                    if (ImGui.Button($"{Program.MidiState.GetControlName(description.Id, description.Type)}: {objectName}{vectorField}"))
                    {
                        SelectedEntityComponent = controlState.ControlledObject;
                        scrollEntitiesView = true;
                        scrollSceneGraphView = true;
                    }
                    SubmitFieldPropertyInspector(controlState.FieldPropertyInfo, controlState.ControlledObject, false);
                    ImGui.NewLine();
                }
            }
            ImGui.End();
        }

        private static unsafe void SubmitInvokeWindow()
        {
            ImGui.Begin("Invoke");

            if (ImGui.BeginTabBar("Assemblies Tab Bar", ImGuiTabBarFlags.None))
            {
                if (ImGui.BeginTabItem("Selected Object"))
                {
                    ImGui.InputText("Filter", ref invokeFilter, 500);
                    invokeFilter = invokeFilter.ToLower();

                    bool doInvoke = ImGui.Button("Invoke");

                    ImGui.Separator();

                    ImGui.BeginChild("scrolling", Vector2.Zero, false, ImGuiWindowFlags.HorizontalScrollbar);

                    // TODO: for selected object, do similar to the other assemblies
                    ImGui.Text("(Selected Ojbect not yet implemented. Implement when needed.)");

                    ImGui.EndChild();

                    ImGui.EndTabItem();
                }

                foreach (var assemblyPair in assemblies)
                {
                    if (ImGui.BeginTabItem(assemblyPair.Key))
                    {
                        ImGui.InputText("Filter", ref invokeFilter, 500);
                        invokeFilter = invokeFilter.ToLower();

                        bool doInvoke = ImGui.Button("Invoke"); // TODO: allow input of parameters.

                        ImGui.Separator();

                        ImGui.BeginChild("scrolling", Vector2.Zero, false, ImGuiWindowFlags.HorizontalScrollbar);

                        var names = (from type in assemblyPair.Value.GetTypes()
                                     from method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                                     select (type, method, type.FullName + ":" + method.Name)).Where(a => a.method.GetParameters().Length < 1).ToList();

                        for (int i = 0; i < names.Count; i++)
                        {
                            if (names[i].Item3.ToLower().Contains(invokeFilter))
                            {
                                if (names[i].method.IsPrivate)
                                {
                                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1f));
                                }
                                StringBuilder sb = new StringBuilder();
                                sb.Append('(');
                                foreach (var param in names[i].method.GetParameters())
                                {
                                    sb.Append(param.ParameterType.Name);
                                    sb.Append(' ');
                                    sb.Append(param.Name);
                                }
                                sb.Append(')');
                                var text = $"{names[i].Item3}{sb}";
                                if (ImGui.Selectable(text, invokeSelectedIndex == i))
                                {
                                    invokeSelectedIndex = i;
                                }
                                if (names[i].method.IsPrivate)
                                {
                                    ImGui.PopStyleColor();
                                }
                            }
                        }

                        if (doInvoke && names.Count > invokeSelectedIndex)
                        {
                            try
                            {
                                names[invokeSelectedIndex].method.Invoke(null, null);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        }

                        ImGui.EndChild();

                        ImGui.EndTabItem();
                    }
                }
                ImGui.EndTabBar();
            }

            ImGui.End();
        }

        public static void SubmitComponentGroupFilesWindow(EntityAdmin admin)
        {
            ImGui.Begin("Component Group Files");

            var assetFiles = FileLoader.GetAllComponentGroupPaths();

            if (ImGui.Button("Load to Scene"))
            {
                Serialization.SerializationHelper.LoadComponentGroup(admin, assetFiles[selectedComponentGroupFileIndex], out _, true);
            }
            ImGui.SameLine();
            if (ImGui.Button("Delete"))
            {
                try
                {
                    Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(FileLoader.FullPath(assetFiles[selectedComponentGroupFileIndex]), Microsoft.VisualBasic.FileIO.UIOption.AllDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                } catch { } // Don't care if they cancel the dialog
            }
            ImGui.Separator();

            ImGui.BeginChild("scrolling", Vector2.Zero, false, ImGuiWindowFlags.HorizontalScrollbar);
            for (int i = 0; i < assetFiles.Length; i++)
            {
                if (ImGui.Selectable(assetFiles[i], selectedComponentGroupFileIndex == i))
                {
                    selectedComponentGroupFileIndex = i;
                }
            }
            ImGui.EndChild();

            ImGui.End();
        }

        public static void SubmitComponentGroupsWindow(EntityAdmin admin, ComponentGroup[] componentGroups)
        {
            ImGui.Begin("Component Groups in Scene");

            ImGui.BeginChild("scrolling", Vector2.Zero, false, ImGuiWindowFlags.HorizontalScrollbar);
            for (int i = 0; i < componentGroups.Length; i++)
            {
                if (ImGui.Selectable($"({i}){componentGroups[i].FileName}", SelectedEntityComponent == componentGroups[i]))
                {
                    SelectedEntityComponent = componentGroups[i];
                    scrollEntitiesView = true;
                    scrollSceneGraphView = true;
                }
            }
            ImGui.EndChild();

            ImGui.End();
        }

        public static void CleanUp(EntityAdmin admin)
        {
            var livingObjects = EntityAdminUtil.GetNextTickLivingObjects(admin);
            if (SelectedEntityComponent != null && !livingObjects.Contains(SelectedEntityComponent))
            {
                SelectedEntityComponent = null;
            }

            var midiButtonsToClear = Program.MidiState.ControlStates.Where(pair => pair.Value.ControlledObject != null && !livingObjects.Contains(pair.Value.ControlledObject)).Select(pair => pair.Key).ToList();
            foreach (var button in midiButtonsToClear)
            {
                Program.MidiState.ClearControlState(button);
            }
        }
    }
}
