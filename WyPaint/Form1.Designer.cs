namespace WyPaint
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            groupBox1 = new GroupBox();
            heightBox = new TextBox();
            widthBox = new TextBox();
            label1 = new Label();
            widthLabel = new Label();
            createMap = new Button();
            loadMap = new Button();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(heightBox);
            groupBox1.Controls.Add(widthBox);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(widthLabel);
            groupBox1.Controls.Add(createMap);
            groupBox1.Location = new Point(12, 127);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(410, 263);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Create New Map";
            // 
            // heightBox
            // 
            heightBox.Location = new Point(227, 104);
            heightBox.Name = "heightBox";
            heightBox.Size = new Size(122, 23);
            heightBox.TabIndex = 4;
            heightBox.Text = "10";
            // 
            // widthBox
            // 
            widthBox.Location = new Point(227, 56);
            widthBox.Name = "widthBox";
            widthBox.Size = new Size(122, 23);
            widthBox.TabIndex = 3;
            widthBox.Text = "10";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(42, 107);
            label1.Name = "label1";
            label1.Size = new Size(88, 15);
            label1.TabIndex = 2;
            label1.Text = "Height (in tiles)";
            // 
            // widthLabel
            // 
            widthLabel.AutoSize = true;
            widthLabel.Location = new Point(42, 59);
            widthLabel.Name = "widthLabel";
            widthLabel.Size = new Size(84, 15);
            widthLabel.TabIndex = 1;
            widthLabel.Text = "Width (in tiles)";
            // 
            // createMap
            // 
            createMap.BackColor = SystemColors.ScrollBar;
            createMap.Location = new Point(23, 149);
            createMap.Name = "createMap";
            createMap.Size = new Size(367, 99);
            createMap.TabIndex = 0;
            createMap.Text = "Create Map";
            createMap.UseVisualStyleBackColor = false;
            createMap.Click += MapCreation;
            // 
            // loadMap
            // 
            loadMap.BackColor = SystemColors.ScrollBar;
            loadMap.ForeColor = SystemColors.ControlText;
            loadMap.Location = new Point(12, 12);
            loadMap.Name = "loadMap";
            loadMap.Size = new Size(410, 109);
            loadMap.TabIndex = 1;
            loadMap.Text = "Load Map";
            loadMap.UseVisualStyleBackColor = false;
            loadMap.Click += GetMap;
            // 
            // Form1
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(434, 399);
            Controls.Add(loadMap);
            Controls.Add(groupBox1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "WyPaint";
            FormClosing += CloseCheck;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private Button loadMap;
        private Button createMap;
        private Label label1;
        private Label widthLabel;
        private TextBox heightBox;
        private TextBox widthBox;
    }
}
