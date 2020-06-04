using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Extras.Util
{
    public class EditorUtil
    {
        public static Entity CreateSceneViewCamera()
        {
            var entity = EntityAdmin.Instance.CreateEntity("Editor Camera");
            EntityAdmin.Instance.AddComponent<Transform>(entity);
            var camera =EntityAdmin.Instance.AddComponent<Camera>(entity);
            camera.Priority = uint.MaxValue;
            EntityAdmin.Instance.AddComponent<GamepadBasicFPSMovement>(entity);
            entity.SelfEnabled = false;
            return entity;
        }
    }
}
