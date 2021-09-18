using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace SpaceWarsMono
{

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private GameScenes GAMESCENES = new GameScenes(); 
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            GameScenes.CurrentScene = GameScenes.Scenes["Menu"];
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            GameScenes.spbtch = _spriteBatch;
            // TODO: use this.Content to load your game content here

            Rocket.Texture = Content.Load<Texture2D>("RocketTexture");
            FakeRocket.texture = Content.Load<Texture2D>("RocketTexture");

            Ship.texture = Content.Load<Texture2D>("ShipTexture");
            ClientShip.texture = Content.Load<Texture2D>("ShipTexture");

            Asteroid.Texture = Content.Load<Texture2D>("AssteroidTexture");
            FastAsteroid.Texture = Content.Load<Texture2D>("FastAsteroidTexture");
            BigAsteroid.Texture = Content.Load<Texture2D>("BigAsteroidTexture");

            ScoreCounter.TextBlock = Content.Load<SpriteFont>("Text");
            SqlRecorder.TextBlock = Content.Load<SpriteFont>("Text");
            GameScenes.TextBlock = Content.Load<SpriteFont>("Text2");

            GameScenes.MenuButton = Content.Load<Texture2D>("MenuButton");
            GameScenes.RecordButton = Content.Load<Texture2D>("RecordsButton");
            GameScenes.StartButton = Content.Load<Texture2D>("StartButton");
            GameScenes.LevelModeButton = Content.Load<Texture2D>("LevelModeButton");
            GameScenes.PvpButton = Content.Load<Texture2D>("PvpButton");
            GameScenes.ExitButton = Content.Load<Texture2D>("ExitButton");
            GameScenes.EasyLevelButton = Content.Load<Texture2D>("EasyLevelButton");
            GameScenes.MiddleLevelButton = Content.Load<Texture2D>("MiddleLevelButton");
            GameScenes.HardLevelButton = Content.Load<Texture2D>("HardLevelButton");
            GameScenes.BackButton = Content.Load<Texture2D>("BackButton");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            // TODO: Add your update logic here
            KeyboardState keys = Keyboard.GetState();
            
            GAMESCENES.Update(keys);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.Immediate);

            GAMESCENES.Draw();
            
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
