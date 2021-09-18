using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceWarsMono
{
    interface IBullet
    {
        void Update();
        void Draw(SpriteBatch Sb);
        Rectangle BulletRect { get; }
        bool Hidden { get; }
        bool Alive { get; }
        Vector2 Position { get; set; }
    }
    interface IFakeBullet
    {
        bool Hidden { get; set; }
        bool Alive { get; set; }
        Vector2 Position { get; set; }
        public void Draw(SpriteBatch spbtch);
    }
    class Rocket : IBullet
    {
        
        public static Texture2D Texture;
        public Rectangle BulletRect { get; private set; }
        public Point SpriteSize { get; private set; }
        public Vector2 Position { get; set; }
        public bool Hidden { get { return Position.Y <= 0 || Position.Y >= 500; } }
        public bool Alive { get; private set; }
        private float Speed;
        
        public Rocket(Vector2 p, bool EnemyBullet)
        {
            Position = new Vector2(p.X + 7.5f, p.Y);
            Alive = true;
            if (EnemyBullet)
                Speed = 10f;
            else
                Speed = -10f;
        }

        public Rocket()
        {
            Speed = 10f;
        }

        public void Update()
        {
            Death();
            Position = new Vector2(Position.X, Position.Y + Speed);

            SpriteSize = new Point(Texture.Width, Texture.Height);
            BulletRect = new Rectangle((int)Position.X, (int)Position.Y, SpriteSize.X, SpriteSize.Y);
        }

        public void Draw(SpriteBatch spbtch)
        {
            spbtch.Draw(Texture, Position, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }

        private void Death()
        {
            for (int i = 0; i < AsteroidFactory.Asteroids.Count; i++)
            {
                if (BulletRect.Intersects(AsteroidFactory.Asteroids[i].AsteroidRect))
                {
                    Alive = false;
                    AsteroidFactory.Asteroids[i].Alive = false;

                    if(GameScenes.CurrentScene == GameScenes.Scenes["Start"])
                        ScoreCounter.Score++;
                }
            }
        }
    }
    class FakeRocket : IFakeBullet 
    {
        public static Texture2D texture;
        public bool Alive { get; set; }
        public bool Hidden { get; set; }
        public Vector2 Position { get; set; }
        public void Draw(SpriteBatch spbtch)
        {
            spbtch.Draw(texture, Position, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }
    }
}
