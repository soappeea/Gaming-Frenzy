//Author: Sophia Lin
//File Name: GameObject.cs
//Project Name: PASS3
//Creation Date: December 2, 2023
//Modified Date: December 15, 2023
//Description: Handle game objects
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
    class GameObject
    {
        //Store the object image details
        protected Texture2D img;
        protected float scale = 1f;
        protected int imgWidth;
        protected int imgHeight;
        protected Rectangle rec;

        //Store the object's position
        protected Vector2 pos;

        //Store the rebound scaler
        protected float reboundScaler;

        //Store the rebound rate 
        public const float PLATFORM_REBOUND = -0.8f;
        public const float SKEWER_REBOUND = -0.9f;
        public const float SNOWBALL_REBOUND = -0.15f;

        //Store a general value representing nothing
        protected const int NONE = 0;

        /// <summary>
        /// Create an instance of game object
        /// </summary>
        /// <param name="img">Game object image</param>
        /// <param name="pos">Position of game object</param>
        /// <param name="scale">Scale for the game object</param>
        /// <param name="reboundScaler">Scale of how much the object rebounds</param>
        public GameObject(Texture2D img, Vector2 pos, float scale, float reboundScaler)
        {
            //Set image details
            this.img = img;
            if (img != null)
            {
                this.imgWidth = (int)(img.Width * scale);
                this.imgHeight = (int)(img.Height * scale);

                this.rec = new Rectangle((int)pos.X, (int)pos.Y, imgWidth, imgHeight);
            }

            //Set position
            this.pos = pos;

            //Set rebound scale
            this.reboundScaler = reboundScaler;
        }

        /// <summary>
        /// Retrieve whether the object is active or not
        /// </summary>
        /// <returns>Activity of object</returns>
        public virtual bool GetActive()
        {
            return false;
        } 

        /// <summary>
        /// Retrieve rebound scale
        /// </summary>
        /// <returns>Rebound scale</returns>
        public float GetReboundScaler()
        {
            return reboundScaler;
        }

        /// <summary>
        /// Retrieve rectangle of entire game object
        /// </summary>
        /// <returns>Entire rectangle</returns>
        public virtual Rectangle GetRectangle()
        {
            return rec;
        }

        /// <summary>
        /// Set the image height
        /// </summary>
        /// <param name="height">Height of image</param>
        public void SetImgHeight(int height)
        {
            //Set image height and update rectangle
            this.imgHeight = height;
            this.rec = new Rectangle((int)pos.X, (int)pos.Y, imgWidth, imgHeight);
        }

        /// <summary> 
        /// Update the game objects
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        /// <param name="objects">Other objects in the game</param>
        public virtual void Update(GameTime gameTime, List<GameObject> objects)
        {

        }

        /// <summary>
        /// Draw the object
        /// </summary>
        /// <param name="spriteBatch">Used for drawing sprites</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            //Draw the object
            spriteBatch.Draw(img, rec, Color.White);
        }

        /// <summary>
        /// Activate timer
        /// </summary>
        public virtual void ActivateTimer()
        {
        }
    }
}
