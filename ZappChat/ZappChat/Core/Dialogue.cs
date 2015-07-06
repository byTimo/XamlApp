using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Linq;

namespace ZappChat.Core
{
    public sealed class Dialogue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long RoomId { get; private set; }
        public long QueryId { get; private set; }
        public long CarId { get; private set; }
        public DialogueStatus Status { get; set; }
        public string Query { get; set; }
        public DateTime LastDateTime { get; private set; }
        public string LastMessageDate { get; private set; }
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
        public string VIN { get; set; }
        public string Year { get; set; }

        public List<Message> Messages { get; set; }

        public Dialogue() { }
        public Dialogue(long roomId, string quary, long queryId, string lastUpdate, long carId, DialogueStatus status)
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

        public Dialogue(long roomId, Message message)
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

        public void AddQuery(long queryId,string query, string lastUpdate)
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

        private bool Equals(Dialogue other)
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
            if (obj.GetType() != GetType()) return false;
            return Equals((Dialogue) obj);
        }

        public override int GetHashCode()
        {
            return (int) RoomId;
        }
    }
}
