using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceWarsMono
{
    interface IAsteroid
    {
        void Update();
        void Draw(SpriteBatch spbtch);
        Rectangle AsteroidRect { get; }
        bool Hidden { get; }
        bool Alive { get; set; }
        Vector2 Position { get; }
    }
    class Asteroid : IAsteroid
    {
        public static Texture2D Texture { get; set; }
        public Rectangle AsteroidRect { get; private set; }
        public Point SpriteSize { get; private set; }
        public Vector2 Position { get; private set; }
        public bool Hidden 
        { get 
            {
                if (GameScenes.CurrentScene == GameScenes.Scenes["Start"])
                    return Position.Y >= 500;
                if (GameScenes.CurrentScene == GameScenes.Scenes["WebGamePlay"])
                    return Position.X >= 700;
                else
                    return true;
            }
        }
        public bool Alive { get; set; }

        //private Random AngleGenerator = new Random();
        //private float rotationAngle;
        //private readonly float rotationAngleProccess;                 //хочу чтоб они вращались
        private float speed = 2f;
        public Asteroid(Vector2 p)
        {
            Position = p;
            Alive = true;
            //rotationAngle = AngleGenerator.Next(-10, 10);
            SpriteSize = new Point(60, 60);
        }
        public void Update()
        {
            if (GameScenes.CurrentScene == GameScenes.Scenes["Start"])
                Position = new Vector2(Position.X, Position.Y + speed);
            if (GameScenes.CurrentScene == GameScenes.Scenes["WebGamePlay"])
                Position = new Vector2(Position.X + speed, Position.Y);
            //rotationAngle += rotationAngleProccess
            AsteroidRect = new Rectangle((int)Position.X, (int)Position.Y, SpriteSize.X, SpriteSize.Y);
        }
        public void Draw(SpriteBatch spbtch)
        {
            spbtch.Draw(Texture, Position, null, Color.White, 0, Vector2.One, 1, SpriteEffects.None, 0);
        }
    }

    class FastAsteroid : IAsteroid
    {
        public static Texture2D Texture { get; set; }
        public Rectangle AsteroidRect { get; private set; }
        public Point SpriteSize { get; private set; }
        public Vector2 Position { get; private set; }
        public bool Hidden
        {
            get
            {
                if (GameScenes.CurrentScene == GameScenes.Scenes["Start"])
                    return Position.Y >= 500;
                if (GameScenes.CurrentScene == GameScenes.Scenes["WebGamePlay"])
                    return Position.X >= 700;
                else
                    return true;
            }
        }
        public bool Alive { get; set; }

        private float speed = 4f;
        public FastAsteroid(Vector2 p)
        {
            Position = p;
            Alive = true;
            SpriteSize = new Point(30, 30);
        }
        public void Update()
        {
            if (GameScenes.CurrentScene == GameScenes.Scenes["Start"])
                Position = new Vector2(Position.X, Position.Y + speed);
            if (GameScenes.CurrentScene == GameScenes.Scenes["WebGamePlay"])
                Position = new Vector2(Position.X + speed, Position.Y);
            AsteroidRect = new Rectangle((int)Position.X, (int)Position.Y, SpriteSize.X, SpriteSize.Y);
        }
        public void Draw(SpriteBatch spbtch)
        {
            spbtch.Draw(Texture, Position, null, Color.White, 0, Vector2.One, 1, SpriteEffects.None, 0);
        }
    }

    class BigAsteroid : IAsteroid
    {
        public static Texture2D Texture { get; set; }
        public Rectangle AsteroidRect { get; private set; }
        public Point SpriteSize { get; private set; }
        public Vector2 Position { get; private set; }
        public bool Hidden
        {
            get
            {
                if (GameScenes.CurrentScene == GameScenes.Scenes["Start"])
                    return Position.Y >= 500;
                if (GameScenes.CurrentScene == GameScenes.Scenes["WebGamePlay"])
                    return Position.X >= 700;
                else
                    return true;
            }
        }
        public bool Alive { get; set; }

        private float speed = 1f;
        public BigAsteroid(Vector2 p)
        {
            Position = p;
            Alive = true;
            SpriteSize = new Point(100, 100);
        }
        public void Update()
        {
            if (GameScenes.CurrentScene == GameScenes.Scenes["Start"])
                Position = new Vector2(Position.X, Position.Y + speed);
            if (GameScenes.CurrentScene == GameScenes.Scenes["WebGamePlay"])
                Position = new Vector2(Position.X + speed, Position.Y);
            AsteroidRect = new Rectangle((int)Position.X, (int)Position.Y, SpriteSize.X, SpriteSize.Y);
        }
        public void Draw(SpriteBatch spbtch)
        {
            spbtch.Draw(Texture, Position, null, Color.White, 0, Vector2.One, 1, SpriteEffects.None, 0);
        }
    }
}

