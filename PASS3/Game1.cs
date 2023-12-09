// Author: Sophia Lin
// File Name: Game1.cs
// Project Name: PASS3
// Creation Date: November 30, 2023
// Modified Date: December 9, 2023, 2023
// Description: Platformer with minigames inside
//TODO: CREATE MAP BY DESIGNING PLATFORMS
//CREATE MINIGAME #1

using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
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
        int gameState = GAMEPLAY;

        //Arrays for backgrounds
        const int MENU_BG = 0;
        const int MAIN_GAME_BG = 1;
        const int TRICK_TREAT_BG = 2;
        const int SKEW_BG = 2;
        const int AVALANCHE_BG = 3;
        const int SCAVENGER_BG = 4;
        const int HIGHSCORE_BG = 5;
        const int ENDGAME_BG = 6;
        const int INSTRUCTIONS_BG = 7;

        //Backgrounds
        Texture2D[] bgImgs = new Texture2D[8];
        Rectangle[] bgRecs = new Rectangle[8];

        //Arrays for button data
        const int PLAYBTN = 0;
        const int INSTBTN = 1;
        const int HIGHSCOREBTN = 2;
        const int EXITBTN = 3;
        const int BACKBTN = 4;
        const int MENUBTN = 5;

        //Define buttons
        Button[] btns = new Button[6];
        Texture2D[] btnImgs = new Texture2D[6];

        //Store the platforms
        Texture2D snowBlockImg;
        
        private List<GameObject> gameObjects = new List<GameObject>();

        //Different types of rebound scalers
        private float PLATFORM_REBOUND = -0.8f;


        //Store player animations
        Player player = new Player(new Vector2(0, 0));

        //Input States for Keyboard and Mouse
        KeyboardState prevKb;
        KeyboardState kb;
        MouseState prevMouse;
        MouseState mouse;

        ////Store the basic player data //idk if im gonna do camedra anymore so idk if i need below
        //Texture2D playerImg;
        //Rectangle playerRec;        //This is the player's position within the whole world
        //Vector2 playerPos;
        //Vector2 playerScreenLoc;    //This is the player's position relative to the screen only
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
            IsMouseVisible = true;

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

            // TODO: use this.Content to load your game content here///
            bgImgs[MENU_BG] = Content.Load<Texture2D>("Images/Backgrounds/MainBg");
            bgRecs[MENU_BG] = new Rectangle(0, 0, screenWidth, screenHeight);
            bgImgs[MAIN_GAME_BG] = Content.Load<Texture2D>("Images/Backgrounds/GameBg");
            bgRecs[MAIN_GAME_BG] = new Rectangle(0, 0, screenWidth, screenHeight);

            //Load button texture
            btnImgs[PLAYBTN] = Content.Load<Texture2D>("Images/Sprites/BtnPlay");
            btnImgs[INSTBTN] = Content.Load<Texture2D>("Images/Sprites/BtnInst");
            btnImgs[HIGHSCOREBTN] = Content.Load<Texture2D>("Images/Sprites/BtnHighscore");
            btnImgs[EXITBTN] = Content.Load<Texture2D>("Images/Sprites/BtnExit");
            btnImgs[BACKBTN] = Content.Load<Texture2D>("Images/Sprites/BtnBack");

            //Create button
            btns[PLAYBTN] = new Button(btnImgs[PLAYBTN], new Rectangle((screenWidth / 2) - (btnImgs[PLAYBTN].Width / 2), 300, btnImgs[PLAYBTN].Width, btnImgs[PLAYBTN].Height), Color.Gray);
            btns[INSTBTN] = new Button(btnImgs[INSTBTN], new Rectangle((screenWidth / 2) - (btnImgs[INSTBTN].Width / 2), 400, btnImgs[INSTBTN].Width, btnImgs[INSTBTN].Height), Color.Gray);
            btns[HIGHSCOREBTN] = new Button(btnImgs[HIGHSCOREBTN], new Rectangle((screenWidth / 2) - (btnImgs[HIGHSCOREBTN].Width / 2), 500, btnImgs[HIGHSCOREBTN].Width, btnImgs[HIGHSCOREBTN].Height), Color.Gray);
            btns[EXITBTN] = new Button(btnImgs[EXITBTN], new Rectangle((screenWidth / 2) - (btnImgs[EXITBTN].Width / 2), 600, btnImgs[EXITBTN].Width, btnImgs[EXITBTN].Height), Color.Gray);
            btns[BACKBTN] = new Button(btnImgs[BACKBTN], new Rectangle(50, 800, btnImgs[BACKBTN].Width, btnImgs[BACKBTN].Height), Color.Gray);

            //Load GameObjects
            snowBlockImg = Content.Load<Texture2D>("Images/Sprites/Snow Block 2");

            //Create platforms in game //each snow block's dimensions are 51x48 (48 is vertical)
            gameObjects.Add(new Platform(snowBlockImg, 3, 0.25f, 0, 852, true, PLATFORM_REBOUND));
            gameObjects.Add(new Platform(snowBlockImg, 3, 0.25f, 306, 852, true, PLATFORM_REBOUND));
            gameObjects.Add(new Platform(snowBlockImg, 3, 0.25f, 500, 775, true, PLATFORM_REBOUND));

            player.LoadAnims("PlayerAnims.csv", Content);
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
                    UpdateMenu(mouse);
                    break;
                case INSTRUCTIONS:
                    UpdateInstructions(mouse);
                    break;
                case HIGHSCORES:
                    UpdateHighscores(mouse);
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
        private void UpdateMenu(MouseState mouse)
        {
            //Update each button so it can check its new status relative to the mouse state
            for (int i = 0; i <= EXITBTN; i++)
            {
                btns[i].Update(mouse);
            }

            //Change to play or instruction or highscore screen or exit the game
            if (btns[PLAYBTN].IsHovered() && mouse.LeftButton == ButtonState.Pressed)
            {
                //Change the screen to the gameplay screen
                gameState = GAMEPLAY;

                //    buttonClickSnd.CreateInstance().Play();

                //    //Reset the stats from the previous game
                //    ResetGame();
            }
            else if (btns[INSTBTN].IsHovered() && mouse.LeftButton == ButtonState.Pressed)
            {
                //Change the screen to the instructions screen
                gameState = INSTRUCTIONS;
                //    buttonClickSnd.CreateInstance().Play();
            }
            else if (btns[HIGHSCOREBTN].IsHovered() && mouse.LeftButton == ButtonState.Pressed)
            {
                //Change the screen to the highscore screen
                gameState = HIGHSCORES;
                //    buttonClickSnd.CreateInstance().Play();
            }
            else if (btns[EXITBTN].IsHovered() && mouse.LeftButton == ButtonState.Pressed)
            {
                //Exit the game
                Exit();
            }    
        }

        //Pre: None
        //Post: None
        //Desc: Handle input in the instructions screen
        private void UpdateInstructions(MouseState mouse)
        {
            btns[BACKBTN].Update(mouse);

            //Change to menu screen
            if (btns[BACKBTN].IsHovered() && mouse.LeftButton == ButtonState.Pressed)
            {
                //Change the screen to the menu screen
                gameState = MENU;
                //    buttonClickSnd.CreateInstance().Play();
            }
        }

        //Pre: None
        //Post: None
        //Desc: Handle input in the highscores screen
        private void UpdateHighscores(MouseState mouse)
        {
            btns[BACKBTN].Update(mouse);

            //Change to menu screen
            if (btns[BACKBTN].IsHovered() && mouse.LeftButton == ButtonState.Pressed)
            {
                //Change the screen to the menu screen
                gameState = MENU;
                //    buttonClickSnd.CreateInstance().Play();
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

            player.Update(gameTime, mouse, prevMouse, kb, prevKb, gameObjects, GraphicsDevice);

            //Change to the pause screen if p key is pressed or change to gameover if player loses all health
            if (kb.IsKeyDown(Keys.P) && !prevKb.IsKeyDown(Keys.P))
            {
                //Change to pause screen
                gameState = PAUSE;

                //Pause music
                //MediaPlayer.Pause();
            }
            else if (player.GetPlayerHealth() <= 0)
            {
                gameState = GAMEOVER;
                //gameTimer.IsPaused();

                ////Play game over music
                //MediaPlayer.Stop();
                //MediaPlayer.Play(gameoverMusic);
            }

            ////Trigger the player's attack if the player clicks
            //if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed && isClicked == false)
            //{
            //    //Player clicked mouse
            //    isClicked = true;

            //    //Reset the attack timer
            //    attackTimer.ResetTimer(true);
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
            if (kb.IsKeyDown(Keys.P) && !prevKb.IsKeyDown(Keys.P))
            {
                //Change to play screen
                gameState = GAMEPLAY;

                //Resume music
                //MediaPlayer.Resume();
            }
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
            //Display background
            spriteBatch.Draw(bgImgs[MENU_BG], bgRecs[MENU_BG], Color.White);

            //Display buttons
            for (int i = 0; i <= EXITBTN; i++)
            {
                btns[i].Draw(spriteBatch);
            }

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
            spriteBatch.Draw(bgImgs[MAIN_GAME_BG], bgRecs[MENU_BG], Color.White);

            //Display back button
            btns[BACKBTN].Draw(spriteBatch);
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
            spriteBatch.Draw(bgImgs[MAIN_GAME_BG], bgRecs[MENU_BG], Color.White);
            //spriteBatch.Draw(bgImgs[HIGHSCOREBG], bgRecs[HIGHSCOREBG], Color.Gray);

            //Display back button
            btns[BACKBTN].Draw(spriteBatch);
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
            spriteBatch.Draw(bgImgs[MAIN_GAME_BG], bgRecs[MAIN_GAME_BG], Color.White);

            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].Draw(spriteBatch);
            }

            player.Draw(spriteBatch);

            ////Display clock 
            //spriteBatch.Draw(clockImg, clockRec, Color.White);
        }

        //Pre: None
        //Post: None
        //Desc: Draw the pause screen and menu interface
        private void DrawPause()
        {
            //Display background
            spriteBatch.Draw(bgImgs[MAIN_GAME_BG], bgRecs[MAIN_GAME_BG], Color.White);

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
