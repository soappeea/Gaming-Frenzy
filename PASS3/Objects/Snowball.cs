//Author: Sophia Lin
//File Name: Snowball.cs
//Project Name: PASS3
//Creation Date: December 18, 2023
//Modified Date: December 18, 2023
//Description: Handle snowballs
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
    class Snowball : GameObject
    {
        //Store the state of the snowball
        public const int INACTIVE = 0;
        public const int ACTIVE = 1;
        public const int DEAD = 2;

        //Store the side of the snowball
        private const int TOP = 1;
        private const int BOTTOM = 2;
        private const int LEFT = 3;
        private const int RIGHT = 4;

        //Store the snowball's information
        private int lifeSpan;
        private Timer lifeTimer;
        private float angle;
        private Vector2 vel;
        private Vector2 forces;
        private int state;
        private float opacity;
        private bool fade;


        //Store the snowball's speed tolerance
        private float speedTolerance = 0.005f;

        //Damage amount snowball inflicts on player
        private const int LITTLE_HEALTH = 5;

        //Store status of whether player is damaged or not
        private bool isDamaged;

        /// <summary>
        /// Create an instance of particle
        /// </summary>
        /// <param name="img">Snowball image</param>
        /// <param name="scale">Snowball scale</param>
        /// <param name="lifeSpan">Snowball's lifespan</param>
        /// <param name="angle">Angle snowball is launched at</param>
        /// <param name="speed">Speed snowball travels at</param>
        /// <param name="forces">Forces snowball undergo</param>
        /// <param name="fade">Track if the snowball fades</param>
        /// <param name="reboundScaler">Scale of how much snowball rebounds</param>
        public Snowball(Texture2D img, float scale, int lifeSpan, int angle, float speed, Vector2 forces, bool fade, float reboundScaler, Vector2 pos) : base(img, pos, scale, reboundScaler)
        {
            //Set particle information
            this.img = img;
            this.scale = scale;
            this.rec = new Rectangle(0, 0, (int)(img.Width * scale), (int)(img.Height * scale));
            this.lifeSpan = lifeSpan;
            this.lifeTimer = new Timer(lifeSpan, false);
            this.angle = MathHelper.ToRadians(angle);
            this.vel = new Vector2((float)(speed * Math.Cos(this.angle)), -(float)(speed * Math.Sin(this.angle)));
            this.forces = forces;
            this.fade = fade;


            //Set particle state and appearance
            this.state = INACTIVE;
            this.opacity = 1f;

            //Set damage status of snowball
            isDamaged = false;
        }

        /// <summary>
        /// Retrieve the state of the snowball
        /// </summary>
        /// <returns>The state of the snowball as an int</returns>
        public int GetState()
        {
            return state;
        }

        /// <summary>
        /// Set the position 
        /// </summary>
        /// <param name="pos">Position to set snowball to</param>
        private void SetPosition(Vector2 pos)
        {
            //Set position
            this.pos = pos;

            //Centre the snowball's rectangle
            rec.X = (int)this.pos.X - rec.Width / 2;
            rec.Y = (int)this.pos.Y - rec.Height / 2;
        }

        /// <summary>
        /// Update the snowball
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="platforms">List of platforms for collision detection</param>
        public void Update(GameTime gameTime, List<GameObject> platforms, MinigamePlayer player)
        {

            //Update the life timer
            lifeTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //Handle the snowball's actions according to its timer's state
            if (lifeTimer.IsActive())
            {
                //Snowball fades as time passes
                if (fade) opacity = lifeTimer.GetTimeRemainingInt() / (float)lifeSpan;

                //Affect the snowball's velocity with forces 
                vel += forces;

                //Translate snowball
                Translate(vel * (float)gameTime.ElapsedGameTime.TotalSeconds);

                //Handle collision detection between snowballs and platforms
                if (platforms != null)
                {
                    //Detect collision between top of snowball and platforms
                    CollisionDetection(rec.X + rec.Width / 2, rec.Y, platforms, player, TOP);

                    //Detect collision between bottom of snowball and platforms
                    CollisionDetection(rec.X + rec.Width / 2, rec.Y + rec.Height, platforms, player, BOTTOM);

                    //Detect collision between left of snowball and platforms
                    CollisionDetection(rec.X, rec.Y + rec.Height / 2, platforms, player, LEFT);

                    //Detect collision between right of snowball and platforms
                    CollisionDetection(rec.X + rec.Width, rec.Y + rec.Height / 2, platforms, player, RIGHT);
                }

                //Stop the snowball when it's velocity goes below the tolerance
                if (Math.Abs(vel.X) < speedTolerance && Math.Abs(vel.Y) < speedTolerance)
                {
                    vel = Vector2.Zero;
                    forces = Vector2.Zero;
                }
            }
            else if (lifeTimer.IsFinished())
            {
                //Set snowball's state to dead when it's timer is complete
                state = DEAD;
            }
        }

        /// <summary>
        /// Draw the snowball
        /// </summary>
        /// <param name="spriteBatch">Used for drawing sprites</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //Draw the snowball if its lifespan is active
            if (state == ACTIVE && opacity > 0)
            {
                spriteBatch.Draw(img, rec, Color.White * opacity);
            }
        }

        /// <summary>
        /// Launch the snowball
        /// </summary>
        /// <param name="startPos">The position the snowball launches from</param>
        public void Launch(Vector2 startPos)
        {
            //Set the snowball state to active and launch
            if (state == INACTIVE)
            {
                state = ACTIVE;
                SetPosition(startPos);
                lifeTimer.Activate();
            }
        }

        /// <summary>
        /// Detect collision between snowball and platforms
        /// </summary>
        /// <param name="x">The x-coordinate of the snowball's midpoint</param>
        /// <param name="y">The y-coordinate of the snowball's midpoint</param>
        /// <param name="platforms">The list of platforms</param>
        /// <param name="side">The side that the snowball</param>
        private void CollisionDetection(float x, float y, List<GameObject> platforms, MinigamePlayer player, int side)
        {
            //Check for collision between every platform
            for (int i = 0; i < platforms.Count; i++)
            {
                //Collision occurred
                if (platforms[i].GetRectangle().Contains(x, y))
                {
                    //Perform adjustment and rebound the snowball based on which side collided with
                    switch (side)
                    {
                        case TOP:
                            //Reposition the snowball and adjust velocity
                            pos.Y = platforms[i].GetRectangle().Y + platforms[i].GetRectangle().Height + rec.Height / 2;
                            SetPosition(pos);
                            vel.Y *= reboundScaler;
                            break;
                        case BOTTOM:
                            //Reposition the snowball and adjust velocity
                            pos.Y = platforms[i].GetRectangle().Y - rec.Height / 2;
                            SetPosition(pos);
                            vel.Y *= reboundScaler;
                            break;
                        case LEFT:
                            //Reposition the snowball and adjust velocity
                            pos.X = platforms[i].GetRectangle().X + platforms[i].GetRectangle().Width + rec.Width / 2;
                            SetPosition(pos);
                            vel.X *= reboundScaler;
                            break;
                        case RIGHT:
                            //Reposition the snowball and adjust velocity
                            pos.X = platforms[i].GetRectangle().X - platforms[i].GetRectangle().Width / 2;
                            SetPosition(pos);
                            vel.X *= reboundScaler;
                            break;
                    }
                }
            }

            //Detect collision between player and snowball
            if (player.GetPlayerRec().Contains(x, y))
            {
                //Perform adjustment and rebound the snowball based on which side collided with
                switch (side)
                {
                    case TOP:
                        //Reposition the snowball and adjust velocity
                        pos.Y = player.GetPlayerRec().Y + player.GetPlayerRec().Height + rec.Height / 2;
                        SetPosition(pos);
                        vel.Y *= reboundScaler;
                        break;
                    case BOTTOM:
                        //Reposition the snowball and adjust velocity
                        pos.Y = player.GetPlayerRec().Y - rec.Height / 2;
                        SetPosition(pos);
                        vel.Y *= reboundScaler;
                        break;
                    case LEFT:
                        //Reposition the snowball and adjust velocity
                        pos.X = player.GetPlayerRec().X + player.GetPlayerRec().Width + rec.Width / 2;
                        SetPosition(pos);
                        vel.X *= reboundScaler;
                        break;
                    case RIGHT:
                        //Reposition the snowball and adjust velocity
                        pos.X = player.GetPlayerRec().X - player.GetPlayerRec().Width / 2;
                        SetPosition(pos);
                        vel.X *= reboundScaler;
                        break;
                }

                //Damage player if not yet damaged from particle
                if (!isDamaged)
                {
                    //Inflict damage
                    player.DamagePlayer(LITTLE_HEALTH);

                    //Set damage status to true
                    isDamaged = true;
                }
            }
        } 

        /// <summary>
        /// Translate the location 
        /// </summary>
        /// <param name="deltaVel">Pixels per update</param>
        private void Translate(Vector2 deltaVel)
        {
            //Change the particle's position
            pos += deltaVel;

            //Centre the particle's rectangle
            rec.X = (int)pos.X - rec.Width / 2;
            rec.Y = (int)pos.Y - rec.Height / 2;
        }
    }
}
