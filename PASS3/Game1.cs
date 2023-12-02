// Author: Sophia Lin
// File Name: Game1.cs
// Project Name: PASS3
// Creation Date: November 30, 2023
// Modified Date: November 30, 2023
// Description: Platformer with minigames inside
using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PASS3
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        //Track the screen dimensions
        int screenWidth;
        int screenHeight;

        //Track gamestates
        const int MENU = 0;
        const int INSTRUCTIONS = 1;
        const int HIGHSCORES = 2;
        const int GAMEPLAY = 3;
        const int PAUSE = 4;
        const int GAMEOVER = 5;

        //Track the current game state
        int gameState = MENU;

        //Input States for Keyboard and Mouse
        KeyboardState prevKb;
        KeyboardState kb;
        MouseState prevMouse;
        MouseState mouse;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //Setup screen dimensions
            this.graphics.PreferredBackBufferHeight = 900;
            this.graphics.PreferredBackBufferWidth = 1700;

            //Apply the screen dimension changes
            this.graphics.ApplyChanges();

            //Store the screen dimensions
            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Update all of the input states
            prevKb = kb;
            kb = Keyboard.GetState();
            prevMouse = mouse;
            mouse = Mouse.GetState();
            base.Update(gameTime);

            //Update the current gamestate
            switch (gameState)
            {
                case MENU:
                    UpdateMenu();
                    break;
                case INSTRUCTIONS:
                    UpdateInstructions();
                    break;
                case HIGHSCORES:
                    UpdateHighscores();
                    break;
                case GAMEPLAY:
                    UpdateGame(gameTime);
                    break;
                case PAUSE:
                    UpdatePause();
                    break;
                case GAMEOVER:
                    UpdateGameover();
                    break;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            switch (gameState)
            {
                case MENU:
                    DrawMenu();
                    break;
                case INSTRUCTIONS:
                    DrawInstructions();
                    break;
                case HIGHSCORES:
                    DrawHighscores();
                    break;
                    break;
                case GAMEPLAY:
                    DrawGame();
                    break;
                case PAUSE:
                    DrawPause();
                    break;
                case GAMEOVER:
                    DrawGameOver();
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        #region Update subprograms
        //Pre: None
        //Post: None
        //Desc: Handle input in the menu
        private void UpdateMenu()
        {
            //Change to the next screen state or exit if a button is pressed
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                //Change to the play or instruction or highscore screen or exit the game
                //if (btnRecs[PLAYBTN].Contains(mouse.Position))
                //{
                //    //Change the screen to the mode selection screen
                //    gameState = GAMEPLAY;
                //    buttonClickSnd.CreateInstance().Play();

                //    //Reset the stats from the previous game
                //    ResetGame();
                //}
                //else if (btnRecs[INSTBTN].Contains(mouse.Position))
                //{
                //    //Change the screen to the instruction screen
                //    gameState = INSTRUCTIONS;
                //    buttonClickSnd.CreateInstance().Play();
                //}
                //else if (btnRecs[HIGHSCOREBTN].Contains(mouse.Position))
                //{
                //    //Change the screen to the highscore screen
                //    gameState = HIGHSCORES;
                //    buttonClickSnd.CreateInstance().Play();
                //}
                //else if (btnRecs[EXITBTN].Contains(mouse.Position))
                //{
                //    //Exit the game
                //    Exit();
                //}
            }
        }

        //Pre: None
        //Post: None
        //Desc: Handle input in the instructions screen
        private void UpdateInstructions()
        {
            //Change to another screen if a button is pressed
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                ////Change to menu screen if back button is pressed 
                //if (btnRecs[BACKBTN].Contains(mouse.Position))
                //{
                //    //Change the screen to the menu screen
                //    gameState = MENU;
                //    buttonClickSnd.CreateInstance().Play();
                //}
            }
        }

        //Pre: None
        //Post: None
        //Desc: Handle input in the highscores screen
        private void UpdateHighscores()
        {
            //Change to another screen if a button is pressed
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                ////Change to menu screen if back button is pressed
                //if (btnRecs[BACKBTN].Contains(mouse.Position))
                //{
                //    //Change the screen to the menu screen
                //    gameState = MENU;
                //    buttonClickSnd.CreateInstance().Play();
                //}
            }
        }

        //Pre: None
        //Post: None
        //Desc: Handle input in the mode selections screen
        private void UpdateModes()
        {
            //Change to another screen if a button is pressed
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                ////Change the screen to the game or menu screen based on the button pressed
                //if (btnRecs[EASYBTN].Contains(mouse.Position))
                //{
                //    //Change the screen to the game screen
                //    gameState = GAMEPLAY;
                //    buttonClickSnd.CreateInstance().Play();

                //    //Start the game timer
                //    gameTimer.ResetTimer(true);

                //    //Play game music
                //    MediaPlayer.Stop();
                //    MediaPlayer.Play(gameMusic);
                //}
                //else if (btnRecs[BACKBTN].Contains(mouse.Position))
                //{
                //    //Change the screen to the menu screen
                //    gameState = MENU;
                //    buttonClickSnd.CreateInstance().Play();
                //}
            }
        }

        //Pre: None
        //Post: None
        //Desc: Handle all game related functionality
        private void UpdateGame(GameTime gameTime)
        {
            //Update animation states
            //playerAnims[playerState].Update(gameTime);
            //enemyEasyAnims[enemyEasyState].Update(gameTime);
            //enemyMediumAnims[enemyMediumState].Update(gameTime);
            //enemyHardAnims[enemyHardState].Update(gameTime);

            ////Update timers
            //doorTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //Change to the pause screen if p key is pressed or change to gameover if player loses all health
            if (kb.IsKeyDown(Keys.P) && !prevKb.IsKeyDown(Keys.P))
            {
                //Change to pause screen
                gameState = PAUSE;

                //Pause music
                MediaPlayer.Pause();
            }
            //else if (health <= 0)
            //{
            //    gameState = GAMEOVER;
            //    gameTimer.IsPaused();

            //    //Play game over music
            //    MediaPlayer.Stop();
            //    MediaPlayer.Play(gameoverMusic);
            //}

            ////Trigger the player's attack if the player clicks
            //if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed && isClicked == false)
            //{
            //    //Player clicked mouse
            //    isClicked = true;

            //    //Reset the attack timer
            //    attackTimer.ResetTimer(true);
            //}

            //////Update the player's speed based on input////
            ////Check for right and left input to accelerate the player in the chosen direction
            //if (kb.IsKeyDown(Keys.Right) || kb.IsKeyDown(Keys.D))
            //{
            //    //Change player's direction, state, and animation
            //    dir = POS;
            //    playerState = RUN;
            //    playerAnims[RUN].isAnimating = true;

            //    //Add acceleration to the player's current speed, but keep it within the limits of maxSpeed
            //    playerSpeed.X += ACCEL;

            //    //Change the speed limits and minimums based on whether a power-up has been picked up or not
            //    if (isSpeedBoosted)
            //    {
            //        //With speed boost
            //        playerSpeed.X = MathHelper.Clamp(playerSpeed.X, -maxBoostedSpeed, maxBoostedSpeed);
            //    }
            //    else
            //    {
            //        //Without speed boost
            //        playerSpeed.X = MathHelper.Clamp(playerSpeed.X, -maxSpeed, maxSpeed);
            //    }
            //}
            //else if (kb.IsKeyDown(Keys.Left) || kb.IsKeyDown(Keys.A))
            //{
            //    //Change player's direction, state, and animation
            //    dir = NEG;
            //    playerState = RUN;
            //    playerAnims[RUN].isAnimating = true;

            //    //Subtract acceleration to the player's current speed, but keep it within the limits of maxSpeed
            //    playerSpeed.X -= ACCEL;

            //    //Change the speed limits and minimums based on whether a power-up has been picked up or not
            //    if (isSpeedBoosted)
            //    {
            //        //With speed boost
            //        playerSpeed.X = MathHelper.Clamp(playerSpeed.X, -maxBoostedSpeed, maxBoostedSpeed);
            //    }
            //    else
            //    {
            //        //Without speed boost
            //        playerSpeed.X = MathHelper.Clamp(playerSpeed.X, -maxSpeed, maxSpeed);
            //    }
            //}
            //else
            //{
            //    //Only apply friction if player is on the ground and no input is given
            //    if (grounded == true)
            //    {
            //        //Decelerate if no input for horizontal movement
            //        playerSpeed.X += -Math.Sign(playerSpeed.X) * forces.X;

            //        //If the player has decelerated below the tolerance amount, set the speed to 0
            //        if (Math.Abs(playerSpeed.X) <= TOLERANCE)
            //        {
            //            //Change player state, animation, and speed
            //            playerAnims[RUN].isAnimating = false;
            //            playerState = IDLE;
            //            playerSpeed.X = 0f;
            //        }
            //    }
            //}

            ////Jump if the player hits up key or w key and is on the ground
            //if ((kb.IsKeyDown(Keys.Up) || kb.IsKeyDown(Keys.W)) && grounded == true)
            //{
            //    //Play a jump sound 
            //    jumpSnd.CreateInstance().Play();

            //    //Apply jump speed
            //    playerSpeed.Y = jumpSpeed;

            //    //Change player state and animation
            //    playerState = JUMP;
            //    playerAnims[JUMP].isAnimating = true;
            //}

            ////Only allow attack animation if the attack timer is active and the player has clicked
            //if (attackTimer.IsActive() && isClicked == true)
            //{
            //    //Change the player state and animation
            //    playerState = ATTACK;
            //    playerAnims[ATTACK].isAnimating = true;
            //}

            ////Stop the attack animation if the attack timer is finished
            //if (attackTimer.IsFinished())
            //{
            //    //Change animation
            //    playerAnims[ATTACK].isAnimating = false;

            //    //Reset for later use
            //    isClicked = false;
            //}

            ////Add gravity to the y component of the player's speed
            //playerSpeed.Y += forces.Y;

            ////Change the position of the player 
            //playerPos.X += playerSpeed.X;
            //playerPos.Y += playerSpeed.Y;
            //playerAnims[playerState].destRec.X = (int)playerPos.X;
            //playerAnims[playerState].destRec.Y = (int)playerPos.Y;

            ////Remove the enemy and its health bar if it is defeated
            //if (enemyHealth[EASY] <= 0)
            //{
            //    enemyEasyAnims[enemyEasyState].destRec = Rectangle.Empty;
            //    enemyHealthBarRec[0] = Rectangle.Empty;
            //    actualEnemyHealthBarRec[0] = Rectangle.Empty;
            //}
            //else if (enemyHealth[MEDIUM] <= 0)
            //{
            //    enemyMediumAnims[enemyMediumState].destRec = Rectangle.Empty;
            //    enemyHealthBarRec[1] = Rectangle.Empty;
            //    actualEnemyHealthBarRec[1] = Rectangle.Empty;
            //}
            //else if (enemyHealth[HARD] <= 0)
            //{
            //    enemyHardAnims[enemyHardState].destRec = Rectangle.Empty;
            //    enemyHealthBarRec[2] = Rectangle.Empty;
            //    actualEnemyHealthBarRec[2] = Rectangle.Empty;
            //}

        }

        //Pre: None
        //Post: None
        //Desc: Handle input on the pause screen 
        private void UpdatePause()
        {
            ////Change to the play screen if p key is pressed
            //if (kb.IsKeyDown(Keys.P) && !prevKb.IsKeyDown(Keys.P))
            //{
            //    //Change to play screen
            //    gameState = GAMEPLAY;

            //    //Resume music
            //    MediaPlayer.Resume();
            //}
        }

        //Pre: None
        //Post: None
        //Desc: Handle input in the game over screen
        private void UpdateGameover()
        {
            ////Change to another screen if a button is pressed
            //if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            //{
            //    //Change to menu screen if menu button is pressed 
            //    if (btnRecs[MENUBTN].Contains(mouse.Position))
            //    {
            //        //Change to menu screen
            //        gameState = MENU;
            //        buttonClickSnd.CreateInstance().Play();

            //        //Play menu music
            //        MediaPlayer.Stop();
            //        MediaPlayer.Play(menuMusic);
            //    }
            //}
        }
        #endregion

        #region Draw subprograms
        //Pre: None
        //Post: None
        //Desc: Draw the menu interface
        private void DrawMenu()
        {
            ////Display background
            //spriteBatch.Draw(bgImgs[MENU], bgRecs[MENU], Color.White);

            ////Display buttons
            //spriteBatch.Draw(btnImgs[PLAYBTN], btnRecs[PLAYBTN], Color.White);
            //spriteBatch.Draw(btnImgs[INSTBTN], btnRecs[INSTBTN], Color.White);
            //spriteBatch.Draw(btnImgs[HIGHSCOREBTN], btnRecs[HIGHSCOREBTN], Color.White);
            //spriteBatch.Draw(btnImgs[EXITBTN], btnRecs[EXITBTN], Color.White);

            ////Display title
            //spriteBatch.Draw(titleImg, titleRec, Color.White);
        }

        //Pre: None
        //Post: None
        //Desc: Draw the game instructions and back button
        private void DrawInstructions()
        {
            ////Display background
            //spriteBatch.Draw(bgImgs[MENUBG], bgRecs[MENUBG], Color.White);

            ////Display font
            //spriteBatch.Draw(bgImgs[INSTRUCTIONSBG], bgRecs[INSTRUCTIONSBG], Color.White);

            ////Display button
            //spriteBatch.Draw(btnImgs[BACKBTN], btnRecs[BACKBTN], Color.White);
        }

        //Pre: None
        //Post: None
        //Desc: Draw the highscores and back button
        private void DrawHighscores()
        {
            //Display backgrounds
            //spriteBatch.Draw(bgImgs[MENUBG], bgRecs[MENUBG], Color.White);
            //spriteBatch.Draw(bgImgs[HIGHSCOREBG], bgRecs[HIGHSCOREBG], Color.Gray);

            ////Display font
            //spriteBatch.Draw(highscoresImg, highscoresRec, Color.White);
            //spriteBatch.Draw(easyImg, easyRec, Color.White);
            //spriteBatch.Draw(mediumImg, mediumRec, Color.White);
            //spriteBatch.Draw(hardImg, hardRec, Color.White);

            ////Display button
            //spriteBatch.Draw(btnImgs[BACKBTN], btnRecs[BACKBTN], Color.White);

            //Display highscore data

        }

        //Pre: None
        //Post: None
        //Desc: Draw all game elements
        private void DrawGame()
        {
            //Display background
            //spriteBatch.Draw(bgImgs[GAMEBG], bgRecs[GAMEBG], Color.White);

        
            ////Display the player based on the direction it is facing
            //if (dir == POS)
            //{
            //    //If the player is facing right, do not flip the animation
            //    playerAnims[playerState].Draw(spriteBatch, Color.White, Animation.FLIP_NONE);
            //}
            //else if (dir == NEG)
            //{
            //    //If the player is facing left, flip the animation
            //    playerAnims[playerState].Draw(spriteBatch, Color.White, Animation.FLIP_HORIZONTAL);
            //}

            
            ////Display clock 
            //spriteBatch.Draw(clockImg, clockRec, Color.White);
        }

        //Pre: None
        //Post: None
        //Desc: Draw the pause screen and menu interface
        private void DrawPause()
        {
            ////Display background
            //spriteBatch.Draw(bgImgs[GAMEBG], bgRecs[GAMEBG], Color.White);

            ////Display title
            //spriteBatch.Draw(pausedImg, pausedRec, Color.White);

            ////Let player know that game is paused and how to unpause
            //spriteBatch.DrawString(mainFont, "PRESS P TO UNPAUSE", new Vector2(630, 475), Color.White);
        }

        //Pre: None
        //Post: None
        //Desc: Draw the Game Over screen and menu interface
        private void DrawGameOver()
        {
            ////Display background
            //spriteBatch.Draw(bgImgs[ENDGAMEBG], bgRecs[ENDGAMEBG], Color.White);

            ////Display title
            //spriteBatch.Draw(gameoverImg, gameoverRec, Color.White);

            ////Display button
            //spriteBatch.Draw(btnImgs[MENUBTN], btnRecs[MENUBTN], Color.White);

            //if (health <= 0)
            //{
            //    spriteBatch.DrawString(mainFont, "FAILED", new Vector2(725, 100), Color.DarkRed);
            //}
            //else
            //{
            //    //Display total amount of time taken for player to beat level
            //    spriteBatch.DrawString(mainFont, "Time:" + Math.Round(timeInSeconds, 2), new Vector2(690, 100), Color.White);
            //}
        }
        #endregion


    }
}
