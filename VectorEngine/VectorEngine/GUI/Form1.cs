using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VectorEngine.Engine;

namespace VectorEngineGUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            StringBuilder sb = new StringBuilder();
            foreach (var system in EntityAdmin.Instance.Systems)
            {
                var type = system.GetType();
                sb.Append(type.Name);
                sb.Append(Environment.NewLine);
            }
            systemsTextBox.Text = sb.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
