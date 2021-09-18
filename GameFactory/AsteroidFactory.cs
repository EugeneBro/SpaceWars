using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceWarsMono
{
    static class AsteroidFactory
    {
        public static List<IAsteroid> Asteroids = new List<IAsteroid>();

        static Random XYgenerator = new Random();
        static int RandomX;
        static int RandomY;

        static DateTime GenTimeA = DateTime.MinValue;
        static DateTime GenTimeF = DateTime.MinValue;
        static DateTime GenTimeB = DateTime.MinValue;

        static int CooldownForAsteroidEzLevel = 4;
        static int CooldownForBigAsteroidEzLevel = 8;

        static int CooldownForAsteroidMidLevel = 2;
        static int CooldownForFastAsteroidMidLevel = 4;
        static int CooldownForBigAsteroidMidLevel = 8;

        static int CooldownForAsteroidHardLevel = 1;
        static int CooldownForFastAsteroidHardLevel = 1;
        static int CooldownForBigAsteroidHardLevel = 2;

        static int CooldownForAsteroidWebLevel = 3;
        static int CooldownForFastAsteroidWebLevel = 2;
        static int CooldownForBigAsteroidWebLevel = 6;

        static void AsteroidSpawn(int Cooldown)                             
        {
            if (DateTime.Now >= GenTimeA.AddSeconds(Cooldown))
            {
                if (GameScenes.CurrentScene == GameScenes.Scenes["Start"])
                {
                    RandomX = XYgenerator.Next(0, 740);
                    Asteroids.Add(new Asteroid(new Vector2(RandomX, -60))); // генеряться за экраном, так же как и удалются
                }
                if (GameScenes.CurrentScene == GameScenes.Scenes["WebGamePlay"])
                {
                    RandomY = XYgenerator.Next(100, 300);
                    Asteroids.Add(new Asteroid(new Vector2(-60, RandomY))); // генеряться за экраном, так же как и удалются
                }

                GenTimeA = DateTime.Now;
            }
        }

        static void FastAsteroidSpawn(int Cooldown)
        {
            if (DateTime.Now >= GenTimeF.AddSeconds(Cooldown))
            {
                if (GameScenes.CurrentScene == GameScenes.Scenes["Start"])
                {
                    RandomX = XYgenerator.Next(0, 740);
                    Asteroids.Add(new FastAsteroid(new Vector2(RandomX, -60))); // генеряться за экраном, так же как и удалются
                }
                if (GameScenes.CurrentScene == GameScenes.Scenes["WebGamePlay"])
                {
                    RandomY = XYgenerator.Next(100, 300);
                    Asteroids.Add(new FastAsteroid(new Vector2(-60, RandomY))); // генеряться за экраном, так же как и удалются
                }

                GenTimeF = DateTime.Now;
            }
        }

        static void BigAsteroidSpawn(int Cooldown)
        {
            if (DateTime.Now >= GenTimeB.AddSeconds(Cooldown))
            {
                if (GameScenes.CurrentScene == GameScenes.Scenes["Start"])
                {
                    RandomX = XYgenerator.Next(0, 740);
                    Asteroids.Add(new BigAsteroid(new Vector2(RandomX, -60))); // генеряться за экраном, так же как и удалются
                }
                if (GameScenes.CurrentScene == GameScenes.Scenes["WebGamePlay"])
                {
                    RandomY = XYgenerator.Next(100, 300);
                    Asteroids.Add(new BigAsteroid(new Vector2(-60, RandomY))); // генеряться за экраном, так же как и удалются
                }

                GenTimeB = DateTime.Now;
            }
        }

        static void Conveyor(int level)
        {
            switch (level)
            {
                case 0:                               
                    {
                        AsteroidSpawn(CooldownForAsteroidEzLevel);
                        BigAsteroidSpawn(CooldownForBigAsteroidEzLevel);
                    }
                    break;

                case 1:
                    {
                        AsteroidSpawn(CooldownForAsteroidMidLevel);
                        FastAsteroidSpawn(CooldownForFastAsteroidMidLevel);
                        BigAsteroidSpawn(CooldownForBigAsteroidMidLevel);
                    }
                    break;

                case 2:
                    {
                        AsteroidSpawn(CooldownForAsteroidHardLevel);
                        FastAsteroidSpawn(CooldownForFastAsteroidHardLevel);
                        BigAsteroidSpawn(CooldownForBigAsteroidHardLevel);
                    }
                    break;

                case 3:
                    {
                        AsteroidSpawn(CooldownForAsteroidWebLevel);
                        FastAsteroidSpawn(CooldownForFastAsteroidWebLevel);
                        BigAsteroidSpawn(CooldownForBigAsteroidWebLevel);
                    }
                    break;
            }
        }

        public static void Update(int level)
        {
            Conveyor(level);

            for (int i = 0; i < Asteroids.Count; i++)
            {
                Asteroids[i].Update();

                if (Asteroids[i].Hidden || !Asteroids[i].Alive)
                {
                    Asteroids.RemoveAt(i);
                }
            }
        }

        public static void Draw(SpriteBatch spbtch)
        {
            for (int i = 0; i < Asteroids.Count; i++)
            {
                Asteroids[i].Draw(spbtch);
            }
        }
    }

}
