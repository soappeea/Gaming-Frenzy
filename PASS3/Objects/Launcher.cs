//Author: Sophia Lin
//File Name: Launcher.cs
//Project Name: PASS3
//Creation Date: December 18, 2023
//Modified Date: December 18, 2023
//Description: Handle avalanche launcher
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
    class Launcher : GameObject
    {
        //Store environment information
        private const int WALL_LEFT = 0;
        private const int WALL_RIGHT = 1;

        //Store direction launcher is moving in
        private const int DIR = -1;

        //Store the increment value for when calculating RNG values, to overcome the exclusion on the maximum
        protected int INCREMENT = 1;

        //Store snowball-related information
        protected int numLaunched;
        protected int launchTimeMin;
        protected int launchTimeMax;
        protected Texture2D partImg;
        protected float scaleMin;
        protected float scaleMax;
        protected int lifeMin;
        protected int lifeMax;
        protected int angleMin;
        protected int angleMax;
        protected int speedMin;
        protected int speedMax;
        protected Vector2 forces;
        protected bool fade;

        //Store the launch timer to know when to launch snowballs (intermission)
        protected Timer launchTimer;

        //Store transparency
        protected const float transparency = 0.7f;

        //Store list of snowballs
        protected ObjectQueue snowballs = new ObjectQueue();

        //Store value of no snowballs
        public const int NO_SNOWBALLS = 0;

        //Store launcher info
        private int launcherSpeedMin;
        private int launcherSpeedMax;
        private int launcherSpeed;


        /// <summary>
        /// Create an instance of launcher
        /// </summary>
        /// <param name="img">Launcher image</param>
        /// <param name="pos">Position of the launcher</param>
        /// <param name="scale">Scale for the launcher image</param>
        /// <param name="reboundScaler">Scale of how much the snowballs from the particle rebound</param>
        /// <param name="launchTimeMin">Minimum launch time</param>
        /// <param name="launchTimeMax">Maximum launch time</param>
        /// <param name="partImg">Snowball image</param>
        /// <param name="scaleMin">Minimum scale of the snowball image</param>
        /// <param name="scaleMax">Maximum scale of the snowball image</param>
        /// <param name="lifeMin">Minimum lifespan of the snowball</param>
        /// <param name="lifeMax">Maximum lifespan of the snowball</param>
        /// <param name="angleMin">Minimum angle of the snowball</param>
        /// <param name="angleMax">Maximum angle of the snowball</param>
        /// <param name="speedMin">Minimum speed of the snowball</param>
        /// <param name="speedMax">Maximum speed of the snowball</param>
        /// <param name="forces">Forces that act on the snowball</param>
        /// <param name="fade">Track if the snowball fades</param>
        /// <param name="launcherSpeedMin">Minimum speed of the launcher itself</param>
        /// <param name="launcherSpeedMax">Maximum speed of the launcher itself</param>
        public Launcher(Texture2D img, Vector2 pos, float scale, float reboundScaler, int launchTimeMin, int launchTimeMax, Texture2D partImg,
                        float scaleMin, float scaleMax, int lifeMin, int lifeMax, int angleMin, int angleMax, int speedMin, int speedMax,
                        Vector2 forces, bool fade, int launcherSpeedMin, int launcherSpeedMax) : base(img, pos, scale, reboundScaler)
        {

            //Set image dimensions
            this.imgWidth = (int)(img.Width * scale);
            this.imgHeight = (int)(img.Height * scale);

            //Set snowball-related information
            this.launchTimeMin = launchTimeMin;
            this.launchTimeMax = launchTimeMax;
            this.partImg = partImg;
            this.scaleMin = scaleMin;
            this.scaleMax = scaleMax;
            this.lifeMin = lifeMin;
            this.lifeMax = lifeMax;
            this.angleMin = angleMin;
            this.angleMax = angleMax;
            this.speedMin = speedMin;
            this.speedMax = speedMax;

            //Set forces 
            this.forces = forces;

            //Set if the snowball fades
            this.fade = fade;

            //Set launch timer
            this.launchTimer = new Timer(GetRandInt(launchTimeMin, launchTimeMax), false);

            //Set launcher speed
            this.launcherSpeedMin = launcherSpeedMin;
            this.launcherSpeedMax = launcherSpeedMax;
            launcherSpeed = GetRandInt(launcherSpeedMin, launcherSpeedMax);

        }

        /// <summary>
        /// Retrieve launcher rectangle
        /// </summary>
        /// <returns>Launcher rectangle</returns>
        public override Rectangle GetRectangle()
        {
            return new Rectangle((int)(pos.X - this.imgWidth / 2),
                                         (int)(pos.Y - this.imgHeight / 2), imgWidth, imgHeight);
        }

        /// <summary>
        /// Generate a random integer
        /// </summary>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <returns>Random integer</returns>
        protected int GetRandInt(int min, int max)
        {
            //Store the randomly generated integer
            int randNum = Game1.rng.Next(min, max + INCREMENT);
            return randNum;
        }

        /// <summary>
        /// Generate a random float
        /// </summary>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <returns>Random float</returns>
        protected float GetRandFloat(int min, int max)
        {
            //Conversion factor
            int deciConvert = 100;

            //Store the randomly generated float
            float randFloat = (float)(Game1.rng.Next(min, max + INCREMENT)) / deciConvert;
            return randFloat;
        }

        /// <summary>
        /// Set position of launcher
        /// </summary>
        /// <param name="speed">Speed that the launcher moves at</param>
        public void SetPos(int speed)
        {
            //Move the launcher along the X direction
            pos.X += speed;
        }

        /// <summary>
        /// Create the snowball
        /// </summary>
        /// <returns>The snowball object</returns>
        protected Snowball CreateSnowballs()
        {
            //Conversion 
            int deciConvert = 100;

            //Randomize the scale, lifespan, angle, and speed of the snowball
            float partScale = GetRandFloat((int)(scaleMin * deciConvert), (int)(scaleMax * deciConvert));
            int lifeSpan = GetRandInt(lifeMin, lifeMax);
            int angle = GetRandInt(angleMin, angleMax);
            float speed = GetRandFloat((int)(speedMin * deciConvert), (int)(speedMax * deciConvert));

            //Create an instance of snowball
            Snowball newSnowball = new Snowball(partImg, partScale, lifeSpan, angle,
                                speed, forces, fade, reboundScaler, new Vector2(0, 0));

            return newSnowball;
        }

        /// <summary>
        /// Update the emitter
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="platforms">List of platforms for collision detection</param>
        public void Update(GameTime gameTime, List<GameObject> platforms, MinigamePlayer player)
        {
            //Update launch timer
            launchTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //Translate the launcher
            TranslateLauncher();

            //Launch the snowballs one by one infinitely
            Launch();

            //Perform actions to snowballs based on their state
            for (int i = 0; i < snowballs.Size(); i++)
            {
                //Update snowball or remove based on state
                if (snowballs.Peek().GetState() == Snowball.ACTIVE)
                {
                    //Update snowball
                    snowballs.Peek().Update(gameTime, platforms, player);
                }
                else if (snowballs.Peek().GetState() == Snowball.DEAD)
                {
                    //Remove snowball
                    snowballs.Dequeue();
                }
            }
        }

        /// <summary>
        /// The emitter and snowballs are drawn
        /// </summary>
        /// <param name="spriteBatch">Used for drawing sprites</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //Draw snowballs from emitter if queue has snowball
            if (snowballs.Size() > NONE)
            {
                snowballs.Peek().Draw(spriteBatch);
            }
            //Draw emitter
            spriteBatch.Draw(img, GetRectangle(), Color.White);
        }

        /// <summary>
        /// Translate the launcher 
        /// </summary>
        private void TranslateLauncher()
        {
            //Alter direction if the cloud hits either the left or right platform wall
            if (pos.X + imgWidth / 2 >= Game1.screenWidth || pos.X - imgWidth / 2 <= NONE)
            {
                launcherSpeed *= DIR;
            }

            //Move the launcher
            SetPos(launcherSpeed);
        }

        /// <summary>
        /// Handle the creation and launch of the snowballs in the emitter
        /// </summary>
        private void Launch()
        {
            //Add and launch the snowballs when the launch timer is inactive
            if (!launchTimer.IsActive())
            {
                //Add a snowballs to the emitter
                snowballs.Enqueue(CreateSnowballs());

                //Launch a snowball as long as there is a snowball present
                if (snowballs.Size() > NO_SNOWBALLS)
                {
                    //Increment the amount of snowballs launched
                    numLaunched++;

                    //Launch the snowball
                    snowballs.Peek().Launch(new Vector2(pos.X, pos.Y/*emitterX, emitterY*/));
                }

                //Reset the launch timer once the snowball is launched so the next snowball can launch once it is over
                launchTimer.ResetTimer(true, GetRandInt(launchTimeMin, launchTimeMax));
            }
        }


    }
}
