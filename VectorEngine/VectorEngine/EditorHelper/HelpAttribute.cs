using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.EditorHelper
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class HelpAttribute : Attribute
    {
        public string HelpText;

        public HelpAttribute(string helpText)
        {
            HelpText = helpText;
        }
    }
}
