//Author: Sophia Lin
//File Name: GameLogo.cs
//Project Name: PASS3
//Creation Date: December 11, 2023
//Modified Date: December 11, 2023
//Description: Handle mini game logo object
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
    class GameLogo
    {
        //Store the object image
        private Texture2D img;
        private int imgWidth;
        private int imgHeight;
        private Rectangle rec;

        //Store status of whether game is activated or not
        private bool statusGame;

        //Store gamestate of minigame logo
        private int gameState;

        /// <summary>
        /// Create an instance of game logo
        /// </summary>
        /// <param name="img">Game logo image</param>
        /// <param name="pos">Position</param>
        /// <param name="scale">Scale for game logo image</param>
        /// <param name="gameState">Gamestate of logo</param>
        public GameLogo(Texture2D img, Vector2 pos, float scale, int gameState)
        {
            //Set image information
            this.img = img;
            this.imgWidth = (int)(img.Width * scale);
            this.imgHeight = (int)(img.Height * scale);
            this.rec = new Rectangle((int)pos.X, (int)pos.Y, imgWidth, imgHeight);

            //Set status to not active
            statusGame = false;

            //Set minigame state of logo
            this.gameState = gameState;
        }

        /// <summary>
        /// Retrieve rectangle of minigame logo
        /// </summary>
        /// <returns>Minigame logo rectangle</returns>
        public Rectangle GetRectangle()
        {
            return rec;
        }

        /// <summary>
        /// Retrieve status of minigame logo
        /// </summary>
        /// <returns>Activity of minigame logo</returns>
        public bool GetStatus()
        {
            return statusGame;
        }

        /// <summary>
        /// Retrieve minigame state of logo
        /// </summary>
        /// <returns>Identifier of minigame state</returns>
        public int GetMinigameState()
        {
            return gameState;
        }

        /// <summary>
        /// Set status of minigame logo
        /// </summary>
        /// <param name="status">Activity of minigame logo</param>
        public void SetStatus(bool status)
        {
            statusGame = status;
        }

        /// <summary>
        /// Display minigame logo
        /// </summary>
        /// <param name="spriteBatch">Used for drawing sprites</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            //Display minigame logo
            spriteBatch.Draw(img, rec, Color.White);
        }
    }
}
