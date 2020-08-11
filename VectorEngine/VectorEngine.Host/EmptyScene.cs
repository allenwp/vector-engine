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

            // TOOD: Fill in default MIDI assignemnts

            return result;
        }
        
        /// <summary>
        ///  
        /// </summary>
        private static List<Component> CreateDefaultSingletons()
        {
            List<Component> result = new List<Component>();
            result.Add(CreateSingleton<GameTimeSingleton>("GameTime Singleton"));
            result.Add(CreateSingleton<GamepadSingleton>("Gamepad Singleton"));
            result.Add(CreateSingleton<SamplerSingleton>("Sampler Singleton"));
            result.AddRange(CreateSceneViewCamera());
            return result;
        }

        private static T CreateSingleton<T>(string name) where T : Component, new()
        {
            var entity = new Entity(name);
            return AddComponent<T>(entity);
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
