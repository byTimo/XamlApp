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
        public int Id { get; private set; }
        public MessageStatus Status { get; set; }
        public string Author { get; private set; }

        public DateTime DateTime { get; private set; }

        //TODO Использовать, как строковое представление даты, когда появиться реализация сообщений
        public string Date { get; private set; }
        public string Text { get; private set; }

        public Message(int id, string author, string text, DateTime date, MessageStatus messageStatus)
        {
            Id = id;
            Author = author;
            Text = text;
            DateTime = date;
            Date = date.ToString("M", new CultureInfo("ru-RU"));
            Status = messageStatus;
        }
        public Message(int id, string author, string text, MessageStatus messageStatus) : this(id, author, text, DateTime.Now, messageStatus) { }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Message) obj);
        }
        protected bool Equals(Message other)
        {
            return string.Equals(Author, other.Author) && string.Equals(Date, other.Date) && string.Equals(Text, other.Text);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Author != null ? Author.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Date != null ? Date.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Text != null ? Text.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
