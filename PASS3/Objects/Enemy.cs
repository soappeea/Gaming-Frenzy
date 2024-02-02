//Author: Sophia Lin
//File Name: Enemy.cs
//Project Name: PASS3
//Creation Date: December 28, 2023
//Modified Date: December 28, 2023
//Description: Handle enemy
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
    class Enemy : GameObject
    {
        //Define directions
        protected const int POS = 1;
        protected const int NEG = -1;

        //Direction enemy is travelling in
        protected int dir = POS;

        //Tracking enemy states  
        protected const int IDLE = 0;
        protected const int RUN = 1; //rightwards
        protected const int ATTACK = 2;

        //Track the current enemy state 
        protected int enemyState = IDLE;

        //Store enemy animations
        protected Animation[] enemyAnims;

        public const int HORIZ = 0;
        public const int VERT = 1;

        ////define body parts to be used for collision rectangle array indices
        private const int FEET = 0;
        private const int HEAD = 1;
        private const int LEFT = 2;
        private const int RIGHT = 3;

        //Track the 4 inner rectangles of the enemy for collision detection
        private Rectangle[] enemyRecs = new Rectangle[4];

        //Store the movement speed of the enemy
        private float enemySpeed = 2f;

        //Store increment value
        protected const int ADD = 1;

        /// <summary>
        /// Create an instance of enemy
        /// </summary>
        /// <param name="img">Enemy image</param>
        /// <param name="pos">Position of enemy</param>
        /// <param name="scale">Scale for enemy image</param>
        /// <param name="reboundScaler">Rebound scale of enemy</param>
        public Enemy(Texture2D img, Vector2 pos, float scale, float reboundScaler) : base(img, pos, scale, reboundScaler)
        {
        }

        /// <summary>
        /// Retrieve enemy position
        /// </summary>
        /// <returns>Current position</returns>
        public Vector2 GetPos()
        {
            return pos;
        }

        /// <summary>
        /// Retrieve enemy speed
        /// </summary>
        /// <returns>Current speed</returns>
        public float GetSpeed()
        {
            return enemySpeed;
        }

        /// <summary>
        /// Retrieve enemy rectangle
        /// </summary>
        /// <returns>Entire enemy rectangle</returns>
        public override Rectangle GetRectangle()
        {
            return enemyAnims[enemyState].GetDestRec();
        }

        /// <summary>
        /// Retrieve enemy's individual rectangles
        /// </summary>
        /// <returns>Array of all individual rectangles</returns>
        public Rectangle[] GetRectangles()
        {
            return enemyRecs;
        }

        /// <summary>
        /// Set enemy position
        /// </summary>
        /// <param name="moveAmount">Amount enemy must move by</param>
        public void SetPos(Vector2 moveAmount)
        {
            //Change position
            pos += moveAmount;
        }

        /// <summary>
        /// Set enemy destination rectangle
        /// </summary>
        public void SetDestRec()
        {
            enemyAnims[enemyState].TranslateTo((int)pos.X, (int)pos.Y);
        }

        /// <summary>
        /// Set individual enemy rectangles
        /// </summary>
        /// <param name="enemyRecs">Array of enemy individual rectangles</param>
        /// <param name="enemyAnims">Array of enemy animations</param>
        protected virtual void SetEnemyRecs(Rectangle[] enemyRecs, Animation[] enemyAnims)
        {
            //Define player Collision Recs based on its position and scaled size
            enemyRecs[HEAD] = new Rectangle(enemyAnims[enemyState].GetDestRec().X + (int)(0.25f * enemyAnims[enemyState].GetDestRec().Width), enemyAnims[enemyState].GetDestRec().Y,
                                             (int)(enemyAnims[enemyState].GetDestRec().Width * 0.6f), (int)(enemyAnims[enemyState].GetDestRec().Height * 0.35f));
            enemyRecs[LEFT] = new Rectangle(enemyAnims[enemyState].GetDestRec().X, enemyRecs[HEAD].Y + enemyRecs[HEAD].Height,
                                             (int)(enemyAnims[enemyState].GetDestRec().Width * 0.45f), (int)(enemyAnims[enemyState].GetDestRec().Height * 0.55f));
            enemyRecs[RIGHT] = new Rectangle(enemyRecs[LEFT].X + enemyRecs[LEFT].Width + ADD, enemyRecs[HEAD].Y + enemyRecs[HEAD].Height,
                                             (int)(enemyAnims[enemyState].GetDestRec().Width * 0.5f), (int)(enemyAnims[enemyState].GetDestRec().Height * 0.55f));
            enemyRecs[FEET] = new Rectangle(enemyAnims[enemyState].GetDestRec().X + (int)(0.25f * enemyAnims[enemyState].GetDestRec().Width), enemyRecs[LEFT].Y + enemyRecs[LEFT].Height,
                                             (int)(enemyAnims[enemyState].GetDestRec().Width * 0.6f), (int)(enemyAnims[enemyState].GetDestRec().Height * 0.1f));
        }

        /// <summary>
        /// Update enemy
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        /// <param name="objects">Objects enemy might interact with</param>
        public override void Update(GameTime gameTime, List<GameObject> objects)
        {
            //Update player animation based on enemyState
            enemyAnims[enemyState].Update(gameTime);

            //Set enemy destination rectangle
            SetDestRec();

            //Set enemy individual rectangles
            SetEnemyRecs(enemyRecs, enemyAnims);

            //Detect for collision between objects
            CollisionDetection(objects);
        }

        /// <summary>
        /// Display enemy
        /// </summary>
        /// <param name="spriteBatch">Used for drawing sprites</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //Display the player based on the direction it is facing
            if (dir == POS)
            {
                //If the player is facing right, do not flip the animation
                enemyAnims[enemyState].Draw(spriteBatch, Color.White, Animation.FLIP_NONE);
            }
            else if (dir == NEG)
            {
                //If the player is facing left, flip the animation
                enemyAnims[enemyState].Draw(spriteBatch, Color.White, Animation.FLIP_HORIZONTAL);
            }
        }

        /// <summary>
        /// Detect collisions between other objects
        /// </summary>
        /// <param name="objects">Game objects</param>
        /// <returns></returns>
        public bool CollisionDetection(List<GameObject> objects)
        {
            bool collision = false;

            //Test collision between the enemy and every game object
            for (int i = 0; i < objects.Count; i++)
            {
                //Check for a bounding box collision first
                if (enemyAnims[enemyState].GetDestRec().Intersects(objects[i].GetRectangle()))
                {
                    //Shift the player to just outside of the collision location depending on body part
                    if (enemyRecs[FEET].Intersects(objects[i].GetRectangle()))
                    {
                        enemyAnims[enemyState].TranslateTo(enemyAnims[enemyState].GetDestRec().X, objects[i].GetRectangle().Y - enemyAnims[enemyState].GetDestRec().Height);
                        pos.Y = enemyAnims[enemyState].GetDestRec().Y;
                        collision = true;
                        return collision;
                    }
                    else if (enemyRecs[LEFT].Intersects(objects[i].GetRectangle()))
                    {
                        enemyAnims[enemyState].TranslateTo(objects[i].GetRectangle().X + objects[i].GetRectangle().Width + ADD, enemyAnims[enemyState].GetDestRec().Y);
                        pos.X = enemyAnims[enemyState].GetDestRec().X;
                        collision = true;
                        return collision;
                    }
                    else if (enemyRecs[RIGHT].Intersects(objects[i].GetRectangle()))
                    {
                        enemyAnims[enemyState].TranslateTo(objects[i].GetRectangle().X - enemyAnims[enemyState].GetDestRec().Width - ADD, enemyAnims[enemyState].GetDestRec().Y);
                        pos.X = enemyAnims[enemyState].GetDestRec().X;
                        collision = true;
                        return collision;
                    }
                    else if (enemyRecs[HEAD].Intersects(objects[i].GetRectangle()))
                    {
                        enemyAnims[enemyState].TranslateTo(enemyAnims[enemyState].GetDestRec().X, objects[i].GetRectangle().Y + objects[i].GetRectangle().Height + ADD);
                        pos.Y = enemyAnims[enemyState].GetDestRec().Y;
                        collision = true;
                        return collision;
                    }

                    //If a collision occured then the player's collision rectangles need to be adjusted
                    if (collision == true)
                    {
                        SetEnemyRecs(enemyRecs, enemyAnims);
                        collision = false;
                    }
                }
            }

            return collision;
        }

        /// <summary>
        /// Load enemy animations
        /// </summary>
        /// <param name="fileName">File to load animation from</param>
        /// <param name="Content">Content manager for loading game assets</param>
        public void LoadAnims(string fileName, ContentManager Content)
        {
            //Load animations
            enemyAnims = Animation.LoadAnimations(fileName, Content);
        }
    }
}
