using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace ZappChat.Core
{
    public static class DialogueStore
    {
        private static ZappDbContext instance;

        public static ZappDbContext Instance
        {
            get { return instance ?? (instance = new ZappDbContext()); }
        }

        public sealed class ZappDbContext : DbContext
        {
            public ZappDbContext() : base("ZappDbConnectionString") { }
            public DbSet<Dialogue> Dialogues { get; set; }
            public DbSet<Message> Messages { get; set; }
            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
            }
        }

        static DialogueStore()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        }

        public static void LoadDbInformation()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ZappDbContext, Migrations.Configuration>());
            Instance.Dialogues.Load();
            Instance.Messages.Load();
        }
        public static Dialogue GetDialogueOnRoomId(long roomId)
        {
            return Instance.Dialogues.FirstOrDefault(d => d.RoomId == roomId);
        }

        public static void AddDialogue(Dialogue dialogue)
        {
            Instance.Dialogues.Add(dialogue);
            Instance.SaveChanges();
        }

        public static void SaveChanges()
        {
            Instance.SaveChanges();
        }
        public static IEnumerable<Dialogue> GetAllDialogues()
        {
            return Instance.Dialogues;
        }
    }
}
