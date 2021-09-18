using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceWarsMono
{
    static class ScoreCounter
    {
        public static SpriteFont TextBlock;
        public static int Score = 0;

        public static void Draw(SpriteBatch spbtch)
        {
            spbtch.DrawString(TextBlock, $"Score: {Score}", new Vector2(650, 0), Color.White);
        }
        public static void DrawNewRecord(SpriteBatch spbtch)
        {
            spbtch.DrawString(TextBlock, $"NEW RECORD: {Score}", new Vector2(300, 200), Color.White);
        }
        public static void DrawAllRecords(SpriteBatch spbtch)
        {
            using FileStream stream = new FileStream(@"C:\Users\Юзер\source\repos\SpaceWarsMono\XMLRecords.xml", FileMode.Open);
            XmlSerializer formatter = new XmlSerializer(typeof(List<RecordItem>));

            List<RecordItem> Records = new List<RecordItem>();

            Records = (List<RecordItem>)formatter.Deserialize(stream);
            Records.Reverse();

            int Ystep = 50;

            if (Records.Count != 0)
            {
                foreach (var R in Records)
                {
                    spbtch.DrawString(TextBlock, R.Name, new Vector2(100, Ystep), Color.White);
                    spbtch.DrawString(TextBlock, R.LevelMode, new Vector2(250, Ystep), Color.White);
                    spbtch.DrawString(TextBlock, $"{ R.Record }", new Vector2(400, Ystep), Color.White);
                    spbtch.DrawString(TextBlock, $"{ R.RecordTime }", new Vector2(600, Ystep), Color.White);

                    Ystep += 50;
                }
            }
        }
    }

    [Serializable]
    public class RecordItem
    {
        [XmlElement("playername")]
        public string Name;

        [XmlElement("level")]
        public string LevelMode;

        [XmlElement("score")]
        public int Record;
        [XmlElement("DateTime")]
        public DateTime RecordTime;

        public RecordItem()
        { }
        public RecordItem(string n, string l, int r)
        {
            Name = n;
            LevelMode = l;
            Record = r;
            RecordTime = DateTime.Now;
        }
    }
    class XmlRecorder
    {
        private static List<RecordItem> Records = new List<RecordItem>();
        public static void WriteNewRecord(RecordItem record)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(List<RecordItem>));

            using (FileStream stream = new FileStream(@"C:\Users\Юзер\source\repos\SpaceWarsMono\XMLRecords.xml", FileMode.OpenOrCreate))
            {
                Records.Add(record);
                formatter.Serialize(stream, Records);
            }
        }
        public static bool Compare(int r)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(List<RecordItem>));

            using (FileStream stream = new FileStream(@"C:\Users\Юзер\source\repos\SpaceWarsMono\XMLRecords.xml", FileMode.Open))
            {
                Records = (List<RecordItem>)formatter.Deserialize(stream);

                var items = from R in Records
                            orderby R.Record descending
                            select R;

                try
                {
                    if (r > items.First<RecordItem>().Record) return true;
                    else return false;
                }
                catch (Exception)
                {
                    return true;
                }
            }
        }
    }
    
}
