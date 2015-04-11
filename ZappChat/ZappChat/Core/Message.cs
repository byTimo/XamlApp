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
        public string Author { get; private set; }
        public string Date { get; private set; }
        public string Text { get; private set; }

        public Message(string author, string text, DateTime date )
        {
            Author = author;
            Text = text;
            Date = date.ToString("M", new CultureInfo("ru-RU"));
        }

        public Message(string author, string text, string date)
        {
            Author = author;
            Text = text;
            Date = date;
        }
        public Message(string author, string text) : this(author, text, DateTime.Now) { }

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
