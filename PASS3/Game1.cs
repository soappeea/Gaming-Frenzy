// Author: Sophia Lin
// File Name: Game1.cs
// Project Name: PASS3
// Creation Date: November 30, 2023
// Modified Date: January 19, 2024
// Description: Platformer with minigames inside
//TODO:
//make a paused font image
/*COURSE CONTENT USAGE:
 * 2D Arrays/Lists: 2D array for map for tile world and list for highscores, objects, and paths
 * File I/O: Loading player and enemy animations, loading and storing highscores
 * OOP: Used inheritance where necessary, encapsulated based on requirements (some constants remain public)
    * Classes are as follows:
        * Game1
        * GameObject 
            * Enemy
            * Launcher
            * MovingPlatform
            * Platform
            * Reactable
            * Skewer
            * Snowball
            * Spike
        * Player
            *  MaingamePlayer
            *  MinigamePlayer
        * GameLogo
        * ArrayQueue
        * ObjectQueue
        * Node
 * Queues: Queueing skewers in skewer minigame, queueing snowballs to fall from launchers in avalanche minigame
 * Sorting and Searching: Sorting highscores based on different filters (e.g minigame score, gems collected, total game score), 
                          Searching to check if node is found within a list, searching to detect interaction between player and specific object (e.g minigame logo)
 */
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
using System.Diagnostics;

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
        public static int screenWidth;
        public static int screenHeight;

        //Track gamestates
        private const int MENU = 0;
        private const int PRE_GAME = 1;
        private const int INSTRUCTIONS = 2;
        private const int HIGHSCORES = 3;
        private const int MAIN_GAMEPLAY = 4;
        private const int SKEWERS_GAMEPLAY = 5;
        private const int AVALANCHE_GAMEPLAY = 6;
        private const int TRICK_GAMEPLAY = 7;
        private const int PAUSE = 8;
        private const int GAMEOVER = 9;

        //Track the current game state 
        private int gameState = MENU;
        private int prevState = MAIN_GAMEPLAY;

        //Arrays for backgrounds
        private const int MENU_BG = 0;
        private const int MAIN_GAME_BG = 1;
        private const int SKEWERS_BG = 2;
        private const int AVALANCHE_BG = 3;
        private const int GAMEOVER_BG = 4;

        //Backgrounds
        private Texture2D[] bgImgs = new Texture2D[5];
        private Rectangle[] bgRecs = new Rectangle[5];

        //Arrays for button data
        private const int PLAYBTN = 0;
        private const int INSTBTN = 1;
        private const int HIGHSCOREBTN = 2;
        private const int EXITBTN = 3;
        private const int BACKBTN = 4;
        private const int MENUBTN = 5;
        private const int CONTBTN = 6;

        //Define buttons
        private Button[] btns = new Button[7];
        private Texture2D[] btnImgs = new Texture2D[7];

        //Store textbox image
        private Texture2D textboxImg;
        private Rectangle textboxRec;

        //Store text images
        private Texture2D text2Img;
        private Rectangle text2Rec;
        private Texture2D menuFontImg;
        private Rectangle menuFontRec;
        private Texture2D instFontImg;
        private Rectangle instFontRec;
        private Texture2D hsFontImg;
        private Rectangle hsFontRec;
        private Texture2D gameOverFontImg;
        private Rectangle gameOverFontRec;
        private Texture2D leaderboardImg;
        private Rectangle leaderboardRec;
        private Texture2D instructionsImg;
        private Rectangle instructionsRec;

        //Store the platforms
        private Texture2D snowBlockImg;
        private Texture2D icePlatformImg;

        //Store the logo images
        private Texture2D[] logoImgs = new Texture2D[3];

        //Store minigame object images
        private Texture2D skewerImg;
        private Texture2D blankImg;
        private Texture2D borderImg;
        private Texture2D bushImg;
        private Texture2D rockImg;
        private Texture2D trees1Img;
        private Texture2D cloudImg;
        private Texture2D snowballImg;
        private Texture2D wallImg;
        private Texture2D candy1Img;
        private Texture2D candy2Img;
        private Texture2D candy3Img;
        private Texture2D jumpBoostImg;
        private Texture2D speedBoostImg;
        private Texture2D healthBoostImg;
        private Texture2D spikeImg;
        private Texture2D portalImg;
        private Texture2D gemImg;

        //Store game-related objects
        private List<Reactable> mainGamePowerups = new List<Reactable>();
        private List<GameObject> mainGameObjects = new List<GameObject>();
        private List<Reactable> mainGameGems = new List<Reactable>();
        private List<GameLogo> minigameLogos = new List<GameLogo>();
        private List<GameObject> skewersGameObjects = new List<GameObject>();
        private List<Reactable> skewersGameGems = new List<Reactable>();
        private List<GameObject> avalancheGamePlatforms = new List<GameObject>();
        private List<Launcher> avalancheLaunchers = new List<Launcher>();
        private List<Reactable> avalancheGameGems = new List<Reactable>();
        private List<GameObject> trickGameObjects = new List<GameObject>();
        private List<Reactable> trickGameCandies = new List<Reactable>();
        private List<Reactable> trickGameGems = new List<Reactable>();
        private List<Reactable> miniGamePowerups = new List<Reactable>();
        Reactable portal;

        //Store players
        private MaingamePlayer mainPlayer = new MaingamePlayer(new Vector2(0, 737));
        private MinigamePlayer minigamePlayer = new MinigamePlayer(new Vector2(900, 800));

        //Store enemy
        private Enemy enemy = new Enemy(null, new Vector2(840, 450), 0f, GameObject.PLATFORM_REBOUND);

        //Store directions
        private const int POS = 1;
        private const int NEG = -1;

        //Store general default number
        private const int NONE = 0;

        //Store increrment value
        private const int INCREMENT = 1;

        //forces Vectors
        private Vector2 gravity = new Vector2(0, 9.8f);

        //Identifiers for boosts and gems
        private const int JUMP_BOOST = 0;
        private const int SPEED_BOOST = 1;
        private const int HEALTH_BOOST = 2;
        private const int GEM = 3;

        //Store status of name entering 
        private bool capturingName = true;

        //Main game timer
        private Timer maingameTimer;

        //Minigame timers
        private Timer skewerTimer;
        private const int SKEWER_TIME = 9000;
        private Timer skewerGameTimer;
        private Timer avalancheGameTimer;
        private Timer trickGameTimer;
        private const int MINIGAME_TIME = 30000;

        //Maintain size of tiles 
        public const int TILE_SIZE = 50;

        //Maintain tile types by number representation
        public const int CHASER = 0;
        public const int TARGET = 1;
        public const int BLANK = 2;
        public const int WALL = 3;

        //Grid scaling variables
        public const int NUM_ROWS = 900 / TILE_SIZE;
        public const int NUM_COLS = 1700 / TILE_SIZE;

        //Define Colours based on Numbered tile types
        private Color[] tileColours = new Color[] { Color.LightGreen,   //Chaser
                                            Color.DarkOrchid,   //Target
                                            Color.White,        //Blank
                                            Color.Red,          //Wall
                                                        }; 

        //Store the currently active map
        private int[,] map = new int[,]{
        {BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK},
        {BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,WALL,WALL,WALL,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,BLANK},
        {BLANK,WALL,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,WALL,WALL,WALL,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,WALL,BLANK,BLANK,WALL,WALL,WALL,WALL,BLANK},
        {BLANK,WALL,WALL,BLANK,WALL,WALL,WALL,WALL,BLANK,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,WALL,WALL},
        {BLANK,BLANK,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,WALL,WALL,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,CHASER,BLANK,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,WALL},
        {BLANK,BLANK,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK},
        {BLANK,WALL,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,WALL,BLANK,TARGET,BLANK,BLANK,BLANK,BLANK,BLANK},
        {BLANK,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,WALL,BLANK,BLANK,BLANK},
        {WALL,WALL,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,WALL,WALL,WALL,BLANK,BLANK},
        {BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,WALL,BLANK,BLANK,BLANK},
        {BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK},
        {BLANK,BLANK,BLANK,WALL,BLANK,BLANK,WALL,WALL,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK},
        {WALL,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,WALL,WALL,WALL,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,WALL,WALL,WALL,BLANK},
        {WALL,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,WALL,WALL,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK},
        {WALL,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,WALL,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK},
        {BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,WALL,WALL,WALL,BLANK,BLANK,WALL,WALL},
        {BLANK,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,WALL},
        {BLANK,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,WALL,WALL,WALL,BLANK,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,BLANK,WALL,BLANK,BLANK,BLANK,BLANK,BLANK,WALL}};

        //////////////////////////////////////
        //  PATH FINDING VARIABLES
        //////////////////////////////////////
        private const int NOT_FOUND = -1;

        //Define movement cost multiplier based on Numbered tile types
        private float[] tileCosts = new float[] { 1f,                 //Chaser (can't effect itself)
                                          1f,                 //Target (can't effect the chaser
                                          1f,                 //Blank (Standard tile type)
                                          10000f,             //Wall (Will cause no movement
                                          6f,                 //Water (6x as slow)
                                          8f,                 //Mud (8x as slow)
                                          0.5f };             //Road (movement speeds up)

        private float hvCost = 10f;                //Cost to move horizontally, vertically on standard terrain
        private float diagCost = 14f;              //Cost to move diagonally on standard terrain    

        //Maintain a map that tracks all of the tile (Node) information
        private Node[,] tileMap;

        //Track the beginning and end of the path
        private Node start;
        private Node end;

        //Track all of the path Nodes
        private List<Node> path = new List<Node>();
        private List<Vector2> pathFollowing = new List<Vector2>();

        //Track the index in the path array of the current point the object is moving toward
        private int curPoint = 0;

        //Maintain two lists, one of Nodes to check and one of potential Nodes
        private List<Node> open = new List<Node>();
        private List<Node> closed = new List<Node>();

        //Store tile image
        private Texture2D tileImg;

        //Store current row and column of player and enemy
        int playerTileRow;
        int playerTileCol;
        int enemyTileRow;
        int enemyTileCol;

        //Candy informations
        private const int CANDY1 = 0;
        private const int CANDY2 = 1;
        private const int CANDY3 = 2;
        private List<int> candiesCollected = new List<int>();
        private const int MAX_CANDY = 3;

        //Store highscore buttons
        private Texture2D[] highscoreBtnImgs = new Texture2D[5];
        private Button[] highscoreBtns = new Button[5];

        //Store maximum highscores possible to be displayed
        private const int MAX_DISPLAY_HIGHSCORES = 5;

        //Store current number of gems collected in game
        private int totalGems;

        //Store highscores 
        private List<double> currentHighscores = new List<double>();
        private List<string> playerNames = new List<string>();
        private List<double> totalHighScores = new List<double>();
        private List<double> skewerHighScores = new List<double>();
        private List<double> avalancheHighScores = new List<double>();
        private List<double> trickHighScores = new List<double>();
        private List<double> gemHighScores = new List<double>();

        //Store sorted highscore (based on whichever option is selected to be sorted at the moment)
        private List<string> sortedNames = new List<string>();
        private List<double> sortedTotalHs = new List<double>();
        private List<double> sortedSkewerHs = new List<double>();
        private List<double> sortedAvalancheHs = new List<double>();
        private List<double> sortedTrickHs = new List<double>();
        private List<double> sortedGemHs = new List<double>();

        //Store highscore identifier
        private const int TOTAL_HS = 0;
        private const int SKEWER_HS = 1;
        private const int AVALANCHE_HS = 2;
        private const int TRICK_HS = 3;
        private const int GEM_HS = 4;

        //Store current highscore being sorted
        private int currentHs = Int32.MinValue;

        //Store scores
        private int totalScore;
        private int skewerScore;
        private int avalancheScore;
        private int trickScore;
        private int[] numGems = new int[4];

        //Points a single gem is worth
        private const int GEM_FACTOR = 200;

        //Game identifier
        private const int SKEWER = 0;
        private const int AVALANCHE = 1;
        private const int TRICK = 2;
        private const int MAIN = 3;

        //=Minigame score ranges
        private const int MAX_MINIGAME_SCORE = 500;
        private const int MIDDLE_MINIGAME_SCORE = 250;
        private const int LITTLE_MINIGAME_SCORE = 20;

        //Health ranges
        private const int HIGH_HEALTH = 60;
        private const int LOW_HEALTH = 20;
        private const int ADD_HEALTH = 10;

        //Time ranges
        private const int FIRST_TIME = 120000;
        private const int SECOND_TIME = 180000;
        private const int THIRD_TIME = 240000;

        //Maingame score ranges
        private const int FIRST_SCORE = 4000;
        private const int SECOND_SCORE = 3000;
        private const int THIRD_SCORE = 2000;
        private const int FOURTH_SCORE = 1500;

        //Store player health percentage
        private float playerHealthPercent = 0f; 

        //UI element images
        private Texture2D healthBarImg;
        private Texture2D clockImg;

        //UI elements rectangles
        private Rectangle playerHealthBarRec;
        private Rectangle actualPlayerHealthBarRec;
        private Rectangle[] powerupUiRec = new Rectangle[2];
        private Rectangle clockRec;

        //Scalers for size of objects
        private float textScale = 1.5f;
        private float platformScale = 0.25f;
        private float logoScale = 0.25f;
        private float portalScale = 0.15f;
        private float spikeScale = 1.3f;
        private float gemScale = 0.05f;
        private float vertScale = 172f;
        private float horizScale = 323f;
        private float bushScale = 1.5f;
        private float rockScale = 1.5f;
        private float treeScale = 0.50f;
        private float cloudScale = 1.5f;
        private float candyScale = 0.1f;
        private float clockScaler = 1.5f;
        private float healthBarScaler = 5f;
        float boostUiScaler = 1.5f;

        //Store Sound Effects
        SoundEffect collectSnd;
        SoundEffect buttonClickSnd;

        //Store songs
        Song menuMusic;
        Song maingameMusic;
        Song gameoverMusic;
        Song skewergameMusic;
        Song avalanchegameMusic;
        Song trickgameMusic;
        Song failedgameMusic;

        //Input States for Keyboard and Mouse
        private KeyboardState prevKb;
        private KeyboardState kb;
        private MouseState prevMouse;
        private MouseState mouse;

        //Store File I/O components
        private StreamWriter outFile = null;
        private StreamReader inFile = null;

        //Store fonts
        public static SpriteFont statsFont;
        public static SpriteFont tinyStatsFont;
        private static SpriteFont mainFont;

        //Create random generator
        public static Random rng = new Random();

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

            //Read highscores file
            ReadFile("highscores.txt");

            //Load font
            statsFont = Content.Load<SpriteFont>("Fonts/StatsFont");
            tinyStatsFont = Content.Load<SpriteFont>("Fonts/TinyStatsFont");
            mainFont = Content.Load<SpriteFont>("Fonts/MainFont");

            //Load backgrounds
            bgImgs[MENU_BG] = Content.Load<Texture2D>("Images/Backgrounds/MainBg");
            bgRecs[MENU_BG] = new Rectangle(0, 0, screenWidth, screenHeight);
            bgImgs[MAIN_GAME_BG] = Content.Load<Texture2D>("Images/Backgrounds/GameBg");
            bgRecs[MAIN_GAME_BG] = new Rectangle(0, 0, screenWidth, screenHeight);
            bgImgs[SKEWERS_BG] = Content.Load<Texture2D>("Images/Backgrounds/MinigameBg");
            bgRecs[SKEWERS_BG] = new Rectangle(0, 0, screenWidth, screenHeight);
            bgImgs[AVALANCHE_BG] = Content.Load<Texture2D>("Images/Backgrounds/AvalancheBg1");
            bgRecs[AVALANCHE_BG] = new Rectangle(0, 0, screenWidth, screenHeight);
            bgImgs[GAMEOVER_BG] = Content.Load<Texture2D>("Images/Backgrounds/GameOverBg");
            bgRecs[GAMEOVER_BG] = new Rectangle(0, 0, screenWidth, screenHeight);

            //Load button texture
            btnImgs[PLAYBTN] = Content.Load<Texture2D>("Images/Sprites/BtnPlay");
            btnImgs[INSTBTN] = Content.Load<Texture2D>("Images/Sprites/BtnInst");
            btnImgs[HIGHSCOREBTN] = Content.Load<Texture2D>("Images/Sprites/BtnHighscore");
            btnImgs[EXITBTN] = Content.Load<Texture2D>("Images/Sprites/BtnExit");
            btnImgs[BACKBTN] = Content.Load<Texture2D>("Images/Sprites/BtnBack");
            btnImgs[MENUBTN] = Content.Load<Texture2D>("Images/Sprites/BtnMenu");
            btnImgs[CONTBTN] = Content.Load<Texture2D>("Images/Sprites/BtnContinue");

            //Create button
            btns[PLAYBTN] = new Button(btnImgs[PLAYBTN], new Rectangle((screenWidth / 2) - (btnImgs[PLAYBTN].Width / 2), 300, btnImgs[PLAYBTN].Width, btnImgs[PLAYBTN].Height), Color.Gray);
            btns[INSTBTN] = new Button(btnImgs[INSTBTN], new Rectangle((screenWidth / 2) - (btnImgs[INSTBTN].Width / 2), 400, btnImgs[INSTBTN].Width, btnImgs[INSTBTN].Height), Color.Gray);
            btns[HIGHSCOREBTN] = new Button(btnImgs[HIGHSCOREBTN], new Rectangle((screenWidth / 2) - (btnImgs[HIGHSCOREBTN].Width / 2), 500, btnImgs[HIGHSCOREBTN].Width, btnImgs[HIGHSCOREBTN].Height), Color.Gray);
            btns[EXITBTN] = new Button(btnImgs[EXITBTN], new Rectangle((screenWidth / 2) - (btnImgs[EXITBTN].Width / 2), 600, btnImgs[EXITBTN].Width, btnImgs[EXITBTN].Height), Color.Gray);
            btns[BACKBTN] = new Button(btnImgs[BACKBTN], new Rectangle(50, 800, btnImgs[BACKBTN].Width, btnImgs[BACKBTN].Height), Color.Gray);
            btns[MENUBTN] = new Button(btnImgs[MENUBTN], new Rectangle((screenWidth / 2) - (btnImgs[MENUBTN].Width / 2), 750, btnImgs[MENUBTN].Width, btnImgs[MENUBTN].Height), Color.Gray);
            btns[CONTBTN] = new Button(btnImgs[CONTBTN], new Rectangle((screenWidth / 2) - (btnImgs[PLAYBTN].Width / 2), 750, btnImgs[CONTBTN].Width, btnImgs[CONTBTN].Height), Color.Gray);

            //Load font images
            textboxImg = Content.Load<Texture2D>("Images/Sprites/Textbox");
            textboxRec = new Rectangle(650, 388, 400, 125);
            text2Img = Content.Load<Texture2D>("Images/Texts/TextImg2");
            text2Rec = new Rectangle(550, 100, (int)(textScale * text2Img.Width), (int)(textScale * text2Img.Height));
            menuFontImg = Content.Load<Texture2D>("Images/Texts/Title");
            menuFontRec = new Rectangle(693, 50, menuFontImg.Width, menuFontImg.Height);
            instFontImg = Content.Load<Texture2D>("Images/Texts/Inst");
            instFontRec = new Rectangle(565, 25, instFontImg.Width, instFontImg.Height);
            hsFontImg = Content.Load<Texture2D>("Images/Texts/Highscores");
            hsFontRec = new Rectangle(614, 25, hsFontImg.Width, hsFontImg.Height);
            gameOverFontImg = Content.Load<Texture2D>("Images/Texts/GameOver");
            gameOverFontRec = new Rectangle(630, 50, gameOverFontImg.Width, gameOverFontImg.Height);
            leaderboardImg = Content.Load<Texture2D>("Images/Sprites/Leaderboard");
            leaderboardRec = new Rectangle(350, 215, leaderboardImg.Width, leaderboardImg.Height);
            instructionsImg = Content.Load<Texture2D>("Images/Sprites/InstructionsDisplay");
            instructionsRec = new Rectangle(340, 150, instructionsImg.Width, instructionsImg.Height);

            //Load highscore button images
            highscoreBtnImgs[TOTAL_HS] = Content.Load<Texture2D>("Images/Sprites/HsTotal");
            highscoreBtnImgs[SKEWER_HS] = Content.Load<Texture2D>("Images/Sprites/HsSkewer");
            highscoreBtnImgs[AVALANCHE_HS] = Content.Load<Texture2D>("Images/Sprites/HsAvalanche");
            highscoreBtnImgs[TRICK_HS] = Content.Load<Texture2D>("Images/Sprites/HsTrick");
            highscoreBtnImgs[GEM_HS] = Content.Load<Texture2D>("Images/Sprites/HsGems");

            //Load highscore button rectangles
            highscoreBtns[TOTAL_HS] = new Button(highscoreBtnImgs[TOTAL_HS], new Rectangle(30, 130, btnImgs[TOTAL_HS].Width, btnImgs[TOTAL_HS].Height), Color.Gray);
            highscoreBtns[SKEWER_HS] = new Button(highscoreBtnImgs[SKEWER_HS], new Rectangle(365, 130, btnImgs[SKEWER_HS].Width, btnImgs[SKEWER_HS].Height), Color.Gray);
            highscoreBtns[AVALANCHE_HS] = new Button(highscoreBtnImgs[AVALANCHE_HS], new Rectangle(705, 130, btnImgs[AVALANCHE_HS].Width, btnImgs[AVALANCHE_HS].Height), Color.Gray);
            highscoreBtns[TRICK_HS] = new Button(highscoreBtnImgs[TRICK_HS], new Rectangle(1055, 130, btnImgs[TRICK_HS].Width, btnImgs[TRICK_HS].Height), Color.Gray);
            highscoreBtns[GEM_HS] = new Button(highscoreBtnImgs[GEM_HS], new Rectangle(1380, 130, btnImgs[GEM_HS].Width, btnImgs[GEM_HS].Height), Color.Gray);

            //Load object images
            snowBlockImg = Content.Load<Texture2D>("Images/Sprites/Snow Block 2");
            icePlatformImg = Content.Load<Texture2D>("Images/Sprites/IcePlatform");
            logoImgs[0] = Content.Load<Texture2D>("Images/Sprites/LogoSkewers");
            logoImgs[1] = Content.Load<Texture2D>("Images/Sprites/LogoAvalanche");
            logoImgs[2] = Content.Load<Texture2D>("Images/Sprites/LogoTrick");
            jumpBoostImg = Content.Load<Texture2D>("Images/Sprites/PowerUpJump");
            speedBoostImg = Content.Load<Texture2D>("Images/Sprites/PowerUpSpeed");
            healthBoostImg = Content.Load<Texture2D>("Images/Sprites/PowerUpHealth");
            spikeImg = Content.Load<Texture2D>("Images/Sprites/Spikes");
            gemImg = Content.Load<Texture2D>("Images/Sprites/Gem");
            portalImg = Content.Load<Texture2D>("Images/Sprites/Portal");

            ////MAIN GAME////
            //Set main game timer
            maingameTimer = new Timer(Timer.INFINITE_TIMER, false);

            //Create platforms in game
            //1st level of blocks
            mainGameObjects.Add(new Platform(snowBlockImg, 3, platformScale, 0, 852, true, GameObject.PLATFORM_REBOUND));  
            mainGameObjects.Add(new Platform(snowBlockImg, 3, platformScale, 290, 852, true, GameObject.PLATFORM_REBOUND));
            mainGameObjects.Add(new Platform(snowBlockImg, 1, platformScale, 710, 852, true, GameObject.PLATFORM_REBOUND));
            mainGameObjects.Add(new Platform(snowBlockImg, 2, platformScale, 800, 852, true, GameObject.PLATFORM_REBOUND));
            mainGameObjects.Add(new Platform(snowBlockImg, 1, platformScale, 1000, 852, true, GameObject.PLATFORM_REBOUND));
            mainGameObjects.Add(new Platform(snowBlockImg, 3, platformScale, 1100, 852, true, GameObject.PLATFORM_REBOUND));

            //2nd level of blocks
            mainGameObjects.Add(new Platform(snowBlockImg, 3, platformScale, 500, 750, true, GameObject.PLATFORM_REBOUND)); 

            //3rd level of blocks
            mainGameObjects.Add(new Platform(snowBlockImg, 4, platformScale, 255, 648, true, GameObject.PLATFORM_REBOUND));
            mainGameObjects.Add(new Platform(snowBlockImg, 3, platformScale, 710, 648, true, GameObject.PLATFORM_REBOUND));
            mainGameObjects.Add(new Platform(snowBlockImg, 2, platformScale, 1000, 648, true, GameObject.PLATFORM_REBOUND));

            //4th level of blocks
            mainGameObjects.Add(new Platform(snowBlockImg, 5, platformScale, 0, 546, true, GameObject.PLATFORM_REBOUND));
            mainGameObjects.Add(new Platform(snowBlockImg, 3, platformScale, 500, 546, true, GameObject.PLATFORM_REBOUND));
            mainGameObjects.Add(new Platform(snowBlockImg, 1, platformScale, 890, 546, true, GameObject.PLATFORM_REBOUND));

            //5th level of blocks
            mainGameObjects.Add(new Platform(snowBlockImg, 5, platformScale, 200, 430, true, GameObject.PLATFORM_REBOUND));

            //6th level of blocks
            mainGameObjects.Add(new Platform(snowBlockImg, 3, platformScale, 1100, 400, true, GameObject.PLATFORM_REBOUND));
            mainGameObjects.Add(new Platform(snowBlockImg, 1, platformScale, 1649, 400, true, GameObject.PLATFORM_REBOUND));

            //7th level of blocks
            mainGameObjects.Add(new Platform(snowBlockImg, 6, platformScale, 1300, 298, true, GameObject.PLATFORM_REBOUND));
            mainGameObjects.Add(new Platform(snowBlockImg, 6, platformScale, 743, 298, true, GameObject.PLATFORM_REBOUND));

            //9th level of blocks
            mainGameObjects.Add(new Platform(snowBlockImg, 6, platformScale, 408, 94, true, GameObject.PLATFORM_REBOUND));

            //Create moving platforms in game
            mainGameObjects.Add(new MovingPlatform(icePlatformImg, new Vector2(1300, 790), platformScale, GameObject.PLATFORM_REBOUND, 1f, POS));
            mainGameObjects.Add(new MovingPlatform(icePlatformImg, new Vector2(1583, 690), platformScale, GameObject.PLATFORM_REBOUND, 1.5f, NEG));
            mainGameObjects.Add(new MovingPlatform(icePlatformImg, new Vector2(1400, 590), platformScale, GameObject.PLATFORM_REBOUND, 0.5f, POS));
            mainGameObjects.Add(new MovingPlatform(icePlatformImg, new Vector2(1340, 490), platformScale, GameObject.PLATFORM_REBOUND, 1.25f, POS));

            //Add spikes to game
            int spikeHeight = 25;
            mainGameObjects.Add(new Spike(spikeImg, new Vector2(330,624),spikeScale, GameObject.PLATFORM_REBOUND));
            mainGameObjects[mainGameObjects.Count - INCREMENT].SetImgHeight(spikeHeight);
            mainGameObjects.Add(new Spike(spikeImg, new Vector2(400, 405), spikeScale, GameObject.PLATFORM_REBOUND));
            mainGameObjects[mainGameObjects.Count - INCREMENT].SetImgHeight(spikeHeight);
            mainGameObjects.Add(new Spike(spikeImg, new Vector2(1350, 275), spikeScale, GameObject.PLATFORM_REBOUND));
            mainGameObjects[mainGameObjects.Count - INCREMENT].SetImgHeight(spikeHeight);

            //Add gems to game
            mainGameGems.Add(new Reactable(gemImg, new Vector2(500, 500), gemScale, GameObject.PLATFORM_REBOUND, GEM));
            mainGameGems.Add(new Reactable(gemImg, new Vector2(825, 800), gemScale, GameObject.PLATFORM_REBOUND, GEM));
            mainGameGems.Add(new Reactable(gemImg, new Vector2(1190, 350), gemScale, GameObject.PLATFORM_REBOUND, GEM));
            mainGameGems.Add(new Reactable(gemImg, new Vector2(1650, 550), gemScale, GameObject.PLATFORM_REBOUND, GEM));

            //Add portal to game
            portal = new Reactable(portalImg, new Vector2(500, 16), portalScale, GameObject.PLATFORM_REBOUND, 0);

            //Add minigame logos in game
            minigameLogos.Add(new GameLogo(logoImgs[0], new Vector2(0, 440), logoScale, SKEWERS_GAMEPLAY));
            minigameLogos.Add(new GameLogo(logoImgs[1], new Vector2(800, 198), logoScale, AVALANCHE_GAMEPLAY));
            minigameLogos.Add(new GameLogo(logoImgs[2], new Vector2(1500, 185), logoScale, TRICK_GAMEPLAY));

            //Add powerups to maingame
            mainGamePowerups.Add(new Reactable(jumpBoostImg, new Vector2(300, 360), 1f, GameObject.PLATFORM_REBOUND, JUMP_BOOST));
            mainGamePowerups.Add(new Reactable(speedBoostImg, new Vector2(750, 550), 1f, GameObject.PLATFORM_REBOUND, SPEED_BOOST));
            mainGamePowerups.Add(new Reactable(healthBoostImg, new Vector2(750, 250), 1f, GameObject.PLATFORM_REBOUND, HEALTH_BOOST));


            ////SKEWER MINIGAME////
            //Load minigame timer and skewer timer
            skewerTimer = new Timer(SKEWER_TIME, false);
            skewerGameTimer = new Timer(MINIGAME_TIME, false);

            //Store identifier for side skewer is on
            int right = 0;
            int left = 1;
            int top = 2;
            int bottom = 3;

            //Load skewer game images
            skewerImg = Content.Load<Texture2D>("Images/Sprites/Skewer");
            blankImg = Content.Load<Texture2D>("Images/Sprites/Blank");
            borderImg = Content.Load<Texture2D>("Images/Sprites/Border");
            bushImg = Content.Load<Texture2D>("Images/Sprites/Bush");
            rockImg = Content.Load<Texture2D>("Images/Sprites/Rock");
            trees1Img = Content.Load<Texture2D>("Images/Sprites/Trees1");

            //Top left corner
            skewersGameObjects.Add(new Platform(snowBlockImg, 4, platformScale, 0, 0, true, GameObject.PLATFORM_REBOUND));
            skewersGameObjects.Add(new Platform(snowBlockImg, 4, platformScale, 0, 48, true, GameObject.PLATFORM_REBOUND));
            skewersGameObjects.Add(new Platform(snowBlockImg, 4, platformScale, 0, 96, true, GameObject.PLATFORM_REBOUND));
            skewersGameObjects.Add(new Platform(snowBlockImg, 4, platformScale, 0, 144, true, GameObject.PLATFORM_REBOUND));

            //Top right corner
            skewersGameObjects.Add(new Platform(snowBlockImg, 4, platformScale, 1496, 0, true, GameObject.PLATFORM_REBOUND));
            skewersGameObjects.Add(new Platform(snowBlockImg, 4, platformScale, 1496, 48, true, GameObject.PLATFORM_REBOUND));
            skewersGameObjects.Add(new Platform(snowBlockImg, 4, platformScale, 1496, 96, true, GameObject.PLATFORM_REBOUND));
            skewersGameObjects.Add(new Platform(snowBlockImg, 4, platformScale, 1496, 144, true, GameObject.PLATFORM_REBOUND));

            //Bottom left corner
            skewersGameObjects.Add(new Platform(snowBlockImg, 4, platformScale, 0, 708, true, GameObject.PLATFORM_REBOUND));
            skewersGameObjects.Add(new Platform(snowBlockImg, 4, platformScale, 0, 756, true, GameObject.PLATFORM_REBOUND));
            skewersGameObjects.Add(new Platform(snowBlockImg, 4, platformScale, 0, 804, true, GameObject.PLATFORM_REBOUND));
            skewersGameObjects.Add(new Platform(snowBlockImg, 4, platformScale, 0, 852, true, GameObject.PLATFORM_REBOUND));

            //Bottom right corner
            skewersGameObjects.Add(new Platform(snowBlockImg, 4, platformScale, 1496, 708, true, GameObject.PLATFORM_REBOUND));
            skewersGameObjects.Add(new Platform(snowBlockImg, 4, platformScale, 1496, 756, true, GameObject.PLATFORM_REBOUND));
            skewersGameObjects.Add(new Platform(snowBlockImg, 4, platformScale, 1496, 804, true, GameObject.PLATFORM_REBOUND));
            skewersGameObjects.Add(new Platform(snowBlockImg, 4, platformScale, 1496, 852, true, GameObject.PLATFORM_REBOUND));

            //Add skewers
            //bottom
            skewersGameObjects.Add(new Skewer(blankImg, new Vector2(204, 804), horizScale, GameObject.SKEWER_REBOUND, bottom, borderImg)); 
            skewersGameObjects.Add(new Skewer(blankImg, new Vector2(527, 804), horizScale, GameObject.SKEWER_REBOUND, bottom, borderImg));
            skewersGameObjects.Add(new Skewer(blankImg, new Vector2(850, 804), horizScale, GameObject.SKEWER_REBOUND, bottom, borderImg));
            skewersGameObjects.Add(new Skewer(blankImg, new Vector2(1173, 804), horizScale, GameObject.SKEWER_REBOUND, bottom, borderImg));

            //top
            skewersGameObjects.Add(new Skewer(blankImg, new Vector2(204, 0), horizScale, GameObject.SKEWER_REBOUND, top, borderImg));
            skewersGameObjects.Add(new Skewer(blankImg, new Vector2(527, 0), horizScale, GameObject.SKEWER_REBOUND, top, borderImg));
            skewersGameObjects.Add(new Skewer(blankImg, new Vector2(850, 0), horizScale, GameObject.SKEWER_REBOUND, top, borderImg));
            skewersGameObjects.Add(new Skewer(blankImg, new Vector2(1173, 0), horizScale, GameObject.SKEWER_REBOUND, top, borderImg));

            //left
            skewersGameObjects.Add(new Skewer(blankImg, new Vector2(0, 192), vertScale, GameObject.SKEWER_REBOUND, left, borderImg));
            skewersGameObjects.Add(new Skewer(blankImg, new Vector2(0, 364), vertScale, GameObject.SKEWER_REBOUND, left, borderImg));
            skewersGameObjects.Add(new Skewer(blankImg, new Vector2(0, 536), vertScale, GameObject.SKEWER_REBOUND, left, borderImg));

            //right
            skewersGameObjects.Add(new Skewer(blankImg, new Vector2(1598, 192), vertScale, GameObject.SKEWER_REBOUND, right, borderImg));
            skewersGameObjects.Add(new Skewer(blankImg, new Vector2(1598, 364), vertScale, GameObject.SKEWER_REBOUND, right, borderImg));
            skewersGameObjects.Add(new Skewer(blankImg, new Vector2(1598, 536), vertScale, GameObject.SKEWER_REBOUND, right, borderImg));

            //Add objects in game
            skewersGameObjects.Add(new GameObject(bushImg, new Vector2(300, 400), bushScale, GameObject.PLATFORM_REBOUND));
            skewersGameObjects.Add(new GameObject(rockImg, new Vector2(800, 400), rockScale, GameObject.PLATFORM_REBOUND));
            skewersGameObjects.Add(new GameObject(trees1Img, new Vector2(950, 575), treeScale, GameObject.PLATFORM_REBOUND));

            //Add spikes to game
            skewersGameObjects.Add(new Spike(spikeImg, new Vector2(700, 600), spikeScale, GameObject.PLATFORM_REBOUND));
            skewersGameObjects[skewersGameObjects.Count - INCREMENT].SetImgHeight(spikeHeight);
            skewersGameObjects.Add(new Spike(spikeImg, new Vector2(1300, 450), spikeScale, GameObject.PLATFORM_REBOUND));
            skewersGameObjects[skewersGameObjects.Count - INCREMENT].SetImgHeight(spikeHeight);

            //Add gems to game
            skewersGameGems.Add(new Reactable(gemImg, new Vector2(500, 500), gemScale, GameObject.PLATFORM_REBOUND, GEM));
            skewersGameGems.Add(new Reactable(gemImg, new Vector2(825, 700), gemScale, GameObject.PLATFORM_REBOUND, GEM));
           
            ////AVALANCHE GAME////
            //Load avalanche game images
            cloudImg = Content.Load<Texture2D>("Images/Sprites/Cloud");
            snowballImg = Content.Load<Texture2D>("Images/Sprites/Snowball");

            //Add avalanche launchers
            avalancheLaunchers.Add(new Launcher(cloudImg, new Vector2(125, 75), cloudScale, GameObject.SNOWBALL_REBOUND, 2000, 3500, snowballImg, 0.7f, 2f,
                                    2000, 4000, 0, 180, 5, 25, gravity, true, 1, 2));
            avalancheLaunchers.Add(new Launcher(cloudImg, new Vector2(1500, 75), cloudScale, GameObject.SNOWBALL_REBOUND, 2000, 3250, snowballImg, 0.7f, 1f,
                                   3500, 4000, 0, 180, 5, 25, gravity, true, 1, 2));
            avalancheLaunchers.Add(new Launcher(cloudImg, new Vector2(700, 75), cloudScale, GameObject.SNOWBALL_REBOUND, 2500, 3000, snowballImg, 0.1f, 1.5f,
                                  3500, 4000, 0, 180, 5, 25, gravity, true, 1, 2));

            //Load avalanche game timer
            avalancheGameTimer = new Timer(MINIGAME_TIME, false);

            //Add platforms
            avalancheGamePlatforms.Add(new Platform(snowBlockImg, 34, platformScale, 0, 804, true, GameObject.PLATFORM_REBOUND));
            avalancheGamePlatforms.Add(new Platform(snowBlockImg, 34, platformScale, 0, 852, true, GameObject.PLATFORM_REBOUND));

            //Add gems to avalanche game
            avalancheGameGems.Add(new Reactable(gemImg, new Vector2(0, 750), gemScale, GameObject.PLATFORM_REBOUND, GEM));
            avalancheGameGems.Add(new Reactable(gemImg, new Vector2(1500, 750), gemScale, GameObject.PLATFORM_REBOUND, GEM));

            ////TRICK GAME////
            //Load trick game timer
            trickGameTimer = new Timer(MINIGAME_TIME, false);
            
            //Load trick game images
            tileImg = Content.Load<Texture2D>("Images/Sprites/Tile");
            wallImg = Content.Load<Texture2D>("Images/Sprites/Snow Block 1");
            candy1Img = Content.Load<Texture2D>("Images/Sprites/Candy1");
            candy2Img = Content.Load<Texture2D>("Images/Sprites/Candy2");
            candy3Img = Content.Load<Texture2D>("Images/Sprites/Candy3");

            //Add candies to game
            trickGameCandies.Add(new Reactable(candy1Img, new Vector2(0, 800), candyScale, GameObject.PLATFORM_REBOUND, CANDY1));
            trickGameCandies.Add(new Reactable(candy2Img, new Vector2(700, 750), candyScale, GameObject.PLATFORM_REBOUND, CANDY2));
            trickGameCandies.Add(new Reactable(candy3Img, new Vector2(1650, 330), candyScale, GameObject.PLATFORM_REBOUND, CANDY3));

            //Maintain the grid size of the maps X = Rows = 12 and Y = Columns = 20 in our case
            Vector2 mapSize = new Vector2(NUM_ROWS, NUM_COLS);

            //Create 18x34 tile map from int map setup
            tileMap = new Node[NUM_ROWS, NUM_COLS];
            for (int row = 0; row < NUM_ROWS; row++)
            {
                for (int col = 0; col < NUM_COLS; col++)
                {
                    //Based on the int map array create a Node of that type in the same grid coordinates
                    tileMap[row, col] = new Node(row, col, map[row, col], tileColours[map[row, col]], mapSize);

                    //Find and track the start and end of the path
                    if (map[row, col] == CHASER)
                    {
                        start = tileMap[row, col];
                    }
                    else if (map[row, col] == TARGET)
                    {
                        end = tileMap[row, col];
                    }
                }
            }

            //Add wall objects in game (as well as in logic for pathfinding)
            for (int row = 0; row < map.GetLength(0); row++)
            {
                for (int col = 0; col < map.GetLength(1); col++)
                {
                    if (map[row, col] == WALL)
                    {
                        //Calculate the position based on tile size and grid position
                        Vector2 wallPosition = new Vector2(col * TILE_SIZE, row * TILE_SIZE);

                        //Add the wall object to the list
                        trickGameObjects.Add(new GameObject(wallImg, wallPosition, 1f, 0f));
                    }
                }
            }

            //Setup necessary pathing data, adjacent Nodes(Tiles) and the H cost from each Node to the Target
            for (int row = 0; row < NUM_ROWS; row++)
            {
                for (int col = 0; col < NUM_COLS; col++)
                {
                    //For each tile, find all valid tiles surrounding it that are not walls, off the map or obstacles
                    tileMap[row, col].SetAdjacencies(tileMap);
                }
            }

            //Add spikes to game
            trickGameObjects.Add(new Spike(spikeImg, new Vector2(325, 75), spikeScale, GameObject.PLATFORM_REBOUND));
            trickGameObjects[trickGameObjects.Count - INCREMENT].SetImgHeight(spikeHeight);
            trickGameObjects.Add(new Spike(spikeImg, new Vector2(1100, 700), spikeScale, GameObject.PLATFORM_REBOUND));
            trickGameObjects[trickGameObjects.Count - INCREMENT].SetImgHeight(spikeHeight);
            trickGameObjects.Add(new Spike(spikeImg, new Vector2(250, 700), spikeScale, GameObject.PLATFORM_REBOUND));
            trickGameObjects[trickGameObjects.Count - INCREMENT].SetImgHeight(spikeHeight);
            trickGameObjects.Add(new Spike(spikeImg, new Vector2(1300, 50), spikeScale, GameObject.PLATFORM_REBOUND));
            trickGameObjects[trickGameObjects.Count - INCREMENT].SetImgHeight(spikeHeight);
            trickGameObjects.Add(new Spike(spikeImg, new Vector2(1350, 600), spikeScale, GameObject.PLATFORM_REBOUND));
            trickGameObjects[trickGameObjects.Count - INCREMENT].SetImgHeight(spikeHeight);

            //Add gems to game
            trickGameGems.Add(new Reactable(gemImg, new Vector2(0, 750), gemScale, GameObject.PLATFORM_REBOUND, GEM));
            trickGameGems.Add(new Reactable(gemImg, new Vector2(825, 250), gemScale, GameObject.PLATFORM_REBOUND, GEM));

            //Calculate the H cost to get from EVERY tile to the end space 
            SetHCosts(tileMap, end.GetRow(), end.GetCol());

            //Load UI elements images
            healthBarImg = Content.Load<Texture2D>("Images/Sprites/HealthBar");
            clockImg = Content.Load<Texture2D>("Images/Sprites/Clock");

            //Set UI elements Rectangles
            actualPlayerHealthBarRec = new Rectangle(20, 10, (int)(healthBarImg.Width * healthBarScaler), (int)(healthBarImg.Height * healthBarScaler));
            playerHealthBarRec = new Rectangle(20, 10, (int)(healthBarImg.Width * healthBarScaler), (int)(healthBarImg.Height * healthBarScaler));
            clockRec = new Rectangle(1400, 0, (int)(clockImg.Width * clockScaler), (int)(clockImg.Height * clockScaler));
            powerupUiRec[0] = new Rectangle(1625, 80, (int)(speedBoostImg.Width * boostUiScaler), (int)(speedBoostImg.Height * boostUiScaler));
            powerupUiRec[1] = new Rectangle(1625, 160, (int)(jumpBoostImg.Width * boostUiScaler), (int)(jumpBoostImg.Height * boostUiScaler));

            //Load audio
            collectSnd = Content.Load<SoundEffect>("Audio/Sound/Collect effect");
            buttonClickSnd = Content.Load<SoundEffect>("Audio/Sound/Button Click");
            menuMusic = Content.Load<Song>("Audio/Music/Menu music");
            maingameMusic = Content.Load<Song>("Audio/Music/Game music");
            gameoverMusic = Content.Load<Song>("Audio/Music/Game over music");
            skewergameMusic = Content.Load<Song>("Audio/Music/SkewerGame Music");
            avalanchegameMusic = Content.Load<Song>("Audio/Music/AvalancheGame Music");
            trickgameMusic = Content.Load<Song>("Audio/Music/TrickGame Music");
            failedgameMusic = Content.Load<Song>("Audio/Music/FailedGameOver Music");

            //Set audio
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(menuMusic);
            SoundEffect.MasterVolume = 0.7f;

            //Load player content
            mainPlayer.LoadContent(Content);
            minigamePlayer.LoadContent(Content);

            //Load animations
            mainPlayer.LoadAnims("PlayerAnims.csv", Content);
            minigamePlayer.LoadAnims("MinigamePlayerAnims.csv", Content);
            enemy.LoadAnims("EnemyAnims.csv", Content);
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
                case PRE_GAME:
                    UpdatePregame();
                    break;
                case INSTRUCTIONS:
                    UpdateInstructions();
                    break;
                case HIGHSCORES:
                    UpdateHighscores();
                    break;
                case MAIN_GAMEPLAY:
                    UpdateMainGame(gameTime);
                    break;
                case SKEWERS_GAMEPLAY:
                    UpdateSkewersGame(gameTime);
                    break;
                case AVALANCHE_GAMEPLAY:
                    UpdateAvalancheGame(gameTime);
                    break;
                case TRICK_GAMEPLAY:
                    UpdateTrickGame(gameTime);
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

            spriteBatch.Begin();
            switch (gameState)
            {
                case MENU:
                    DrawMenu();
                    break;
                case PRE_GAME:
                    DrawPregame();
                    break;
                case INSTRUCTIONS:
                    DrawInstructions();
                    break;
                case HIGHSCORES:
                    DrawHighscores();
                    break;
                case MAIN_GAMEPLAY:
                    DrawMainGame();
                    break;
                case SKEWERS_GAMEPLAY:
                    DrawSkewersGame();
                    break;
                case AVALANCHE_GAMEPLAY:
                    DrawAvalancheGame();
                    break;
                case TRICK_GAMEPLAY:
                    DrawTrickGame();
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
        /// <summary>
        /// Handle input in the menu
        /// </summary>
        private void UpdateMenu()
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
                gameState = PRE_GAME;

                //Add new player name
                string blank = "";
                playerNames.Add(blank);
                buttonClickSnd.CreateInstance().Play();
            }
            else if (btns[INSTBTN].IsHovered() && mouse.LeftButton == ButtonState.Pressed)
            {
                //Change the screen to the instructions screen
                gameState = INSTRUCTIONS;
                buttonClickSnd.CreateInstance().Play();
            }
            else if (btns[HIGHSCOREBTN].IsHovered() && mouse.LeftButton == ButtonState.Pressed)
            {
                //Change the screen to the highscore screen
                gameState = HIGHSCORES;
                currentHs = TOTAL_HS;
                buttonClickSnd.CreateInstance().Play();
            }
            else if (btns[EXITBTN].IsHovered() && mouse.LeftButton == ButtonState.Pressed)
            {
                //Exit the game
                Exit();
            }    
        }

        /// <summary>
        /// Handle input in the pregame
        /// </summary>
        private void UpdatePregame()
        {
            //Update buttons and keyboard
            btns[CONTBTN].Update(mouse);
            btns[BACKBTN].Update(mouse);
            kb = Keyboard.GetState();

            //Add char to name if name is being captured
            if (capturingName)
            {
                //Detect for pressed key
                char pressedKey;
                bool success = TryConvertKeyboardInput(kb, prevKb, out pressedKey);
                int maxLength = 10;

                //Capture player name input
                if (success)
                {
                    if (pressedKey != (char)0 && playerNames[playerNames.Count - INCREMENT].Length < maxLength)
                    {
                        // Append the pressed key to the player's name
                        playerNames[playerNames.Count - INCREMENT] += pressedKey;
                    }
                }

                //Set previous keyboard state to current keyboard state
                prevKb = kb;
            }


            //Change to play or instruction or highscore screen or exit the game
            if (btns[CONTBTN].IsHovered() && mouse.LeftButton == ButtonState.Pressed && playerNames[playerNames.Count - INCREMENT].Length > NONE)
            {
                //Change the screen to the gameplay screen
                gameState = MAIN_GAMEPLAY;

                //Start the game timer
                maingameTimer.ResetTimer(true);

                //Play game music
                MediaPlayer.Stop();
                MediaPlayer.Play(maingameMusic);
                buttonClickSnd.CreateInstance().Play();
            }
            if (btns[BACKBTN].IsHovered() && mouse.LeftButton == ButtonState.Pressed)
            {
                //Change the screen to the menu screen
                gameState = MENU;
                playerNames.RemoveAt(playerNames.Count - 1);
                buttonClickSnd.CreateInstance().Play();
            }
        }

        /// <summary>
        /// Handle input in the instructions screen
        /// </summary>
        private void UpdateInstructions()
        {
            //Update button
            btns[BACKBTN].Update(mouse);

            //Change to menu screen if button is pressed
            if (btns[BACKBTN].IsHovered() && mouse.LeftButton == ButtonState.Pressed)
            {
                //Change the screen to the menu screen
                gameState = MENU;
                buttonClickSnd.CreateInstance().Play();
            }
        }

        /// <summary>
        /// Handle input in the highscores screen
        /// </summary>
        private void UpdateHighscores()
        {
            //Update button
            btns[BACKBTN].Update(mouse);

            for (int i = 0; i < highscoreBtns.Length; i++)
            {
                highscoreBtns[i].Update(mouse);
            }

            //Change to menu screen if button is pressed
            if (btns[BACKBTN].IsHovered() && mouse.LeftButton == ButtonState.Pressed)
            {
                //Change the screen to the menu screen
                gameState = MENU;
                buttonClickSnd.CreateInstance().Play();
            }

            //Settings based on current highscore being sorted
            for (int i = 0; i < highscoreBtns.Length; i++)
            {
                //Sort based on current button clicked
                if (highscoreBtns[i].IsHovered() && mouse.LeftButton == ButtonState.Pressed)
                {
                    //Set identifier for current highscore being sorted
                    currentHs = i + INCREMENT;

                    //Create a new list or copy contents to avoid reference sharing
                    sortedNames = new List<string>(playerNames);
                    sortedTotalHs = new List<double>(totalHighScores);
                    sortedSkewerHs = new List<double>(skewerHighScores);
                    sortedAvalancheHs = new List<double>(avalancheHighScores);
                    sortedTrickHs = new List<double>(trickHighScores);
                    sortedGemHs = new List<double>(gemHighScores);
                    buttonClickSnd.CreateInstance().Play();
                }
            }

            //Sort if the identifier has a current highscore to sort
            if (currentHs != NONE)
            {
                //Sort based on highscore selected to sort
                switch (currentHs)
                {
                    case TOTAL_HS + INCREMENT:
                        currentHighscores = SortHighscore(sortedTotalHs);
                        break;
                    case SKEWER_HS + INCREMENT:
                        currentHighscores = SortHighscore(sortedSkewerHs);
                        break;
                    case AVALANCHE_HS + INCREMENT:
                        currentHighscores = SortHighscore(sortedAvalancheHs);
                        break;
                    case TRICK_HS + INCREMENT:
                        currentHighscores = SortHighscore(sortedTrickHs);
                        break;
                    case GEM_HS + INCREMENT:
                        currentHighscores = SortHighscore(sortedGemHs);
                        break;
                }
            }
        }

        /// <summary>
        /// Handle all platformer game related functionality
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        private void UpdateMainGame(GameTime gameTime)
        {
            //Update timer
            maingameTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //Update player
            mainPlayer.Update(gameTime, mouse, prevMouse, kb, mainGameObjects, minigameLogos,
                                trickGameCandies, portal, mainGamePowerups,
                                miniGamePowerups, enemy.GetRectangles(), mainGameGems);

            //Update objects
            for (int i = 0; i < mainGameObjects.Count; i++)
            {
                mainGameObjects[i].Update(gameTime, mainGameObjects);
            }

            //Perform power up ability
            if (mainGamePowerups.Count > 0)
            {
                 //Search for current powerup being interacted wtih and perform power up ability
                 for (int i = 0; i < mainGamePowerups.Count; i++)
                {
                    //Perform ability if powerup is interacted with
                    if (mainGamePowerups[i].GetStatus())
                    {
                        //Activate powerup
                        if (mainGamePowerups[i].GetId() == JUMP_BOOST)
                        {
                            //Jump powerup
                            mainPlayer.SetForceStatus(true);
                        }
                        else if (mainGamePowerups[i].GetId() == SPEED_BOOST)
                        {
                            //Speed powerup
                            mainPlayer.SetSpeedStatus(true);
                        }
                        else if (mainGamePowerups[i].GetId() == HEALTH_BOOST)
                        {
                            //Health boost
                            mainPlayer.AddPlayerHealth(ADD_HEALTH);
                        }

                        //Remove powerup after being interacted with
                        mainGamePowerups.Remove(mainGamePowerups[i]);
                        collectSnd.CreateInstance().Play();
                    }
                }
            }

            //Collection of gems
            if (mainGameGems.Count > NONE)
            {
                //Detect if gem is interacted with, then collect
                for (int i = 0; i < mainGameGems.Count; i++)
                {
                    //Collect gems if is interacted with
                    if (mainGameGems[i].GetStatus())
                    {
                        //Add to collection of gems and remove from game
                        numGems[MAIN]++;
                        mainGameGems.Remove(mainGameGems[i]);
                        collectSnd.CreateInstance().Play();
                    }
                }
            }
        
            //Detect minigame logo collided with and send player to the minigame
            if (minigameLogos.Count > 0)
            {
                //Search for current minigame being interacted with
                for (int i = 0; i < minigameLogos.Count; i++)
                {
                    //Send to minigame if logo is interacted with
                    if (minigameLogos[i].GetStatus())
                    {
                        //Determine desired minigame state
                        gameState = minigameLogos[i].GetMinigameState();

                        //Switch to minigame based on desired minigame state
                        if (minigameLogos[i].GetMinigameState() == SKEWERS_GAMEPLAY)
                        {
                            //Allow player to move in all directions
                            minigamePlayer.SetPlayerDirs(true);

                            //Set player position
                            minigamePlayer.SetPosition(new Vector2(850, 450));

                            //Set player's speed
                            minigamePlayer.SetSpeed(MinigamePlayer.SKEWER_SPEED);

                            //Do not detect for walls, detect for skewer bound collision
                            minigamePlayer.SetWallState(false);

                            //No enemy
                            minigamePlayer.SetEnemyStatus(false);

                            //Reset minigame player health
                            minigamePlayer.SetPlayerHealth((int)Player.MAX_HEALTH);

                            //Add powerups
                            miniGamePowerups.Add(new Reactable(speedBoostImg, new Vector2(1140, 350), 1f, GameObject.PLATFORM_REBOUND, SPEED_BOOST));
                            miniGamePowerups.Add(new Reactable(healthBoostImg, new Vector2(400, 500), 1f, GameObject.PLATFORM_REBOUND, HEALTH_BOOST));

                            //Reset minigame timer
                            skewerGameTimer.ResetTimer(true);

                            //Play skewer music
                            MediaPlayer.Stop();
                            MediaPlayer.Play(skewergameMusic);
                        }
                        else if (minigameLogos[i].GetMinigameState() == AVALANCHE_GAMEPLAY)
                        {
                            //Allow player to move in all directions
                            minigamePlayer.SetPlayerDirs(false);

                            //Set player position
                            minigamePlayer.SetPosition(new Vector2(850, 762));

                            //Set player's speed
                            minigamePlayer.SetSpeed(MinigamePlayer.OTHER_SPEED);

                            //Detect for wall collision
                            minigamePlayer.SetWallState(true);

                            //No enemy
                            minigamePlayer.SetEnemyStatus(false);

                            //Reset minigame player health
                            minigamePlayer.SetPlayerHealth((int)Player.MAX_HEALTH);

                            //Add powerups
                            miniGamePowerups.Add(new Reactable(speedBoostImg, new Vector2(1140, 750), 1f, GameObject.PLATFORM_REBOUND, SPEED_BOOST));
                            miniGamePowerups.Add(new Reactable(healthBoostImg, new Vector2(400, 750), 1f, GameObject.PLATFORM_REBOUND, HEALTH_BOOST));

                            //Reset minigame timer
                            avalancheGameTimer.ResetTimer(true);

                            //Play avalanche music
                            MediaPlayer.Stop();
                            MediaPlayer.Play(avalanchegameMusic);
                        }
                        else if (minigameLogos[i].GetMinigameState() == TRICK_GAMEPLAY)
                        {
                            //Allow player to move in all directions
                            minigamePlayer.SetPlayerDirs(true);

                            //Set player position
                            minigamePlayer.SetPosition(new Vector2(850, 500));

                            //Set player's speed
                            minigamePlayer.SetSpeed(MinigamePlayer.OTHER_SPEED);

                            //Detect for wall collision
                            minigamePlayer.SetWallState(true);

                            //Activate enemy
                            minigamePlayer.SetEnemyStatus(true);

                            //Reset minigame player health
                            minigamePlayer.SetPlayerHealth((int)Player.MAX_HEALTH);

                            //Add powerups
                            miniGamePowerups.Add(new Reactable(speedBoostImg, new Vector2(1140, 350), 1f, GameObject.PLATFORM_REBOUND, SPEED_BOOST));
                            miniGamePowerups.Add(new Reactable(healthBoostImg, new Vector2(210, 750), 1f, GameObject.PLATFORM_REBOUND, HEALTH_BOOST));

                            //Reset minigame timer
                            trickGameTimer.ResetTimer(true);

                            //Play trick music
                            MediaPlayer.Stop();
                            MediaPlayer.Play(trickgameMusic);
                        }

                        //Remove minigame logo after interacted to prevent player from playing again 
                        minigameLogos.Remove(minigameLogos[i]);
                    }
                }
            }

            //Adjust the player health bars according to player health
            ModifyHealthBar();

            //Calculate score based on time passed
            if (maingameTimer.GetTimePassed() < FIRST_TIME)
            {
                //First time base score
                totalScore = FIRST_SCORE + skewerScore + avalancheScore + trickScore + (GEM_FACTOR * numGems[MAIN]);
            }
            else if (maingameTimer.GetTimePassed() >= FIRST_TIME && maingameTimer.GetTimePassed() < SECOND_TIME)
            {
                //Second time base score
                totalScore = SECOND_SCORE + skewerScore + avalancheScore + trickScore + (GEM_FACTOR * numGems[MAIN]);
            }
            else if (maingameTimer.GetTimePassed() >= SECOND_TIME && maingameTimer.GetTimePassed() < THIRD_TIME)
            {
                //Third time base score
                totalScore = THIRD_SCORE + skewerScore + avalancheScore + trickScore + (GEM_FACTOR * numGems[MAIN]);
            }
            else
            {
                //Fourth time base score
                totalScore = FOURTH_SCORE + skewerScore + avalancheScore + trickScore + (GEM_FACTOR * numGems[MAIN]);
            }


            //Change to the pause screen if p key is pressed or change to gameover if player loses all health
            if (kb.IsKeyDown(Keys.P) && !prevKb.IsKeyDown(Keys.P))
            {
                //Change to pause screen
                gameState = PAUSE;

                //Set previous state to main gameplay screen
                prevState = MAIN_GAMEPLAY;

                //Pause music
                MediaPlayer.Pause();
            }
            else if (mainPlayer.GetPlayerHealth() <= 0 || portal.GetStatus() || mainPlayer.GetPos().Y + mainPlayer.GetPlayerRec().Width > screenHeight)
            {
                //Calculate total number of gems collected
                for (int i = 0; i < numGems.Length; i++)
                {
                    totalGems += numGems[i];
                }

                //If player falls into void, they only receive points from what they gained from minigames/gem collection
                if (mainPlayer.GetPos().Y + mainPlayer.GetPlayerRec().Width > screenHeight)
                {
                    totalScore = skewerScore + avalancheScore + trickScore + (GEM_FACTOR * numGems[MAIN]);
                    //Play game over music
                    MediaPlayer.Stop();
                    MediaPlayer.Play(failedgameMusic);
                }
                else
                {
                    //Play game over music
                    MediaPlayer.Stop();
                    MediaPlayer.Play(gameoverMusic);
                }

                //Change to gameover state
                gameState = GAMEOVER;

                //Stop maingame timer
                maingameTimer.IsPaused();
            }
        }

        /// <summary>
        /// Handle all skewer game related functionality
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        private void UpdateSkewersGame(GameTime gameTime)
        {
            //Update player
            minigamePlayer.Update(gameTime, mouse, prevMouse, kb, skewersGameObjects, minigameLogos, 
                                 trickGameCandies, portal, mainGamePowerups, miniGamePowerups,
                                enemy.GetRectangles(), skewersGameGems);

            //Update timers
            skewerTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            skewerGameTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //Update skewer game objects
            for (int i = 0; i < skewersGameObjects.Count; i++)
            {
                skewersGameObjects[i].Update(gameTime, skewersGameObjects);
            }

            //Create queue for skewers
            ArrayQueue skewerQueue = new ArrayQueue(4);

            //Store number of skewers to be queued
            int numSkewers = 0;

            //Index in gameobjects for skewers
            int skewersStart = 16;
            int totSkewers = 14;

            //Generate number of skewers and queue if skewer timer is not currently active
            if (!skewerTimer.IsActive())
            {
                //Reset skewer timer
                skewerTimer.ResetTimer(true);

                //Determine random number of skewers to queue at once
                numSkewers = rng.Next(1, 4);

                //Queue random skewers
                for (int i = 0; i < numSkewers; i++)
                {
                    //Choose random index of skewer to queue
                    int index = rng.Next(skewersStart, skewersStart + totSkewers);
                    skewerQueue.Enqueue(index);
                }
            }

            //Dequeue skewer and then perform its operation
            while (skewerQueue.Size() > 0)
            {
                //Dequeue skewer
                int i = skewerQueue.Dequeue();

                //Activate current skewer's timer for duration of operation
                skewersGameObjects[i].ActivateTimer();

                //Set minigame player's damage status to false (regardless of whether it has been damaged by skewer or not)
                minigamePlayer.SetDamageStatus();
            }

            //Perform power up ability
            if (miniGamePowerups.Count > 0)
            {
                //Search for current powerup being interacted wtih and perform power up ability
                for (int i = 0; i < miniGamePowerups.Count; i++)
                {
                    //Perform ability if powerup is interacted with
                    if (miniGamePowerups[i].GetStatus())
                    {
                        //Activate powerup
                        if (miniGamePowerups[i].GetId() == SPEED_BOOST)
                        {
                            //Speed powerup
                            minigamePlayer.SetSpeedStatus(true);
                        }
                        else if (miniGamePowerups[i].GetId() == HEALTH_BOOST)
                        {
                            //Health boost
                            minigamePlayer.AddPlayerHealth(ADD_HEALTH);
                        }

                        //Remove powerup after interaction
                        miniGamePowerups.Remove(miniGamePowerups[i]);
                        collectSnd.CreateInstance().Play();
                    }
                }
            }

            //Collection of gems
            if (skewersGameGems.Count > NONE)
            {
                //Detect if gem is interacted with, then collect
                for (int i = 0; i < skewersGameGems.Count; i++)
                {
                    //Collect gems if is interacted with
                    if (skewersGameGems[i].GetStatus())
                    {
                        //Add to collection of gems and remove from game
                        numGems[SKEWER]++;
                        skewersGameGems.Remove(skewersGameGems[i]);
                        collectSnd.CreateInstance().Play();
                    }
                }
            }

            //Calculate score based on health and gems collected
            if (minigamePlayer.GetPlayerHealth() >= 60)
            {
                //Top base score
                skewerScore = MAX_MINIGAME_SCORE + (GEM_FACTOR * numGems[SKEWER]);
            }
            else if (minigamePlayer.GetPlayerHealth() < 60 && minigamePlayer.GetPlayerHealth() >= 20)
            {
                //Medium base score
                skewerScore = MIDDLE_MINIGAME_SCORE + (GEM_FACTOR * numGems[SKEWER]);
            }
            else if (minigamePlayer.GetPlayerHealth() < 20 && minigamePlayer.GetPlayerHealth() > NONE)
            {
                //Low base score
                skewerScore = LITTLE_MINIGAME_SCORE + (GEM_FACTOR * numGems[SKEWER]);
            }
            else
            {
                //No base score
                skewerScore = GEM_FACTOR * numGems[SKEWER];
            }

            //Adjust the player health bars according to player health
            ModifyHealthBar();

            //If player DIES or timer ENDS then go back to maingameplay or if game is pausing go to pause state
            if (minigamePlayer.GetPlayerHealth() <= NONE || skewerGameTimer.IsFinished())
            {
                //Set state to main gameplay
                gameState = MAIN_GAMEPLAY;

                //Clear minigame powerups
                miniGamePowerups.Clear();

                //Play main game music
                MediaPlayer.Stop();
                MediaPlayer.Play(maingameMusic);
            }
            else if (kb.IsKeyDown(Keys.P) && !prevKb.IsKeyDown(Keys.P))
            {
                //Change to pause screen
                gameState = PAUSE;

                //Set previous state to skewers screen
                prevState = SKEWERS_GAMEPLAY;

                //Pause music
                MediaPlayer.Pause();
            }
        }

        /// <summary>
        /// Handle all avalanche game related functionality
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        private void UpdateAvalancheGame(GameTime gameTime)
        {
            //Update minigame timer
            avalancheGameTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //Update player
            minigamePlayer.Update(gameTime, mouse, prevMouse, kb, avalancheGamePlatforms, minigameLogos,
                                trickGameCandies, portal, mainGamePowerups, miniGamePowerups, 
                                enemy.GetRectangles(), avalancheGameGems);

            //Perform power up ability
            if (miniGamePowerups.Count > 0)
            {
                //Search for current powerup being interacted wtih and perform power up ability
                for (int i = 0; i < miniGamePowerups.Count; i++)
                {
                    //Perform ability if powerup is interacted with
                    if (miniGamePowerups[i].GetStatus())
                    {
                        //Activate powerup
                        if (miniGamePowerups[i].GetId() == SPEED_BOOST)
                        {
                            //Speed powerup
                            minigamePlayer.SetSpeedStatus(true);
                        }
                        else if (miniGamePowerups[i].GetId() == HEALTH_BOOST)
                        {
                            //Health boost
                            minigamePlayer.AddPlayerHealth(ADD_HEALTH);
                        }

                        //Remove powerup after being interacted with
                        miniGamePowerups.Remove(miniGamePowerups[i]);
                        collectSnd.CreateInstance().Play();
                    }
                }
            }

            //Collection of gems
            if (avalancheGameGems.Count > NONE)
            {
                //Detect if gem is interacted with, then collect
                for (int i = 0; i < avalancheGameGems.Count; i++)
                {
                    //Collect gems if is interacted with
                    if (avalancheGameGems[i].GetStatus())
                    {
                        //Add to collection of gems and remove from game
                        numGems[AVALANCHE]++;
                        avalancheGameGems.Remove(avalancheGameGems[i]);
                        collectSnd.CreateInstance().Play();
                    }
                }
            }

            //Update avalanche launchers
            for (int i = 0; i < avalancheLaunchers.Count; i++)
            {
                avalancheLaunchers[i].Update(gameTime, avalancheGamePlatforms, minigamePlayer);
            }

            //Calculate score based on health and gems collected
            if (minigamePlayer.GetPlayerHealth() >= 60)
            {
                //Top base score
                avalancheScore = MAX_MINIGAME_SCORE + (GEM_FACTOR * numGems[AVALANCHE]);
            }
            else if (minigamePlayer.GetPlayerHealth() < 60 && minigamePlayer.GetPlayerHealth() >= 20)
            {
                //Medium base score
                avalancheScore = MIDDLE_MINIGAME_SCORE + (GEM_FACTOR * numGems[AVALANCHE]);
            }
            else if (minigamePlayer.GetPlayerHealth() < 20 && minigamePlayer.GetPlayerHealth() > NONE)
            {
                //Low base score
                avalancheScore = LITTLE_MINIGAME_SCORE + (GEM_FACTOR * numGems[AVALANCHE]);
            }
            else
            {
                //No base score
                avalancheScore = GEM_FACTOR * numGems[AVALANCHE];
            }

            //Adjust the player health bars according to player health
            ModifyHealthBar();

            //If player DIES or timer ENDS then go back to maingameplay or if game is pausing go to pause state
            if (minigamePlayer.GetPlayerHealth() <= NONE || avalancheGameTimer.IsFinished())
            {                     
                //Set state to main gameplay state
                gameState = MAIN_GAMEPLAY;

                //Remove minigame powerups
                miniGamePowerups.Clear();

                //Play main game music
                MediaPlayer.Stop();
                MediaPlayer.Play(maingameMusic);
            }
            else if (kb.IsKeyDown(Keys.P) && !prevKb.IsKeyDown(Keys.P))
            {
                //Change to pause screen
                gameState = PAUSE;

                //Set previous state to avalanche screen
                prevState = AVALANCHE_GAMEPLAY;

                //Pause music
                MediaPlayer.Pause();
            }
        }

        /// <summary>
        /// Handle all trick game related functionality
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        private void UpdateTrickGame(GameTime gameTime)
        {
            //Update player
            minigamePlayer.Update(gameTime, mouse, prevMouse, kb, trickGameObjects, minigameLogos,
                                trickGameCandies, portal, mainGamePowerups, miniGamePowerups,
                                enemy.GetRectangles(), trickGameGems);
            
            //Update enemy
            enemy.Update(gameTime, trickGameObjects);

            //Update minigame timer
            trickGameTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //Set row and column relative to position
            playerTileRow = (int)(minigamePlayer.GetPos().Y / TILE_SIZE); 
            playerTileCol = (int)(minigamePlayer.GetPos().X / TILE_SIZE);
            enemyTileRow = (int)(enemy.GetPos().Y / TILE_SIZE); 
            enemyTileCol = (int)(enemy.GetPos().X / TILE_SIZE); 


            //Ensure the indices are within bounds
            playerTileRow = Math.Max(0, Math.Min(playerTileRow, NUM_ROWS - 1));
            playerTileCol = Math.Max(0, Math.Min(playerTileCol, NUM_COLS - 1));
            enemyTileRow = Math.Max(0, Math.Min(enemyTileRow, NUM_ROWS - 1));
            enemyTileCol = Math.Max(0, Math.Min(enemyTileCol, NUM_COLS - 1));

            //Update the start and end positions based on player's and enemy's tile coordinates
            start = tileMap[enemyTileRow, enemyTileCol];
            end = tileMap[playerTileRow, playerTileCol];

            //Calculate the H cost to get from EVERY tile to the end space
            SetHCosts(tileMap, end.GetRow(), end.GetCol());
            path = new List<Node>();
            path = FindPath(tileMap, start, end);
        
            //Store the positions of each node in the pathfollowing list
            pathFollowing.Clear();

            //Store the node position of each node in the path found
            foreach (var node in path)
            {
                //Store node position 
                Vector2 nodePosition = new Vector2(node.GetCol() * TILE_SIZE, node.GetRow() * TILE_SIZE);

                //Add position to pathfollowing list
                pathFollowing.Add(nodePosition);
            }

            //Move enemy towards player if there is a path and the current point is not at the end of the path yet
            if (pathFollowing.Count > 0 && curPoint < pathFollowing.Count)
            {
                //Move the enemy if it is not caught up with the player yet
                if (enemy.GetRectangle().X != pathFollowing[pathFollowing.Count - 1].X ||
                    enemy.GetRectangle().Y != pathFollowing[pathFollowing.Count - 1].Y)
                {
                    //Set enemy position to amount it should move by
                    enemy.SetPos(GetMoveAmount(enemy.GetPos(), pathFollowing[curPoint], enemy.GetSpeed()));

                    //Check for to see if destination was reached, and we should now move to the next point
                    if (curPoint < pathFollowing.Count - 1 &&
                        enemy.GetRectangle().X == pathFollowing[curPoint].X &&
                        enemy.GetRectangle().Y == pathFollowing[curPoint].Y)
                    {
                        //Move to next point
                        curPoint++;
                    }
                }
            }

            //Add candies to collected candies if there are still candies remaining in game
            if (trickGameCandies.Count > 0)
            {
                //Detect if candy has been interacted with
                for (int i = 0; i < trickGameCandies.Count; i++)
                {
                    //Add candy if candy has been interacted with
                    if (trickGameCandies[i].GetStatus())
                    {
                        //Add candy to collected candy collection
                        candiesCollected.Add(i);

                        //Remove candy after interaction
                        trickGameCandies.Remove(trickGameCandies[i]);
                        collectSnd.CreateInstance().Play();
                    }
                }
            }

            //Collection of gems
            if (trickGameGems.Count > NONE)
            {
                //Detect if gem is interacted with, then collect
                for (int i = 0; i < trickGameGems.Count; i++)
                {
                    //Collect gems if is interacted with
                    if (trickGameGems[i].GetStatus())
                    {
                        //Add to collection of gems and remove from game
                        numGems[TRICK]++;
                        trickGameGems.Remove(trickGameGems[i]);
                        collectSnd.CreateInstance().Play();
                    }
                }
            }

            //Perform power up ability
            if (miniGamePowerups.Count > 0)
            {
                //Search for current powerup being interacted wtih and perform power up ability
                for (int i = 0; i < miniGamePowerups.Count; i++)
                {
                    //Perform ability if powerup is interacted with
                    if (miniGamePowerups[i].GetStatus())
                    {
                        //Activate powerup
                        if (miniGamePowerups[i].GetId() == SPEED_BOOST)
                        {
                            //Speed powerup
                            minigamePlayer.SetSpeedStatus(true);
                        }
                        else if (miniGamePowerups[i].GetId() == HEALTH_BOOST)
                        {
                            //Health boost
                            minigamePlayer.AddPlayerHealth(ADD_HEALTH);
                        }

                        //Remove powerup after being interacted with
                        miniGamePowerups.Remove(miniGamePowerups[i]);
                        collectSnd.CreateInstance().Play();
                    }
                }
            }

            //Calculate score based on health and gems collected
            if (minigamePlayer.GetPlayerHealth() >= HIGH_HEALTH)
            {
                //Top base score
                trickScore = MAX_MINIGAME_SCORE + (GEM_FACTOR * numGems[TRICK]);
            }
            else if (minigamePlayer.GetPlayerHealth() < HIGH_HEALTH && minigamePlayer.GetPlayerHealth() >= LOW_HEALTH)
            {
                //Medium base score
                trickScore = MIDDLE_MINIGAME_SCORE + (GEM_FACTOR * numGems[TRICK]);
            }
            else if (minigamePlayer.GetPlayerHealth() < LOW_HEALTH && minigamePlayer.GetPlayerHealth() > NONE)
            {
                //Low base score
                trickScore = LITTLE_MINIGAME_SCORE + (GEM_FACTOR * numGems[TRICK]);
            }
            else
            {
                //No base score
                trickScore = GEM_FACTOR * numGems[TRICK];
            }

            //Adjust the player health bars according to player health
            ModifyHealthBar();

            //If player DIES or timer ENDS then go back to maingameplay or if game is pausing go to pause state
            if (minigamePlayer.GetPlayerHealth() <= NONE || trickGameTimer.IsFinished() || candiesCollected.Count == MAX_CANDY) 
            {
                //Set game state to main gameplay screen
                gameState = MAIN_GAMEPLAY;

                //Clear minigame powerups
                miniGamePowerups.Clear();

                //Set enemy active status to false
                minigamePlayer.SetEnemyStatus(false);

                //Play main game music
                MediaPlayer.Stop();
                MediaPlayer.Play(maingameMusic);
            }
            else if (kb.IsKeyDown(Keys.P) && !prevKb.IsKeyDown(Keys.P))
            {
                //Change to pause screen
                gameState = PAUSE;

                //Set previous state to trick screen
                prevState = TRICK_GAMEPLAY;

                //Pause music
                MediaPlayer.Pause();
            }
        }

        /// <summary>
        /// Handle all pause functionality
        /// </summary>
        private void UpdatePause()
        {
            //Change to the previous screen if unpausing game
            if (kb.IsKeyDown(Keys.P) && !prevKb.IsKeyDown(Keys.P))
            {
                //Change to play screen
                gameState = prevState;

                //Resume music
                MediaPlayer.Resume();
            }
        }

        /// <summary>
        /// Handle input in the game over screen
        /// </summary>
        private void UpdateGameover()
        {
            //Update menu button
            btns[MENUBTN].Update(mouse);

            //Change to menu screen if button is pressed
            if (btns[MENUBTN].IsHovered() && mouse.LeftButton == ButtonState.Pressed)
            {
                //Change the screen to the menu screen
                gameState = MENU;

                //Add highscores
                totalHighScores.Add(totalScore);
                skewerHighScores.Add(skewerScore);
                trickHighScores.Add(trickScore);
                avalancheHighScores.Add(avalancheScore);
                gemHighScores.Add(totalGems);

                //Sort highscores according to total highscores in descending order
                if (totalHighScores.Count > 1)
                {
                    //Store the current values being inserted
                    double tempTotal;
                    string tempString;
                    double tempSkewer;
                    double tempAvalanche;
                    double tempTrick;
                    double tempGem;

                    //Store the index the value will be inserted at
                    int j;

                    //Sort based on total highscore
                    for (int i = 1; i < totalHighScores.Count; i++)
                    {
                        //Store the next element to be inserted
                        tempTotal = totalHighScores[i];
                        tempString = playerNames[i];
                        tempSkewer = skewerHighScores[i];
                        tempAvalanche = avalancheHighScores[i];
                        tempTrick = trickHighScores[i];
                        tempGem = gemHighScores[i];

                        //Shift all "sorted" elements that are greater than the new value to the right
                        for (j = i; j > 0; j--)
                        {
                            //Swap if the value is lesser than the insertion value
                            if (totalHighScores[j - 1] < tempTotal)
                            {
                                totalHighScores[j] = totalHighScores[j - 1];
                                playerNames[j] = playerNames[j - 1];
                                skewerHighScores[j] = skewerHighScores[j - 1];
                                avalancheHighScores[j] = avalancheHighScores[j - 1];
                                trickHighScores[j] = trickHighScores[j - 1];
                                gemHighScores[j] = gemHighScores[j - 1];
                            }
                            else
                            {
                                //The unsorted value is smaller, done shifting, insert now
                                break;
                            }
                        }

                        //The insertion location has been found, now insert the values
                        totalHighScores[j] = tempTotal;
                        playerNames[j] = tempString;
                        skewerHighScores[j] = tempSkewer;
                        avalancheHighScores[j] = tempAvalanche;
                        trickHighScores[j] = tempTrick;
                        gemHighScores[j] = tempGem;
                    }
                }

                //Save highscores
                SaveData("highscores.txt");

                //Reset the game
                ResetGame();
                buttonClickSnd.CreateInstance().Play();

                //Play menu music
                MediaPlayer.Stop();
                MediaPlayer.Play(menuMusic);
            }
        }
        #endregion

        #region Draw subprograms
        /// <summary>
        ///  Draw the menu interface
        /// </summary>
        private void DrawMenu()
        {
            //Display background
            spriteBatch.Draw(bgImgs[MENU_BG], bgRecs[MENU_BG], Color.White);

            //Display buttons
            for (int i = 0; i <= EXITBTN; i++)
            {
                btns[i].Draw(spriteBatch);
            }

            //Display font image
            spriteBatch.Draw(menuFontImg, menuFontRec, Color.White);
        }

        /// <summary>
        /// Draw the pregame interface
        /// </summary>
        private void DrawPregame()
        {
            //Display background
            spriteBatch.Draw(bgImgs[MENU_BG], bgRecs[MENU_BG], Color.White);

            //Display textbox and text images
            spriteBatch.Draw(textboxImg, textboxRec, Color.White);
            spriteBatch.Draw(text2Img, text2Rec, Color.White);

            //Display player name
            spriteBatch.DrawString(mainFont, playerNames[playerNames.Count - 1], new Vector2(675, 410), Color.Black);

            //Display buttons
            btns[CONTBTN].Draw(spriteBatch);
            btns[BACKBTN].Draw(spriteBatch);
       
        }

        /// <summary>
        ///  Draw the game instructions screen
        /// </summary>
        private void DrawInstructions()
        {
            //Display background
            spriteBatch.Draw(bgImgs[MAIN_GAME_BG], bgRecs[MENU_BG], Color.White);

            //Display back button
            btns[BACKBTN].Draw(spriteBatch);

            //Display font image
            spriteBatch.Draw(instFontImg, instFontRec, Color.White);

            //Display instructions
            spriteBatch.Draw(instructionsImg, instructionsRec, Color.White);
        }

        /// <summary>
        /// Draw the highscores screen
        /// </summary>
        private void DrawHighscores()
        {
            //Display backgrounds
            spriteBatch.Draw(bgImgs[MAIN_GAME_BG], bgRecs[MENU_BG], Color.White);

            //Display back button
            btns[BACKBTN].Draw(spriteBatch);

            //Display buttons to sort according to preferred highscore
            for (int i = 0; i < highscoreBtns.Length; i++)
            {
                highscoreBtns[i].Draw(spriteBatch);
            }

            //Display font image
            spriteBatch.Draw(hsFontImg, hsFontRec, Color.White);
            spriteBatch.Draw(leaderboardImg, leaderboardRec, Color.White);

            //Store how many highscores there are
            int numHighscores = NONE;

            //Display highscores if they exist
            if (currentHighscores != null)
            {
                //Display a maximum of 5 highscores on screen
                if (currentHighscores.Count > MAX_DISPLAY_HIGHSCORES)
                {
                    //Set number of highscores to display to maximum
                    numHighscores = MAX_DISPLAY_HIGHSCORES;
                }
                else
                {
                    //Set number of highscores to display to current number of highscores
                    numHighscores = currentHighscores.Count;
                }

                //Show highscores if they exist
                if (numHighscores > NONE)
                {
                    //Display highscore and their respective player
                    for (int i = 0; i < numHighscores; i++)
                    {
                        //Display info
                        spriteBatch.DrawString(statsFont, sortedNames[i], new Vector2(475, 425 + (75 * i)), Color.Black);
                        spriteBatch.DrawString(statsFont, Convert.ToString(currentHighscores[i]), new Vector2(875, 425 + (75 * i)), Color.Black);
                    }
                }
            }
            
            //Display what the highscore is based on the current type of highscore being sorted
            switch (currentHs)
            {
                case TOTAL_HS + INCREMENT:
                    spriteBatch.DrawString(mainFont, "Total Points", new Vector2(875, 310), Color.Black);
                    break;
                case SKEWER_HS + INCREMENT:
                    spriteBatch.DrawString(mainFont, "Skewer Points", new Vector2(875, 310), Color.Black);
                    break;
                case AVALANCHE_HS + INCREMENT:
                    spriteBatch.DrawString(mainFont, "Avalanche Points", new Vector2(875, 310), Color.Black);
                    break;
                case TRICK_HS + INCREMENT:
                    spriteBatch.DrawString(mainFont, "Trick Points", new Vector2(875, 310), Color.Black);
                    break;
                case GEM_HS + INCREMENT:
                    spriteBatch.DrawString(mainFont, "Gems Collected", new Vector2(875, 310), Color.Black);
                    break;
            }    
        }

        /// <summary>
        /// Draw all platformer game elements
        /// </summary>
        private void DrawMainGame()
        {
            //Display background
            spriteBatch.Draw(bgImgs[MAIN_GAME_BG], bgRecs[MAIN_GAME_BG], Color.White);

            //Display main game objects
            for (int i = 0; i < mainGameObjects.Count; i++)
            {
                mainGameObjects[i].Draw(spriteBatch);
            }

            //Display minigame logos
            for (int i = 0; i < minigameLogos.Count; i++)
            {
                minigameLogos[i].Draw(spriteBatch);
            }

            //Display powerups
            for (int i = 0; i < mainGamePowerups.Count; i++)
            {
                mainGamePowerups[i].Draw(spriteBatch);
            }

            //Display the gems
            for (int i = 0; i < mainGameGems.Count; i++)
            {
                mainGameGems[i].Draw(spriteBatch);
            }

            //Display portal
            portal.Draw(spriteBatch);

            //Display player
            mainPlayer.Draw(spriteBatch);

            //Display the player's health bar
            spriteBatch.Draw(blankImg, playerHealthBarRec, Color.Lerp(Color.Red, Color.Green, playerHealthPercent));
            spriteBatch.Draw(healthBarImg, actualPlayerHealthBarRec, Color.White);

            //Display clock 
            spriteBatch.Draw(clockImg, clockRec, Color.White);

            //Display the game information
            spriteBatch.DrawString(statsFont, maingameTimer.GetTimePassedAsString(Timer.FORMAT_MIN_SEC_MIL), new Vector2(1500, 10), Color.Black);
            spriteBatch.DrawString(statsFont, "PRESS P TO PAUSE", new Vector2(1000, 10), Color.Black);
            spriteBatch.DrawString(statsFont, "Total Score: " + totalScore, new Vector2(15, 60), Color.Black);

            //Display speed powerup status
            if (mainPlayer.GetSpeedBoosted())
            {
                spriteBatch.Draw(speedBoostImg, powerupUiRec[0], Color.White);
                spriteBatch.DrawString(tinyStatsFont, mainPlayer.GetSpeedTime(), new Vector2(1625, 100), Color.Black);
            }
            else if (!mainPlayer.GetSpeedBoosted())
            {
                spriteBatch.Draw(speedBoostImg, powerupUiRec[0], Color.Gray);
            }

            //Display jump powerup status
            if (mainPlayer.GetJumpBoosted())
            {
                spriteBatch.Draw(jumpBoostImg, powerupUiRec[1], Color.White);
                spriteBatch.DrawString(tinyStatsFont, mainPlayer.GetJumpTime(), new Vector2(1625, 180), Color.Black);
            }
            else if (!mainPlayer.GetJumpBoosted())
            {
                spriteBatch.Draw(jumpBoostImg, powerupUiRec[1], Color.Gray);
            }
        }

        /// <summary>
        /// Draw skewer game screen elements
        /// </summary>
        private void DrawSkewersGame()
        {
            //Display background
            spriteBatch.Draw(bgImgs[SKEWERS_BG], bgRecs[SKEWERS_BG], Color.White);

            //Display objects
            for (int i = 0; i < skewersGameObjects.Count; i++)
            {
                skewersGameObjects[i].Draw(spriteBatch);
            }
            
            //Display powerups
            for (int i = 0; i < miniGamePowerups.Count; i++)
            {
                miniGamePowerups[i].Draw(spriteBatch);
            }

            //Display gems
            for (int i = 0; i < skewersGameGems.Count; i++)
            {
                skewersGameGems[i].Draw(spriteBatch);
            }

            //Display the player's health bar
            spriteBatch.Draw(blankImg, playerHealthBarRec, Color.Lerp(Color.Red, Color.Green, playerHealthPercent));
            spriteBatch.Draw(healthBarImg, actualPlayerHealthBarRec, Color.White);

            //Display clock 
            spriteBatch.Draw(clockImg, clockRec, Color.White);

            //Display statistics in game
            spriteBatch.DrawString(statsFont, skewerGameTimer.GetTimeRemainingAsString(Timer.FORMAT_SEC_MIL), new Vector2(1500, 10), Color.Black);
            spriteBatch.DrawString(statsFont, "Total Score: " + skewerScore, new Vector2(15, 60), Color.Black);

            //Display speed powerup status
            if (minigamePlayer.GetSpeedBoosted())
            {
                spriteBatch.Draw(speedBoostImg, powerupUiRec[0], Color.White);
            }
            else if (!minigamePlayer.GetSpeedBoosted())
            {
                spriteBatch.Draw(speedBoostImg, powerupUiRec[0], Color.Gray);
            }

            //Display player
            minigamePlayer.Draw(spriteBatch);
        }

        /// <summary>
        /// Draw avalanche game elements
        /// </summary>
        private void DrawAvalancheGame()
        {
            //Display the background
            spriteBatch.Draw(bgImgs[AVALANCHE_BG], bgRecs[AVALANCHE_BG], Color.White);

            //Display platforms
            for (int i = 0; i < avalancheGamePlatforms.Count; i++)
            {
                avalancheGamePlatforms[i].Draw(spriteBatch);
            }

            //Display launchers
            for (int i = 0; i < avalancheLaunchers.Count; i++)
            {
                avalancheLaunchers[i].Draw(spriteBatch);
            }

            //Display powerups
            for (int i = 0; i < miniGamePowerups.Count; i++)
            {
                miniGamePowerups[i].Draw(spriteBatch);
            }

            //Display gems
            for (int i = 0; i < avalancheGameGems.Count; i++)
            {
                avalancheGameGems[i].Draw(spriteBatch);
            }

            //Display the player's health bar
            spriteBatch.Draw(blankImg, playerHealthBarRec, Color.Lerp(Color.Red, Color.Green, playerHealthPercent));
            spriteBatch.Draw(healthBarImg, actualPlayerHealthBarRec, Color.White);

            //Display clock 
            spriteBatch.Draw(clockImg, clockRec, Color.White);

            //Display game statistics
            spriteBatch.DrawString(statsFont, avalancheGameTimer.GetTimeRemainingAsString(Timer.FORMAT_SEC_MIL), new Vector2(1500, 10), Color.White);
            spriteBatch.DrawString(statsFont, "Total Score: " + avalancheScore, new Vector2(15, 60), Color.White);

            //Display speed powerup status
            if (minigamePlayer.GetSpeedBoosted())
            {
                spriteBatch.Draw(speedBoostImg, powerupUiRec[0], Color.White);
            }
            else if (!minigamePlayer.GetSpeedBoosted())
            {
                spriteBatch.Draw(speedBoostImg, powerupUiRec[0], Color.Gray);
            }

            //Display player
            minigamePlayer.Draw(spriteBatch);
        }

        /// <summary>
        /// Draw all trick game elements
        /// </summary>
        private void DrawTrickGame()
        {
            //Display the background
            spriteBatch.Draw(bgImgs[SKEWERS_BG], bgRecs[SKEWERS_BG], Color.White);

            //Display objects
            for (int i = 0; i < trickGameObjects.Count; i++)
            {
                trickGameObjects[i].Draw(spriteBatch);
            }

            //Display candies
            for (int i = 0; i < trickGameCandies.Count; i++)
            {
                trickGameCandies[i].Draw(spriteBatch);
            }

            //Display powerups
            for (int i = 0; i < miniGamePowerups.Count; i++)
            {
                miniGamePowerups[i].Draw(spriteBatch);
            }

            //Display gems
            for (int i = 0; i < trickGameGems.Count; i++)
            {
                trickGameGems[i].Draw(spriteBatch);
            }

            //Display the player's health bar
            spriteBatch.Draw(blankImg, playerHealthBarRec, Color.Lerp(Color.Red, Color.Green, playerHealthPercent));
            spriteBatch.Draw(healthBarImg, actualPlayerHealthBarRec, Color.White);

            //Display clock 
            spriteBatch.Draw(clockImg, clockRec, Color.White);

            //Display statistics
            spriteBatch.DrawString(statsFont, "Total Score: " + trickScore, new Vector2(15, 60), Color.Black);
            spriteBatch.DrawString(statsFont, trickGameTimer.GetTimeRemainingAsString(Timer.FORMAT_SEC_MIL), new Vector2(1500, 10), Color.Black);

            //Display speed powerup status
            if (minigamePlayer.GetSpeedBoosted())
            {
                spriteBatch.Draw(speedBoostImg, powerupUiRec[0], Color.White);
            }
            else if (!minigamePlayer.GetSpeedBoosted())
            {
                spriteBatch.Draw(speedBoostImg, powerupUiRec[0], Color.Gray);
            }

            //Display player
            minigamePlayer.Draw(spriteBatch);

            //Display enemy
            enemy.Draw(spriteBatch);
        }

        /// <summary>
        /// Draw the pause screen
        /// </summary>
        private void DrawPause() 
        {
            //Display background
            spriteBatch.Draw(bgImgs[MAIN_GAME_BG], bgRecs[MAIN_GAME_BG], Color.White);

            //Display title
            //spriteBatch.Draw(pausedImg, pausedRec, Color.White);

            //Let player know that game is paused and how to unpause
            spriteBatch.DrawString(mainFont, "PRESS P TO UNPAUSE", new Vector2(500, 400), Color.Black);
        }

        /// <summary>
        /// Draw the game over screen
        /// </summary>
        private void DrawGameOver()
        {
            //Display background
            spriteBatch.Draw(bgImgs[GAMEOVER_BG], bgRecs[GAMEOVER_BG], Color.White);

            //Display font image
            spriteBatch.Draw(gameOverFontImg, gameOverFontRec, Color.White);

            //Display final scores
            spriteBatch.DrawString(statsFont, "Skewer Score: " + skewerScore, new Vector2(700, 200), Color.Black);
            spriteBatch.DrawString(statsFont, "Avalanche Score: " + avalancheScore, new Vector2(700, 300), Color.Black);
            spriteBatch.DrawString(statsFont, "Trick Score: " + trickScore, new Vector2(700, 400), Color.Black);
            spriteBatch.DrawString(statsFont, "Score: " + totalScore, new Vector2(700, 500), Color.Black);

            //Display button
            btns[MENUBTN].Draw(spriteBatch);
        }
        #endregion

        #region Pathfinding subprograms
        /// <summary>
        /// Find the shortest path, if it exists from the starting chaser tile to the ending target tile
        /// </summary>
        /// <param name="map">The collection of all the tiles in the game world</param>
        /// <param name="start">The tile the path is to begin at</param>
        /// <param name="end">The tile the path is to end at</param>
        /// <returns>An ordered sequence of grid coordinates representing the shortest path from start to end, the sequence is empty if no path exists</returns>
        private List<Node> FindPath(Node[,] map, Node start, Node end)
        {
            //Maintain a resulting path to return
            List<Node> result = new List<Node>();
            //open.Clear();
            //closed.Clear();


            //Variables to be recalculated in each iteration of finding potential path nodes
            float minF = 10000f;        
            int minIndex = 0;           
            Node curNode;                

            //Set start G cost to of start Node to 0, since it cost nothing to go from start to start
            start.SetGVal(NONE);
            start.SetFVal(start.GetGVal() + start.GetHVal());

            //Add enemy to the open list
            open.Add(start);

            //Loop until a path is found or it is impossible to find a path
            while (true)
            {
                //Find smallest F cost in open list, consider it the current node and remove it from the open list
                minF = 10000f;
                for (int i = 0; i < open.Count; i++)
                {
                    if (open[i].GetFVal() < minF)
                    {
                        //Set the current minimum F and index it is at
                        minF = open[i].GetFVal();
                        minIndex = i;
                    }
                }

                //Minimum F cost has been found at minIndex, setup the current Node by
                //tracking it, removing it from the open list and adding it to the closed list
                curNode = open[minIndex];
                open.RemoveAt(minIndex);
                closed.Add(curNode);

                //If the added node is the target, then stop (Path found)
                if (curNode.GetId() == end.GetId())
                {
                    //Path found, stop searching
                    break;
                }

                //Go through current node's adjacent Nodes and perform actions to it
                Node compNode;
                for (int i = 0; i < curNode.GetAdjacent().Count; i++)
                {
                    //Retrieve the next adjacent Node of curNode, this will be our comparison Node
                    compNode = curNode.GetAdjacent()[i];

                    //Check if adjacent node is not in the closed list and it is a walkable type
                    if(compNode.GetTileType() != WALL && /*(*/ContainsNode(closed, compNode) == NOT_FOUND/* || compNode.GetParent() == null)*/)
                    {
                        //At this point we know that compNode will be added or is in the open list.
                        float newG = GetGCost(compNode, curNode);

                        //Calculate the H cost to get from EVERY tile to the end space
                        SetHCosts(map, end.GetRow(), end.GetCol());

                        //Check the Open List
                        if (ContainsNode(open, compNode) == NOT_FOUND)
                        {
                            //Set parent
                            compNode.SetParent(curNode);
                            compNode.SetGVal(newG);
                            compNode.SetFVal(compNode.GetGVal() + compNode.GetHVal());

                            //Add it to the open list
                            open.Add(compNode);
                        }
                        else
                        {
                            //Adjacent node is in the open list, compare its current G against its potential new G to see which is better(lower)
                            if (newG < compNode.GetGVal())
                            {
                                //The new parent is a better parent, reset compNode's parent and G, F Costs to reflect this
                                compNode.SetParent(curNode);
                                compNode.SetGVal(newG);
                                compNode.SetFVal(compNode.GetGVal() + compNode.GetHVal());
                            }
                        }
                    }
                }

                //If the open list is empty then stop(No path possible)
                if (open.Count == 0)
                {
                    //Path is not possible, stop searching
                    break;
                }
            }

            //If a path is found, retrace the steps starting at the end going through each parent until the start is reached
            if (ContainsNode(closed, end) != NOT_FOUND)
            {
                //If a path was found, trace it back from the end Node
                Node pathNode = end;

                //Keep tracking back until the start Node, which has no parent is reached
                while (pathNode != null)
                {
                    //Always add the next path Node to the front of the list to maintain order
                    result.Insert(0, pathNode);
                    
                    //Get the next parent
                    pathNode = pathNode.GetParent();
                }
            }

            //Return the resulting path
            return result;
        }

        /// <summary>
        /// Determine the H cost from the given grid coordinate to the target using the Manhattan heuristic
        /// </summary>
        /// <param name="tileRow">The row of the current tile</param>
        /// <param name="tileCol">The column of the current tile</param>
        /// <param name="targetRow">The row of the target tile</param>
        /// <param name="targetCol">The column of the target tile</param>
        /// <returns>The cost to get from the current tile to the Target</returns>
        private float GetHCost(int tileRow, int tileCol, int targetRow, int targetCol)
        {
            //Cost to move from the current location to the target making only horizontal and vertical movements
            return (float)Math.Abs(targetRow - tileRow) * hvCost + (float)Math.Abs(targetCol - tileCol) * hvCost;
        }

        /// <summary>
        /// Calculate the H cost of all tiles to the target
        /// </summary>
        /// <param name="map">The collection of all the tiles in the game world</param>
        /// <param name="targetRow">The row of the target tile</param>
        /// <param name="targetCol">The column of the target tile</param>
        private void SetHCosts(Node[,] map, int targetRow, int targetCol)
        {
            //Set cost for each title to end
            for (int row = 0; row < NUM_ROWS; row++)
            {
                for (int col = 0; col < NUM_COLS; col++)
                {
                    //Calculate and set the cost for EACH tile to the end space
                    map[row, col].SetHVal(GetHCost(row, col, targetRow, targetCol));
                    map[row, col].SetFVal(map[row, col].GetGVal() + map[row, col].GetHVal());
                }
            }
        }

        /// <summary>
        /// Calculate the cost from the starting chaser location to the given tile
        /// </summary>
        /// <param name="compNode">The tile being tested</param>
        /// <param name="parentNode">The parent of the tile being tested</param>
        /// <returns>The cumulative cost to get from the starting chaser tile to the tile being tested</returns>
        private float GetGCost(Node compNode, Node parentNode)
        {
            //Detect compNode's position relative to parentNode and calculate G cost
            if (compNode.GetRow() == parentNode.GetRow() || compNode.GetCol() == parentNode.GetCol())
            {
                //compNode is either directly horizontal or vertical to curNode
                return parentNode.GetGVal() + hvCost * tileCosts[compNode.GetTileType()];
            }
            else
            {
                //compNode is diagonal to curNode
                return parentNode.GetGVal() + diagCost * tileCosts[compNode.GetTileType()];
            }
        }

        /// <summary>
        /// Determine if a given tile exists within a given collection of tiles
        /// </summary>
        /// <param name="nodeList">The collection to check within</param>
        /// <param name="checkNode">The tile to be searched for</param>
        /// <returns>True if checkNode is inside nodeList, False otherwise</returns>
        private int ContainsNode(List<Node> nodeList, Node checkNode)
        {
            //Search if any nodes in nodeList are the node trying to be found
            for (int i = 0; i < nodeList.Count; i++)
            {
                //If both Nodes have the same unique ID (Node number), they are a match
                if (nodeList[i].GetId() == checkNode.GetId())
                {
                    //Node found in list, return the index
                    return i;
                }
            }

            //Node was not found in entire list, return invalid index
            return NOT_FOUND;
        }

        /// <summary>
        /// Calculate the direction and movement amount for the object to progress towards its destination
        /// </summary>
        /// <param name="location">The current location of the mover</param>
        /// <param name="destination">The target location of the mover</param>
        /// <param name="moveSpeed">The mover's movement speed</param>
        /// <returns></returns>
        private Vector2 GetMoveAmount(Vector2 location, Vector2 destination, float moveSpeed)
        {
            //Track the amount the object will move towards its destination in ONE update
            Vector2 moveAmount = new Vector2(0, 0);

            //Calculate the Vector (line) between the location and destination point
            Vector2 moveDirection = destination - location;

            //Get the distance between the location and the destination in terms of X and Y components
            float distX = moveDirection.X;
            float distY = moveDirection.Y;

            //Normalize the direction vector
            moveDirection.Normalize();

            //Move the enemy
            if (!float.IsNaN(moveDirection.X) && !float.IsNaN(moveDirection.Y))
            {
                moveAmount.X = Math.Sign(distX) * Math.Min(Math.Abs(moveDirection.X) * moveSpeed, Math.Abs(distX));
                moveAmount.Y = Math.Sign(distY) * Math.Min(Math.Abs(moveDirection.Y) * moveSpeed, Math.Abs(distY));
            }

            //Return the resulting move changes
            return moveAmount;
        }
        #endregion

        #region Game Functionality subprograms
        /// <summary>
        /// Reset the game 
        /// </summary>
        private void ResetGame()
        {
            //Set previous state to platformer main game
            prevState = MAIN_GAMEPLAY;

            //Add minigame logos back
            minigameLogos.Add(new GameLogo(logoImgs[0], new Vector2(0, 440), logoScale, SKEWERS_GAMEPLAY));
            minigameLogos.Add(new GameLogo(logoImgs[1], new Vector2(800, 198), logoScale, AVALANCHE_GAMEPLAY));
            minigameLogos.Add(new GameLogo(logoImgs[2], new Vector2(1500, 185), logoScale, TRICK_GAMEPLAY));

            //Add powerups back
            mainGamePowerups.Add(new Reactable(jumpBoostImg, new Vector2(300, 360), 1f, GameObject.PLATFORM_REBOUND, JUMP_BOOST));
            mainGamePowerups.Add(new Reactable(speedBoostImg, new Vector2(1140, 350), 1f, GameObject.PLATFORM_REBOUND, SPEED_BOOST));
            mainGamePowerups.Add(new Reactable(healthBoostImg, new Vector2(750, 250), 1f, GameObject.PLATFORM_REBOUND, HEALTH_BOOST));

            //Add gems back to main game
            mainGameGems.Add(new Reactable(gemImg, new Vector2(500, 500), gemScale, GameObject.PLATFORM_REBOUND, GEM));
            mainGameGems.Add(new Reactable(gemImg, new Vector2(825, 800), gemScale, GameObject.PLATFORM_REBOUND, GEM));
            mainGameGems.Add(new Reactable(gemImg, new Vector2(1190, 350), gemScale, GameObject.PLATFORM_REBOUND, GEM));
            mainGameGems.Add(new Reactable(gemImg, new Vector2(1650, 550), gemScale, GameObject.PLATFORM_REBOUND, GEM));

            //Add gems back to skewer game
            skewersGameGems.Add(new Reactable(gemImg, new Vector2(500, 500), gemScale, GameObject.PLATFORM_REBOUND, GEM));
            skewersGameGems.Add(new Reactable(gemImg, new Vector2(825, 700), gemScale, GameObject.PLATFORM_REBOUND, GEM));

            //Add gems back to avalanche game
            avalancheGameGems.Add(new Reactable(gemImg, new Vector2(0, 750), gemScale, GameObject.PLATFORM_REBOUND, GEM));
            avalancheGameGems.Add(new Reactable(gemImg, new Vector2(1500, 750), gemScale, GameObject.PLATFORM_REBOUND, GEM));

            //Add gems back to trick game
            trickGameGems.Add(new Reactable(gemImg, new Vector2(0, 750), gemScale, GameObject.PLATFORM_REBOUND, GEM));
            trickGameGems.Add(new Reactable(gemImg, new Vector2(825, 250), gemScale, GameObject.PLATFORM_REBOUND, GEM));

            //Add candies back to trick game
            trickGameCandies.Add(new Reactable(candy1Img, new Vector2(0, 800), candyScale, GameObject.PLATFORM_REBOUND, CANDY1));
            trickGameCandies.Add(new Reactable(candy2Img, new Vector2(700, 750), candyScale, GameObject.PLATFORM_REBOUND, CANDY2));
            trickGameCandies.Add(new Reactable(candy3Img, new Vector2(1650, 330), candyScale, GameObject.PLATFORM_REBOUND, CANDY3));

            //Reset candy collection 
            candiesCollected.Clear();

            //Set portal status to not touched
            portal.SetStatus(false);

            //Reset player location
            mainPlayer = new MaingamePlayer(new Vector2(0, 737));
            minigamePlayer = new MinigamePlayer(new Vector2(900, 800));

            //Reset player health
            mainPlayer.SetPlayerHealth((int)Player.MAX_HEALTH);
            minigamePlayer.SetPlayerHealth((int)Player.MAX_HEALTH);

            //Reset enemy location
            enemy.SetPos(new Vector2(840, 450));

            //Reset status of name entering 
            capturingName = true;

            //Reset scores
            totalScore = NONE;
            skewerScore = NONE;
            avalancheScore = NONE;
            trickScore = NONE;

            //Reset num of gems collected
            for (int i = 0; i < numGems.Length; i++)
            {
                numGems[i] = NONE;
            }

            //Reload animations
            mainPlayer.LoadAnims("PlayerAnims.csv", Content);
            minigamePlayer.LoadAnims("MinigamePlayerAnims.csv", Content);
            enemy.LoadAnims("EnemyAnims.csv", Content);
        }

        /// <summary>
        /// Change both the player and enemy's health bar based on the amount of health they have remaining
        /// </summary>
        private void ModifyHealthBar()
        {
            //Adjust the amount of health for the player based on which gamestate they are in
            if (gameState == MAIN_GAMEPLAY)
            {
                playerHealthPercent = mainPlayer.GetPlayerHealth() / Player.MAX_HEALTH;
            }
            else
            {
                playerHealthPercent = minigamePlayer.GetPlayerHealth() / Player.MAX_HEALTH;
            }

            //Change the width of the health inside the bar
            playerHealthBarRec.Width = (int)(actualPlayerHealthBarRec.Width * playerHealthPercent);
        }
        #endregion

        #region External subprograms
        /// <summary>
        /// Read data in to game 
        /// </summary>
        /// <param name="fileName">Name of file to read from</param>
        private void ReadFile(string fileName)
        {
            try
            {
                //Create array for incoming info
                string[] data;

                //Get the current working directory
                string currentDirectory = Directory.GetCurrentDirectory();

                //Combine the current directory with the file name to get the full path
                string filePath = Path.Combine(currentDirectory, fileName);

                //Instantiate variable
                inFile = File.OpenText(filePath);

                //Read in info on each line and store each value into their respective variables
                while (!inFile.EndOfStream)
                {
                    //Split the data based on delimiter
                    data = inFile.ReadLine().Split(',');

                    //Store each value in the appropriate array
                    playerNames.Add(data[0]);
                    totalHighScores.Add(Convert.ToInt32(data[1]));
                    skewerHighScores.Add(Convert.ToInt32(data[2]));
                    avalancheHighScores.Add(Convert.ToInt32(data[3]));
                    trickHighScores.Add(Convert.ToInt32(data[4]));
                    gemHighScores.Add(Convert.ToInt32(data[5]));
                }
            }
            catch (FileNotFoundException fnf)
            {
                //Display error message when file is not found
                Debug.WriteLine("ERROR: File was not found.");
            }
            catch (FormatException fe)
            {
                //Display error message when format exception occurs
                Debug.WriteLine("ERROR: File was not properly saved");
            }
            catch (Exception e)
            {
                //Display general error message
                Debug.WriteLine("ERROR: " + e.Message);
            }
            finally
            {
                //Close the file
                if (inFile != null)
                {
                    inFile.Close();
                }
            }
        }

        /// <summary>
        /// Save player data to text file
        /// </summary>
        /// <param name="filePath">Name of file path</param>
        private void SaveData(string filePath)
        {
            try
            {
                //Instantiate variable
                outFile = File.CreateText(filePath);

                //Write stats to file
                for (int i = 0; i < playerNames.Count; i++)
                {
                    outFile.WriteLine(playerNames[i] + "," + totalHighScores[i] + "," + skewerHighScores[i] + "," + avalancheHighScores[i] + "," + trickHighScores[i] + "," + gemHighScores[i]);
                }
            }
            catch (Exception e)
            {
                //Display general error message
                Debug.WriteLine("ERROR: " + e.Message);
            }
            finally
            {
                //Close the file
                if (outFile != null)
                {
                    outFile.Close();
                }
            }
        }

        /// <summary>
        /// Convert keyboard input into chars
        /// </summary>
        /// <param name="keyboard">Current key pressed</param>
        /// <param name="oldKeyboard">Key pressed in old frame</param>
        /// <param name="key"></param>
        /// <returns>If succesful conversion</returns>
        private bool TryConvertKeyboardInput(KeyboardState keyboard, KeyboardState oldKeyboard, out char key)
        {
            //Store array of pressed keys
            Keys[] keys = keyboard.GetPressedKeys();

            //Check if shift key is pressed
            bool shift = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);

            //Check if at least one key is pressed and it was not pressed in the previous frame
            if (keys.Length > 0 && !oldKeyboard.IsKeyDown(keys[0]))
            {
                //Handle different keys
                switch (keys[0])
                {
                    //Alphabet keys
                    case Keys.A: if (shift) { key = 'A'; } else { key = 'a'; } return true;
                    case Keys.B: if (shift) { key = 'B'; } else { key = 'b'; } return true;
                    case Keys.C: if (shift) { key = 'C'; } else { key = 'c'; } return true;
                    case Keys.D: if (shift) { key = 'D'; } else { key = 'd'; } return true;
                    case Keys.E: if (shift) { key = 'E'; } else { key = 'e'; } return true;
                    case Keys.F: if (shift) { key = 'F'; } else { key = 'f'; } return true;
                    case Keys.G: if (shift) { key = 'G'; } else { key = 'g'; } return true;
                    case Keys.H: if (shift) { key = 'H'; } else { key = 'h'; } return true;
                    case Keys.I: if (shift) { key = 'I'; } else { key = 'i'; } return true;
                    case Keys.J: if (shift) { key = 'J'; } else { key = 'j'; } return true;
                    case Keys.K: if (shift) { key = 'K'; } else { key = 'k'; } return true;
                    case Keys.L: if (shift) { key = 'L'; } else { key = 'l'; } return true;
                    case Keys.M: if (shift) { key = 'M'; } else { key = 'm'; } return true;
                    case Keys.N: if (shift) { key = 'N'; } else { key = 'n'; } return true;
                    case Keys.O: if (shift) { key = 'O'; } else { key = 'o'; } return true;
                    case Keys.P: if (shift) { key = 'P'; } else { key = 'p'; } return true;
                    case Keys.Q: if (shift) { key = 'Q'; } else { key = 'q'; } return true;
                    case Keys.R: if (shift) { key = 'R'; } else { key = 'r'; } return true;
                    case Keys.S: if (shift) { key = 'S'; } else { key = 's'; } return true;
                    case Keys.T: if (shift) { key = 'T'; } else { key = 't'; } return true;
                    case Keys.U: if (shift) { key = 'U'; } else { key = 'u'; } return true;
                    case Keys.V: if (shift) { key = 'V'; } else { key = 'v'; } return true;
                    case Keys.W: if (shift) { key = 'W'; } else { key = 'w'; } return true;
                    case Keys.X: if (shift) { key = 'X'; } else { key = 'x'; } return true;
                    case Keys.Y: if (shift) { key = 'Y'; } else { key = 'y'; } return true;
                    case Keys.Z: if (shift) { key = 'Z'; } else { key = 'z'; } return true;

                    //Decimal keys
                    case Keys.D0: key = '0'; return true;
                    case Keys.D1: key = '1'; return true;
                    case Keys.D2: key = '2'; return true;
                    case Keys.D3: key = '3'; return true;
                    case Keys.D4: key = '4'; return true;
                    case Keys.D5: key = '5'; return true;
                    case Keys.D6: key = '6'; return true;
                    case Keys.D7: key = '7'; return true;
                    case Keys.D8: key = '8'; return true;
                    case Keys.D9: key = '9'; return true;
                }
            }

            //If no valid key was pressed, return false
            key = (char)0;
            return false;
        }

        /// <summary>
        /// Sort highscore based on selected highscore
        /// </summary>
        /// <param name="highscores">Highscores to be sorted</param>
        /// <returns>Sorted highscore list</returns>
        private List<double> SortHighscore(List<double> highscores)
        {
            //Store the current values being inserted
            double temp;
            string tempString;

            //Store the index the value will be inserted at
            int j;

            //Sort highscores in descending order
            for (int i = 1; i < highscores.Count; i++)
            {
                //Store the next element to be inserted
                temp = highscores[i];
                tempString = sortedNames[i];

                //Shift all "sorted" elements that are greater than the new value to the right
                for (j = i; j > 0; j--)
                {
                    // Swap if the value is lesser than the insertion value
                    if (highscores[j - 1] < temp)
                    {
                        highscores[j] = highscores[j - 1];
                        sortedNames[j] = sortedNames[j - 1];
                    }
                    else
                    {
                        //The unsorted value is smaller, done shifting, insert now
                        break;
                    }
                }

                //The insertion location has been found, now insert the values
                highscores[j] = temp;
                sortedNames[j] = tempString;
            }

            //Return sorted highscore
            return highscores;
        }
        #endregion
    }
}
