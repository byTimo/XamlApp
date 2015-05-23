using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZappChat.Core
{
    public class Dialogue
    {
        public ulong RoomId { get; private set; }
        public DialogueStatus Status { get; set; }
        public string Query { get; set; }
        public DateTime LastDateTime { get; private set; }
        public string LastMessageDate { get; private set; }
        public List<Message> Messages { get; set; }
        public ulong QueryId { get; private set; }

        public ulong CarId { get; private set; }

        public string CarBrand { get; set; }
        public string CarModel { get; set; }
        public string VIN { get; set; }
        public string Year { get; set; }

        public Dialogue() { }
        public Dialogue(ulong roomId, string quary, ulong queryId, string lastUpdate, ulong carId, DialogueStatus status)
        {
            RoomId = roomId;
            QueryId = queryId;
            LastDateTime = DateTime.ParseExact(lastUpdate, "MM'/'dd'/'yyyy' 'HH':'mm':'ss", CultureInfo.InvariantCulture).AddHours(-1);
            CarId = carId;
            LastMessageDate = LastDateTime.ToString("M", new CultureInfo("ru-RU"));
            Query = quary;
            Messages = new List<Message>();
            Status = status;
        }

        public Dialogue(ulong roomId, Message message)
        {
            RoomId = roomId;
            LastDateTime = message.DateTime;
            LastMessageDate = LastDateTime.ToString("M", new CultureInfo("ru-RU"));
            Messages = new List<Message> {message};
            Status = DialogueStatus.Created;

        }
        public void AddMessage(Message newMessage)
        {
            LastDateTime = newMessage.DateTime;
            LastMessageDate = newMessage.Date;
            Messages.Add(newMessage);
        }

        public void AddQuery(ulong queryId,string query, string lastUpdate)
        {
            QueryId = queryId;
            Query = query;
            LastDateTime = DateTime.Parse(lastUpdate);
            LastMessageDate = LastDateTime.ToString("M", new CultureInfo("ru-RU"));
        }
        public string GetTitleMessage()
        {
            return Query ?? (GetLastMessage() != null ? GetLastMessage().Text : "");
        }

        public Message GetLastMessage()
        {
            return Messages.LastOrDefault(x => x.Type == MessageType.Incoming);
        }
        protected bool Equals(Dialogue other)
        {
            return RoomId == other.RoomId;
        }

        public void SetCarInformation(string brand, string model, string vin, string year)
        {
            CarBrand = brand;
            CarModel = model;
            VIN = vin;
            Year = year;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Dialogue) obj);
        }

        public override int GetHashCode()
        {
            return (int) RoomId;
        }
    }
}
