﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine
{
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class Entity
    {
        public string Name { get; set; }

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

        public bool SelfEnabled { get; set; } = true;
        public bool IsActive
        {
            get
            {
                if (!SelfEnabled)
                {
                    return false;
                }
                else
                {
                    var transform = GetComponent<Transform>(true);
                    if (transform != null && transform.Parent != null)
                    {
                        return transform.Parent.Entity.IsActive;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }

        /// <summary>
        /// Don't modify this directly, use Entity static util methods instead.
        /// </summary>
        public List<Component> Components { get; private set; }

        /// <summary>
        /// Do not use. Create through EntityAdmin instead!
        /// </summary>
        /// <param name="name"></param>
        public Entity(string name)
        {
            Name = name;
            Guid = Guid.NewGuid();
            Components = new List<Component>();
        }

        [OnDeserialized]
        private void InitGuid(StreamingContext context)
        {
            Guid = Guid.NewGuid();
        }

        public bool HasComponent<T>(bool includeInactive = false) where T : Component
        {
            return Components.Where(comp => comp is T && (includeInactive ? true : comp.IsActive)).Count() > 0;
        }

        public T GetComponent<T> (bool includeInactive = false) where T : Component
        {
            return Components.Where(comp => comp is T && (includeInactive ? true : comp.IsActive)).FirstOrDefault() as T;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
