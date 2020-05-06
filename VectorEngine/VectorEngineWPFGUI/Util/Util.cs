using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VectorEngineWPFGUI.Util
{
    public class Util
    {
        /// <summary>
        /// Taken from https://stackoverflow.com/a/32628562/1123295
        /// </summary>
        public static TreeViewItem FindTviFromObjectRecursive(ItemsControl ic, object o)
        {
            //Search for the object model in first level children (recursively)
            TreeViewItem tvi = ic.ItemContainerGenerator.ContainerFromItem(o) as TreeViewItem;
            if (tvi != null)
            {
                return tvi;
            }
            //Loop through user object models
            foreach (object i in ic.Items)
            {
                //Get the TreeViewItem associated with the iterated object model
                TreeViewItem tvi2 = ic.ItemContainerGenerator.ContainerFromItem(i) as TreeViewItem;
                if (tvi2 != null)
                {
                    tvi = FindTviFromObjectRecursive(tvi2, o);
                    if (tvi != null)
                    {
                        return tvi;
                    }
                }
            }
            return null;
        }
    }
}
