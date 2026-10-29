using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShapeUtils;

namespace DijkstrasDemon
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Graph grid;
        private Button pointSetter;
        private SpriteFont font;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = 950;
            grid = new Graph(30, 20);
            _graphics.PreferredBackBufferHeight = 800;
            _graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Rectangle buttonRect = new Rectangle(Window.ClientBounds.Width / 2 - 150, 50, 300, 50);
            font = Content.Load<SpriteFont>("font");
            pointSetter = new Button(buttonRect, font, "StartPoint/EndPoint", Content.Load<Texture2D>("whitePixel"));
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            grid.Update();
            pointSetter.Update();
            // TODO: Add your update logic here
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            ShapeBatch.Begin(GraphicsDevice);
            grid.Draw();
            ShapeBatch.End();
            _spriteBatch.Begin();
            pointSetter.Draw(_spriteBatch);
            _spriteBatch.DrawString(font, "Press Space to begin search", new Vector2(325, 725), Color.White);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
