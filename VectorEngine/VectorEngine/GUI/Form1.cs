﻿using System;
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

            var entityNodes = new List<TreeNode>();
            foreach (var entity in EntityAdmin.Instance.Entities)
            {
                var components = entity.Components;
                var componentsTreeNodes = new TreeNode[components.Count];
                for (int i = 0; i < components.Count; i++)
                {
                    componentsTreeNodes[i] = new TreeNode(components[i].GetType().Name);
                    componentsTreeNodes[i].Tag = components[i];
                }
                entityNodes.Add(new TreeNode(entity.Name, componentsTreeNodes) { Tag = entity });
            }
            entitesTreeView.Nodes.AddRange(entityNodes.ToArray());

            // Get all the transforms without parents. These are our root nodes.
            var list = EntityAdmin.Instance.GetComponents<Transform>(true).Where(trans => trans.Parent == null).ToList();
            var sceneGraphNodes = GetSceneGraphNodes(list);
            sceneGraphTreeView.Nodes.AddRange(sceneGraphNodes);
        }

        private TreeNode[] GetSceneGraphNodes(IEnumerable<Transform> transforms)
        {
            var nodes = new TreeNode[transforms.Count()];
            int i = 0;
            foreach (var transform in transforms)
            {
                if (transform.Children.Count > 0)
                {
                    var childNodes = GetSceneGraphNodes(transform.Children);
                    nodes[i] = new TreeNode(transform.Entity.Name, childNodes) { Tag = transform };
                }
                else
                {
                    nodes[i] = new TreeNode(transform.Entity.Name) { Tag = transform };
                }
                i++;
            }
            return nodes;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
