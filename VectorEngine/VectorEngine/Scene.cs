using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.EditorHelper;

namespace VectorEngine
{
    public class Scene
    {
        public static readonly string MAIN_SCENE_FILENAME = "scene.ves";

        public List<Component> Components = new List<Component>();

        public EditorState EditorState = new EditorState();
    }
}
