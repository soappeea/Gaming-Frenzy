//Author: Sophia Lin
//File Name: Player.cs
//Project Name: PASS3
//Creation Date: December 3, 2023
//Modified Date: December 9, 2023
//Description: Handle Player Info
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
    class Player
    {
        //define body parts to be used for collision rectangle array indices
        const int FEET = 0;
        const int HEAD = 1;
        const int LEFT = 2;
        const int RIGHT = 3;

        //Track the player's horizontal acceleration data
        private const float ACCEL = 0.8f;
        private const float FRICTION = ACCEL * 0.9f;
        private const float TOLERANCE = 0.9f;

        //Define directions
        private const int POS = 1;
        private const int NEG = -1;

        //Direction player is travelling in
        private int dir = POS;

        //Player movement data
        private float maxSpeed = 2f;
        //float maxBoostedSpeed = 6f;
        private Vector2 playerSpeed = new Vector2(0f, 0f);

        //Store the force of gravity
        private const float GRAVITY = 9.8f / 60;

        //Track the forces working against the player every update
        private Vector2 forces = new Vector2(FRICTION, GRAVITY);

        //The initial jump speed that will be reduced by gravity each update
        private float jumpSpeed = -4f;

        //Track if player is on the ground or not
        private bool grounded = false;

        //Player Health
        private const float MAX_HEALTH = 100f;
        private float health = MAX_HEALTH;

        //Tracking player states
        private const int IDLE = 0;
        private const int RUN = 1;
        private const int JUMP = 2;
        private const int ATTACK = 3;

        //Track the current player state 
        private int playerState = IDLE;

        //Track the 4 inner rectangles of the player for collision detection
        Rectangle[] playerRecs = new Rectangle[4];

        //Track the 4 visible versions of the player's collision recs (for testing display purposes)
        GameRectangle[] playerVisibleRecs = new GameRectangle[4];

        //Specify whether the collision rectangles should be displayed (for testing purposes)
        bool showCollisionRecs = true;


        private Vector2 playerPos;
        private Animation[] playerAnims;



        public Player(Vector2 playerPos/*, GraphicsDevice graphicsDevice*//*, ContentManager content*/)
        {
            this.playerPos = playerPos;
            //SetPlayerRecs(playerRecs, playerAnims/*, graphicsDevice*/);
        }

        public int GetPlayerState()
        {
            return playerState;
        }

        public float GetPlayerHealth()
        {
            return health;
        }

        public void LoadAnims(string fileName, ContentManager Content)
        {
            playerAnims = Animation.LoadAnimations(fileName, Content);
        }

        public virtual void Update(GameTime gameTime, MouseState mouse, MouseState prevMouse, KeyboardState kb, KeyboardState prevKb, List<GameObject> objects, GraphicsDevice graphicsDevice)
        {
            //update player animation based on playerstate
            playerAnims[playerState].Update(gameTime);

            //process input, left and right
            ////Update the player's speed based on input////
            //Check for right and left input to accelerate the player in the chosen direction
            if (kb.IsKeyDown(Keys.D)/* && !prevKb.IsKeyDown(Keys.D)*/)
            {
                //Change player's direction, state, and animation
                dir = POS;
                playerState = RUN;

                if (!playerAnims[RUN].IsAnimating())
                {
                    playerAnims[RUN].Activate(true);
                }

                //Add acceleration to the player's current speed, but keep it within the limits of maxSpeed
                playerSpeed.X += ACCEL;

                ////Change the speed limits and minimums based on whether a power-up has been picked up or not
                //if (isSpeedBoosted)
                //{
                //    //With speed boost
                //    playerSpeed.X = MathHelper.Clamp(playerSpeed.X, -maxBoostedSpeed, maxBoostedSpeed);
                //}
                //else
                //{
                //Without speed boost
                playerSpeed.X = MathHelper.Clamp(playerSpeed.X, -maxSpeed, maxSpeed);
                //}
            }
            else if (kb.IsKeyDown(Keys.A) /*&& !prevKb.IsKeyDown(Keys.A)*/)
            {
                //Change player's direction, state, and animation
                dir = NEG;
                playerState = RUN;
                if (!playerAnims[RUN].IsAnimating())
                {
                    playerAnims[RUN].Activate(true);
                }

                //Subtract acceleration to the player's current speed, but keep it within the limits of maxSpeed
                playerSpeed.X -= ACCEL;

                ////Change the speed limits and minimums based on whether a power-up has been picked up or not
                //if (isSpeedBoosted)
                //{
                //    //With speed boost
                //    playerSpeed.X = MathHelper.Clamp(playerSpeed.X, -maxBoostedSpeed, maxBoostedSpeed);
                //}
                //else
                //{
                //Without speed boost
                playerSpeed.X = MathHelper.Clamp(playerSpeed.X, -maxSpeed, maxSpeed);
                //}
            }
            else
            {
                //Only apply friction if player is on the ground and no input is given
                if (grounded == true)
                {
                    //Decelerate if no input for horizontal movement
                    playerSpeed.X += -Math.Sign(playerSpeed.X) * forces.X;

                    //If the player has decelerated below the tolerance amount, set the speed to 0
                    if (Math.Abs(playerSpeed.X) <= TOLERANCE)
                    {
                        //Change player state, animation, and speed
                        playerAnims[RUN].Activate(true);
                        playerState = IDLE;
                        playerSpeed.X = 0f;
                    }
                }
            }

            ////Jump if the player hits up key or w key and is on the ground
            if ((kb.IsKeyDown(Keys.Space) || kb.IsKeyDown(Keys.W)) && grounded == true)
            {
                //Play a jump sound 
                //jumpSnd.CreateInstance().Play();

                //Apply jump speed
                playerSpeed.Y = jumpSpeed;

                //Change player state and animation
                playerState = JUMP;
                playerAnims[JUMP].Activate(true);

            }

            //Add gravity to the y component of the player's speed
            playerSpeed.Y += forces.Y;

            //use vector velocity storing both x and y speed, then playerAnims[playerState].Translate(velocity);
            //Change the position of the player 
            playerPos.X += playerSpeed.X;
            playerPos.Y += playerSpeed.Y;
            playerAnims[playerState].TranslateTo((int)playerPos.X, (int)playerPos.Y/*new Vector2(playerSpeed.X, playerSpeed.Y)*/);
            //playerAnims[playerState].destRec.Y = (int)playerPos.Y;

            SetPlayerRecs(playerRecs, playerAnims, graphicsDevice);
            ObjectCollisionDetection(/*playerAnims, */objects, graphicsDevice);
        }

        private void SetPlayerRecs(Rectangle[] playerRecs, Animation[] playerAnims, GraphicsDevice graphicsDevice)
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

            //Setup the visible recs for testing purposes
            if (showCollisionRecs == true)
            {
                playerVisibleRecs[HEAD] = new GameRectangle(graphicsDevice, playerRecs[HEAD]);
                playerVisibleRecs[LEFT] = new GameRectangle(graphicsDevice, playerRecs[LEFT]);
                playerVisibleRecs[RIGHT] = new GameRectangle(graphicsDevice, playerRecs[RIGHT]);
                playerVisibleRecs[FEET] = new GameRectangle(graphicsDevice, playerRecs[FEET]);
            }
        }

        private void ObjectCollisionDetection(/*Animation[] playerAnims,*/ List<GameObject> objects, GraphicsDevice graphicsDevice)
        {
            bool collision = false;

            //Test collision between the player and every game object
            for (int i = 0; i < objects.Count; i++)
            {
                //Check for a bounding box collision first
                if (playerAnims[playerState].GetDestRec().Intersects(objects[i].GetRectangle()))
                {
                    //Shift the player to just outside of the collision location depending on body part
                    if (playerRecs[FEET].Intersects(objects[i].GetRectangle()))
                    {
                        playerAnims[playerState].TranslateTo(playerAnims[playerState].GetDestRec().X, objects[i].GetRectangle().Y - playerAnims[playerState].GetDestRec().Height);
                        playerPos.Y = playerAnims[playerState].GetDestRec().Y;
                        playerSpeed.Y = 0f;
                        grounded = true;
                        collision = true;
                    }
                    else if (playerRecs[LEFT].Intersects(objects[i].GetRectangle()))
                    {
                        playerAnims[playerState].TranslateTo(objects[i].GetRectangle().X + objects[i].GetRectangle().Width + 1, playerAnims[playerState].GetDestRec().Y);
                        playerPos.X = playerAnims[playerState].GetDestRec().X;
                        playerSpeed.X *= objects[i].GetReboundScaler();
                        collision = true;
                    }
                    else if (playerRecs[RIGHT].Intersects(objects[i].GetRectangle()))
                    {
                        playerAnims[playerState].TranslateTo(objects[i].GetRectangle().X - playerAnims[playerState].GetDestRec().Width - 1, playerAnims[playerState].GetDestRec().Y);
                        playerPos.X = playerAnims[playerState].GetDestRec().X;
                        playerSpeed.X = objects[i].GetReboundScaler();
                        collision = true;
                    }
                    else if (playerRecs[HEAD].Intersects(objects[i].GetRectangle()))
                    {
                        playerAnims[playerState].TranslateTo(playerAnims[playerState].GetDestRec().X, objects[i].GetRectangle().Y + objects[i].GetRectangle().Height + 1);
                        playerPos.Y = playerAnims[playerState].GetDestRec().Y;
                        playerSpeed.Y *= NEG;
                        collision = true;
                    }

                    //If a collision occured then the player's collision rectangles need to be adjusted
                    if (collision == true)
                    {
                        SetPlayerRecs(playerRecs, playerAnims, graphicsDevice);
                        collision = false;
                    }
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            //Display the player based on the direction it is facing
            if (dir == POS)
            {
                //If the player is facing right, do not flip the animation
                playerAnims[playerState].Draw(spriteBatch, Color.White, Animation.FLIP_NONE);
            }
            else if (dir == NEG)
            {
                //If the player is facing left, flip the animation
                playerAnims[playerState].Draw(spriteBatch, Color.White, Animation.FLIP_HORIZONTAL);
            }

            if (showCollisionRecs)
            {
                playerVisibleRecs[HEAD].Draw(spriteBatch, Color.Yellow * 0.5f, true);
                playerVisibleRecs[LEFT].Draw(spriteBatch, Color.Red * 0.5f, true);
                playerVisibleRecs[RIGHT].Draw(spriteBatch, Color.Blue * 0.5f, true);
                playerVisibleRecs[FEET].Draw(spriteBatch, Color.Green * 0.5f, true);
            }

            //spriteBatch.DrawString(statsFont, "Player COORDS:" + "(" + playerAnims[playerState].destRec.X + "," + playerAnims[playerState].destRec.Y + ") ,"/* + attackaffected*/, new Vector2(300, 0), Color.White);
        }
    }
}
