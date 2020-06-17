using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Host.Util
{
    public class ImGuiUtil
    {
        public static unsafe void BeginDisable()
        {
            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.5f);
        }
        public static unsafe void EndDisable()
        {
            ImGui.PopStyleVar();
        }
    }
}
