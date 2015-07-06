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

        private static ZappDbContext Instance
        {
            get { return instance ?? (instance = new ZappDbContext()); }
        }

        private sealed class ZappDbContext : DbContext
        {
            public ZappDbContext() : base("ZappDbConnectionString") { }
            public DbSet<Dialogue> Dialogues { get; set; }
            public DbSet<Message> Messages { get; set; }
            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Dialogue>().HasKey(c => c.RoomId);
                modelBuilder.Entity<Message>().HasKey(m => m.MessageId);
            }
        }

        static DialogueStore()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
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

        public static void RemoveDialogue(Dialogue dialogue)
        {
            Instance.Dialogues.Remove(dialogue);
        }
        public static void RemoveDialogueOnRoomId(long roomId)
        {
            var removingDialogue = GetDialogueOnRoomId(roomId);
            if(removingDialogue == null) return;
            Instance.Dialogues.Remove(removingDialogue);
        }

        public static IEnumerable<Dialogue> GetDialoguesWithQuery()
        {
            return Instance.Dialogues.Where(d => d.QueryId != 0);
        }

        public static IEnumerable<Dialogue> GetDialoguesWithoutQuery()
        {
            return Instance.Dialogues.Where(d => d.QueryId == 0);
        }
    }
}
