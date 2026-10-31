using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GrabMan
{

    enum PlayerStates
    {
        FaceLeft,
        FaceRight,
        WalkLeft,
        WalkRight
    }

    [Flags]
    enum InputDirection
    {
        Left = 1<<0,            //1
        Right = 1<<1,           //2
        Up = 1<<2,              //4
        Down = 1<<3,            //8
        None = 1<<4,            //16
        LeftUp = Left | Up,     //5
        RightUp = Right | Up,   //6
        LeftDown = Left | Down, //9
        RightDown = Right | Down//10
    }

    /// <summary>
    /// Controls all behavior in regards to the player
    /// </summary>
    internal class Player : GameObject
    {
        private int score = 0;
        //Animation fields
        int currentFrame = 1;
        double timeCounter = 0;
        double secondsPerFrame;
        int spritesInSheet;
        int widthOfSingleSprite;
        private Vector2 position; //duct-tape solution for delta time movement.

        public Powerup Held { get; set; }
        /// <summary>
        /// The player's score on the current stage
        /// </summary>
        public int Score { get => score; set => score = value; }
        /// <summary>
        /// The current animation state of the player
        /// </summary>
        public PlayerStates CurrentState { get; set; }
        /// <summary>
        /// The base speed that determiens the number of pixels travelled in any direction per frame
        /// </summary>
        public int Speed { get; set; }

        /// <summary>
        /// The "base" constructor.
        /// </summary>
        /// <param name="texture">The player's sprite sheet</param>
        /// <param name="box">Represents the player's position in space</param>
        /// <param name="speed">The "base speed" of the player (5 pixels per frame by default)</param>
        /// <param name="spritesInSheet">Number of individual sprites within the sprite sheet</param>
        public Player(Texture2D texture, Rectangle box, int spritesInSheet, int speed = 250) : base(texture, box)
        {
            Speed = speed; position = new Vector2(box.X, box.Y);
            this.spritesInSheet = spritesInSheet;
            this.widthOfSingleSprite = texture.Width / spritesInSheet;
            secondsPerFrame = 1.0f / Game1.fps;
            CurrentState = PlayerStates.FaceRight;
        }

        /// <summary>
        /// Alternate constructor in which the rectangle is created herein based on 
        /// passed data
        /// </summary>
        /// <param name="texture">The player's sprite sheet</param>
        /// <param name="x">The starting x position of the player</param>
        /// <param name="y">The starting y position of the player</param>
        /// <param name="spritesInSheet">Number of individual sprites within the sprite sheet</param>
        /// <param name="speed">The "base speed" of the player (5 pixels per frame by default)</param>
        public Player(Texture2D texture, int x, int y, int spritesInSheet, int speed = 250) : base(texture)
        {
            Speed = speed; position = new Vector2(x, y);
            this.spritesInSheet = spritesInSheet;
            this.widthOfSingleSprite = texture.Width / spritesInSheet;
            secondsPerFrame = 1.0f / Game1.fps;
            int height = texture.Height;
            Box = new Rectangle(x, y, widthOfSingleSprite, height);
            CurrentState = PlayerStates.FaceRight;
        }

        /// <summary>
        /// Grabs the "movement direction" of the player and then adjusts playerstate and 
        /// position accordingly
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            InputDirection direction = GetDirection(Keyboard.GetState());
            if(Powerup.Held != null) { Powerup.Held.Update(gameTime); }
            if(direction.HasFlag(InputDirection.Right))
            {
                switch(CurrentState)
                {
                    case PlayerStates.FaceRight:
                        CurrentState = PlayerStates.WalkRight;
                        break;
                    case PlayerStates.WalkRight:
                        //X += Speed;
                        position.X += Speed * LevelManager.DeltaTime;
                        break;
                    case PlayerStates.FaceLeft:
                    case PlayerStates.WalkLeft:
                        CurrentState = PlayerStates.FaceRight;
                        break;
                }
            } else if(direction.HasFlag(InputDirection.Left))
            {
                switch(CurrentState)
                {
                    case PlayerStates.FaceRight:
                    case PlayerStates.WalkRight:
                        CurrentState = PlayerStates.FaceLeft;
                        break;
                    case PlayerStates.FaceLeft:
                        CurrentState = PlayerStates.WalkLeft;
                        break;
                    case PlayerStates.WalkLeft:
                        //X -= Speed;
                        position.X -= Speed * LevelManager.DeltaTime;
                        break;
                }
            } else if(direction.HasFlag(InputDirection.None))
            {
                if(CurrentState == PlayerStates.WalkLeft) { CurrentState = PlayerStates.FaceLeft; }
                else if(CurrentState == PlayerStates.WalkRight) { CurrentState = PlayerStates.FaceRight; }
            }

            if (direction.HasFlag(InputDirection.Up))
            {
                if (CurrentState == PlayerStates.FaceRight) { CurrentState = PlayerStates.WalkRight; }
                else if(CurrentState == PlayerStates.FaceLeft) { CurrentState = PlayerStates.WalkLeft; }
                //Y -= Speed;
                position.Y -= Speed * LevelManager.DeltaTime;
                if (position.Y < 0) { position.Y = 0; }
            }
            else if (direction.HasFlag(InputDirection.Down))
            {
                if (CurrentState == PlayerStates.FaceRight) { CurrentState = PlayerStates.WalkRight; }
                else if(CurrentState == PlayerStates.FaceLeft) { CurrentState = PlayerStates.WalkLeft; }
                //Y += Speed;
                position.Y += Speed * LevelManager.DeltaTime;
                if (position.Y + Height > Game1.Viewport.Height) { position.Y = Game1.Viewport.Height - Height; }
            }

            //If the player goes off screen on the left or right side 
            //They wrap around to the other edge
            if (position.X <= 0 - widthOfSingleSprite)
            {
                position.X = Game1.Viewport.Width;
            }
            else if (position.X >= Game1.Viewport.Width)
            {
                position.X = 0 - widthOfSingleSprite;
            }
            UpdateAnimation(gameTime);
        }

        /// <summary>
        /// Ascertains the direction the player is moving based on their input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private InputDirection GetDirection(KeyboardState input)
        {
            List<InputDirection> directions = new List<InputDirection>();
            if(input.IsKeyDown(Keys.D) || input.IsKeyDown(Keys.Right)) 
                directions.Add(InputDirection.Right);
            if(input.IsKeyDown(Keys.A) || input.IsKeyDown(Keys.Left))
                directions.Add(InputDirection.Left);
            if(input.IsKeyDown(Keys.S) || input.IsKeyDown(Keys.Down))
                directions.Add(InputDirection.Down);
            if(input.IsKeyDown(Keys.W) || input.IsKeyDown(Keys.Up))
                directions.Add(InputDirection.Up);

            if(directions.Count == 4 || directions.Count == 0)
            {
                return InputDirection.None;
            } else if(directions.Count == 1)
            {
                return directions[0];
            } else
            {
                if(directions.Contains(InputDirection.Down))
                {
                    if(directions.Contains(InputDirection.Left)) { return InputDirection.LeftDown; }
                    else { return InputDirection.RightDown; }
                } else
                {
                    if(directions.Contains(InputDirection.Left)) { return InputDirection.LeftUp; }
                    else { return InputDirection.RightUp; }
                }
            }
        }

        /// <summary>
        /// Changes the frame that will be used if enough time has passed. Also 
        /// Updates the position of the sprite's box to be as clsoe as possibel to the object's position
        /// </summary>
        /// <param name="gameTime">Information related to game time</param>
        private void UpdateAnimation(GameTime gameTime)
        {
            Box = new Rectangle((int)Math.Round(position.X),
                (int)Math.Round(position.Y),
                Width,
                Height);
            timeCounter += gameTime.ElapsedGameTime.TotalSeconds;

            /*If a 'frame' worth of time has passed the animation frame 
            is updated*/
            if(timeCounter >= secondsPerFrame)
            {
                currentFrame++;
                //"loops" the frame value if it was on the sheet's 'last' sprite
                if(currentFrame >= 4) { currentFrame = 1; }
                /*Takes away a frame worth of time such that any remaining time will
                carry over to the next call*/
                timeCounter -= secondsPerFrame;
            }
        }

        /// <summary>
        /// Decides how the player should be drawn based on their PlayerState
        /// </summary>
        /// <param name="sb">SpriteBatch</param>
        public override void Draw(SpriteBatch sb)
        {
            switch(CurrentState)
            {
                case PlayerStates.FaceRight:
                    DrawStill(sb, SpriteEffects.None);
                    break;
                case PlayerStates.FaceLeft:
                    DrawStill(sb, SpriteEffects.FlipHorizontally);
                    break;
                case PlayerStates.WalkRight:
                    DrawMoving(sb, SpriteEffects.None);
                    break;
                case PlayerStates.WalkLeft:
                    DrawMoving(sb, SpriteEffects.FlipHorizontally);
                    break;
            }
        }

        /// <summary>
        /// Draws the player in one of their running animation frames
        /// </summary>
        /// <param name="sb">SpriteBatch</param>
        /// <param name="flip">Whether the player should be facing left</param>
        private void DrawMoving(SpriteBatch sb, SpriteEffects flip)
        {
            sb.Draw(
                texture,
                Box,
                new Rectangle(Width * currentFrame, 0, Width, Height),
                Color.White,
                0f,
                Vector2.Zero,
                flip,
                1.0f);
        }

        /// <summary>
        /// Draws the player in their default standing sprite
        /// </summary>
        /// <param name="sb">SpriteBatch</param>
        /// <param name="flip">Whether the player should be facing left</param>
        private void DrawStill(SpriteBatch sb, SpriteEffects flip)
        {
            sb.Draw(
                texture,
                Box,
                new Rectangle(0, 0, Width, Height),
                Color.White,
                0f,
                Vector2.Zero,
                flip,
                1.0f);
        }
    }
}
