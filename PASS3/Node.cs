//Author: Sophia Lin
//File Name: Node.cs
//Project Name: PASS3
//Creation Date: December 26, 2023
//Modified Date: December 26, 2023
//Description: A data class to represent each tile in a tile based world
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using GameUtility;

namespace PASS3
{
    class Node
    {
        //Path costs to get from the start to this tile (g) and from this tile to the target (h)
        private float f = 0f;
        private float g = 0f;
        private float h = 0f;

        //Store the Tile that led to this tile so it can be backtracked later to create the path
        private Node parent = null;

        //Tile specific data
        private int row;
        private int col;
        private int id;
        private int tileType;

        //Maintain a list of all valid tiles touching this tile that can be walked to
        private List<Node> adjacent = new List<Node>();

        //Tile graphical data
        private Rectangle rec;
        private Color colour;

        /// <summary>
        /// Create a new instance of node
        /// </summary>
        /// <param name="row">The node's row in the tile world</param>
        /// <param name="col">The node's column in the tile world</param>
        /// <param name="tileType">The node's tiletype</param>
        /// <param name="colour">The node tile image's colour</param>
        /// <param name="mapSize">The dimensions of the map</param>
        public Node(int row, int col, int tileType, Color colour, Vector2 mapSize)
        {
            //Store the grid location of the tile by row and column
            this.row = row;
            this.col = col;

            //Calculate the tiles numerical order (left to right, top to bottom) in the grid
            this.id = row * (int)mapSize.Y + col;

            //Store the tile's graphical data
            this.tileType = tileType;
            this.colour = colour;

            //Create a rectangle representing the tile's coordinates and size
            rec = new Rectangle(col * Game1.TILE_SIZE, row * Game1.TILE_SIZE, Game1.TILE_SIZE, Game1.TILE_SIZE);
        }

        /// <summary>
        /// Retrieve the node row
        /// </summary>
        /// <returns>Node row in tile world</returns>
        public int GetRow()
        {
            return row;
        }

        /// <summary>
        /// Retrieve the node column
        /// </summary>
        /// <returns>Node column in tile world</returns>
        public int GetCol()
        {
            return col;
        }

        /// <summary>
        /// Retrieve tile type
        /// </summary>
        /// <returns>Node's tile type</returns>
        public int GetTileType()
        {
            return tileType;
        }

        /// <summary>
        /// Retrieve node image colour
        /// </summary>
        /// <returns>Node image colour</returns>
        public Color GetColor()
        {
            return colour;
        }

        /// <summary>
        /// Retrieve the node rectangle
        /// </summary>
        /// <returns>Node rectangle</returns>
        public Rectangle GetRectangle()
        {
            return rec;
        }

        /// <summary>
        /// Retrieve adjacents to the node
        /// </summary>
        /// <returns>List of adjacent nodes to current node</returns>
        public List<Node> GetAdjacent()
        {
            return adjacent;
        }

        /// <summary>
        /// Retrieve G value
        /// </summary>
        /// <returns>G value</returns>
        public float GetGVal()
        {
            return g;
        }

        /// <summary>
        /// Retrieve F value
        /// </summary>
        /// <returns>F value</returns>
        public float GetFVal()
        {
            return f;
        }

        /// <summary>
        /// Retrieve H value
        /// </summary>
        /// <returns>H value</returns>
        public float GetHVal()
        {
            return h;
        }

        /// <summary>
        /// Retrieve the parent
        /// </summary>
        /// <returns>Parent of node</returns>
        public Node GetParent()
        {
            return parent;
        }

        /// <summary>
        /// Retrieve id of node
        /// </summary>
        /// <returns>Id of node</returns>
        public int GetId()
        {
            return id;
        }

        /// <summary>
        /// Set G value
        /// </summary>
        /// <param name="val">G value</param>
        public void SetGVal(float val)
        {
            g = val;
        }

        /// <summary>
        /// Set F value
        /// </summary>
        /// <param name="val">F value</param>
        public void SetFVal(float val)
        {
            f = val;
        }

        /// <summary>
        /// Set H value
        /// </summary>
        /// <param name="val">H value</param>
        public void SetHVal(float val)
        {
            h = val;
        }

        /// <summary>
        /// Set parent of node
        /// </summary>
        /// <param name="node">Parent node</param>
        public void SetParent(Node node)
        {
            parent = node;
        }

        /// <summary>
        /// Determine and store the tile's adjacent tiles
        /// </summary>
        /// <param name="map">The collection of all the tiles in the game world</param>
        public void SetAdjacencies(Node[,] map)
        {
            //Only add walkable terrain
            for (int curRow = row - 1; curRow <= row + 1; curRow++)
            {
                for (int curCol = col - 1; curCol <= col + 1; curCol++)
                {
                    //Do not add itself
                    if (row != curRow || col != curCol)
                    {
                        //Add only Nodes at valid row and columns that it walkable terrain
                        if (curRow >= 0 && curRow < Game1.NUM_ROWS &&        //Within bounds vertically
                            curCol >= 0 && curCol < Game1.NUM_COLS &&        //Within bound horizontally
                            map[curRow, curCol].tileType != Game1.WALL)      //Valid terrain type
                        {
                            adjacent.Add(map[curRow, curCol]);
                        }
                    }
                }
            }
        }
    }
}
