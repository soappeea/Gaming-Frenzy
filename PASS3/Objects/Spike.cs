//Author: Sophia Lin
//File Name: Enemy.cs
//Project Name: PASS3
//Creation Date: January 15, 2024
//Modified Date: January 15, 2024
//Description: Handle spike
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
    class Spike : GameObject
    {
        /// <summary>
        /// Create instance of spike
        /// </summary>
        /// <param name="img">Spike image</param>
        /// <param name="pos">Spike position</param>
        /// <param name="scale">Scale of spike image</param>
        /// <param name="reboundScaler">Scale of spike rebound</param>
        public Spike(Texture2D img, Vector2 pos, float scale, float reboundScaler) : base(img, pos, scale, reboundScaler)
        {
        }
    }
}
