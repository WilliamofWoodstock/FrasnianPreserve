namespace WyPaint
{
    public partial class Form1 : Form
    {
        private bool shutoff = true; //Whether this form's closing is menat to end the program
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Checks if the user entered viable width & height requirements, 
        /// creating a new map if they were met.
        /// </summary>
        /// <param name="sender">Original object (Create map button)</param>
        /// <param name="e">EventArgs</param>
        private void MapCreation(object sender, EventArgs e)
        {
            string errors = "Errors:";
            int width = 0; int height = 0;
            try  
            { 
                width = Int32.Parse(widthBox.Text);
                if(width < 10) { errors += "\n - Width must be at least 10"; }
                else if(width > 30) { errors += "\n - Width cannot be greater than 30 tiles"; }
            }
            catch { errors += "\n - Width must be a whole number"; }

            try 
            { 
                height = Int32.Parse(heightBox.Text);
                if(height < 10) { errors += "\n - Height must be at least 10 tiles"; }
                else if(height > 30) { errors += "\n - Height cannot be greater than 30 tiles."; }
            }
            catch { errors += "\n - Height must be a whole number"; }

            if(errors.Length > 7) 
            { MessageBox.Show(errors, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            else 
            {
                LevelEditor map = new LevelEditor(width, height);
                map.Show(); shutoff = false;
                Close();
            }
        }

        /// <summary>
        /// Allows the user to immediately load a .level file 
        /// which is opened if valid.
        /// </summary>
        /// <param name="sender">Original object (Load map button)</param>
        /// <param name="e">EventArgs</param>
        private void GetMap(object sender, EventArgs e)
        {
            OpenFileDialog unseenServant = new OpenFileDialog();
            unseenServant.Title = "Open a Level file";
            unseenServant.Filter = "Level Files|*.level";
            DialogResult choice = unseenServant.ShowDialog();
            if(choice == DialogResult.OK)
            {
                LevelEditor map = new LevelEditor(unseenServant.FileName, unseenServant.SafeFileName);
                map.Show(); shutoff = false;
                Close();
            }
        }

        /// <summary>
        /// When this form is closed, it checks if its meant to end the program too.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">FormClosingEventARrgs</param>
        private void CloseCheck(object sender, FormClosingEventArgs e)
        {
            if(e.CloseReason == CloseReason.UserClosing && shutoff) { Application.Exit(); }
        }
    }
}
