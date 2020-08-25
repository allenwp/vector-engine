using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Extras;

namespace VectorEngine.Host
{
    public class EmptyScene
    {
        public static readonly string EDITOR_CAM_ENTITY_NAME = "Editor Camera";

        public static Scene GetEmptyScene()
        {
            Scene result = new Scene();

            result.Components.AddRange(CreateDefaultSingletons());

            var gameTime = result.Components.Find(comp => comp.GetType() == typeof(GameTimeSingleton));
            result.EditorState.MidiAssignments.Add(new EditorHelper.MidiAssignments(16, gameTime, "Paused"));
            var camera = result.Components.Find(comp => comp.EntityName == EDITOR_CAM_ENTITY_NAME);
            result.EditorState.MidiAssignments.Add(new EditorHelper.MidiAssignments(17, camera, "SelfEnabled"));

            return result;
        }
        
        /// <summary>
        ///  
        /// </summary>
        private static List<Component> CreateDefaultSingletons()
        {
            List<Component> result = new List<Component>();
            result.AddRange(CreateSingleton<GameTimeSingleton>("GameTime Singleton"));
            result.AddRange(CreateSingleton<GamepadSingleton>("Gamepad Singleton"));
            result.AddRange(CreateSingleton<SamplerSingleton>("Sampler Singleton"));
            result.AddRange(CreateSceneViewCamera());
            return result;
        }

        private static List<Component> CreateSingleton<T>(string name) where T : Component, new()
        {
            var result = new List<Component>();
            var entity = new Entity(name);
            result.Add(AddComponent<T>(entity));
            result.Add(AddComponent<DontDestroyOnClear>(entity));
            return result;
        }

        public static List<Component> CreateSceneViewCamera()
        {
            var result = new List<Component>();

            var entity = new Entity(EDITOR_CAM_ENTITY_NAME);
            entity.SelfEnabled = false;

            result.Add(AddComponent<Transform>(entity));

            var camera = AddComponent<Camera>(entity);
            camera.Priority = uint.MaxValue;
            result.Add(camera);

            var movement = AddComponent<GamepadBasicFPSMovement>(entity);
            movement.UseRealTime = true;
            movement.TranslateSpeed = 100f;
            result.Add(movement);

            result.Add(AddComponent<DontDestroyOnClear>(entity));

            return result;
        }

        /// <summary>
        /// Similiar to EntityAdmin.AddComponent except it doesn't check for required systems or add the
        /// component to any collections.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        private static T AddComponent<T>(Entity entity) where T : Component, new()
        {
            var newComponent = new T();
            newComponent.Entity = entity;
            entity.Components.Add(newComponent);
            return newComponent;
        }
    }        
}
