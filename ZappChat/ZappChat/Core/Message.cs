using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZappChat.Core
{
    public class Message
    {
        public uint Id { get; private set; }
        public MessageStatus Status { get; set; }
        public string Author { get; private set; }
        public DateTime DateTime { get; private set; }
        public string Date { get; private set; }
        public string Text { get; private set; }
        public MessageType Type { get; private set; }
        public string Hash { get; private set; }

        public Message(uint id, string author, string text, DateTime date, MessageStatus messageStatus)
        {
            Id = id;
            Author = author;
            Text = text;
            DateTime = date;
            Date = date.ToString("M", new CultureInfo("ru-RU"));
            Status = messageStatus;
        }
        public Message(uint id, string author, string text, MessageStatus messageStatus) : this(id, author, text, DateTime.Now, messageStatus) { }

        public Message(uint id, string message, string type, string hash, string date, string userName)
        {
            Id = id;
            Text = message;
            Type = type == "incomming" ? MessageType.Incoming : MessageType.Outgoing;
            Hash = hash;
            Author = userName;
            DateTime = DateTime.ParseExact(date, "MM'/'dd'/'yyyy' 'HH':'mm':'ss", CultureInfo.InvariantCulture);
            Date = DateTime.ToString("M", new CultureInfo("ru-RU"));
        }

        protected bool Equals(Message other)
        {
            return Id == other.Id && string.Equals(Hash, other.Hash);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Message) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) Id*397) ^ Hash.GetHashCode();
            }
        }
    }
}
