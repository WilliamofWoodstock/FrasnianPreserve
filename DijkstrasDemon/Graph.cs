using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using ShapeUtils;

namespace DijkstrasDemon
{
    internal class Graph
    {
        private Tile[][] tileGrid;
        private MouseState previous;
        /// <summary>
        /// Represents the shortest path from StartTile to EndTile
        /// </summary>
        public List<Tile> Path { get; private set; }
        //Describes the positional offsets of neighbor tiles (Cardinal directions only)
        static readonly (int ro, int co)[] NeighborOffsets =
        {
                      (-1, 0), 
            (0, -1),           (0, 1),
                      (1, 0)
        };

        /// <summary>
        /// Tracks whether the graph is actively performing a search
        /// </summary>
        public static bool Busy { get; protected set; }
        
        /// <summary>
        /// Creates a grid of tiles.
        /// </summary>
        /// <param name="width">Width of grid in tiles (AKA Column count)</param>
        /// <param name="height">Height of grid in tiles (AKA Row count)</param>
        public Graph(int width = 30, int height = 30)
        {
            tileGrid = new Tile[height][];
            for(int i = 0; i < tileGrid.Length; i++)
            {
                tileGrid[i] = new Tile[width];
                for(int j = 0; j < tileGrid[i].Length; j++)
                {
                    tileGrid[i][j] = new Tile(25 + (30 * j), 120 + (30 * i), 30);
                }
            }
            tileGrid[15][15].IsWall = true; tileGrid[14][14].IsWall = true; tileGrid[13][13].IsWall = true; tileGrid[12][12].IsWall = true;
            tileGrid[11][11].IsWall = true; tileGrid[10][10].IsWall = true;
        }

        /// <summary>
        /// Finds all of the neighbors of a tile (based on index)
        /// </summary>
        /// <param name="r"></param>
        /// <param name="c"></param>
        /// <returns>Collection of all cardinal direction neighbors</returns>
        private IEnumerable<Tile> GetNeighbors(int r, int c)
        {
            foreach(var (ro,co) in NeighborOffsets)
            {
                int nr = ro + r;
                int nc = co + c;

                if(nr < 0 || nc < 0 || nr >= tileGrid.Length || nc >= tileGrid[nr].Length) { continue; }

                yield return tileGrid[nr][nc];
            }
        }
        /// <summary>
        /// Finds all of the neighbors of a tile (based on index)
        /// </summary>
        /// <param name="index">A tuple representing the tile's index (row, col)</param>
        /// <returns>Collection of all cardinal direction neighbors</returns>
        private IEnumerable<Tile> GetNeighbors((int row,int col) index)
        {
            return GetNeighbors(index.row, index.col);
        }
        /// <summary>
        /// Finds all of the neighbors of a tile (based on tile)
        /// </summary>
        /// <param name="tile">The tile whose neighbors will be searched for</param>
        /// <returns>Collection of all cardinal direction neighbors</returns>
        private IEnumerable<Tile> GetNeighbors(Tile tile)
        {
            return GetNeighbors(GetIndex(tile));
        }

        /// <summary>
        /// Gets the index of a specific tile within the grid
        /// </summary>
        /// <param name="tile">The tile to be searching for</param>
        /// <returns>A tuple in which the first item is the row index, and the 
        /// second one is the column index</returns>
        private (int,int) GetIndex(Tile tile)
        {
            for (int r = 0; r < tileGrid.Length - 1; r++)
            {
                for (int c = 0; c < tileGrid[r].Length; c++)
                {
                    if(tileGrid[r][c] == tile) { return(r,c); }
                }
            }
            return (-1, -1); //realistically should never happen
        }

        public void Update()
        {
            KeyboardState kb = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();
            bool change = false;
            foreach(Tile tile in tileGrid.SelectMany(i => i)) 
            { 
                tile.CheckClick(out change);
                if (change) { Reset(); }
            }

            if(kb.IsKeyDown(Keys.Space) && !Busy && 
                Tile.StartTile is not null && Tile.EndTile is not null)
            {
                ShortestPath();
                Stack<Tile> tempPath = new Stack<Tile>();

                bool done = false; 
                tempPath.Push(Tile.EndTile); Tile current = Tile.EndTile;
                while(!done) 
                {
                    if(current.PathNeighbor is null) { done = true; }
                    else 
                    { 
                        tempPath.Push(current);
                        current = current.PathNeighbor;
                    }
                }
                Path.AddRange(tempPath.ToArray());
            }
        }

        /// <summary>
        /// Applies Dijsktra's algorithm to find the shortest path between StartTile and EndTile 
        /// (though only travels in the cardinal directions)
        /// </summary>
        public void ShortestPath()
        {
            Busy = true; Reset();
            Tile current = Tile.StartTile; current.Permanent = true; current.TotalDistance = 0;
            while(!Complete())
            {
                foreach(Tile neighbor in GetNeighbors(current))
                {
                    if(!neighbor.Permanent && !neighbor.IsWall)
                    {
                        if(current.TotalDistance + 1 < neighbor.TotalDistance)
                        {
                            neighbor.TotalDistance = current.TotalDistance + 1;
                            neighbor.PathNeighbor = current;
                        }
                    }
                }

                Tile leastNonPerm = null;
                for(int r = 0; r < tileGrid.Length; r++)
                {
                    for(int c = 0; c < tileGrid[r].Length; c++)
                    {
                        if(!tileGrid[r][c].Permanent && !tileGrid[r][c].IsWall)
                        {
                            if(leastNonPerm is not null)
                            {
                                if(leastNonPerm.TotalDistance > tileGrid[r][c].TotalDistance) { leastNonPerm = tileGrid[r][c]; }
                            }
                            else { leastNonPerm = tileGrid[r][c]; }
                        }
                    }
                }
                current = leastNonPerm; current.Permanent = true;
            }
            Busy = false;
        }

        /// <summary>
        /// Resets all tile Dijsktra data to their default values.
        /// </summary>
        private void Reset()
        {
            Path = new List<Tile>();
            foreach(Tile tile in tileGrid.SelectMany(i => i))
            {
                tile.PathNeighbor = null;
                tile.TotalDistance = int.MaxValue;
                tile.Permanent = false;
            }
        }

        /// <summary>
        /// Checks for whether the Algorithm is "Done", of which it is done if all Permanent tiles 
        /// on the grid no longer have non-Permanent non-Wall neighbors.
        /// </summary>
        /// <returns>True if the algorithm is 'done', false if otherwise</returns>
        private bool Complete()
        {
            if(Tile.EndTile.Permanent) { return true; }
            for(int r = 0; r < tileGrid.Length - 1; r++)
            {
                for(int c = 0; c < tileGrid[r].Length - 1; c++)
                {
                    Tile tile = tileGrid[r][c];
                    if(tile.Permanent)
                    {
                        foreach(Tile neighbor in GetNeighbors(r,c))
                        {
                            if(!neighbor.IsWall && !neighbor.Permanent) { return false; }
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Calls the draw method on all tiles it contains
        /// </summary>
        public void Draw()
        {
            foreach(Tile tile in tileGrid.SelectMany(i => i))
            {
                if(Path is not null) { tile.Draw(this); }
                else { tile.Draw(); }
            }
        }
    }
}
