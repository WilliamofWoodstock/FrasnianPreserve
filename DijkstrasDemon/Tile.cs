using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ShapeUtils;
using System;
using System.Collections.Generic;

namespace DijkstrasDemon
{
    internal class Tile
    {
        private Rectangle box;
        private bool lmbLastFrame;
        private bool rmbLastFrame;

        /// <summary>
        /// The tile at the start of the search. Path Neighbor will always be null for them.
        /// </summary>
        public static Tile StartTile { get; set; }
        /// <summary>
        /// The tile at the end of the search.
        /// </summary>
        public static Tile EndTile { get; set; }
        /// <summary>
        /// Indicates whether this tile is a wall that cannot be used for a path
        /// </summary>
        public bool IsWall { get; set; }
        /// <summary>
        /// Total path cost to move here (Dijkstra data)
        /// </summary>
        public int TotalDistance { get; set; }
        /// <summary>
        /// Closest neighbor on path back to StartTile (Dijkstra)
        /// </summary>
        public Tile PathNeighbor { get; set; }
        public bool Permanent { get; set; }

        public Tile(int x, int y, int size)
        {
            box = new Rectangle(x, y, size, size);
            TotalDistance = int.MaxValue;
            PathNeighbor = null;
        }

        /// <summary>
        /// Sees if the mouse clicked it, and with which input. 
        /// Walls are madde with LMB whereas start/end tile is made with RMB
        /// </summary>
        public void CheckClick(out bool change)
        {
            MouseState mouse = Mouse.GetState();
            Point cursor = new Point(mouse.X, mouse.Y);
            change = false;

            bool lmbPressed = mouse.LeftButton == ButtonState.Pressed && box.Contains(cursor);
            bool lmb = lmbPressed && !lmbLastFrame;

            bool rmbPressed = mouse.RightButton == ButtonState.Pressed && box.Contains(cursor);
            bool rmb = rmbPressed && !rmbLastFrame;
            if(lmb && !Graph.Busy && StartTile != this && EndTile != this)
            {
                change = true;
                if(!IsWall) { IsWall = true; }
                else { IsWall = false; }
            }
            if(rmb && !Graph.Busy)
            {
                change = true;
                if(Button.Mode == MouseMode.StartPoint) { StartTile = this; IsWall = false; }
                else { EndTile = this; IsWall = false; }
            }
            
            lmbLastFrame = lmbPressed; rmbLastFrame = rmbPressed;
        }

        /// <summary>
        /// Draws the tile, color adjusted based on whether it's a 
        /// wall, start or end tile, permament, or part of the shortest path.
        /// </summary>
        /// <param name="grid">The Graph that's storing this tile</param>
        public void Draw(Graph grid)
        {
            if(this == StartTile)  { ShapeBatch.Box(box, Color.Blue); }
            else if(this == EndTile) { ShapeBatch.Box(box, Color.Red); }
            else if(IsWall) { ShapeBatch.Box(box, Color.Gray); }
            else if(grid.Path.Contains(this)) { ShapeBatch.Box(box, Color.Violet); }
            else if (Permanent) { ShapeBatch.Box(box, Color.LightGoldenrodYellow); }
            else { ShapeBatch.BoxOutline(box, Color.White); }
        }
        /// <summary>
        /// Draws the tile, color adjusted based on whether it's a 
        /// wall, start or end tile, permament, or part of the shortest path.
        /// </summary>
        public void Draw()
        {
            if (this == StartTile) { ShapeBatch.Box(box, Color.Blue); }
            else if (this == EndTile) { ShapeBatch.Box(box, Color.Red); }
            else if (IsWall) { ShapeBatch.Box(box, Color.Gray); }
            else { ShapeBatch.BoxOutline(box, Color.White); }
        }
    }
}
