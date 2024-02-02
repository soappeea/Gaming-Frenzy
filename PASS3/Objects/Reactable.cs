//Author: Sophia Lin
//File Name: Reactable.cs
//Project Name: PASS3
//Creation Date: January 9, 2024
//Modified Date: January 9, 2024
//Description: Handle reactables
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
    class Reactable : GameObject
    {
        //Store status of whether the reactable is collected
        private bool isCollected;

        //Store identification of reactable
        private int id;

        /// <summary>
        /// Create new instance of reactable
        /// </summary>
        /// <param name="img">Reactable image</param>
        /// <param name="pos">Position of the reactable</param>
        /// <param name="scale">Scale of the reactable image</param>
        /// <param name="reboundScaler">Scale of how much the reactable rebounds other objects</param>
        /// <param name="id">Identification of the reactable</param>
        public Reactable(Texture2D img, Vector2 pos, float scale, float reboundScaler, int id) : base(img, pos, scale, reboundScaler)
        {
            //Set identification
            this.id = id;
        }

        /// <summary>
        /// Retrieve identification
        /// </summary>
        /// <returns>Identification of reactable</returns>
        public int GetId()
        {
            return id;
        }

        /// <summary>
        /// Retrieve status of collectable
        /// </summary>
        /// <returns>If reactable is collected</returns>
        public bool GetStatus()
        {
            return isCollected;
        }

        /// <summary>
        /// Set status of reactable
        /// </summary>
        /// <param name="status">If reactable is collected</param>
        public void SetStatus(bool status)
        {
            isCollected = status;
        }
    }
}
