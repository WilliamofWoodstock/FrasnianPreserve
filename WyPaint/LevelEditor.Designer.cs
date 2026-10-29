namespace WyPaint
{
    partial class LevelEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LevelEditor));
            palette = new GroupBox();
            magenta = new Button();
            yellow = new Button();
            darkBlue = new Button();
            teal = new Button();
            black = new Button();
            silver = new Button();
            brown = new Button();
            green = new Button();
            skyBlue = new Button();
            crimson = new Button();
            currBox = new GroupBox();
            currentTile = new PictureBox();
            map = new GroupBox();
            tileGrid = new FlowLayoutPanel();
            saveButton = new Button();
            loadButton = new Button();
            palette.SuspendLayout();
            currBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)currentTile).BeginInit();
            map.SuspendLayout();
            SuspendLayout();
            // 
            // palette
            // 
            palette.Controls.Add(magenta);
            palette.Controls.Add(yellow);
            palette.Controls.Add(darkBlue);
            palette.Controls.Add(teal);
            palette.Controls.Add(black);
            palette.Controls.Add(silver);
            palette.Controls.Add(brown);
            palette.Controls.Add(green);
            palette.Controls.Add(skyBlue);
            palette.Controls.Add(crimson);
            palette.Location = new Point(12, 12);
            palette.Name = "palette";
            palette.Size = new Size(174, 398);
            palette.TabIndex = 0;
            palette.TabStop = false;
            palette.Text = "Tile Selector";
            // 
            // magenta
            // 
            magenta.BackColor = Color.DarkMagenta;
            magenta.Location = new Point(87, 322);
            magenta.Name = "magenta";
            magenta.Size = new Size(81, 69);
            magenta.TabIndex = 8;
            magenta.UseVisualStyleBackColor = false;
            magenta.Click += ChangeColor;
            // 
            // yellow
            // 
            yellow.BackColor = Color.Yellow;
            yellow.Location = new Point(6, 322);
            yellow.Name = "yellow";
            yellow.Size = new Size(81, 69);
            yellow.TabIndex = 7;
            yellow.UseVisualStyleBackColor = false;
            yellow.Click += ChangeColor;
            // 
            // darkBlue
            // 
            darkBlue.BackColor = Color.DarkBlue;
            darkBlue.Location = new Point(87, 247);
            darkBlue.Name = "darkBlue";
            darkBlue.Size = new Size(81, 69);
            darkBlue.TabIndex = 6;
            darkBlue.UseVisualStyleBackColor = false;
            darkBlue.Click += ChangeColor;
            // 
            // teal
            // 
            teal.BackColor = Color.Teal;
            teal.Location = new Point(6, 247);
            teal.Name = "teal";
            teal.Size = new Size(81, 69);
            teal.TabIndex = 1;
            teal.UseVisualStyleBackColor = false;
            teal.Click += ChangeColor;
            // 
            // black
            // 
            black.BackColor = Color.Black;
            black.Location = new Point(87, 172);
            black.Name = "black";
            black.Size = new Size(81, 69);
            black.TabIndex = 5;
            black.Text = "button6";
            black.UseVisualStyleBackColor = false;
            black.Click += ChangeColor;
            // 
            // silver
            // 
            silver.BackColor = Color.Silver;
            silver.Location = new Point(6, 172);
            silver.Name = "silver";
            silver.Size = new Size(81, 69);
            silver.TabIndex = 4;
            silver.UseVisualStyleBackColor = false;
            silver.Click += ChangeColor;
            // 
            // brown
            // 
            brown.BackColor = Color.Sienna;
            brown.Location = new Point(87, 97);
            brown.Name = "brown";
            brown.Size = new Size(81, 69);
            brown.TabIndex = 3;
            brown.UseVisualStyleBackColor = false;
            brown.Click += ChangeColor;
            // 
            // green
            // 
            green.BackColor = Color.ForestGreen;
            green.Location = new Point(6, 97);
            green.Name = "green";
            green.Size = new Size(81, 69);
            green.TabIndex = 2;
            green.UseVisualStyleBackColor = false;
            green.Click += ChangeColor;
            // 
            // skyBlue
            // 
            skyBlue.BackColor = Color.SkyBlue;
            skyBlue.Location = new Point(87, 22);
            skyBlue.Name = "skyBlue";
            skyBlue.Size = new Size(81, 69);
            skyBlue.TabIndex = 1;
            skyBlue.UseVisualStyleBackColor = false;
            skyBlue.Click += ChangeColor;
            // 
            // crimson
            // 
            crimson.BackColor = Color.Crimson;
            crimson.Location = new Point(6, 22);
            crimson.Name = "crimson";
            crimson.Size = new Size(81, 69);
            crimson.TabIndex = 0;
            crimson.UseVisualStyleBackColor = false;
            crimson.Click += ChangeColor;
            // 
            // currBox
            // 
            currBox.Controls.Add(currentTile);
            currBox.Location = new Point(12, 409);
            currBox.Name = "currBox";
            currBox.Size = new Size(174, 154);
            currBox.TabIndex = 1;
            currBox.TabStop = false;
            currBox.Text = "Current Tile";
            // 
            // currentTile
            // 
            currentTile.Location = new Point(40, 39);
            currentTile.Name = "currentTile";
            currentTile.Size = new Size(93, 82);
            currentTile.TabIndex = 0;
            currentTile.TabStop = false;
            // 
            // map
            // 
            map.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            map.Controls.Add(tileGrid);
            map.Location = new Point(193, 12);
            map.Name = "map";
            map.Size = new Size(815, 831);
            map.TabIndex = 2;
            map.TabStop = false;
            map.Text = "Map";
            // 
            // tileGrid
            // 
            tileGrid.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tileGrid.Location = new Point(6, 22);
            tileGrid.Margin = new Padding(0);
            tileGrid.Name = "tileGrid";
            tileGrid.Size = new Size(800, 800);
            tileGrid.TabIndex = 0;
            // 
            // saveButton
            // 
            saveButton.BackColor = SystemColors.ButtonShadow;
            saveButton.Location = new Point(37, 569);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(120, 83);
            saveButton.TabIndex = 3;
            saveButton.Text = "Save File";
            saveButton.UseVisualStyleBackColor = false;
            saveButton.Click += SaveMap;
            // 
            // loadButton
            // 
            loadButton.BackColor = SystemColors.ButtonShadow;
            loadButton.Location = new Point(37, 673);
            loadButton.Name = "loadButton";
            loadButton.Size = new Size(120, 83);
            loadButton.TabIndex = 4;
            loadButton.Text = "Load File";
            loadButton.UseVisualStyleBackColor = false;
            loadButton.Click += LoadMap;
            // 
            // LevelEditor
            // 
            AllowDrop = true;
            AutoScaleMode = AutoScaleMode.None;
            AutoScroll = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(1020, 843);
            Controls.Add(loadButton);
            Controls.Add(saveButton);
            Controls.Add(map);
            Controls.Add(currBox);
            Controls.Add(palette);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "LevelEditor";
            Text = "WyPaint Editor";
            FormClosing += ExitTicket;
            palette.ResumeLayout(false);
            currBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)currentTile).EndInit();
            map.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox palette;
        private GroupBox currBox;
        private PictureBox currentTile;
        private GroupBox map;
        private Button saveButton;
        private Button black;
        private Button silver;
        private Button brown;
        private Button green;
        private Button skyBlue;
        private Button crimson;
        private Button darkBlue;
        private Button teal;
        private Button magenta;
        private Button yellow;
        private Button loadButton;
        private FlowLayoutPanel tileGrid;
    }
}