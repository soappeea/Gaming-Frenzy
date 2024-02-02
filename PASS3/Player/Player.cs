//Author: Sophia Lin
//File Name: Player.cs
//Project Name: PASS3
//Creation Date: December 11, 2023
//Modified Date: January 18, 2024
//Description: Handle Player 
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
        //Track the player's horizontal acceleration data
        protected const float ACCEL = 0.8f;
        protected const float FRICTION = ACCEL * 0.9f;
        protected const float TOLERANCE = 0.9f;

        //The initial jump speed that will be reduced by gravity each update
        protected float jumpSpeed = -6f;

        //Track if player is on the ground or not
        protected bool grounded = false;

        //Define gravity constant
        protected const float GRAVITY = 9.8f / 60;

        //Define directions
        protected const int POS = 1;
        protected const int NEG = -1;

        //Direction player is travelling in
        protected int dir = POS;

        //Player movement data
        protected float maxSpeed = 2f;
        protected float maxBoostedSpeed = 3.5f;
        protected Vector2 playerSpeed = new Vector2(0f, 0f);

        //Store speed boost
        protected bool isSpeedBoost;
        protected Timer speedTimer;

        //Detect if player is damaged
        protected bool isDamaged;

        //Player Health
        public const float MAX_HEALTH = 100f;
        protected const int MINIMAL_HEALTH = 30;
        protected float health;

        //Tracking player states  
        protected const int IDLE = 0;
        protected const int RUN = 1; 

        //Track the current player state 
        protected int playerState = IDLE;

        protected Vector2 playerPos;
        protected Animation[] playerAnims;

        //Define body parts to be used for collision rectangle array indices
        protected const int FEET = 0;
        protected const int HEAD = 1;
        protected const int LEFT = 2;
        protected const int RIGHT = 3;

        //Track the 4 inner rectangles of the player for collision detection
        protected Rectangle[] playerRecs = new Rectangle[4];

        //Track the 4 visible versions of the player's collision recs (for testing display purposes)
        protected GameRectangle[] playerVisibleRecs = new GameRectangle[4];

        //Specify whether the collision rectangles should be displayed (for testing purposes)
        protected bool showCollisionRecs = true;

        //Define general default values
        protected const int ADD = 1;
        protected const int NONE = 0;

        //Store powerup time duration
        protected const int POWERUP_TIME = 5000;

        //Define the spike damage cool down timer
        protected Timer spikeTimer;
        protected const int SPIKE_TIME = 1000;

        //Load audio
        protected SoundEffect jumpSnd;
        protected SoundEffect damagedSnd;

        /// <summary>
        /// Create an instance of player
        /// </summary>
        /// <param name="playerPos">Player position</param>
        public Player(Vector2 playerPos)
        {
            //Set player position
            this.playerPos = playerPos;

            //Set player health
            health = MAX_HEALTH;

            //Set up timers
            speedTimer = new Timer(POWERUP_TIME, false);
            spikeTimer = new Timer(SPIKE_TIME, false);
        }

        /// <summary>
        /// Retrieve position of player
        /// </summary>
        /// <returns>Player position</returns>
        public Vector2 GetPos()
        {
            return playerPos;
        }

        /// <summary>
        /// Retrieve player health
        /// </summary>
        /// <returns>Player health</returns>
        public float GetPlayerHealth()
        {
            return health;
        }

        /// <summary>
        /// Retrieve player rectangle
        /// </summary>
        /// <returns>Player rectangle</returns>
        public Rectangle GetPlayerRec()
        {
            return playerAnims[playerState].GetDestRec();
        }

        /// <summary>
        /// Retrieve player speed timer
        /// </summary>
        /// <returns>Player speed timer as a string</returns>
        public string GetSpeedTime()
        {
            return speedTimer.GetTimeRemainingAsString(Timer.FORMAT_SEC_MIL);
        }

        /// <summary>
        /// Retrieve status of speed boost on player
        /// </summary>
        /// <returns>If speed boost is active</returns>
        public bool GetSpeedBoosted()
        {
            return isSpeedBoost;
        }

        /// <summary>
        /// Set individual player rectangles
        /// </summary>
        /// <param name="playerRecs">Individual player rectangles</param>
        /// <param name="playerAnims">Player animations</param>
        protected virtual void SetPlayerRecs(Rectangle[] playerRecs, Animation[] playerAnims)
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
        /// Set speed boost status of player
        /// </summary>
        /// <param name="isSpeedBoost">Status of speed boost</param>
        public void SetSpeedStatus(bool isSpeedBoost)
        {
            //Set speed boost status
            this.isSpeedBoost = isSpeedBoost;

            //Start speed boost timer
            speedTimer.ResetTimer(true);
        }

        /// <summary>
        /// Set player health
        /// </summary>
        /// <param name="health">Amount of health player should have</param>
        public void SetPlayerHealth(int health)
        {
            this.health = health;
        }

        /// <summary>
        /// Update the player
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
        public virtual void Update(GameTime gameTime, MouseState mouse, MouseState prevMouse, KeyboardState kb, List<GameObject> objects, 
                                    List<GameLogo> logos, List<Reactable> candies, Reactable portal, List<Reactable> mainPowerups, 
                                    List<Reactable> miniPowerups, Rectangle[] enemyRecs, List<Reactable> gems)
        {
            //Update player animation based on playerstate
            playerAnims[playerState].Update(gameTime);
            speedTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            spikeTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //Deactivate speed boost if speed timer is over
            if (!speedTimer.IsActive())
            {
                //Set status to inactive
                isSpeedBoost = false;
            }
        }

        /// <summary>
        /// Display the player
        /// </summary>
        /// <param name="spriteBatch">Used for drawing sprites</param>
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
        }

        /// <summary>
        /// Load player animations
        /// </summary>
        /// <param name="fileName">Name of file to load from</param>
        /// <param name="Content">Content manager for loading game assets</param>
        public void LoadAnims(string fileName, ContentManager Content)
        {
            //Load the animation
            playerAnims = Animation.LoadAnimations(fileName, Content);
        }

        /// <summary>
        /// Load content
        /// </summary>
        /// <param name="Content">Content manager for loading game assets</param>
        public void LoadContent(ContentManager Content)
        {
            jumpSnd = Content.Load<SoundEffect>("Audio/Sound/Jump Effect");
            damagedSnd = Content.Load<SoundEffect>("Audio/Sound/Hurt Effect");
        }

        /// <summary>
        /// Inflict damage on player
        /// </summary>
        /// <param name="hpDeduct">Amount to deduct player health by</param>
        public virtual void DamagePlayer(int hpDeduct)
        {
            //Deduct health
            health -= hpDeduct;
        }

        /// <summary>
        /// Increase health in player
        /// </summary>
        /// <param name="health">Amount to add to player health by</param>
        public void AddPlayerHealth(int health)
        {
            //Add to health
            this.health += health;

            //Set health to maximum health amount if it exceeds the amount
            if (this.health > MAX_HEALTH)
            {
                this.health = MAX_HEALTH;
            }
        }

        /// <summary>
        /// Detect collision between reactables
        /// </summary>
        /// <param name="reactableObjects">Objects that can be reactable</param>
        protected virtual void ReactableCollisionDetection(List<Reactable> reactableObjects)
        {
            //Check for a bounding box collision first
            for (int i = 0; i < reactableObjects.Count; i++)
            {
                if (playerAnims[playerState].GetDestRec().Intersects(reactableObjects[i].GetRectangle()))
                {
                    //Set collision status to true
                    reactableObjects[i].SetStatus(true);
                }
            }
        }

        /// <summary>
        /// Detect wall collision with the player and stop their movement to keep them on screen
        /// </summary>
        protected virtual void WallCollisionDetection()
        {
            //Store bound of left wall
            int left = NONE;

            //If the player hits the side walls, pull them in bounds and stop their horizontal movement
            if (playerAnims[playerState].GetDestRec().X < left)
            {
                //Player past left side of screen, realign to be exactly the left side and stop movement
                playerAnims[playerState].TranslateTo(left, playerAnims[playerState].GetDestRec().Y);
                playerPos.X = playerAnims[playerState].GetDestRec().X;
                playerSpeed.X = NONE;
            }
            else if (playerAnims[playerState].GetDestRec().Right > Game1.screenWidth)
            {
                //Player past right side of screen, realign to be exactly the right side and stop movement
                playerAnims[playerState].TranslateTo(Game1.screenWidth - playerAnims[playerState].GetDestRec().Width, playerAnims[playerState].GetDestRec().Y);
                playerPos.X = playerAnims[playerState].GetDestRec().X;
                playerSpeed.X = NONE;
            }

            //If the player hits the top wall, pull them in bounds and stop their vertical movement 
            if (playerAnims[playerState].GetDestRec().Y < NONE)
            {
                //Player past top side of screen, realign to be exactly the top side and stop movement
                playerAnims[playerState].TranslateTo(playerAnims[playerState].GetDestRec().X, NONE);
                playerPos.Y = playerAnims[playerState].GetDestRec().Y;
                playerSpeed.Y = NONE;
            }
            else if (playerAnims[playerState].GetDestRec().Y + playerAnims[playerState].GetDestRec().Height > Game1.screenHeight)
            {
                //Player past bottom side of screen, realign to be exactly the top side and stop movement
                playerAnims[playerState].TranslateTo(playerAnims[playerState].GetDestRec().X, Game1.screenHeight - playerAnims[playerState].GetDestRec().Height);
                playerPos.Y = playerAnims[playerState].GetDestRec().Y;
                playerSpeed.Y = NONE;
            }
        }
    }
}
