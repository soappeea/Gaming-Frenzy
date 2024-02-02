//Author: Sophia Lin
//File Name: MinigamePlayer.cs
//Project Name: PASS3
//Creation Date: December 10, 2023
//Modified Date: January 6, 2024
//Description: Handle Minigame Player
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
    class MinigamePlayer : Player
    {

        //Additional player animations
        private const int RUN_UP = 2;
        private const int RUN_DOWN = 3;

        //Store speeds based on which minigame
        public const float SKEWER_SPEED = 2f;
        public const float OTHER_SPEED = 3f;

        //Track the forces working against the player every update
        private Vector2 forces = new Vector2(FRICTION, FRICTION);

        //Store whether player can move in all directions
        private bool allDirs;

        //Determine if normal wall collision detection
        private bool normalWalls;

        //Store enemy status and attack timer
        private bool enemyActive;
        Timer attackTimer;

        /// <summary>
        /// Create an instance of the minigame player
        /// </summary>
        /// <param name="playerPos">Minigame player position</param>
        public MinigamePlayer(Vector2 playerPos) : base(playerPos)
        {
            //Set attack timer
            attackTimer = new Timer(2000, false);
        }

        /// <summary>
        /// Deactivate damage status
        /// </summary>
        public void SetDamageStatus()
        {
            isDamaged = false;
        }

        /// <summary>
        /// Set whether the minigame has normal walls or not
        /// </summary>
        /// <param name="status">Status of normal walls</param>
        public void SetWallState(bool status)
        {
            normalWalls = status;
        }

        /// <summary>
        /// Set player position
        /// </summary>
        /// <param name="pos">New player position</param>
        public void SetPosition(Vector2 pos)
        {
            this.playerPos = pos;
        }

        /// <summary>
        /// Set whether player can move in all directions
        /// </summary>
        /// <param name="allDirs">Whether player can move in every direction</param>
        public void SetPlayerDirs(bool allDirs)
        {
            this.allDirs = allDirs;
        }

        /// <summary>
        /// Set player speed
        /// </summary>
        /// <param name="speed">New player speed</param>
        public void SetSpeed(float speed)
        {
            this.maxSpeed = speed;
        }

        /// <summary>
        /// Set enemy status
        /// </summary>
        /// <param name="status">Enemy status</param>
        public void SetEnemyStatus(bool status)
        {
            enemyActive = status;
        }

        /// <summary>
        /// Update the minigame player
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        /// <param name="mouse">Mouse state</param>
        /// <param name="prevMouse">Previous mouse state</param>
        /// <param name="kb">Keyboard state</param>
        /// <param name="objects">Objects in the game</param>
        /// <param name="logos">Minigame logos</param>
        /// <param name="candies">Candies in minigame</param>
        /// <param name="portal">Portal in main game</param>
        /// <param name="mainPowerups">Powerups in main game</param>
        /// <param name="miniPowerups">Powerups in minigame</param>
        /// <param name="enemyRecs">Enemy rectangles</param>
        /// <param name="gems">Gems in game</param>
        public override void Update(GameTime gameTime, MouseState mouse, MouseState prevMouse, KeyboardState kb, List<GameObject> objects, 
                                    List<GameLogo> logos, List<Reactable> candies, Reactable portal, List<Reactable> mainPowerups, 
                                    List<Reactable> miniPowerups, Rectangle[] enemyRecs, List<Reactable> gems)
        {
            //Update player
            base.Update(gameTime, mouse, prevMouse, kb, objects, logos, candies, portal, mainPowerups, miniPowerups, enemyRecs, gems);

            //Update enemy attack timer if enemy state is active
            if (enemyActive)
            {
                attackTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            }

            //Update the player's speed based on input
            //Check for to accelerate the player in the chosen direction
            if (kb.IsKeyDown(Keys.D) || kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.W) || kb.IsKeyDown(Keys.S))
            {
                //Left or right
                if (kb.IsKeyDown(Keys.D))
                {
                    //Change player's direction, state
                    dir = POS;
                    playerState = RUN;

                    //Change player animation
                    if (!playerAnims[RUN].IsAnimating())
                    {
                        playerAnims[RUN].Activate(true);
                    }

                    //Add acceleration to the player's current speed, but keep it within the limits of maxSpeed
                    playerSpeed.X += ACCEL;

                    ////Change the speed limits and minimums based on whether a power-up has been picked up or not
                    if (isSpeedBoost)
                    {
                        //With speed boost
                        playerSpeed.X = MathHelper.Clamp(playerSpeed.X, -maxBoostedSpeed, maxBoostedSpeed);
                    }
                    else
                    {
                       // Without speed boost
                        playerSpeed.X = MathHelper.Clamp(playerSpeed.X, -maxSpeed, maxSpeed);
                    }
                }
                else if (kb.IsKeyDown(Keys.A) /*&& !prevKb.IsKeyDown(Keys.A)*/)
                {
                    //Change player's direction, state
                    dir = NEG;
                    playerState = RUN;

                    //Change player animation
                    if (!playerAnims[RUN].IsAnimating())
                    {
                        playerAnims[RUN].Activate(true);
                    }

                    //Subtract acceleration to the player's current speed, but keep it within the limits of maxSpeed
                    playerSpeed.X -= ACCEL;

                    //Change the speed limits and minimums based on whether a power-up has been picked up or not
                    if (isSpeedBoost)
                    {
                        //With speed boost
                        playerSpeed.X = MathHelper.Clamp(playerSpeed.X, -maxBoostedSpeed, maxBoostedSpeed);
                    }
                    else
                    {
                        //Without speed boost
                        playerSpeed.X = MathHelper.Clamp(playerSpeed.X, -maxSpeed, maxSpeed);
                    }
                }

                if (allDirs)
                {
                    //Up or down
                    if (kb.IsKeyDown(Keys.W))
                    {
                        //Change player's state
                        playerState = RUN_UP;

                        //Change player animation
                        if (!playerAnims[RUN_UP].IsAnimating())
                        {
                            playerAnims[RUN_UP].Activate(true);
                        }

                        //Subtract acceleration to the player's current speed, but keep it within the limits of maxSpeed
                        playerSpeed.Y -= ACCEL;

                        //Change the speed limits and minimums based on whether a power-up has been picked up or not
                        if (isSpeedBoost)
                        {
                            //With speed boost
                            playerSpeed.Y = MathHelper.Clamp(playerSpeed.Y, -maxBoostedSpeed, maxBoostedSpeed);
                        }
                        else
                        {
                            //Without speed boost
                            playerSpeed.Y = MathHelper.Clamp(playerSpeed.Y, -maxSpeed, maxSpeed);
                        }
                    }
                    else if (kb.IsKeyDown(Keys.S))
                    {
                        //Change player's state
                        playerState = RUN_DOWN;

                        //Change player animation
                        if (!playerAnims[RUN_DOWN].IsAnimating())
                        {
                            playerAnims[RUN_DOWN].Activate(true);
                        }

                        //Add acceleration to the player's current speed, but keep it within the limits of maxSpeed
                        playerSpeed.Y += ACCEL;

                        //Change the speed limits and minimums based on whether a power-up has been picked up or not
                        if (isSpeedBoost)
                        {
                            //With speed boost
                            playerSpeed.Y = MathHelper.Clamp(playerSpeed.Y, -maxBoostedSpeed, maxBoostedSpeed);
                        }
                        else
                        {
                            //Without speed boost
                            playerSpeed.Y = MathHelper.Clamp(playerSpeed.Y, -maxSpeed, maxSpeed);
                        }
                    }
                }
            }
            else
            {

                //Decelerate if no input for movement
                playerSpeed.X += -Math.Sign(playerSpeed.X) * forces.X;
                playerSpeed.Y += -Math.Sign(playerSpeed.Y) * forces.Y;

                //If the player has decelerated below the tolerance amount, set the speed to 0
                if (Math.Abs(playerSpeed.X) <= TOLERANCE && Math.Abs(playerSpeed.Y) <= TOLERANCE)
                {
                    //Change player state
                    playerState = IDLE;

                    //Change player animation
                    if (!playerAnims[IDLE].IsAnimating())
                    {
                        playerAnims[IDLE].Activate(true);
                    }

                    //Change player speed
                    playerSpeed.X = 0f;
                    playerSpeed.Y = 0f;
                }
            }

            //Change the position of the player 
            playerPos.X += playerSpeed.X;
            playerPos.Y += playerSpeed.Y;
            playerAnims[playerState].TranslateTo((int)playerPos.X, (int)playerPos.Y);

            //Update the player rectangles
            SetPlayerRecs(playerRecs, playerAnims);

            //Detect collision between player and objects
            ObjectCollisionDetection(objects);

            //Detect collision between player and reactables
            ReactableCollisionDetection(miniPowerups);

            //Detect collision between player and gems
            ReactableCollisionDetection(gems);

            //Detect bounds of "walls" based on whether there are normal walls or not
            if (normalWalls == false)
            {
                SkewerCollisionDetection();
            }
            else
            {
                WallCollisionDetection();
            }

            //Detect enemy attack if enemy is active
            if (enemyActive)
            {
                EnemyAttackDetection(enemyRecs);
            }

            //Detect candy collision if there are candies remaining
            if (candies.Count > 0)
            {
                CandyCollection(candies);
            }
        }

        /// <summary>
        /// Display minigame player
        /// </summary>
        /// <param name="spriteBatch">Used for drawing sprites</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //Display the player based on the direction it is facing
            if (dir == NEG)
            {
                //If the player is facing left, flip the animation
                playerAnims[playerState].Draw(spriteBatch, Color.White, Animation.FLIP_HORIZONTAL);
            }
            else
            {
                //If the player is facing right, up, or down, do not flip the animation
                playerAnims[playerState].Draw(spriteBatch, Color.White, Animation.FLIP_NONE);
            }

            //Display speed timer if activated
            if (isSpeedBoost)
            {
                spriteBatch.DrawString(Game1.tinyStatsFont, speedTimer.GetTimeRemainingAsString(Timer.FORMAT_SEC_MIL), new Vector2(1625, 100), Color.Black);
            }
        }

        /// <summary>
        /// Detect collisions between other objects
        /// </summary>
        /// <param name="objects">Game objects</param>
        private void ObjectCollisionDetection(List<GameObject> objects)
        {
            bool collision = false;

            //Test collision between the player and every game object
            for (int i = 0; i < objects.Count; i++)
            {
                //Check for a bounding box collision first
                if (playerAnims[playerState].GetDestRec().Intersects(objects[i].GetRectangle()))
                {
                    //Inflict damage on player upon collision
                    if (objects[i] is Skewer && !isDamaged)
                    {
                        //Damage player
                        DamagePlayer(MINIMAL_HEALTH);
                        damagedSnd.CreateInstance().Play();

                        //Set damage status to active
                        isDamaged = true;
                    }
                    else if (objects[i] is Spike && !spikeTimer.IsActive())
                    {
                        //Damage player
                        DamagePlayer(MINIMAL_HEALTH);
                        damagedSnd.CreateInstance().Play();

                        //Start spike cooldown damage timer
                        spikeTimer.ResetTimer(true);
                    }

                    //Shift the player to just outside of the collision location depending on body part
                    if (playerRecs[FEET].Intersects(objects[i].GetRectangle()) && !(objects[i] is Snowball))
                    {
                        playerAnims[playerState].TranslateTo(playerAnims[playerState].GetDestRec().X, objects[i].GetRectangle().Y - playerAnims[playerState].GetDestRec().Height - ADD);
                        playerPos.Y = playerAnims[playerState].GetDestRec().Y;
                        playerSpeed.Y *= objects[i].GetReboundScaler();
                        collision = true;
                    }
                    else if (playerRecs[LEFT].Intersects(objects[i].GetRectangle()))
                    {
                        playerAnims[playerState].TranslateTo(objects[i].GetRectangle().X + objects[i].GetRectangle().Width + ADD + ADD, playerAnims[playerState].GetDestRec().Y);
                        playerPos.X = playerAnims[playerState].GetDestRec().X;
                        playerSpeed.X *= objects[i].GetReboundScaler();
                        collision = true;
                    }
                    else if (playerRecs[RIGHT].Intersects(objects[i].GetRectangle()))
                    {
                        playerAnims[playerState].TranslateTo(objects[i].GetRectangle().X - playerAnims[playerState].GetDestRec().Width - ADD - ADD, playerAnims[playerState].GetDestRec().Y);
                        playerPos.X = playerAnims[playerState].GetDestRec().X;
                        playerSpeed.X = objects[i].GetReboundScaler();
                        collision = true;
                    }
                    else if (playerRecs[HEAD].Intersects(objects[i].GetRectangle()))
                    {
                        playerAnims[playerState].TranslateTo(playerAnims[playerState].GetDestRec().X, objects[i].GetRectangle().Y + objects[i].GetRectangle().Height + ADD + ADD);
                        playerPos.Y = playerAnims[playerState].GetDestRec().Y;
                        playerSpeed.Y *= objects[i].GetReboundScaler();
                        collision = true;
                    }

                    //If a collision occured then the player's collision rectangles need to be adjusted
                    if (collision == true)
                    {
                        SetPlayerRecs(playerRecs, playerAnims);
                        collision = false;
                    }
                }
            }
        }

        /// <summary>
        /// Detect collision with enemy
        /// </summary>
        /// <param name="enemyRecs">Enemy individual rectangles</param>
        private void EnemyAttackDetection(Rectangle[] enemyRecs)
        {
            //Detect if enemy collides with player
            for (int i = 0; i < enemyRecs.Count(); i++)
            {
                if (enemyRecs[i].Intersects(playerAnims[playerState].GetDestRec()))
                {
                    //Damage player if the attack cooldown timer is not active
                    if (!attackTimer.IsActive())
                    {
                        //Damage player
                        DamagePlayer(MINIMAL_HEALTH);
                        damagedSnd.CreateInstance().Play();

                        //Start attack cooldown timer
                        attackTimer.ResetTimer(true);
                    }
                }
            }
        }

        /// <summary>
        /// Detect collision with bounds in skewer game
        /// </summary>
        private void SkewerCollisionDetection()
        {
            //Player is not initially colliding with a wall
            bool collision = false;

            //If the player hits the side walls, pull them in bounds and stop their horizontal movement
            if (playerAnims[playerState].GetDestRec().X < Skewer.LEFT_BOUND)
            {
                //Player past left side of screen, realign to be exactly the left side and stop movement
                playerAnims[playerState].TranslateTo(Skewer.LEFT_BOUND + 1, playerAnims[playerState].GetDestRec().Y);
                playerPos.X = playerAnims[playerState].GetDestRec().X;
                playerSpeed.X = NONE;
                collision = true;
            }
            else if (playerAnims[playerState].GetDestRec().Right > Skewer.RIGHT_BOUND)
            {
                //Player past right side of screen, realign to be exactly the right side and stop movement
                playerAnims[playerState].TranslateTo(Skewer.RIGHT_BOUND - playerAnims[playerState].GetDestRec().Width, playerAnims[playerState].GetDestRec().Y);
                playerPos.X = playerAnims[playerState].GetDestRec().X;
                playerSpeed.X = NONE;
                collision = true;
            }

            //If the player hits the top wall, pull them in bounds and stop their vertical movement 
            if (playerAnims[playerState].GetDestRec().Y < Skewer.TOP_BOUND)
            {
                //Player past top side of screen, realign to be exactly the top side and stop movement
                playerAnims[playerState].TranslateTo(playerAnims[playerState].GetDestRec().X, Skewer.TOP_BOUND);
                playerPos.Y = playerAnims[playerState].GetDestRec().Y;
                playerSpeed.Y = NONE;
                collision = true;
            }
            else if (playerAnims[playerState].GetDestRec().Y + playerAnims[playerState].GetDestRec().Height > Skewer.BOTTOM_BOUND)
            {
                //Player past bottom side of screen, realign to be exactly the bottom side and stop movement
                playerAnims[playerState].TranslateTo(playerAnims[playerState].GetDestRec().X, Skewer.BOTTOM_BOUND - playerAnims[playerState].GetDestRec().Height);
                playerPos.Y = playerAnims[playerState].GetDestRec().Y;
                playerSpeed.Y = NONE;
                collision = true;
            }


            //If a collision occured then the player's collision rectangles need to be adjusted
            if (collision == true)
            {
                //Adjust the player's collision rectangles
                SetPlayerRecs(playerRecs, playerAnims);
            }
        }

        /// <summary>
        /// Detect collision with candy
        /// </summary>
        /// <param name="candies">Candy objects</param>
        private void CandyCollection(List<Reactable> candies)
        {
            //Detect collision with candy
            for (int i = 0; i < candies.Count; i++)
            {
                //Check for a bounding box collision first
                if (playerAnims[playerState].GetDestRec().Intersects(candies[i].GetRectangle()))
                {
                    //Set collision status to active
                    candies[i].SetStatus(true);
                }
            }
        }
    }
}
