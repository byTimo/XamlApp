using System;
using System.Windows.Threading;

namespace ZappChat.Core
{
    public interface INotification
    {
        Dialogue Dialogue { get; set; }
        NotificationType Type { get; set; }
        DispatcherTimer ReshowTimer { get; set; }

        void SetCarInfo(string brand, string model, string vin, string year);
    }
}
