//Author: Sophia Lin
//File Name: MovingPlatform.cs
//Project Name: PASS3
//Creation Date: December 10, 2023
//Modified Date: December 10, 2023
//Description: Handle Moving Platforms
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
    class MovingPlatform : GameObject
    {
        //Store directions
        private const int POS = 1;
        private const int NEG = -1;

        //Store current direction platform is moving in
        private int dir;

        //Store bounds for the platform to move within
        private const int LEFT_BOUND = 1300;
        private const int RIGHT_BOUND = 1583;

        //Store platform speed
        private float platformSpeed;

        //Store final speed that platform will move at
        private float finalSpeed;

        public MovingPlatform(Texture2D platformImg, Vector2 pos, float scale, float reboundScaler, float platformSpeed, int dir) : base(platformImg, pos, scale, reboundScaler)
        {
            //Set image
            this.img = platformImg;

            //Set speed
            this.platformSpeed = platformSpeed;

            //Set initial direction
            this.dir = dir;
        }

        /// <summary>
        /// Retrieve the speed that the platform is moving at
        /// </summary>
        /// <returns>Final speed of platform</returns>
        public float GetSpeed()
        {
            return finalSpeed;
        }

        /// <summary>
        /// Update the moving platform
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime, List<GameObject> objects)
        {
            //Alter direction based on bounds the platform crosses
            if (pos.X <= LEFT_BOUND)
            {
                dir = POS;
            }
            else if (pos.X >= RIGHT_BOUND)
            {
                dir = NEG;
            }

            //Calculate final speed
            finalSpeed = dir * platformSpeed;

            //Update position and rectangle of platform
            pos.X += finalSpeed;
            rec.X = (int)pos.X;
        }
    }
}
