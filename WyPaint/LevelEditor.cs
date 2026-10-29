using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WyPaint
{
    public partial class LevelEditor : Form
    {
        //tracks whether the user has changes they haven't saved
        private bool unsaved = false;
        private Color currentColor = SystemColors.Control;
        private PictureBox[] mapGrid;
        int mapWidth; int mapHeight;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapWidth"></param>
        /// <param name="mapHeight"></param>
        public LevelEditor(int mapWidth, int mapHeight)
        {
            InitializeComponent();
            this.mapWidth = mapWidth; this.mapHeight = mapHeight;
            int tileCount = mapWidth * mapHeight;
            /*Obtains the ratio between width and heigth and 
             adjusts the screen accordingly*/
            double factor = (double)mapWidth / mapHeight;
            int targetWidth = (int)Math.Round(800 * factor);
            int difference = targetWidth - tileGrid.Width;
            tileGrid.Width += difference;
            map.Width += difference;
            this.Width += difference;
            //Recalculates tile dimensions after the resize
            int tileWidth = tileGrid.Width / mapWidth;
            int tileHeight = tileGrid.Height / mapHeight;

            mapGrid = new PictureBox[tileCount];
            for(int i = 0; i < mapGrid.Length; i++)
            {
                PictureBox tile = new PictureBox();
                tile.Margin = new Padding(0);
                tile.Size = new Size(tileWidth, tileHeight);
                tile.MouseDown += FillSquare; tile.MouseDown += UncolorTile;
                tile.MouseMove += FillSquare; tile.MouseMove += UncolorTile;
                tile.TabStop = false;
                mapGrid[i] = tile;
                tileGrid.Controls.Add(tile);
            }
        }

        /// <summary>
        /// Alternate constructor in which the editor is loading a 
        /// specific .level file passed from main form.
        /// </summary>
        /// <param name="filename">file path + name from OpenFileDialog</param>
        /// <param name="purename">file name without path</param>
        public LevelEditor(string filename, string purename)
        {
            InitializeComponent();
            FileProcessing(filename, purename);
        }

        /// <summary>
        /// Changes the user's currentColor based on which button 
        /// was pressed. Only meant for the palette buttons
        /// </summary>
        /// <param name="sender">Triggering button</param>
        /// <param name="e">EventArgs</param>
        private void ChangeColor(object sender, EventArgs e)
        {
            Button choice = (Button)sender;
            currentColor = choice.BackColor;
            currentTile.BackColor = choice.BackColor;
        }

        /// <summary>
        /// Colors in the picturebox if the user selected a color and 
        /// clicked the tile with the left mouse button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FillSquare(object? sender, MouseEventArgs e)
        {
            
            if(currentColor != SystemColors.Control && sender != null && e.Button == MouseButtons.Left)
            {
                PictureBox tile = (PictureBox)sender;
                tile.Capture = false;
                tile.BackColor = currentColor;
                if(!unsaved)
                {
                    unsaved = true;
                    this.Text = $"{this.Text} *";
                }
            }
        }

        /// <summary>
        /// If the user holds or presses right click, the tile is reverted to 
        /// its default "colorless" appearance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UncolorTile(object? sender, MouseEventArgs e)
        {
            if (currentColor != DefaultBackColor && sender != null && e.Button == MouseButtons.Right)
            {
                PictureBox tile = (PictureBox)sender;
                tile.Capture = false;
                tile.BackColor = SystemColors.Control;
                if (!unsaved)
                {
                    unsaved = true;
                    this.Text = $"{this.Text} *";
                }
            }
        }

        /// <summary>
        /// Creates a .level (binary) file that stores the data of the 
        /// images at hand, with name & location decided by the user.
        /// </summary>
        /// <param name="sender">Save Button</param>
        /// <param name="e">EventArgs</param>
        private void SaveMap(object? sender, EventArgs e)
        {
            SaveFileDialog unseenServant = new SaveFileDialog();
            unseenServant.Title = "Save a Level file";
            unseenServant.Filter = "Level Files|*.level";
            DialogResult choice = unseenServant.ShowDialog();
            if(choice == DialogResult.OK)
            {
                BinaryWriter output = null!;
                try
                {
                    output = new BinaryWriter(File.OpenWrite(unseenServant.FileName));
                    output.Write(mapWidth * mapHeight); //Full tile Count
                    output.Write(mapWidth); output.Write(mapHeight);

                    /*Holds wahtever color's currently being evaluated*/
                    Color countedColor = mapGrid[0].BackColor;
                    output.Write(countedColor.ToArgb());
                    int streak = 1; //How many tiles in a row have the countedColor
                    /*Goes through the grid of boxes and compares each tile's color
                    to the current color being evaluated, swapping out comparison 
                    once a different color is spotted*/
                    for(int i = 1; i < mapGrid.Length; i++)
                    {
                        Color current = mapGrid[i].BackColor;
                        if (current != countedColor)
                        {
                            output.Write(streak); streak = 1;
                            output.Write(current.ToArgb());
                            //If this is the last tile, it writes 1
                            //because we know it will be the only one
                            if(i == mapGrid.Length - 1) { output.Write(1); }
                            else { countedColor = current; }
                        /*If the last tile is the same as the one before it 
                        we have to increment streak once before writing it*/
                        } else if(i == mapGrid.Length - 1)
                        {
                            streak++; output.Write(streak);
                        } else { streak++; }
                    }
                    //Shows new file name in title if save is sucessful & informs user
                    MessageBox.Show("File saved sucessfully", "File Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    unsaved = false; string path = unseenServant.FileName;
                    this.Text = $"WyPaint - {Path.GetFileName(path)}";
                } catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "File Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                } finally { if(output != null) output.Close(); }
            }
        }

        /// <summary>
        /// Loads a tile map from a .level file passed by a method. It does so by 
        /// loading the map in "chunks" of tiles with the same color
        /// </summary>
        /// <param name="filename">Full file path</param>
        /// <param name="purename">Just file name & extension</param>
        private void FileProcessing(string filename, string purename)
        {
            BinaryReader input = null!;
            try
            {
                input = new BinaryReader(File.OpenRead(filename));
                int newTileCount = input.ReadInt32();
                int newWidth = input.ReadInt32(); int newHeight = input.ReadInt32();
                int tiles = 0; //tracks how many tiles have been processed by the reader
                int index = 0; //the position within a particular 'chunk'

                /*If the loaded image has the same dimensions as the one currently being used
                 * We can take a shortcut and recolor the tiles instead of making new ones*/
                if (newWidth == mapWidth && newHeight == mapHeight)
                {
                    while (tiles < newTileCount)
                    {
                        Color color = Color.FromArgb(input.ReadInt32());
                        int count = input.ReadInt32();
                        for (int i = 0; i < count; i++)
                        {
                            mapGrid[index].BackColor = color;
                            index++; tiles++;
                        }
                    }
                }
                else
                {
                    mapWidth = newWidth; mapHeight = newHeight;
                    /*This is the same as the code from the constructor that adjusts
                    several components if the width & height aren't the same*/
                    double factor = (double)newWidth / newHeight;
                    int targetWidth = (int)Math.Round(800 * factor);
                    int difference = targetWidth - tileGrid.Width;
                    tileGrid.Width += difference;
                    map.Width += difference;
                    this.Width += difference;

                    //removes all pictureboxes from the tileGrid panel if any are there
                    tileGrid.Controls.Clear();
                    mapGrid = new PictureBox[newTileCount];
                    //Recalculates tile dimensions after the resize
                    int tileWidth = tileGrid.Width / mapWidth;
                    int tileHeight = tileGrid.Height / mapHeight;

                    while (tiles < newTileCount)
                    {
                        Color color = Color.FromArgb(input.ReadInt32());
                        int count = input.ReadInt32();
                        //Mostly the same as the constructor loop except with color set too.
                        for (int i = 0; i < count; i++)
                        {
                            PictureBox tile = new PictureBox();
                            tile.Margin = new Padding(0);
                            tile.Size = new Size(tileWidth, tileHeight);
                            tile.MouseDown += FillSquare; tile.MouseDown += UncolorTile;
                            tile.MouseMove += FillSquare; tile.MouseMove += UncolorTile;
                            tile.TabStop = false;
                            tile.BackColor = color;
                            //Uses index because i is not trackign the array's index
                            mapGrid[index] = tile;
                            tileGrid.Controls.Add(tile);

                            index++; tiles++;
                        }
                    }
                }
                //title change happens only if load-in is successful
                MessageBox.Show("File loaded sucessfully", "File loaded", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (unsaved) { unsaved = false; }
                this.Text = $"WyPaint Editor - {purename}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "File Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { if (input != null) input.Close(); }
        }

        /// <summary>
        /// Obtains file information from the .level file passed by the user 
        /// and then passes it to the FileProcessing method.
        /// </summary>
        /// <param name="sender">Original object (Load Map button)</param>
        /// <param name="e">EventArgs</param>
        private void LoadMap(object sender, EventArgs e)
        {
            /*If the user tries loading another file while there are unsaved changes
            they will be asked if they want to proceed without saving*/
            DialogResult choice = DialogResult.Yes;
            if(unsaved)
            {
                choice = MessageBox.Show("You still have unsaved changes. Are you sure you want to load a new file?",
                "Unsaved changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            }

            if(!unsaved || choice == DialogResult.Yes)
            {
                OpenFileDialog unseenServant = new OpenFileDialog();
                unseenServant.Title = "Load a Level file";
                unseenServant.Filter = "Level Files|*.level";
                DialogResult fileChoice = unseenServant.ShowDialog();
                if (fileChoice == DialogResult.OK)
                {
                    FileProcessing(unseenServant.FileName, unseenServant.SafeFileName);
                }
            }
        }

        /// <summary>
        /// If the user closes this window while there are unsaved changes, 
        /// they will be asked whether they want to proceed without saving.
        /// </summary>
        /// <param name="sender">The level editor form</param>
        /// <param name="e">FormClosingEventArgs</param>
        private void ExitTicket(object sender, FormClosingEventArgs e)
        {
            Form1 menu = new Form1();
            if(e.CloseReason != CloseReason.ApplicationExitCall) //Prevents these dialogs from happening twice
            {
                if (unsaved)
                {
                    DialogResult saveChoice = MessageBox.Show(
                        "There are unsaved changes. Are you sure you want to quit?",
                        "Unsaved changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    DialogResult exit = MessageBox.Show("Would you like to go back to the menu?", "",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (saveChoice == DialogResult.No) { e.Cancel = true; }
                    if (exit == DialogResult.Yes) { menu.Show(); }
                    else { Application.Exit(); }
                }
                else
                {
                    DialogResult exit = MessageBox.Show("Would you like to go back to the menu or cancel?", "",
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (exit == DialogResult.Yes) { menu.Show(); }
                    else if (exit == DialogResult.Cancel) { e.Cancel = true; }
                    else { Application.Exit(); }
                }
            }
        }
    }
}
