namespace FxEngineEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            dfsdfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            sdfsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            zipAssetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            recentLibsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            binaryAssetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            archivedAssetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            toolStripButton6 = new System.Windows.Forms.ToolStripButton();
            toolStripButton7 = new System.Windows.Forms.ToolStripButton();
            toolStripButton8 = new System.Windows.Forms.ToolStripButton();
            menuStrip1.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { dfsdfToolStripMenuItem });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            menuStrip1.Size = new System.Drawing.Size(1381, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // dfsdfToolStripMenuItem
            // 
            dfsdfToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { sdfsToolStripMenuItem, saveToolStripMenuItem, saveAsToolStripMenuItem, loadToolStripMenuItem, recentLibsToolStripMenuItem, exportToolStripMenuItem });
            dfsdfToolStripMenuItem.Name = "dfsdfToolStripMenuItem";
            dfsdfToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            dfsdfToolStripMenuItem.Text = "Library";
            dfsdfToolStripMenuItem.Click += dfsdfToolStripMenuItem_Click;
            // 
            // sdfsToolStripMenuItem
            // 
            sdfsToolStripMenuItem.Image = FxStudio.Properties.Resources.document;
            sdfsToolStripMenuItem.Name = "sdfsToolStripMenuItem";
            sdfsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            sdfsToolStripMenuItem.Text = "New";
            sdfsToolStripMenuItem.Click += sdfsToolStripMenuItem_Click;
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Image = FxStudio.Properties.Resources.disk;
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            saveToolStripMenuItem.Text = "Save";
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { zipAssetToolStripMenuItem });
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            saveAsToolStripMenuItem.Text = "Save as";
            saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
            // 
            // zipAssetToolStripMenuItem
            // 
            zipAssetToolStripMenuItem.Name = "zipAssetToolStripMenuItem";
            zipAssetToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            zipAssetToolStripMenuItem.Text = "zip asset";
            zipAssetToolStripMenuItem.Click += zipAssetToolStripMenuItem_Click;
            // 
            // loadToolStripMenuItem
            // 
            loadToolStripMenuItem.Image = FxStudio.Properties.Resources.folder_open;
            loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            loadToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            loadToolStripMenuItem.Text = "Load";
            loadToolStripMenuItem.Click += loadToolStripMenuItem_Click;
            // 
            // recentLibsToolStripMenuItem
            // 
            recentLibsToolStripMenuItem.Name = "recentLibsToolStripMenuItem";
            recentLibsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            recentLibsToolStripMenuItem.Text = "Recent libs";
            recentLibsToolStripMenuItem.Click += recentLibsToolStripMenuItem_Click;
            // 
            // exportToolStripMenuItem
            // 
            exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { binaryAssetToolStripMenuItem, archivedAssetToolStripMenuItem });
            exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            exportToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            exportToolStripMenuItem.Text = "Export";
            // 
            // binaryAssetToolStripMenuItem
            // 
            binaryAssetToolStripMenuItem.Name = "binaryAssetToolStripMenuItem";
            binaryAssetToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            binaryAssetToolStripMenuItem.Text = "binary asset";
            binaryAssetToolStripMenuItem.Click += binaryAssetToolStripMenuItem_Click;
            // 
            // archivedAssetToolStripMenuItem
            // 
            archivedAssetToolStripMenuItem.Name = "archivedAssetToolStripMenuItem";
            archivedAssetToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            archivedAssetToolStripMenuItem.Text = "archived asset";
            archivedAssetToolStripMenuItem.Click += archivedAssetToolStripMenuItem_Click;
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripButton1, toolStripButton2, toolStripButton3, toolStripButton5, toolStripButton4, toolStripButton6, toolStripButton7, toolStripButton8 });
            toolStrip1.Location = new System.Drawing.Point(0, 24);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(1381, 25);
            toolStrip1.TabIndex = 2;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripButton1.Image = (System.Drawing.Image)resources.GetObject("toolStripButton1.Image");
            toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButton1.Name = "toolStripButton1";
            toolStripButton1.Size = new System.Drawing.Size(79, 22);
            toolStripButton1.Text = "Prefab editor";
            toolStripButton1.Click += toolStripButton1_Click;
            toolStripButton1.MouseEnter += toolStripButton1_MouseEnter;
            // 
            // toolStripButton2
            // 
            toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripButton2.Image = (System.Drawing.Image)resources.GetObject("toolStripButton2.Image");
            toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButton2.Name = "toolStripButton2";
            toolStripButton2.Size = new System.Drawing.Size(63, 22);
            toolStripButton2.Text = "Tile editor";
            toolStripButton2.Click += toolStripButton2_Click;
            toolStripButton2.MouseEnter += toolStripButton2_MouseEnter;
            toolStripButton2.MouseHover += toolStripButton2_MouseHover;
            // 
            // toolStripButton3
            // 
            toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripButton3.Image = (System.Drawing.Image)resources.GetObject("toolStripButton3.Image");
            toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButton3.Name = "toolStripButton3";
            toolStripButton3.Size = new System.Drawing.Size(72, 22);
            toolStripButton3.Text = "Level editor";
            toolStripButton3.Click += toolStripButton3_Click;
            toolStripButton3.MouseEnter += toolStripButton3_MouseEnter;
            toolStripButton3.MouseHover += toolStripButton3_MouseHover;
            // 
            // toolStripButton5
            // 
            toolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripButton5.Image = (System.Drawing.Image)resources.GetObject("toolStripButton5.Image");
            toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButton5.Name = "toolStripButton5";
            toolStripButton5.Size = new System.Drawing.Size(63, 22);
            toolStripButton5.Text = "Gui Editor";
            toolStripButton5.Click += toolStripButton5_Click;
            // 
            // toolStripButton4
            // 
            toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripButton4.Image = (System.Drawing.Image)resources.GetObject("toolStripButton4.Image");
            toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButton4.Name = "toolStripButton4";
            toolStripButton4.Size = new System.Drawing.Size(91, 22);
            toolStripButton4.Text = "Library settings";
            toolStripButton4.Click += toolStripButton4_Click;
            // 
            // toolStripButton6
            // 
            toolStripButton6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripButton6.Image = (System.Drawing.Image)resources.GetObject("toolStripButton6.Image");
            toolStripButton6.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButton6.Name = "toolStripButton6";
            toolStripButton6.Size = new System.Drawing.Size(83, 22);
            toolStripButton6.Text = "Texture editor";
            toolStripButton6.Click += toolStripButton6_Click;
            // 
            // toolStripButton7
            // 
            toolStripButton7.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripButton7.Image = (System.Drawing.Image)resources.GetObject("toolStripButton7.Image");
            toolStripButton7.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButton7.Name = "toolStripButton7";
            toolStripButton7.Size = new System.Drawing.Size(81, 22);
            toolStripButton7.Text = "Tile set editor";
            toolStripButton7.Click += toolStripButton7_Click;
            // 
            // toolStripButton8
            // 
            toolStripButton8.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripButton8.Image = (System.Drawing.Image)resources.GetObject("toolStripButton8.Image");
            toolStripButton8.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButton8.Name = "toolStripButton8";
            toolStripButton8.Size = new System.Drawing.Size(92, 22);
            toolStripButton8.Text = "Asset navigator";
            toolStripButton8.Click += toolStripButton8_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1381, 878);
            Controls.Add(toolStrip1);
            Controls.Add(menuStrip1);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            IsMdiContainer = true;
            MainMenuStrip = menuStrip1;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "Form1";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "FxStudio Assets Editor";
            FormClosing += Form1_FormClosing;
            GiveFeedback += Form1_GiveFeedback;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem dfsdfToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sdfsToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ToolStripButton toolStripButton5;
        private System.Windows.Forms.ToolStripMenuItem recentLibsToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButton6;
        private System.Windows.Forms.ToolStripButton toolStripButton7;
        private System.Windows.Forms.ToolStripButton toolStripButton8;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem binaryAssetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem archivedAssetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zipAssetToolStripMenuItem;
    }
}

