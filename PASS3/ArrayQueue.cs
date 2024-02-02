//Author: Sophia Lin
//File Name: ArrayQueue.cs
//Project Name: PASS3
//Creation Date: December 15, 2023
//Modified Date: December 16, 2023
//Description: Handle queue operations
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
    class ArrayQueue
    {
        //A public constant to used to check if a returned value is a bad result
        public const int NO_ELEMENT = Int32.MinValue;

        //Maintain the only two attributes of a queue, itself and its size
        private int[] queue;
        private int size;

        /// <summary>
        /// Create an instance of the array queue
        /// </summary>
        /// <param name="maxSize">Maximum size the queue can be</param>
        public ArrayQueue(int maxSize)
        {
            //Set initial size to 0 and initialize maximum size
            size = 0;
            queue = new int[maxSize];
        }

        /// <summary>
        /// Add num to back of queue
        /// </summary>
        /// <param name="num">Integer to be added</param>
        public void Enqueue(int num)
        {
            //Add to queue if the size is not at the maximum yet
            if (size < queue.Length)
            {
                //Add the new item to the end of the queue
                queue[size] = num;
                size++;
            }
        }

        /// <summary>
        /// Remove integer from front of queue
        /// </summary>
        /// <returns>Integer being removed from front of queue</returns>
        public int Dequeue()
        {
            //Assume the queue is empty
            int result = NO_ELEMENT;

            //Remove integer queue if it is not empty
            if (!IsEmpty())
            {
                //Returning the front of the queue
                result = queue[0];

                //Loop and move all items one element forward
                for (int i = 1; i < size; i++)
                {
                    queue[i - 1] = queue[i];
                }

                //Item has been removed, reduce size
                size--;
            }

            return result;
        }

        /// <summary>
        /// Retrieve integer at front of queue
        /// </summary>
        /// <returns>Integer at front of queue</returns>
        public int Peek()
        {
            //Assume the queue is empty
            int result = NO_ELEMENT;

            //Find the front of the queue if it is not empty
            if (!IsEmpty())
            {
                //Returning the front of the queue
                result = queue[0];
            }

            return result;
        }

        /// <summary>
        /// Retrieve size of queue
        /// </summary>
        /// <returns>Size of queue</returns>
        public int Size()
        {
            return size;
        }

        /// <summary>
        /// Compare the size of the queue against 0 to determine its empty status
        /// </summary>
        /// <returns>True if the size of the queue is 0, false otherwise</returns>
        public bool IsEmpty()
        {
            return size == 0;
        }
    }
}
