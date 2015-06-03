namespace ZappChat.Core
{
    public interface INotification
    {
        Dialogue Dialogue { get; set; }

        void SetCarInfo(string brand, string model, string vin, string year);

        void CloseNotify();
    }
}
