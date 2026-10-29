using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace GrabMan
{
    internal delegate void TimeStop(Powerup sender, int duration);
    /// <summary>
    /// Holds data related to the timestop powerup
    /// </summary>
    internal class Powerup : GameObject
    {
        private int duration; //random value between 5-10 seconds
        private SoundEffect soundEffect;
        /// <summary>
        /// reference to the player object
        /// </summary>
        public Player Player { get; set; }
        /// <summary>
        /// Tracks the powerup currently in posession by the player
        /// </summary>
        public static Powerup Held { get; set; }

        public event TimeStop Begin;

        /// <summary>
        /// Creates a new powerup (only needs position because dimensions are always the same
        /// </summary>
        /// <param name="texture">The powerup's sprite</param>
        /// <param name="x">XPos of powerup</param>
        /// <param name="y">YPos of powerup</param>
        /// <param name="player">Reference to the player object</param>
        /// <param name="soundEffect">Sound effect that'll play when its activated</param>
        public Powerup(Texture2D texture, int x, int y, Player player, SoundEffect soundEffect) : 
            base(texture, new Rectangle(x, y, 60, 60))
        {
            Player = player;
            duration = Game1.rnd.Next(5, 11);
            this.soundEffect = soundEffect;
        }

        /// <summary>
        /// Checks whether the player picked up this powerup and if so whether 
        /// they activated it.
        /// </summary>
        /// <param name="gameTime">Game time information</param>
        public override void Update(GameTime gameTime)
        {
            if(Held == this)
            {
                KeyboardState input = Keyboard.GetState();
                if (input.IsKeyDown(Keys.Space))
                {
                    Begin?.Invoke(this, duration);
                    soundEffect.Play();
                    Held = null;
                }
            } else
            {
                if(CheckCollision(Player.Box))
                {
                    Held = this;
                }
            }
        }

        /// <summary>
        /// Draws this power up if it's not being hedl by the player
        /// </summary>
        /// <param name="sb"></param>
        public override void Draw(SpriteBatch sb)
        {
            if(Held != this)
            {
                sb.Draw(texture, Box, Color.White);
            }
        }
    }
}
