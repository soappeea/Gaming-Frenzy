//Author: Sophia Lin
//File Name: GameObject.cs
//Project Name: PASS3
//Creation Date: December 2, 2023
//Modified Date: December 9, 2023
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
        //Store the object image
        protected Texture2D img;
        protected float scale = 1f;
        protected int imgWidth;
        protected int imgHeight;
        protected Rectangle rec;

        //Store the object's details
        protected Vector2 pos;
        protected bool envCollisions;
        protected float reboundScaler;

        public GameObject(Texture2D img, Vector2 pos, float scale, bool envCollisions, float reboundScaler)
        {
            this.img = img;
            this.pos = pos;

            this.imgWidth = (int)(img.Width * scale);
            this.imgHeight = (int)(img.Height * scale);

            this.rec = new Rectangle((int)pos.X, (int)pos.Y, imgWidth, imgHeight);

            this.envCollisions = envCollisions;
            this.reboundScaler = reboundScaler;
        }

        //Handle collision detection here

        //;override collision in checkpoint
        public float GetReboundScaler()
        {
            return reboundScaler;
        }

        public virtual Rectangle GetRectangle()
        {
            return rec;
        }

        /// <summary>
        /// Draw the platform
        /// </summary>
        /// <param name="spriteBatch">Used for drawing sprites</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            //Draw each of the blocks that make up the whole platform
            spriteBatch.Draw(img, rec, Color.White);
        }
    }
}
