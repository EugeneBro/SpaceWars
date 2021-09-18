using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceWarsMono
{
    
    class SqlRecorder
    {
        class SqlRecordItem
        {
            public string Name { get; set; }
            public string LevelMode { get; set; }
            public int Record { get; set; }
            public DateTime RecordTime { get; set; }
            [Key]
            public int Id { get; set; }
        }

        class Context : DbContext
        {
            public DbSet<SqlRecordItem> SQLRecords { get; set; }
            public Context()
            {
                Database.EnsureCreated();
            }
            public Context(DbContextOptions<Context> options) : base(options)
            {

            }
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                if (!optionsBuilder.IsConfigured)
                {
                    optionsBuilder.UseSqlServer("Server=localhost;Database=TESTDB;Trusted_Connection=True;");
                }
            }
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {

            }
        }

        private static List<SqlRecordItem> SqlRecords = new List<SqlRecordItem>();
        private static SqlRecordItem Item = new SqlRecordItem 
        {
            Name = "0",
            LevelMode = "0",
            Record = 0,
            RecordTime = DateTime.MinValue,
        };
        private static int ActualRecord;

        public static SpriteFont TextBlock;
        public static bool ScoreNewRecord;
       
        private static SqlRecordItem ConfigRecordItem(string level, string name, int score)
        {
            return new SqlRecordItem 
            {
                LevelMode = level,
                Name = name,
                Record = score,
                RecordTime = DateTime.Now
            };  
        }
        private static async void WriteSqlNewRecord()
        {
            using Context db = new Context();
            ActualRecord = Item.Record;

            await db.SQLRecords.AddAsync(Item);
            await db.SaveChangesAsync();
        }
        public static async void Update()
        {
            using Context db = new Context();

            SqlRecords = await db.SQLRecords.ToListAsync();
            SqlRecords.Reverse();
        }
        public static async void Compare(string level, string name, int score)
        {
            using Context db = new Context();
            SqlRecords = await db.SQLRecords.ToListAsync();

            if (SqlRecords.Count() == 0) SqlRecords.Add(Item);
            Item = SqlRecords.Last();
            ActualRecord = Item.Record;

            if (score > ActualRecord)
            {
                Item = ConfigRecordItem(level, name, score);
                WriteSqlNewRecord();
                ScoreNewRecord = true;
            }
            else
            {
                ScoreNewRecord = false;
            }
        }
        public static void DrawAllSqlRecords(SpriteBatch spbtch)
        { 
            int Ystep = 50;

            if (SqlRecords.Count != 0)
            {
                
                foreach (var R in SqlRecords)
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
}
