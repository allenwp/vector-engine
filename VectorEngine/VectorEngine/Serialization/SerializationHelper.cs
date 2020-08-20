using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Serialization
{
    public class SerializationHelper
    {
        private class SerializationComponentCallback
        {
            public List<Component> Components;
            public void Callback(Component comp)
            {
                Components.Add(comp);
            }
        }

        public static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            // Maintains object references, obviously
            PreserveReferencesHandling = PreserveReferencesHandling.All,

            // Handles certain cases of circular references
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,

            // Allows inheritance to be correctly deserialized to original subclasses
            TypeNameHandling = TypeNameHandling.All,

            // Not needed with [JsonObject(MemberSerialization.Fields)], but convenient for avoiding public constructors by having a private parameterless constructor
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };

        public static string Serialize(object obj, List<Component> serializedComponents = null, bool log = false)
        {
            SerializationComponentCallback callback = new SerializationComponentCallback();
            if (serializedComponents != null)
            {
                callback.Components = serializedComponents;
                ObjectGraphHelper.OnSerializedComponent += callback.Callback;
            }

            if (log)
            {
                JsonSettings.TraceWriter = new MemoryTraceWriter() { LevelFilter = System.Diagnostics.TraceLevel.Warning };
            }
            var result = JsonConvert.SerializeObject(obj, Formatting.Indented, JsonSettings);
            if (log)
            {
                Console.WriteLine(JsonSettings.TraceWriter);
            }

            if (serializedComponents != null)
            {
                ObjectGraphHelper.OnSerializedComponent -= callback.Callback;
            }

            return result;
        }

        public static T Deserialize<T>(string json, List<Component> serializedComponents = null, bool log = false)
        {
            SerializationComponentCallback callback = new SerializationComponentCallback();
            if (serializedComponents != null)
            {
                callback.Components = serializedComponents;
                ObjectGraphHelper.OnDeserializedComponent += callback.Callback;
            }

            if (log)
            {
                JsonSettings.TraceWriter = new MemoryTraceWriter() { LevelFilter = System.Diagnostics.TraceLevel.Warning };
            }
            var result = JsonConvert.DeserializeObject<T>(json, JsonSettings);
            if (log)
            {
                Console.WriteLine(JsonSettings.TraceWriter);
            }
            
            if (serializedComponents != null)
            {
                ObjectGraphHelper.OnDeserializedComponent -= callback.Callback;
            }

            return result;
        }

    }
}
