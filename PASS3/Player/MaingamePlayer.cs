//Author: Sophia Lin
//File Name: MaingamePlayer.cs
//Project Name: PASS3
//Creation Date: December 3, 2023
//Modified Date: January 6, 2024
//Description: Handle Maingame Player 
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
    class MaingamePlayer : Player
    {
        //Store the jump boost force
        private const float JUMP_BOOST = 3f / 60;

        //Store current force in Y dir
        private float yForce;

        //Track the forces working against the player every update
        private Vector2 forces = new Vector2(FRICTION, GRAVITY);

        //Extra playerState
        private const int JUMP = 2;

        //Store jump boost
        private bool isJumpBoost;
        private Timer jumpTimer;

        /// <summary>
        /// Create an instance of minigame player
        /// </summary>
        /// <param name="playerPos">Minigame player position</param>
        public MaingamePlayer(Vector2 playerPos) : base(playerPos)
        {
            //Set current Y direction force to gravity
            yForce = GRAVITY;

            //Set jump boost timer
            jumpTimer = new Timer(POWERUP_TIME, false);
        }

        /// <summary>
        /// Retrieve jump boost time remaining 
        /// </summary>
        /// <returns>Jump boost time remaining as a string</returns>
        public string GetJumpTime()
        {
            return jumpTimer.GetTimeRemainingAsString(Timer.FORMAT_SEC_MIL);
        }

        /// <summary>
        /// Retrieve jump boost status
        /// </summary>
        /// <returns>Jump boost status</returns>
        public bool GetJumpBoosted()
        {
            return isJumpBoost;
        }

        /// <summary>
        /// Set the status of the jump boost
        /// </summary>
        /// <param name="isJumpBoost">Status of jump boost</param>
        public void SetForceStatus(bool isJumpBoost)
        {
            this.isJumpBoost = isJumpBoost;
        }

        /// <summary>
        /// Set individual player rectangles
        /// </summary>
        /// <param name="playerRecs">Array of player individual rectangles</param>
        /// <param name="playerAnims">Array of player animations</param>
        protected override void SetPlayerRecs(Rectangle[] playerRecs, Animation[] playerAnims)
        {
            //Define player Collision Recs based on its position and scaled size
            playerRecs[HEAD] = new Rectangle(playerAnims[playerState].GetDestRec().X + (int)(0.25f * playerAnims[playerState].GetDestRec().Width), playerAnims[playerState].GetDestRec().Y,
                                             (int)(playerAnims[playerState].GetDestRec().Width * 0.5f), (int)(playerAnims[playerState].GetDestRec().Height * 0.25f));
            playerRecs[LEFT] = new Rectangle(playerAnims[playerState].GetDestRec().X + 5, playerRecs[HEAD].Y + playerRecs[HEAD].Height,
                                             (int)(playerAnims[playerState].GetDestRec().Width * 0.4f), (int)(playerAnims[playerState].GetDestRec().Height * 0.5f));
            playerRecs[RIGHT] = new Rectangle(playerRecs[LEFT].X + playerRecs[LEFT].Width, playerRecs[HEAD].Y + playerRecs[HEAD].Height,
                                             (int)(playerAnims[playerState].GetDestRec().Width * 0.4f), (int)(playerAnims[playerState].GetDestRec().Height * 0.5f));
            playerRecs[FEET] = new Rectangle(playerAnims[playerState].GetDestRec().X + (int)(0.2f * playerAnims[playerState].GetDestRec().Width), playerRecs[LEFT].Y + playerRecs[LEFT].Height,
                                             (int)(playerAnims[playerState].GetDestRec().Width * 0.6f), (int)(playerAnims[playerState].GetDestRec().Height * 0.25f));
        }

        /// <summary>
        /// Update the maingame player
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

            //Update jump boost timer
            jumpTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);   

            //Determine Y-direction force based on whether jump boost is active or not
            if (jumpTimer.IsFinished())
            {
                //Jump boost is inactive
                isJumpBoost = false;

                //Set Y direction force to gravity
                yForce = GRAVITY;
            }
            else if (isJumpBoost && !jumpTimer.IsActive())
            {           
                //Start timer for jump boost
                jumpTimer.ResetTimer(true);

                //Set Y direction force to the jump boost force
                yForce = JUMP_BOOST;
            }

            //Update current forces acting on player 
            forces = new Vector2(FRICTION, yForce);

            //Update the player's speed based on input
            //Check for right and left input to accelerate the player in the chosen direction
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
            else if (kb.IsKeyDown(Keys.A))
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
            else
            {
                //Only apply friction if player is on the ground and no input is given
                if (grounded == true)
                {
                    //Decelerate if no input for horizontal movement
                    playerSpeed.X += -Math.Sign(playerSpeed.X) * forces.X;

                    //If the player has decelerated below the tolerance amount, set the speed to 0
                    if (Math.Abs(playerSpeed.X) <= TOLERANCE/* && playerSpeed.Y <= 0*/)
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
                    }
                }
            }

            //Jump if the player hits up key or w key and is on the ground
            if (kb.IsKeyDown(Keys.W) && grounded == true)
            {
                //Play a jump sound 
                jumpSnd.CreateInstance().Play();

                //Apply jump speed
                playerSpeed.Y = jumpSpeed;

                //Change player state and animation
                playerState = JUMP;
                playerAnims[JUMP].Activate(true);
            }

            //Add gravity to the y component of the player's speed
            playerSpeed.Y += forces.Y;

            //Change the position of the player 
            playerPos.X += playerSpeed.X;
            playerPos.Y += playerSpeed.Y;
            playerAnims[playerState].TranslateTo((int)playerPos.X, (int)playerPos.Y);

            //Update the player rectangles
            SetPlayerRecs(playerRecs, playerAnims);

            //Detect collision between player and walls
            WallCollisionDetection();

            //Detect collision between player and objects
            ObjectCollisionDetection(objects);

            //Detect collision between player and reactables
            ReactableCollisionDetection(mainPowerups);

            //Detect collision between player and gems
            ReactableCollisionDetection(gems);

            //Detect collision between player and portal
            PortalCollisionDetection(portal);

            //Detect collision between player and minigame logo if there are any remaining
            if (logos.Count > 0)
            {
                MinigameLogoDetection(logos);
            }
        }

        /// <summary>
        /// Detect portal collision
        /// </summary>
        /// <param name="portal">Portal object</param>
        private void PortalCollisionDetection(Reactable portal)
        {
            //Check for a bounding box collision first
            if (playerAnims[playerState].GetDestRec().Intersects(portal.GetRectangle()))
            {
                //Trigger collision status
                portal.SetStatus(true);
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
                    //Modify player speed if it collides with a moving rectangle to ensure it stays on, if it collides with spike damage player
                    if (objects[i] is MovingPlatform)
                    {
                        //Add moving platform speed to player
                        playerPos.X += ((MovingPlatform)objects[i]).GetSpeed();
                        playerAnims[playerState].TranslateTo((int)playerPos.X, (int)playerPos.Y);

                        //Set player individual rectangles
                        SetPlayerRecs(playerRecs, playerAnims);
                    }
                    else if (objects[i] is Spike && !spikeTimer.IsActive())
                    {
                        //Damage player
                        DamagePlayer(MINIMAL_HEALTH);
                        damagedSnd.CreateInstance().Play();

                        //Reset spike cooldown damage timer
                        spikeTimer.ResetTimer(true);
                    }

                    //Shift the player to just outside of the collision location depending on body part
                    if (playerRecs[FEET].Intersects(objects[i].GetRectangle()))
                    {
                        playerAnims[playerState].TranslateTo(playerAnims[playerState].GetDestRec().X, objects[i].GetRectangle().Y - playerAnims[playerState].GetDestRec().Height + ADD + ADD);
                        playerPos.Y = playerAnims[playerState].GetDestRec().Y;
                        playerSpeed.Y = 0f;
                        grounded = true;
                        collision = true;
                    }
                    else if (playerRecs[LEFT].Intersects(objects[i].GetRectangle()))
                    {
                        playerAnims[playerState].TranslateTo(objects[i].GetRectangle().X + objects[i].GetRectangle().Width + ADD, playerAnims[playerState].GetDestRec().Y);
                        playerPos.X = playerAnims[playerState].GetDestRec().X;
                        playerSpeed.X *= objects[i].GetReboundScaler();
                        collision = true;
                    }
                    else if (playerRecs[RIGHT].Intersects(objects[i].GetRectangle()))
                    {
                        playerAnims[playerState].TranslateTo(objects[i].GetRectangle().X - playerAnims[playerState].GetDestRec().Width - ADD, playerAnims[playerState].GetDestRec().Y);
                        playerPos.X = playerAnims[playerState].GetDestRec().X;
                        playerSpeed.X = objects[i].GetReboundScaler();
                        collision = true;
                    }
                    else if (playerRecs[HEAD].Intersects(objects[i].GetRectangle()))
                    {
                        playerAnims[playerState].TranslateTo(playerAnims[playerState].GetDestRec().X, objects[i].GetRectangle().Y + objects[i].GetRectangle().Height + ADD);
                        playerPos.Y = playerAnims[playerState].GetDestRec().Y;
                        playerSpeed.Y *= NEG;
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
        /// Detect wall collision with the player and stop their movement to keep them on screen
        /// </summary>
        protected override void WallCollisionDetection()
        {
            //Player is not initially colliding with a wall
            bool collision = false;

            //Set left wall bound
            int left = 0;

            //If the player hits the side walls, pull them in bounds and stop their horizontal movement
            if (playerAnims[playerState].GetDestRec().X < left)
            {
                //Player past left side of screen, realign to be exactly the left side and stop movement
                playerAnims[playerState].TranslateTo(left, playerAnims[playerState].GetDestRec().Y);
                playerPos.X = playerAnims[playerState].GetDestRec().X;
                playerSpeed.X = NONE;
                collision = true;
            }
            else if (playerAnims[playerState].GetDestRec().Right > Game1.screenWidth)
            {
                //Player past right side of screen, realign to be exactly the right side and stop movement
                playerAnims[playerState].TranslateTo(Game1.screenWidth - playerAnims[playerState].GetDestRec().Width, playerAnims[playerState].GetDestRec().Y);
                playerPos.X = playerAnims[playerState].GetDestRec().X;
                playerSpeed.X = NONE;
                collision = true;
            }

            //If the player hits the top wall, pull them in bounds and stop their vertical movement 
            if (playerAnims[playerState].GetDestRec().Y < NONE)
            {
                //Player past top side of screen, realign to be exactly the top side and stop movement
                playerAnims[playerState].TranslateTo(playerAnims[playerState].GetDestRec().X, NONE);
                playerPos.Y = playerAnims[playerState].GetDestRec().Y;
                playerSpeed.Y = NONE;
                collision = true;
            }
            else
            {
                grounded = false;
            }

            //If a collision occured then the player's collision rectangles need to be adjusted
            if (collision == true)
            {
                //Adjust the player's collision rectangles
                SetPlayerRecs(playerRecs, playerAnims);
            }
        }

        /// <summary>
        /// Detect collision for minigame logos
        /// </summary>
        /// <param name="logos">Minigame logos</param>
        private void MinigameLogoDetection(List<GameLogo> logos)
        {
            //Test collision between the player and every game object
            for (int i = 0; i < logos.Count; i++)
            {
                //Check for a bounding box collision first
                if (playerAnims[playerState].GetDestRec().Intersects(logos[i].GetRectangle()))
                {
                    //Set collision status to active
                    logos[i].SetStatus(true);
                }
            }
        }
    }
}
