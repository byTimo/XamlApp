using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZappChat.Core
{
    public class Dialogue : IEnumerable
    {
        public List<Message> Messages { get; private set; }
        public string Interlocutor { get; private set; }

        public Dialogue(string interlocutor, List<Message> messages)
        {
            Interlocutor = interlocutor;
            Messages = messages;
        }

        public void AddMessage(Message newMessage)
        {
            Messages.Add(newMessage);
        }

        public Message GetLatMessage()
        {
            return Messages[Messages.Count - 1];
        }
        public IEnumerator GetEnumerator()
        {
            return Messages.GetEnumerator();
        }
    }
}
