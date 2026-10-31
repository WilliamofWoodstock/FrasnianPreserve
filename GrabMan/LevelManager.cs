using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace GrabMan
{
    /// <summary>
    /// Tracks the progress of the game while in play and handles the transition to 
    /// each new stage
    /// </summary>
    internal class LevelManager
    {
        private float timeOnStage = 0f;
        private float timeLimit = 15f;
        private float stoppedTime = 0f;
        /// <summary>
        /// Used to display time to the user
        /// </summary>
        public float TimeRemaining
        {
            get { return timeLimit - timeOnStage; }
        }

        /// <summary>
        /// The amount of time that has passed since the last frame.
        /// </summary>
        public static float DeltaTime { get => deltaTime; }
        private static float deltaTime;

        private int pearlWidth = 50; private int pearlHeight = 50;
        private List<Rectangle> pearls = new List<Rectangle>();
        private Powerup onStage; //tracks the powerUp currently "on stage"

        private int stage;
        /// <summary>
        /// Stage the player is on
        /// </summary>
        public int Stage { get => stage; }
        private int numToMake; //number of pearls to make

        private static LevelManager itself = null;
        /// <summary>
        /// This property ensures that there's only ever one level manager
        /// </summary>
        public static LevelManager Itself
        {
            get
            {
                if(itself == null) { itself = new LevelManager(); }
                return itself;
            }
        }
        /// <summary>
        /// LevelManager's reference to the player object
        /// </summary>
        public Player Player { get; set; }
        /// <summary>
        /// Tracks the player's score
        /// </summary>
        public int TotalScore { get; set; }
        /// <summary>
        /// Sprite used for the pearls
        /// </summary>
        public Texture2D PearlSprite { get; set; }
        /// <summary>
        /// Sprite used for the power up
        /// </summary>
        public Texture2D PowerupSprite { get; set; }
        /// <summary>
        /// Sound used by the power up
        /// </summary>
        public SoundEffect PowerupSound { get; set; }

        /// <summary>
        /// Constructor is private so that only this class can control
        /// when LevelManager objects are made
        /// </summary>
        private LevelManager() { }

        /// <summary>
        /// Tracks and removes pearls as they get collected, counts down the 
        /// time and tracks wehther the player moves on or hit GameOver.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="gameOver"></param>
        public void Update(GameTime gameTime, out bool gameOver)
        {
            deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            CheckCollectibles();
            if(stoppedTime > 0)
            {
                stoppedTime -= deltaTime;
            } else 
            { 
                timeOnStage += deltaTime;
                if(MediaPlayer.State == MediaState.Paused) { MediaPlayer.Resume(); }
                else if(MediaPlayer.State != MediaState.Playing) { MediaPlayer.Play(Game1.GameMusic); }
            }
            if (onStage != null) { onStage.Update(gameTime); }
            if(pearls.Count == 0)
            {
                NextStage();
                gameOver = false;
            }
            else if(timeOnStage >= timeLimit) { gameOver = true; }
            else { gameOver = false; }
        }

        /// <summary>
        /// Sets up the actual game from Stage 1
        /// </summary>
        public void GameStart()
        {
            stage = 1; numToMake = 15; pearls.Clear(); 
            TotalScore = 0; Player.Score = 0;
            timeOnStage = 0f; timeLimit = 15f;
            (int xBound, int yBound) limits = (Game1.Viewport.Width - pearlWidth, Game1.Viewport.Height - pearlHeight);
            for (int i = 1; i <= numToMake; i++)
            {
                int xPos = Game1.rnd.Next(0, limits.xBound);
                int yPos = Game1.rnd.Next(0, limits.yBound);
                pearls.Add(new Rectangle(xPos, yPos, pearlWidth, pearlHeight));
            }
        }

        /// <summary>
        /// Checks and removes pearls that the player collides after incrementing score
        /// </summary>
        private void CheckCollectibles()
        {
            for(int i = pearls.Count - 1; i >= 0; i--)
            {
                if (Player.CheckCollision(pearls[i]))
                {
                    TotalScore++;
                    Player.Score++;
                    pearls.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Updates all info and creates the collectibles for the next stage
        /// </summary>
        private void NextStage()
        {
            stage++; Player.Score = 0;
            onStage = null;
            timeOnStage = 0;
            numToMake += 5;
            (int xBound, int yBound) limits = (Game1.Viewport.Width - pearlWidth, Game1.Viewport.Height - pearlHeight);
            for (int i = 1; i <= numToMake; i++)
            {
                int xPos = Game1.rnd.Next(0, limits.xBound);
                int yPos = Game1.rnd.Next(0, limits.yBound);
                pearls.Add(new Rectangle(xPos, yPos, pearlWidth, pearlHeight));
            }
            int powerupRoll = Game1.rnd.Next(1, 11);
            if(powerupRoll <= 2)
            {
                int xPos = Game1.rnd.Next(0, limits.xBound);
                int yPos = Game1.rnd.Next(0, limits.yBound);
                onStage = new Powerup(PowerupSprite, xPos, yPos, Player, PowerupSound);
                onStage.Begin += StopTime;
            }
        }

        /// <summary>
        /// Draws all collectibles that are still active
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            foreach(Rectangle pearl in pearls)
            {
                sb.Draw(PearlSprite, pearl, Color.White);
            }
            if(onStage != null) { onStage.Draw(sb); }
        }

        /// <summary>
        /// Activates stopped time
        /// </summary>
        /// <param name="duration"></param>
        public void StopTime(Powerup sender, int duration)
        {
            stoppedTime = duration;
            sender.Begin -= StopTime;
            if(MediaPlayer.State != MediaState.Paused) { MediaPlayer.Pause(); }
            if(sender == onStage) { onStage = null; }
        }
    }
}
