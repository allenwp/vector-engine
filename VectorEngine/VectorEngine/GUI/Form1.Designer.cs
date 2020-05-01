namespace VectorEngineGUI
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.systemsGroupBox = new System.Windows.Forms.GroupBox();
            this.systemsTextBox = new System.Windows.Forms.TextBox();
            this.sceneGraphGroupBox = new System.Windows.Forms.GroupBox();
            this.sceneGraphTreeView = new System.Windows.Forms.TreeView();
            this.entitiesGroupBox = new System.Windows.Forms.GroupBox();
            this.entitesTreeView = new System.Windows.Forms.TreeView();
            this.componentGroupBox = new System.Windows.Forms.GroupBox();
            this.midiKnobGroupBox = new System.Windows.Forms.GroupBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.systemsGroupBox.SuspendLayout();
            this.sceneGraphGroupBox.SuspendLayout();
            this.entitiesGroupBox.SuspendLayout();
            this.midiKnobGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.tableLayoutPanel1.Controls.Add(this.systemsGroupBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.sceneGraphGroupBox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.entitiesGroupBox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.componentGroupBox, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.midiKnobGroupBox, 2, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(916, 496);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // systemsGroupBox
            // 
            this.systemsGroupBox.Controls.Add(this.systemsTextBox);
            this.systemsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.systemsGroupBox.Location = new System.Drawing.Point(3, 3);
            this.systemsGroupBox.Name = "systemsGroupBox";
            this.systemsGroupBox.Size = new System.Drawing.Size(296, 132);
            this.systemsGroupBox.TabIndex = 0;
            this.systemsGroupBox.TabStop = false;
            this.systemsGroupBox.Text = "Systems Order";
            // 
            // systemsTextBox
            // 
            this.systemsTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.systemsTextBox.Location = new System.Drawing.Point(3, 18);
            this.systemsTextBox.Multiline = true;
            this.systemsTextBox.Name = "systemsTextBox";
            this.systemsTextBox.ReadOnly = true;
            this.systemsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.systemsTextBox.Size = new System.Drawing.Size(290, 111);
            this.systemsTextBox.TabIndex = 0;
            // 
            // sceneGraphGroupBox
            // 
            this.sceneGraphGroupBox.Controls.Add(this.sceneGraphTreeView);
            this.sceneGraphGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sceneGraphGroupBox.Location = new System.Drawing.Point(3, 141);
            this.sceneGraphGroupBox.Name = "sceneGraphGroupBox";
            this.tableLayoutPanel1.SetRowSpan(this.sceneGraphGroupBox, 2);
            this.sceneGraphGroupBox.Size = new System.Drawing.Size(296, 352);
            this.sceneGraphGroupBox.TabIndex = 1;
            this.sceneGraphGroupBox.TabStop = false;
            this.sceneGraphGroupBox.Text = "Scene Graph";
            // 
            // sceneGraphTreeView
            // 
            this.sceneGraphTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sceneGraphTreeView.HotTracking = true;
            this.sceneGraphTreeView.Location = new System.Drawing.Point(3, 18);
            this.sceneGraphTreeView.Name = "sceneGraphTreeView";
            this.sceneGraphTreeView.Size = new System.Drawing.Size(290, 331);
            this.sceneGraphTreeView.TabIndex = 0;
            this.sceneGraphTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.sceneGraphTreeView_AfterSelect);
            // 
            // entitiesGroupBox
            // 
            this.entitiesGroupBox.Controls.Add(this.entitesTreeView);
            this.entitiesGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entitiesGroupBox.Location = new System.Drawing.Point(305, 3);
            this.entitiesGroupBox.Name = "entitiesGroupBox";
            this.tableLayoutPanel1.SetRowSpan(this.entitiesGroupBox, 3);
            this.entitiesGroupBox.Size = new System.Drawing.Size(296, 490);
            this.entitiesGroupBox.TabIndex = 2;
            this.entitiesGroupBox.TabStop = false;
            this.entitiesGroupBox.Text = "Entities and Components";
            // 
            // entitesTreeView
            // 
            this.entitesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entitesTreeView.HideSelection = false;
            this.entitesTreeView.Location = new System.Drawing.Point(3, 18);
            this.entitesTreeView.Name = "entitesTreeView";
            this.entitesTreeView.Size = new System.Drawing.Size(290, 469);
            this.entitesTreeView.TabIndex = 0;
            // 
            // componentGroupBox
            // 
            this.componentGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.componentGroupBox.Location = new System.Drawing.Point(607, 3);
            this.componentGroupBox.Name = "componentGroupBox";
            this.tableLayoutPanel1.SetRowSpan(this.componentGroupBox, 2);
            this.componentGroupBox.Size = new System.Drawing.Size(306, 389);
            this.componentGroupBox.TabIndex = 3;
            this.componentGroupBox.TabStop = false;
            this.componentGroupBox.Text = "Selected Component";
            // 
            // midiKnobGroupBox
            // 
            this.midiKnobGroupBox.Controls.Add(this.textBox4);
            this.midiKnobGroupBox.Controls.Add(this.textBox3);
            this.midiKnobGroupBox.Controls.Add(this.textBox2);
            this.midiKnobGroupBox.Controls.Add(this.textBox1);
            this.midiKnobGroupBox.Controls.Add(this.button4);
            this.midiKnobGroupBox.Controls.Add(this.button3);
            this.midiKnobGroupBox.Controls.Add(this.button2);
            this.midiKnobGroupBox.Controls.Add(this.button1);
            this.midiKnobGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.midiKnobGroupBox.Location = new System.Drawing.Point(607, 398);
            this.midiKnobGroupBox.Name = "midiKnobGroupBox";
            this.midiKnobGroupBox.Size = new System.Drawing.Size(306, 95);
            this.midiKnobGroupBox.TabIndex = 4;
            this.midiKnobGroupBox.TabStop = false;
            this.midiKnobGroupBox.Text = "MIDI Knobs";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(222, 59);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(66, 22);
            this.textBox4.TabIndex = 7;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(150, 59);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(66, 22);
            this.textBox3.TabIndex = 6;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(78, 59);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(66, 22);
            this.textBox2.TabIndex = 5;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(6, 59);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(66, 22);
            this.textBox1.TabIndex = 4;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(222, 21);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(66, 32);
            this.button4.TabIndex = 3;
            this.button4.Text = "8";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(151, 21);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(65, 32);
            this.button3.TabIndex = 2;
            this.button3.Text = "6";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(78, 21);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(66, 32);
            this.button2.TabIndex = 1;
            this.button2.Text = "4";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 21);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(66, 32);
            this.button1.TabIndex = 0;
            this.button1.Text = "2";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(916, 496);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.systemsGroupBox.ResumeLayout(false);
            this.systemsGroupBox.PerformLayout();
            this.sceneGraphGroupBox.ResumeLayout(false);
            this.entitiesGroupBox.ResumeLayout(false);
            this.midiKnobGroupBox.ResumeLayout(false);
            this.midiKnobGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox systemsGroupBox;
        private System.Windows.Forms.GroupBox sceneGraphGroupBox;
        private System.Windows.Forms.GroupBox entitiesGroupBox;
        private System.Windows.Forms.GroupBox componentGroupBox;
        private System.Windows.Forms.GroupBox midiKnobGroupBox;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TreeView sceneGraphTreeView;
        private System.Windows.Forms.TextBox systemsTextBox;
        private System.Windows.Forms.TreeView entitesTreeView;
    }
}

