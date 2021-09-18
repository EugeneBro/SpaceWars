using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceWarsMono
{
    class Server
    {
        string addr;
        static int localport;
        static int remoteport;
        public bool syncchronized { get; private set; }

        IPEndPoint Rpoint;

        UdpClient udpserver;

        byte[] databuff;

        public static DataModel SendinData;
        public static DataModel ReceivinData;

        BinaryFormatter formatter;

        public Server()
        {
            addr = "127.0.0.1";//"26.14.140.212";
            localport = 4440;
            remoteport = 4444;
            SendinData = new DataModel();
            ReceivinData = new DataModel();
            databuff = new byte[64];

            formatter = new BinaryFormatter();

            Rpoint = new IPEndPoint(IPAddress.Parse(addr), remoteport);

            udpserver = new UdpClient(localport);
            udpserver.Connect(Rpoint);

            Sync();
        }
        bool Sync()
        {
            while (!syncchronized)
            {
                databuff = Encoding.ASCII.GetBytes("READY");
                udpserver.Send(databuff, databuff.Length);

                try
                {
                    databuff = udpserver.Receive(ref Rpoint);
                }
                catch (Exception)
                {
                    syncchronized = false;
                }

                if (Encoding.ASCII.GetString(databuff) == "GO")
                    syncchronized = true;
                else
                    syncchronized = false;
            }

            return syncchronized;
        } //это просто мрак, переделай пожалуйста, умоляю
        public async void GetUpdate() //Бро ну тут условие говнище и прочитай уже про BeginReceive()
        {
            using MemoryStream ms = new MemoryStream();
            UdpReceiveResult receive;

            try
            {
                receive = await udpserver.ReceiveAsync();
            }
            catch (Exception)
            {
                return;
            }

            if (receive.RemoteEndPoint.Address.ToString() == Rpoint.Address.ToString() && receive.RemoteEndPoint.Port == Rpoint.Port)
            {
                ms.Write(receive.Buffer, 0, receive.Buffer.Length);
                ms.Position = 0;
                ReceivinData = (DataModel)formatter.Deserialize(ms);
            }
        }
        public void CreateUpdate(Ship player, IWeapon PlayersWeapon, ClientShip client, IWeapon ClientsWeapon) //добавь обновление количества пуль
        {   
            SendinData.ClientPosition = client.Position.X;
            SendinData.Position = player.Position.X;

            SendinData.ServersBulletsCount = PlayersWeapon.Bullets.Count;
            SendinData.ClientsBulletsCount = ClientsWeapon.Bullets.Count;

            foreach (var r in PlayersWeapon.Bullets)
            {
                SendinData.BulletsPositionX.Add(r.Position.X);
                SendinData.BulletsPositionY.Add(r.Position.Y);
            }

            foreach (var r in ClientsWeapon.Bullets)
            {
                SendinData.ClientBulletsPositionX.Add(r.Position.X);
                SendinData.ClientBulletsPositionY.Add(r.Position.Y);
            }
        }
        public void Update(ref ClientShip client, IWeapon ClientsWeapon)
        {
            client.Position.X += ReceivinData.Position;

            if (ReceivinData.ClientFire)
            {
                ClientsWeapon.Fire(new Vector2(ReceivinData.ClientBulletStartX, 0));
            }
        }
        public async void SendUpdate()
        {
            using MemoryStream ms = new MemoryStream();

            formatter.Serialize(ms, SendinData);
            databuff = ms.GetBuffer();

            await udpserver.SendAsync(databuff, databuff.Length);
        }
        public void Stop()
        {
            udpserver.Close();
        }
    }

    class Client
    {
        UdpClient client;

        string addr;
        int localport;
        int remoteport;
        public bool synchronized { get; private set; }

        IPEndPoint Rpoint;

        byte[] databuff;

        public static DataModel SendinData;
        public static DataModel ReceivinData;

        BinaryFormatter formatter;
        public Client()
        {
            addr = "127.0.0.1";
            localport = 4444;
            remoteport = 4440;

            SendinData = new DataModel();
            ReceivinData = new DataModel();
            databuff = new byte[64];

            formatter = new BinaryFormatter();

            Rpoint = new IPEndPoint(IPAddress.Parse(addr), remoteport);

            client = new UdpClient(localport);
            client.Connect(Rpoint);

            Sync();
        }
        bool Sync()
        {

            while (!synchronized)
            {
                databuff = client.Receive(ref Rpoint);

                if (Encoding.ASCII.GetString(databuff) == "READY")
                {
                    databuff = Encoding.ASCII.GetBytes("GO");
                    client.Send(databuff, databuff.Length);
                    synchronized = true;
                }
            }

            return synchronized;
        }
        public async void GetUpdate()
        {
            using MemoryStream ms = new MemoryStream();
            UdpReceiveResult receive;

            try
            {
                receive = await client.ReceiveAsync();
            }
            catch (Exception)
            {
                return;
            }

            if (receive.RemoteEndPoint.Address.ToString() == Rpoint.Address.ToString() && receive.RemoteEndPoint.Port == Rpoint.Port)
            {
                ms.Write(receive.Buffer, 0, receive.Buffer.Length);
                ms.Position = 0;
                ReceivinData = (DataModel)formatter.Deserialize(ms);
            }
        }
        public void Update(ref Ship Player, IFakeWeapon PlayersWeapon, ref ClientShip ServersPlayer, IFakeWeapon ServersWeapon)
        {
            Player.Position.X = ReceivinData.ClientPosition;
            ServersPlayer.Position.X = ReceivinData.Position;

            while (ServersWeapon.Bullets.Count != ReceivinData.ServersBulletsCount)
            {
                ServersWeapon.Fire();
            }

            while (PlayersWeapon.Bullets.Count != ReceivinData.ClientsBulletsCount)
            {
                ServersWeapon.Fire();
            }

            for (int i = 0; i <= ServersWeapon.Bullets.Count; i++)
            {
                ServersWeapon.Bullets[i].Position = new Vector2(ReceivinData.BulletsPositionX[i], ReceivinData.BulletsPositionY[i]);
            }

            for (int i = 0; i <= PlayersWeapon.Bullets.Count; i++)
            {
                PlayersWeapon.Bullets[i].Position = new Vector2(ReceivinData.ClientBulletsPositionX[i], ReceivinData.ClientBulletsPositionY[i]);
            }
        }
        public void CreateUpdate()
        {

        }
        public async void SendUpdate()
        {
            using MemoryStream ms = new MemoryStream();

            formatter.Serialize(ms, SendinData);
            databuff = ms.GetBuffer();

            await client.SendAsync(databuff, databuff.Length);
        }
        public void Stop()
        { }
    }

    [Serializable]
    class DataModel
    {
        public DataModel()
        {
            BulletsPositionX = new List<float>();
            BulletsPositionY = new List<float>();

            ClientBulletsPositionX = new List<float>();
            ClientBulletsPositionY = new List<float>();
        }

        public float Position;
        public float ClientPosition;

        public List<float> BulletsPositionX;
        public List<float> BulletsPositionY;

        public List<float> ClientBulletsPositionX;
        public List<float> ClientBulletsPositionY;

        public int ClientsBulletsCount;
        public int ServersBulletsCount;

        public float ClientBulletStartX;
        public bool ClientFire;

        //public bool Alive;
        //public bool ClientAlive;
    }
}
