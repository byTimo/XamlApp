﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZappChat.Core
{
    public class Message
    {
        public ulong Id { get; set; }
        public MessageStatus Status { get; set; }
        public string Author { get; private set; }
        public DateTime DateTime { get; private set; }
        public string Date { get; private set; }
        public string Text { get; private set; }
        public MessageType Type { get; private set; }
        public string Hash { get; private set; }
        public bool IsSuccessfully { get; set; }
        public bool IsUnread { get; set; }

        public Message(ulong id, string message, string type, string hash, string date, string userName)
        {
            Id = id;
            Text = message;
            Type = type == "incoming" ? MessageType.Incoming : MessageType.Outgoing;
            Hash = hash;
            Author = userName;
            DateTime = DateTime.ParseExact(date, "MM'/'dd'/'yyyy' 'HH':'mm':'ss", CultureInfo.InvariantCulture).AddHours(-1);
            Date = DateTime.ToString("M", new CultureInfo("ru-RU"));
            IsSuccessfully = true;
            IsUnread = false;
        }

        public Message(ulong id, string message, string type, string hash, string date, string userName, bool isUnread)
            : this(id, message, type, hash, date, userName)
        {
            IsUnread = isUnread;
        }
        protected bool Equals(Message other)
        {
            return string.Equals(Hash, other.Hash);
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
            return (Hash != null ? Hash.GetHashCode() : 0);
        }
    }
}
