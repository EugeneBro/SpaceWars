using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceWarsMono
{
    interface IWeapon
    {
        float ReloadTime { get; }
        bool Reloaded { get; }
        List<IBullet> Bullets { get; }
        void Fire(Vector2 position);
        void Update();
        void Draw(SpriteBatch spbtch);
        //void Drop();
    }
    interface IFakeWeapon
    {
        List<IFakeBullet> Bullets { get; set; }
        void Draw(SpriteBatch spbtch);
        void Fire();
    }
    class RocketLauncher : IWeapon
    {
        public List<IBullet> Bullets { get; set; }

        public bool Reloaded { get; private set; }
        private readonly bool EnemyWeapon;
        private DateTime ShotTime;
        public float ReloadTime { get; private set; }
        public RocketLauncher(bool enemyweapon)
        {
            Bullets = new List<IBullet>();

            EnemyWeapon = enemyweapon;
        }
        public void Fire(Vector2 position)              //добавляем снаряд в "обойму"
        {
            Reload();

            if (Reloaded)
            {
                Bullets.Add(new Rocket(position, EnemyWeapon));
                ShotTime = DateTime.Now;
            }
            Reloaded = false;
        }
        private void Reload()
        {
            ReloadTime = 0.5f;
            if (DateTime.Now >= ShotTime.AddSeconds(ReloadTime)) Reloaded = true;
        }
        public void Update()
        {
            for (int i = 0; i < Bullets.Count; i++)
            {
                Bullets[i].Update();

                if (Bullets[i].Hidden || !Bullets[i].Alive)               //чистим обойму от снарядов находящихся вне экрана
                {
                    Bullets.RemoveAt(i);
                }
            }
        }
        public void Draw(SpriteBatch spbtch)            //отрисовка каждого снаряда
        {
            foreach (var r in Bullets)
            {
                r.Draw(spbtch);
            }
        }
    }
    class FakeRocketLauncher : IFakeWeapon
    {
        public List<IFakeBullet> Bullets { get; set; }
        public FakeRocketLauncher()
        {
            Bullets = new List<IFakeBullet>();
        }
        public void Draw(SpriteBatch spbtch)
        {
            foreach (var r in Bullets)
            {
                r.Draw(spbtch);
            }
        }
        public void Fire()
        {
            Bullets.Add(new FakeRocket());
        }
        
    }
}