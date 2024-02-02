//Author: Sophia Lin
//File Name: ObjectQueue.cs
//Project Name: PASS3
//Creation Date: December 18, 2023
//Modified Date: December 18, 2023
//Description: Handle object queue
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
    class ObjectQueue
    {
        //Maintain the collection of Items
        List<Snowball> queue = new List<Snowball>();

        /// <summary>
        /// Create an instance of the object queue
        /// </summary>
        public ObjectQueue()
        {
        }

        /// <summary>
        /// Add newObject to the back of the queue
        /// </summary>
        /// <param name="newObject">Snowball to be added to queue</param>
        public void Enqueue(Snowball newObject)
        {
            //If size were an issue here (when using an array, or a desired max is set it would need to be verified before adding)
            queue.Add(newObject);
        }

        /// <summary>
        /// Returns and removes the snowball at the front of the queue, null if none exists
        /// </summary>
        /// <returns>Snowball at front of queue</returns>
        public Snowball Dequeue()
        {
            //Maintain the front of the queue for return
            Snowball result = null;

            //Only remove an Item if possible
            if (queue.Count > 0)
            {
                result = queue[0];
                queue.RemoveAt(0);
            }

            return result;
        }

        /// <summary>
        /// Returns the snowball at the front of the queue, null if non exists
        /// </summary>
        /// <returns>Snowball at front of queue</returns>
        public Snowball Peek()
        {
            //Maintain the front of the queue for return
            Snowball result = null;

            //Only retrieve the snowball if possible
            if (queue.Count > 0)
            {
                result = queue[0];
            }

            return result;
        }

        /// <summary>
        /// Returns the current number of snowballs in the queue
        /// </summary>
        /// <returns>Size of the queue</returns>
        public int Size()
        {
            return queue.Count;
        }

        /// <summary>
        /// Compare the size of the queue against 0 to determine its empty status
        /// </summary>
        /// <returns>True if the size of the queue is 0, false otherwise</returns>
        public bool IsEmpty()
        {
            return queue.Count == 0;
        }
    }
}
