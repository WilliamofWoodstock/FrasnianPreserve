using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DijkstrasDemon
{
    enum MouseMode
    {
        StartPoint,
        EndPoint
    }

    internal class Button
    {
        private Rectangle box;
        private string text;
        private SpriteFont font;
        private bool pressedLastFrame;
        private Texture2D texture;

        /// <summary>
        /// Tracks whether this button is currently being clicked.
        /// </summary>
        public bool IsClicked { get; protected set; }

        public static MouseMode Mode { get; protected set; }

        public Button(Rectangle box, SpriteFont font, string text, Texture2D texture)
        {
            this.box = box; this.font = font; this.text = text; Mode = MouseMode.StartPoint;
            this.texture = texture;
        }

        public Button(int x, int y, int width, int height, SpriteFont font, string text, Texture2D texture) : 
            this(new Rectangle(x, y, width, height), font, text, texture)
        { }

        public void Update()
        {
            MouseState mouse = Mouse.GetState();
            Point cursor = new Point(mouse.X, mouse.Y);
            bool pressed = box.Contains(cursor) && mouse.LeftButton == ButtonState.Pressed;
            IsClicked = pressed && !pressedLastFrame;

            if(IsClicked)
            {
                if(Mode == MouseMode.StartPoint) { Mode = MouseMode.EndPoint; }
                else { Mode = MouseMode.StartPoint; }
            }
            pressedLastFrame = pressed;
        }

        public void Draw(SpriteBatch sb)
        {
            Color buttonColor;
            if(Mode == MouseMode.StartPoint) { buttonColor = Color.Blue; }
            else{ buttonColor = Color.Red; }

            sb.Draw(texture, box, buttonColor);
            Vector2 textSize = font.MeasureString(text);
            Vector2 textPos = new Vector2(box.X + (box.Width - textSize.X) / 2f, box.Y + (box.Height - textSize.Y) / 2f);
            sb.DrawString(font, text, textPos, Color.White);
        }
    }
}
