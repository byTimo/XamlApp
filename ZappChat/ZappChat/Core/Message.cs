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

    }
}
