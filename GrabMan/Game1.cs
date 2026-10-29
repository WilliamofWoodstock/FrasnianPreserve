using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace GrabMan
{
    /// <summary>
    /// Holds values for the 3 general "game states"
    /// to make things easier for the UI
    /// </summary>
    enum GameState
    {
        Menu,
        Play,
        GameOver
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D grabManTexture;
        private SpriteFont bigFont; private SpriteFont medFont; private SpriteFont smallFont;
        //Lets the program know if it needs to re-setup the game itself
        private bool newRound = true;
        private KeyboardState previous;

        private Player player;

        Vector2 titleSize; Vector2 titlePos;
        Vector2 startSize; Vector2 startPos;

        //Used for animation
        public const double fps = 10.0;
        public static Random rnd = new Random();
        private static Song gameMusic;
        /// <summary>
        /// References the music that plays during the game itself
        /// </summary>
        public static Song GameMusic { get => gameMusic; }

        private GameState gameState = GameState.Menu;
        private static Rectangle viewport;
        /// <summary>
        /// Allows other classes to reference the game's viewable area
        /// </summary>
        public static Rectangle Viewport { get => viewport; }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = 1000;
            _graphics.PreferredBackBufferHeight = 800;
            _graphics.ApplyChanges();

            bigFont = Content.Load<SpriteFont>("TitleFont");
            medFont = Content.Load<SpriteFont>("MedFont");
            smallFont = Content.Load<SpriteFont>("SmallFont");
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            viewport = Window.ClientBounds;
            grabManTexture = Content.Load<Texture2D>("GrabManSprites");
            LevelManager.Itself.PearlSprite = Content.Load<Texture2D>("PearlSprite");
            LevelManager.Itself.PowerupSprite = Content.Load<Texture2D>("PowerUpSprite");
            LevelManager.Itself.PowerupSound = Content.Load<SoundEffect>("TimeStopLouder");
            gameMusic = Content.Load<Song>("Masked Dedede Remix - Hardcore");
            MediaPlayer.Volume = 0.3f;
            player = new Player(grabManTexture, 40, 40, 4);
            // TODO: use this.Content to load your game content here

            titleSize = bigFont.MeasureString("GRAB MAN");
            titlePos = new Vector2((Window.ClientBounds.Width - titleSize.X) / 2, (Window.ClientBounds.Height - titleSize.Y) / 2);
            startSize = smallFont.MeasureString("Press enter to start playing!");
            startPos = new Vector2((Window.ClientBounds.Width - startSize.X) / 2, Window.ClientBounds.Height / 2 + startSize.Y);
            LevelManager.Itself.Player = player;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            // TODO: Add your update logic here
            KeyboardState input = Keyboard.GetState();
            switch (gameState)
            {
                case GameState.Menu:
                    if(input.IsKeyDown(Keys.Enter) && !(previous.IsKeyDown(Keys.Enter))) 
                    { gameState = GameState.Play; }
                    break;
                case GameState.GameOver:
                    if(input.IsKeyDown(Keys.Enter) && !(previous.IsKeyDown(Keys.Enter))) 
                    { gameState = GameState.Menu; newRound = true; }
                    break;
                case GameState.Play:
                    bool gameOver;
                    if(newRound) { LevelManager.Itself.GameStart(); newRound = false; }
                    player.Update(gameTime);
                    LevelManager.Itself.Update(gameTime, out gameOver);
                    if(gameOver) 
                    { 
                        gameState = GameState.GameOver;
                        MediaPlayer.Stop();
                    }
                    break;
            }
            previous = input;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            // TODO: Add your drawing code here
            switch(gameState)
            {
                case GameState.Menu:
                    GraphicsDevice.Clear(Color.Black);
                    _spriteBatch.DrawString(bigFont, "GRAB MAN", titlePos, Color.LightSeaGreen);
                    _spriteBatch.DrawString(smallFont, "Press enter to start playing!", startPos, Color.LightSeaGreen);
                    break;
                case GameState.Play:
                    GraphicsDevice.Clear(Color.CornflowerBlue);
                    player.Draw(_spriteBatch);
                    LevelManager.Itself.Draw(_spriteBatch);
                    Vector2 stageTextSize = medFont.MeasureString($"Level: {LevelManager.Itself.Stage}");
                    Vector2 scoreTextSize = medFont.MeasureString($"Score: {player.Score}");
                    Vector2 timeTextSize = medFont.MeasureString($"Time: {LevelManager.Itself.TimeRemaining}");

                    Vector2 stageTextPos = new Vector2(20, 10);
                    Vector2 scoreTextPos = new Vector2(20, stageTextPos.Y + stageTextSize.Y);
                    Vector2 timeTextPos = new Vector2(20, scoreTextPos.Y + scoreTextSize.Y);
                    _spriteBatch.DrawString(medFont, $"Level: {LevelManager.Itself.Stage}", stageTextPos, Color.Lime);
                    _spriteBatch.DrawString(medFont, $"Score: {player.Score}", scoreTextPos, Color.Lime);
                    _spriteBatch.DrawString(medFont, $"Time: {LevelManager.Itself.TimeRemaining.ToString("#.00")}", timeTextPos, Color.Lime);
                    break;
                case GameState.GameOver:
                    Vector2 finalScoreSize = smallFont.MeasureString($"Score: {LevelManager.Itself.TotalScore}");
                    Vector2 finalScorePos = new Vector2((Window.ClientBounds.Width - finalScoreSize.X) / 2,
                        Window.ClientBounds.Height / 2 + startSize.Y);
                    Vector2 stageSize = smallFont.MeasureString($"Level reached: {LevelManager.Itself.Stage}");
                    Vector2 stagePos = new Vector2((Window.ClientBounds.Width - stageSize.X)/2, finalScorePos.Y + stageSize.Y);
                    Vector2 playAgainPos = new Vector2(startPos.X, stagePos.Y + startSize.Y);
                    GraphicsDevice.Clear(Color.Black);
                    _spriteBatch.DrawString(bigFont, "GAME OVER", titlePos, Color.LightSeaGreen);
                    _spriteBatch.DrawString(smallFont, $"Score: {LevelManager.Itself.TotalScore}", finalScorePos, Color.LightSeaGreen);
                    _spriteBatch.DrawString(smallFont, $"Level Reached: {LevelManager.Itself.Stage}", stagePos, Color.LightSeaGreen);
                    _spriteBatch.DrawString(smallFont, "Press Enter for main menu", playAgainPos, Color.LightSeaGreen);
                    break;
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
