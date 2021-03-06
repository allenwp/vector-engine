﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine
{
    /// <summary>
    /// Stores game state and has no behaviours.
    /// (Said differnt: Only fields, no methods.)
    /// </summary>
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class Component
    {
        public string Name { get => GetType().Name; }

        [JsonIgnore]
        private Guid guid;
        [EditorHelper.Help("NON SERIALIZED. Used by the editor to help layout trees, etc.")]
        public Guid Guid
        {
            get
            {
                return guid;
            }
            private set
            {
                guid = value;
            }
        }

        public string EntityName { get => Entity.Name; }

        public bool SelfEnabled { get; set; } = true;
        public bool IsActive { get { return SelfEnabled && Entity.IsActive; } }

        /// <summary>
        /// Don't modify this directly. It will be handled by the static Entity util methods
        /// </summary>
        public Entity Entity;

        public override string ToString()
        {
            return $"{Name} ({EntityName})";
        }

        public Component()
        {
            Guid = Guid.NewGuid();
        }

        [OnSerialized]
        public void Serialized(StreamingContext context)
        {
            Serialization.ObjectGraphHelper.OnSerializedComponent?.Invoke(this);
        }

        [OnDeserialized]
        public void Deserialized(StreamingContext context)
        {
            Guid = Guid.NewGuid();
            Serialization.ObjectGraphHelper.OnDeserializedComponent?.Invoke(this);
        }
    }
}
