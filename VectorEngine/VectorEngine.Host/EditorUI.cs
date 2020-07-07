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

namespace VectorEngine.Host
{
    public class EditorUI
    {
        static object selectedEntityComponent = null;
        static object draggedObject = null;

        /// <summary>
        /// When true, this the scene graph view should be scrolled to show the selectedEntityComponent.
        /// </summary>
        static bool scrollSceneGraphView = false;
        /// <summary>
        /// When true, this the entities view should be scrolled to show the selectedEntityComponent.
        /// </summary>
        static bool scrollEntitiesView = false;

        public static unsafe void SubmitUI(EntityAdmin admin)
        {
            SubmitSystemsWindow(admin);
            SubmitSceneGraphWindow(admin);
            SubmitEntitiesWindow(admin);
            SubmitInspectorWindow(admin);
            SubmitMidiWindow();
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
                        SubmitFieldPropertyInspector(new FieldPropertyInfo(info), selectedEntityComponent);
                    }
                }

                var properties = selectedType.GetProperties();

                if (ImGui.CollapsingHeader("Properties", collapsingHeaderFlags))
                {
                    // When SetMethod is private, it will still be writable so long as it's class isn't inherited, so check to see if it's public too for the behaviour I want.
                    foreach (var info in properties.Where(prop => prop.CanRead && prop.CanWrite && prop.SetMethod.IsPublic))
                    {
                        SubmitFieldPropertyInspector(new FieldPropertyInfo(info), selectedEntityComponent);
                    }
                }

                if (ImGui.CollapsingHeader("Read-Only Properties", collapsingHeaderFlags))
                {
                    // When SetMethod is private, it will still be writable so long as it's class isn't inherited, so check to see if it's public too for the behaviour I want.
                    foreach (var info in properties.Where(prop => prop.CanRead && (!prop.CanWrite || !prop.SetMethod.IsPublic)))
                    {
                        var fieldPropertyInfo = new FieldPropertyInfo(info);
                        SubmitReadonlyFieldPropertyInspector(fieldPropertyInfo, selectedEntityComponent);
                        SubmitHelpMarker(fieldPropertyInfo);
                    }
                }
            }

            ImGui.End();
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

        static void SubmitFieldPropertyInspector(FieldPropertyInfo info, object entityComponent, bool showMidi = true)
        {
            ImGui.PushID(GetIdString(info, entityComponent));

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
                if (ImGui.DragFloat(info.Name, ref val))
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
                if (ImGui.InputInt(info.Name, ref val))
                {
                    info.SetValue(entityComponent, val);
                }
            }
            else if (infoType == typeof(uint))
            {
                if (showMidi) SubmitMidiAssignment(entityComponent, info, MidiState.MidiControlDescriptionType.Knob);

                int val = (int)((uint)info.GetValue(entityComponent));
                if (ImGui.InputInt(info.Name, ref val))
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
            ImGui.Text(string.Format("{0}: {1}", info.Name, valText));
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
            
            foreach (var controlStatePair in Program.MidiState.ControlStates)
            {
                var controlState = controlStatePair.Value;
                if (controlState.ControlledObject != null)
                {
                    var description = Program.MidiState.AssignToControlMapping[controlStatePair.Key];
                    string objectName = controlState.ControlledObject.ToString();
                    if (controlState.ControlledObject as Component != null)
                    {
                        objectName = string.Format("{0}: {1}", (controlState.ControlledObject as Component).EntityName, objectName);
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

                        vectorField = string.Format(" ({0})", vectorField);
                    }
                    if (ImGui.Button(string.Format("{0}: {1}{2}", Program.MidiState.GetControlName(description.Id, description.Type), objectName, vectorField)))
                    {
                        selectedEntityComponent = controlState.ControlledObject;
                        scrollEntitiesView = true;
                        scrollSceneGraphView = true;
                    }
                    SubmitFieldPropertyInspector(controlState.FieldPropertyInfo, controlState.ControlledObject, false);
                    ImGui.NewLine();
                }
            }
            ImGui.End();
        }
    }
}
