//Author: Sophia Lin
//File Name: Skewer.cs
//Project Name: PASS3
//Creation Date: December 12, 2023
//Modified Date: December 15, 2023
//Description: Handle skewers
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
    class Skewer : GameObject
    {
        //Store identification of side of skewer
        private const int RIGHT = 0;
        private const int LEFT = 1;
        private const int TOP = 2;
        private const int BOTTOM = 3;

        //Store current side of skewer
        private int side;

        //Store bounds of skewers
        public const int LEFT_BOUND = 204;
        public const int RIGHT_BOUND = 1496;
        public const int TOP_BOUND = 192;
        public const int BOTTOM_BOUND = 708;

        //Store skewer functionality times
        private const int SKEWER_ACTIVATE_TIME = 2000;
        private const int SKEWER_EXTRACT_TIME = 3000;
        private const int SKEWER_RETRACT_TIME = 2000;
        private const int SKEWER_RESET_TIME = 1500;

        //Store skewer functionality timers
        private Timer skewerActivateTimer;
        private Timer skewerExtractTimer;
        private Timer skewerRetractTimer;
        private Timer skewerResetTimer;

        //Store actual rectangle of skewer
        private Rectangle actualRec;

        //Store border image of skewer
        private Texture2D borderImg;

        //Store status of skewer
        private bool isActive;
        private bool isGrowing;

        /// <summary>
        /// Create instance of skewer
        /// </summary>
        /// <param name="skewerImg">Skewer image</param>
        /// <param name="pos">Skewer position</param>
        /// <param name="scale">Scale of skewer image</param>
        /// <param name="reboundScaler">Rebound scale of skewer</param>
        /// <param name="side">Side the skewer is on</param>
        /// <param name="borderImg">Border image for skewer</param>
        public Skewer(Texture2D skewerImg, Vector2 pos, float scale, float reboundScaler, int side, Texture2D borderImg) : base(skewerImg, pos, scale, reboundScaler)
        {
            //Set skewer functionality timers
            skewerActivateTimer = new Timer(SKEWER_ACTIVATE_TIME, false);
            skewerExtractTimer = new Timer(SKEWER_EXTRACT_TIME, false);
            skewerRetractTimer = new Timer(SKEWER_RETRACT_TIME, false);
            skewerResetTimer = new Timer(SKEWER_RESET_TIME, false);

            //Set side of skewer
            this.side = side;

            //Set border image of skewer
            this.borderImg = borderImg;

            //Store actual rectangle of skewer
            actualRec = rec;

            //Store actual dimensions of skewer rectangle
            if (side == TOP)
            {
                actualRec.Y = -227;
            }
            else if (side == LEFT)
            {
                actualRec.X = -70;
            }

            //Set status of skewer
            isActive = false;
            isGrowing = true;
        }

        /// <summary>
        /// Retrieve whether skewer is active or not
        /// </summary>
        /// <returns>Status of skewer</returns>
        public override bool GetActive()
        {
            if (isActive)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Retrieve rectangle of skewer
        /// </summary>
        /// <returns>Skewer rectangle</returns>
        public override Rectangle GetRectangle()
        {
            //Return the current rectangle if active
            if (isActive)
            {
                return rec;
            }

            //If the skewer is inactive, return the actual rectangle due to position reasons
            return actualRec;
        }

        /// <summary>
        /// Update the Skewer
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="objects">Other objects in the game</param>
        public override void Update(GameTime gameTime, List<GameObject> objects)
        {
            //Update timers
            skewerActivateTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            skewerExtractTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            skewerRetractTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            skewerResetTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //Perform operations on skewer if it is active
            if (isActive)
            {
                //Detect object collision
                ObjectCollisionDetection(objects);

                //Handle action of skewing
                ProcessSkewing();
            }
        }

        /// <summary>
        /// Display the skewers
        /// </summary>
        /// <param name="spriteBatch">Used for drawing sprites</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //Draw skewer, rectangle varies based on whether it is active and the side it is on
            if (!isActive && (side == TOP || side == LEFT))
            {
                spriteBatch.Draw(img, actualRec, Color.SaddleBrown);
                spriteBatch.Draw(borderImg, actualRec, Color.White);
            }
            else
            {
                spriteBatch.Draw(img, rec, Color.SaddleBrown);
                spriteBatch.Draw(borderImg, rec, Color.White);
            }
        }

        /// <summary>
        /// Activate skewer game timer
        /// </summary>
        public override void ActivateTimer()
        {
            //Start timer
            skewerActivateTimer.ResetTimer(true);

            //Set to active
            isActive = true;

            //Set to growing
            isGrowing = true;
        }

        /// <summary>
        /// Detect object collision
        /// </summary>
        /// <param name="objects">Other objects in game</param>
        private void ObjectCollisionDetection(List<GameObject> objects)
        {
            //Detect collision between skewer and other objects
            for (int i = 0; i < objects.Count; i++)
            {
                //Ignore if object detected is another skewer
                if (objects[i] == this)
                {
                    continue;
                }

                //Prevent further growth if skewer collides with another object
                if (rec.Intersects(objects[i].GetRectangle()))
                {
                    isGrowing = false;
                }
            }
        }

        /// <summary>
        /// Handle the skewing of the skewers
        /// </summary>
        private void ProcessSkewing()
        {
            //Store benchmarks
            int activateWidth = 102;
            int activateHeight = 96;
            int travelWidth = 1496;
            int travelHeight = 708;

            //Activate skewer
            if (skewerActivateTimer.IsActive())
            {
                //Move the skewer
                TranslateSkewer(activateWidth, activateHeight, false, SKEWER_ACTIVATE_TIME, skewerActivateTimer);
            }

            //Extend skewer
            if (skewerActivateTimer.IsFinished())
            {
                //Start extract timer
                skewerExtractTimer.Activate();

                //Move the skewer if it is growing
                if (isGrowing)
                {
                    //Move the skewer 
                    TranslateSkewer(travelWidth, travelHeight, true, SKEWER_EXTRACT_TIME, skewerExtractTimer);
                }

                //Set the new travel width to the current rectangle's dimensions
                travelWidth = rec.Width;
                travelHeight = rec.Height;
            }

            //Retract skewer
            if (skewerExtractTimer.IsFinished())
            {
                //Start retract timer
                skewerRetractTimer.Activate();

                //Modify travel height if the skewer is on the bottom
                if (side == BOTTOM)
                {
                    travelHeight = Game1.screenHeight - rec.Y;
                }

                //Move the skewer
                TranslateSkewer(travelWidth, travelHeight, false, SKEWER_RETRACT_TIME, skewerRetractTimer);
            }

            //Reset skewer
            if (skewerRetractTimer.IsFinished())
            {
                //Start reset timer
                skewerResetTimer.Activate();

                //Move skewer
                TranslateSkewer(activateWidth, activateHeight, true, SKEWER_RESET_TIME, skewerResetTimer);
            }

            //Deactivate off skewer
            if (skewerResetTimer.IsFinished())
            {
                //Set statuses to inactive
                isActive = false;
                isGrowing = false;
            }
        }

        /// <summary>
        /// Translate the skewer on the screen
        /// </summary>
        /// <param name="travelWidth">Width skewer must travel</param>
        /// <param name="travelHeight">Height skewer must travel</param>
        /// <param name="isGrowth">Status of skewer growth</param>
        /// <param name="timeVal">Amount of time for skewer to grow/shrink</param>
        /// <param name="currentTimer">Time passed during action</param>
        private void TranslateSkewer(int travelWidth, int travelHeight, bool isGrowth, int timeVal, Timer currentTimer)
        {
            //Store reset factor
            int factor = 0;

            //If skewers are growing or shrinking, then perform respective actions to them
            if (isGrowth)
            {
                //Extend skewers in direction based on side it is on
                switch (side)
                {
                    case RIGHT:
                        factor = (int)(travelWidth * (currentTimer.GetTimePassed() / timeVal));
                        rec.Width = factor;
                        rec.X = Game1.screenWidth - factor;
                        break;
                    case LEFT:
                        factor = (int)(travelWidth * (currentTimer.GetTimePassed() / timeVal));
                        rec.Width = factor;
                        break;
                    case TOP:
                        factor = (int)(travelHeight * (currentTimer.GetTimePassed() / timeVal));
                        rec.Height = factor;
                        break;
                    case BOTTOM:
                        factor = (int)(travelHeight * (currentTimer.GetTimePassed() / timeVal));
                        rec.Height = actualRec.Height + factor;
                        rec.Y = Game1.screenHeight - factor;
                        break;
                }
            }
            else
            {
                //Shrink skewers in direction based on side it is on
                switch (side)
                {
                    case RIGHT:
                        factor = (int)(travelWidth * (currentTimer.GetTimeRemaining() / timeVal));
                        rec.Width = factor;
                        rec.X = Game1.screenWidth - factor;
                        break;
                    case LEFT:
                        factor = (int)(travelWidth * (currentTimer.GetTimeRemaining() / timeVal));
                        rec.Width = factor;
                        break;
                    case TOP:
                        factor = (int)(travelHeight * (currentTimer.GetTimeRemaining() / timeVal));
                        rec.Height = factor;
                        break;
                    case BOTTOM:
                        factor = (int)(travelHeight * (currentTimer.GetTimeRemaining() / timeVal));
                        rec.Height = factor;
                        rec.Y = Game1.screenHeight - factor;
                        break;
                }
            }
        }
    }
}
