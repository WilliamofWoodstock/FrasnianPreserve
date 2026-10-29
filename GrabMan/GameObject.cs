using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace GrabMan
{
    /// <summary>
    /// Base class common infrastructure for all game objects
    /// </summary>
    internal abstract class GameObject
    {
        protected Texture2D texture;
        private Rectangle box;
        /// <summary>
        /// The rectangle that contains the object's sprite
        /// </summary>
        public Rectangle Box { get => box; set => box = value; }
        /// <summary>
        /// The x-position of the object (The top left corner)
        /// </summary>
        public int X { get => box.X; set => box.X = value; }
        /// <summary>
        /// The y-position of the object (the top left corner)
        /// </summary>
        public int Y { get => box.Y; set => box.Y = value; }
        /// <summary>
        /// The width of the object
        /// </summary>
        public int Width { get => box.Width; set => box.Width = value; }
        /// <summary>
        /// The height of the object
        /// </summary>
        public int Height { get => box.Height; set => box.Height = value; }

        /// <summary>
        /// Uses a texure and a rectangle to create the game object
        /// </summary>
        /// <param name="texture">The sprite texture used to represent the object</param>
        /// <param name="box">The rectangle that will display the sprite</param>
        public GameObject(Texture2D texture, Rectangle box)
        {
            this.texture = texture; this.box = box;
        }
        /// <summary>
        /// For when you need to make the object without assigning position yet
        /// </summary>
        /// <param name="texture">Object's sprite</param>
        public GameObject(Texture2D texture)
        {
            this.texture = texture;
        }

        /// <summary>
        /// Uses a texture and 4 integers to create a game object, using the 
        /// 4 integers to create the rectangle
        /// </summary>
        /// <param name="texture">The sprite texture used to represent the object</param>
        /// <param name="x">X-coordinate position of the rectangle's top left corner</param>
        /// <param name="y">Y-coordinate position of the rectangle's top left corner</param>
        /// <param name="width">The rectangle's width</param>
        /// <param name="height">The rectangle's height</param>
        public GameObject(Texture2D texture, int x, int y, int width, int height)
        {
            this.texture = texture; box = new Rectangle(x, y, width, height);
        }

        /// <summary>
        /// Abstract method that handles the object's own game logic.
        /// </summary>
        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// Virtual method that draws the object only screen using only the 
        /// texture and box fields with no color filter.
        /// </summary>
        /// <param name="sb"></param>
        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, box, Color.White);
        }

        /// <summary>
        /// Checks if this GameObject is colliding/intersecting with another Rectangle
        /// </summary>
        /// <param name="other">Other hitbox being checked</param>
        /// <returns></returns>
        public virtual bool CheckCollision(Rectangle other)
        {
            if(this.box.Intersects(other)) { return true; }
            return false;
        }
    }
}