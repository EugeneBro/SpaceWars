using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;



namespace SpaceWarsMono
{
    [Serializable]
    class GameScenes
    {
        public static Dictionary<string, IGameScene> Scenes;

        public static Texture2D MenuButton;
        public static Texture2D BackButton;
        public static Texture2D StartButton;
        public static Texture2D LevelModeButton;
        public static Texture2D RecordButton;
        public static Texture2D ExitButton;
        public static Texture2D EasyLevelButton;
        public static Texture2D MiddleLevelButton;
        public static Texture2D HardLevelButton;
        public static Texture2D PvpButton;

        public static SpriteBatch spbtch;
        public static SpriteFont TextBlock;

        public static int CurrentLevel { get; private set; }

        public static List<string> Levels = new List<string>
        {
            "EASY",
            "MIDDLE",
            "HARD",
            "WEB"
        };

        private static Ship Player;
        private static ClientShip ClientPlayer;

        private static RocketLauncher rlauncher; //Gun for PLAYER
        private static RocketLauncher enemyrlauncher; //Gun for CLIENT
                                                 //Ну да, сомневаюсь что дублировать весь шмот для pvp 
                                                 // это безумно хорошая идея, но ничего лучше я пока не придумал

        private static FakeRocketLauncher clientsRlauncher;
        private static FakeRocketLauncher serversRlauncher;

        public GameScenes()
        {

            Menu menu = new Menu();
            GamePlay gameplay = new GamePlay();
            LevelMode levelmode = new LevelMode();
            Records records = new Records();
            GameOver gameover = new GameOver();
            WebGamePlay webgameplay = new WebGamePlay();
            Lobby lobby = new Lobby();
            LobbyConnect connectroom = new LobbyConnect();
            LobbyCreateRoom createroom = new LobbyCreateRoom();

            Scenes = new Dictionary<string, IGameScene>
            {
                { "Menu", menu },
                { "Start", gameplay },
                { "Level", levelmode },
                { "Records", records },
                { "GameOver", gameover },
                { "Lobby", lobby },
                { "WebGamePlay", webgameplay },
                { "CreateRoom", createroom },
                { "ConnectRoom", connectroom }
            };

            rlauncher = new RocketLauncher(false);
        }

        static Server server;
        static Client client;

        public static bool ServerFlag { get; private set; }

        public static IGameScene CurrentScene;
        public interface IGameScene
        {
            void Draw(SpriteBatch s);
            void Draw(SpriteBatch s, IWeapon weapon);
            void Update();
            void Update(KeyboardState keys, IWeapon weapon);
        }

        abstract class GameScenesBackButton //Ебаная хуйня заключается в том, что если кнопки находятся на одном месте
                                            //то они прожимаются разом, так как нажатие слишком "долгое", обязательно пофикси, ты же 
                                            //хочешь чтоб все выглядело оке ;)
        {
            static Point MenuButtonPosition = new Point(0, 0);
            static Point BackButtonPosition = new Point(0, 70);

            static Point ButtonSize = new Point(50, 70);

            static Rectangle MenuButtonRect = new Rectangle(MenuButtonPosition, ButtonSize);
            static Rectangle BackButtonRect = new Rectangle(BackButtonPosition, ButtonSize);
            public void MenuButtonDraw(SpriteBatch s)
            {
                s.Draw(MenuButton, new Vector2(MenuButtonPosition.X, MenuButtonPosition.Y), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }
            public void BackButtonDraw(SpriteBatch s)
            {
                s.Draw(BackButton, new Vector2(BackButtonPosition.X, BackButtonPosition.Y), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }
            public void MenuButtonUpdate()
            {
                MousePoint = new Point(Mouse.GetState().X, Mouse.GetState().Y);

                if (Mouse.GetState().LeftButton == ButtonState.Pressed && MenuButtonRect.Contains(MousePoint))
                {
                    CurrentScene = Scenes["Menu"];
                }
            }
            public void BackButtonUpdate(string PreviousScene)
            {
                MousePoint = new Point(Mouse.GetState().X, Mouse.GetState().Y);

                if (Mouse.GetState().LeftButton == ButtonState.Pressed && BackButtonRect.Contains(MousePoint))
                {
                    CurrentScene = Scenes[PreviousScene];
                }
            }
        }

        static Point MousePoint;
        class Menu : IGameScene
        {
            static Point StartButtonPosition = new Point(0, 50);
            static Point RecordButtonPosition = new Point(0, 150);
            static Point LevelModeButtonPosition = new Point(0, 250);
            static Point PvpButtonPosition = new Point(0, 350);
            static Point ExitButtonPosition = new Point(0, 450);

            static Point ButtonSize = new Point(200, 50); // Hitbox 200 * 50 

            Rectangle StartButtonRect = new Rectangle(StartButtonPosition, ButtonSize);
            Rectangle RecordButtonRect = new Rectangle(RecordButtonPosition, ButtonSize);
            Rectangle LevelModeButtonRect = new Rectangle(LevelModeButtonPosition, ButtonSize);
            Rectangle PvpButtonRect = new Rectangle(PvpButtonPosition, ButtonSize);
            Rectangle ExitButtonRect = new Rectangle(ExitButtonPosition, ButtonSize);

            public void Draw(SpriteBatch s, IWeapon weapon)
            { }
            public void Draw(SpriteBatch s)
            {
                s.Draw(StartButton, new Vector2(StartButtonPosition.X, StartButtonPosition.Y), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                s.Draw(RecordButton, new Vector2(RecordButtonPosition.X, RecordButtonPosition.Y), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                s.Draw(LevelModeButton, new Vector2(LevelModeButtonPosition.X, LevelModeButtonPosition.Y), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                s.Draw(PvpButton, new Vector2(PvpButtonPosition.X, PvpButtonPosition.Y), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                s.Draw(ExitButton, new Vector2(ExitButtonPosition.X, ExitButtonPosition.Y), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

                s.DrawString(TextBlock, $"{Levels[CurrentLevel]}", new Vector2(200, 263), Color.White);
            }

            public void Update(KeyboardState keys, IWeapon weapon)
            { }
            public void Update()
            {

                MousePoint = new Point(Mouse.GetState().X, Mouse.GetState().Y);

                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (StartButtonRect.Contains(MousePoint))
                    {
                        Player = new Ship();
                        ScoreCounter.Score = 0;
                        CurrentScene = Scenes["Start"];
                    }
                    if (RecordButtonRect.Contains(MousePoint))
                    {
                        CurrentScene = Scenes["Records"];
                    }
                    if (LevelModeButtonRect.Contains(MousePoint))
                    {
                        CurrentScene = Scenes["Level"];
                    }
                    if (PvpButtonRect.Contains(MousePoint))
                    {
                        CurrentScene = Scenes["Lobby"];
                    }
                    if (ExitButtonRect.Contains(MousePoint))
                    {
                        //Game.Exit();
                    }
                }
            }
        }
        class GamePlay : IGameScene
        {
            public void Draw(SpriteBatch s)
            { }
            public void Draw(SpriteBatch s, IWeapon weapon)
            {
                Player.Draw(s);
                weapon.Draw(s);
                AsteroidFactory.Draw(s);
                ScoreCounter.Draw(s);
            }
            public void Update()
            { }
            public void Update(KeyboardState keys, IWeapon weapon)
            {
                
                Player.Update(keys, weapon);
                weapon.Update();
                AsteroidFactory.Update(CurrentLevel);
            }
        }
        class GameOver : GameScenesBackButton, IGameScene
        {
            public void Draw(SpriteBatch s, IWeapon weapon)
            { }
            public void Draw(SpriteBatch s)
            {
                MenuButtonDraw(s);

                if (SqlRecorder.ScoreNewRecord)
                {
                    ScoreCounter.DrawNewRecord(s);
                }
            }

            public void Update(KeyboardState keys, IWeapon weapon)
            { }
            public void Update()
            {
                AsteroidFactory.Asteroids.Clear();

                MenuButtonUpdate();
            }
        }
        class LevelMode : GameScenesBackButton, IGameScene
        {
            static Point EasyLevelButtonPosition = new Point(350, 50);
            static Point MiddleLevelButtonPosition = new Point(350, 150);
            static Point HardLevelButtonPosition = new Point(350, 250);

            static Point ButtonSize = new Point(200, 50);

            Rectangle EasyLevelButtonRect = new Rectangle(EasyLevelButtonPosition, ButtonSize);
            Rectangle MiddleLevelButtonRect = new Rectangle(MiddleLevelButtonPosition, ButtonSize);
            Rectangle HardLevelButtonRect = new Rectangle(HardLevelButtonPosition, ButtonSize);

            public void Draw(SpriteBatch s, IWeapon weapon)
            { }
            public void Draw(SpriteBatch s)
            {
                MenuButtonDraw(s);
                s.Draw(EasyLevelButton, new Vector2(EasyLevelButtonPosition.X, EasyLevelButtonPosition.Y), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                s.Draw(MiddleLevelButton, new Vector2(MiddleLevelButtonPosition.X, MiddleLevelButtonPosition.Y), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                s.Draw(HardLevelButton, new Vector2(HardLevelButtonPosition.X, HardLevelButtonPosition.Y), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }
            public void Update(KeyboardState keys, IWeapon weapon)
            { }
            public void Update()
            {
                MousePoint = new Point(Mouse.GetState().X, Mouse.GetState().Y);

                MenuButtonUpdate();

                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (EasyLevelButtonRect.Contains(MousePoint))
                    {
                        CurrentLevel = 0;
                        CurrentScene = Scenes["Menu"];
                    }

                    if (MiddleLevelButtonRect.Contains(MousePoint))
                    {
                        CurrentLevel = 1;
                        CurrentScene = Scenes["Menu"];
                    }

                    if (HardLevelButtonRect.Contains(MousePoint))
                    {
                        CurrentLevel = 2;
                        CurrentScene = Scenes["Menu"];
                    }
                }
            }
        }
        class Records : GameScenesBackButton, IGameScene
        {

            public void Draw(SpriteBatch s, IWeapon weapon)
            { }
            public void Draw(SpriteBatch s)
            {

                MenuButtonDraw(s);

                for (int i = 0; i < 10; i++)
                {
                    ScoreCounter.DrawAllRecords(s);
                }
            }
            public void Update(KeyboardState keys, IWeapon weapon)
            { }
            public void Update()
            {
                //SqlRecorder.Update();

                MenuButtonUpdate();
            }
        }
        class WebGamePlay : IGameScene
        {
            public void Draw(SpriteBatch s)
            {
            
            }
            public void Draw(SpriteBatch s, IWeapon weapon)
            {
                Player.Draw(s);
                ClientPlayer.Draw(s);
                rlauncher.Draw(s);      //Ну это колхоз, сделай красиво
                enemyrlauncher.Draw(s); //и это тоже 


                //weapon.Draw(s);
                //AsteroidFactory.Draw(s);
            }
            public void Update()
            {
            
            }
            public void Update(KeyboardState keys, IWeapon weapon)
            {
                

                if (ServerFlag)
                {
                    Player.Update(keys, weapon);

                    server.SendUpdate();
                    server.GetUpdate();
                    server.Update(ref ClientPlayer, enemyrlauncher);
                    server.CreateUpdate(Player, rlauncher, ClientPlayer, enemyrlauncher);
                }
                else
                {
                    Player.Update(keys);

                    client.SendUpdate();
                    client.GetUpdate();
                    client.Update(ref Player, clientsRlauncher, ref ClientPlayer, serversRlauncher);
                    //client.CreateUpdate();
                }
                
                //AsteroidFactory.Update(CurrentLevel);
            }
        }
        class Lobby : GameScenesBackButton, IGameScene
        {
            static Point ConnectButtonPosition = new Point(200, 100);
            static Point CreateButtonPosition = new Point(200, 300);

            static Point ButtonSize = new Point(200, 50);

            Rectangle ConnectButtonRect = new Rectangle(ConnectButtonPosition, ButtonSize);
            Rectangle CreateButtonRect = new Rectangle(CreateButtonPosition, ButtonSize);

            public void Draw(SpriteBatch s)
            {
                MenuButtonDraw(s);
                s.DrawString(TextBlock, "CONNECT", ConnectButtonPosition.ToVector2(), Color.White);
                s.DrawString(TextBlock, "CREATE", CreateButtonPosition.ToVector2(), Color.White);
            }

            public void Draw(SpriteBatch s, IWeapon weapon)
            {
                throw new NotImplementedException();
            }

            public void Update()
            {
                MousePoint = new Point(Mouse.GetState().X, Mouse.GetState().Y);
                MenuButtonUpdate();

                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (ConnectButtonRect.Contains(MousePoint))
                    {
                        CurrentScene = Scenes["ConnectRoom"];
                        ServerFlag = false;
                    }
                    if (CreateButtonRect.Contains(MousePoint))
                    {
                        CurrentScene = Scenes["CreateRoom"];
                        ServerFlag = true;
                    }
                }
            }

            public void Update(KeyboardState keys, IWeapon weapon)
            {
                throw new NotImplementedException();
            }
        }
        class LobbyCreateRoom : GameScenesBackButton, IGameScene
        {
            static Point StartButtonPosition = new Point(300, 200);
            static Point ButtonSize = new Point(200, 50);

            Rectangle StartButtonRect = new Rectangle(StartButtonPosition, ButtonSize);

            public void Draw(SpriteBatch s)
            {
                BackButtonDraw(s);
                s.DrawString(TextBlock, "START", StartButtonPosition.ToVector2(), Color.White);
            }

            public void Draw(SpriteBatch s, IWeapon weapon)
            {
                throw new NotImplementedException();
            }

            public void Update()
            {
                MousePoint = new Point(Mouse.GetState().X, Mouse.GetState().Y);
                BackButtonUpdate("Lobby");

                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (StartButtonRect.Contains(MousePoint))
                    {
                        server = new Server();

                        CurrentScene = Scenes["WebGamePlay"];
                        CurrentLevel = 3; //Web
                        Player = new Ship();
                        ClientPlayer = new ClientShip();
                        enemyrlauncher = new RocketLauncher(true);
                    }
                }
            }

            public void Update(KeyboardState keys, IWeapon weapon)
            {
                throw new NotImplementedException();
            }
        }
        class LobbyConnect : GameScenesBackButton, IGameScene
        {
            static Point ConnectButtonPosition = new Point(300, 200);
            static Point ButtonSize = new Point(200, 50);

            Rectangle ConnectButtonRect = new Rectangle(ConnectButtonPosition, ButtonSize);

            public void Draw(SpriteBatch s)
            {
                BackButtonDraw(s);
                s.DrawString(TextBlock, "CONNECT", ConnectButtonPosition.ToVector2(), Color.White);
            }

            public void Draw(SpriteBatch s, IWeapon weapon)
            {
                throw new NotImplementedException();
            }

            public void Update()
            {
                MousePoint = new Point(Mouse.GetState().X, Mouse.GetState().Y);

                BackButtonUpdate("Lobby");

                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (ConnectButtonRect.Contains(MousePoint))
                    {
                        client = new Client();

                        CurrentScene = Scenes["WebGamePlay"];
                        CurrentLevel = 3; //Web
                        Player = new Ship();
                        ClientPlayer = new ClientShip();

                        serversRlauncher = new FakeRocketLauncher();
                        clientsRlauncher = new FakeRocketLauncher();
                    }
                }
            }

            public void Update(KeyboardState keys, IWeapon weapon)
            {
                throw new NotImplementedException();
            }
        }
        public void Draw()
        {
            if (CurrentScene == Scenes["Start"] ||
                CurrentScene == Scenes["WebGamePlay"])
            {
                CurrentScene.Draw(spbtch, rlauncher);
            }
            else
            {
                CurrentScene.Draw(spbtch);
            }
        }

        public void Update(KeyboardState keys)
        {
            if (CurrentScene == Scenes["Start"] ||
                CurrentScene == Scenes["WebGamePlay"])
            {
                CurrentScene.Update(keys, rlauncher);
            }
            else
            {
                CurrentScene.Update();
            }
        }
    }
}
