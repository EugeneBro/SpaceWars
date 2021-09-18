using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceWarsMono
{
    abstract class SpaceBody
    {
        public Texture2D texture;
        protected Rectangle BodyRect { get; set; }
        protected Point SpriteSize;
        public Vector2 Position;
        public bool Alive { get; set; }

        public SpaceBody()
        {
            Alive = true;
        }

        public virtual void Update(KeyboardState keys, IWeapon weapon)
        {
            Move(keys);
            Fire(keys, weapon);

            Death();

            BodyRect = new Rectangle((int)Position.X, (int)Position.Y, SpriteSize.X, SpriteSize.Y);
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (Alive) spriteBatch.Draw(texture, Position, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }
        protected virtual void Move(KeyboardState keys)
        {
            if (keys.IsKeyDown(Keys.Left)) Position.X--;
            if (keys.IsKeyDown(Keys.Right)) Position.X++;
            if (keys.IsKeyDown(Keys.Up)) Position.Y--;
            if (keys.IsKeyDown(Keys.Down)) Position.Y++;
        }
        protected virtual void Fire(KeyboardState keys, IWeapon weapon)
        {
            if (keys.IsKeyDown(Keys.Space)) weapon.Fire(Position);
        }
        protected virtual void Death()
        {
            for (int i = 0; i < AsteroidFactory.Asteroids.Count; i++)
            {
                if (BodyRect.Intersects(AsteroidFactory.Asteroids[i].AsteroidRect))
                {
                    Alive = false;
                }
            }
        }

    }

    //***********************************************************************************************
    class Ship : SpaceBody
    {
        new public static Texture2D texture;

        public string Name = "TEST";
        public Ship()
        {
            Position = new Vector2(200, 450);
            SpriteSize = new Point(20, 20);
        }
        protected override void Move(KeyboardState keys)
        {
            if (GameScenes.CurrentScene == GameScenes.Scenes["Start"] || GameScenes.CurrentScene == GameScenes.Scenes["WebGamePlay"])
            {
                if (keys.IsKeyDown(Keys.Left)) Position.X -= 5;
                if (keys.IsKeyDown(Keys.Right)) Position.X += 5;
            }
        }
        //void Fire(KeyboardState keys, IFakeWeapon fakeweapon)
        //{
        //    if (keys.IsKeyDown(Keys.Space)) fakeweapon.Fire(Client.ReceivinData.ServerReloaded);
        //}
        public override void Update(KeyboardState keys, IWeapon weapon)
        {
            if (GameScenes.CurrentScene == GameScenes.Scenes["WebGamePlay"])
            {
                Move(keys);
                Fire(keys, weapon);

                Server.SendinData.Position = Position.X;
                //Server.SendinData.Alive = Alive;
            }
            else 
            {
                base.Update(keys, weapon);
            }
        }
        public void Update(KeyboardState keys)
        {
            if (keys.IsKeyDown(Keys.Left) || keys.IsKeyDown(Keys.Right))
            {
                if (keys.IsKeyDown(Keys.Left))
                    Client.SendinData.Position = -5;
                if (keys.IsKeyDown(Keys.Right))
                    Client.SendinData.Position = 5;
            }
            else
                Client.SendinData.Position = 0;

            if (keys.IsKeyDown(Keys.Space))
            {
                Client.SendinData.ClientBulletStartX = Position.X;
                Client.SendinData.ClientFire = true;
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Alive) spriteBatch.Draw(texture, Position, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }
        protected override void Death()
        {
            for (int i = 0; i < AsteroidFactory.Asteroids.Count; i++)
            {
                if (BodyRect.Intersects(AsteroidFactory.Asteroids[i].AsteroidRect))
                {
                    Alive = false;
                    SqlRecorder.Compare($"{GameScenes.Levels[GameScenes.CurrentLevel]}", $"{Name}", ScoreCounter.Score);

                    GameScenes.CurrentScene = GameScenes.Scenes["GameOver"];
                }
            }
        }
    }
    //***********************************************************************************************
    class ClientShip : SpaceBody
    {
        new public static Texture2D texture;
        public ClientShip()
        {
            Position = new Vector2(200, 0);
            SpriteSize = new Point(20, 20);
        }
        protected override void Move(KeyboardState keys) 
        {

        }//empty
        protected override void Fire(KeyboardState keys, IWeapon weapon)
        {
            
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Alive) spriteBatch.Draw(texture, Position, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }
        public override void Update(KeyboardState keys, IWeapon weapon)
        {

        }
    }
}