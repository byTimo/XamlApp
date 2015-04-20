using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZappChat.Core
{
    public class Dialogue : IEnumerable
    {
        public int Id { get; private set; }
        public bool ContainQuery { get; private set; }

        private string quary;
        public string Query
        {
            get { return quary; }
            set
            {
                quary = value;
                ContainQuery = true;
            }
        }
        public DateTime LastDateTime { get; private set; }
        public string LastMessageDate { get; private set; }
        public List<Message> Messages { get; private set; }
        public string Interlocutor { get; private set; }

        private Dialogue(int id, string interlocutor, DateTime messageDate)
        {
            Id = id;
            Interlocutor = interlocutor;
            LastDateTime = messageDate;
            LastMessageDate = messageDate.ToString("M", new CultureInfo("ru-RU"));
        }

        public Dialogue(int id, string interlocutor, List<Message> messages)
            : this(id, interlocutor, messages[messages.Count - 1].DateTime)
        {
            Messages = messages;
        }
        public Dialogue(int id, string interlocutor, string quary, DateTime time)
            : this(id, interlocutor, time)
        {
            Query = quary;
            Messages = new List<Message>();
        }


        public void AddMessage(Message newMessage)
        {
            LastDateTime = newMessage.DateTime;
            LastMessageDate = newMessage.Date;
            Messages.Add(newMessage);
        }

        public string GetTitleMessage()
        {
            return ContainQuery ? Query : GetLastMessage().Text;
        }

        public Message GetLastMessage()
        {
            return Messages[Messages.Count - 1];
        }
        public IEnumerator GetEnumerator()
        {
            return Messages.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Dialogue) obj);
        }

        protected bool Equals(Dialogue other)
        {
            return Equals(Messages, other.Messages) && string.Equals(Interlocutor, other.Interlocutor);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Messages != null ? Messages.GetHashCode() : 0) * 397) ^ (Interlocutor != null ? Interlocutor.GetHashCode() : 0);
            }
        }
    }
}
