using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ZappChat.Controls;

namespace ZappChat.Core
{

    static class AppNotificationManager
    {
        private static readonly double RightMonitorBorder = SystemParameters.WorkArea.Right;
        private static readonly double BottomMoniorBorder = SystemParameters.WorkArea.Bottom;

        private static INotification currentNotification;
        private static Queue<INotification> otherNotification;
        private static readonly List<INotification> WaitingNotifications;

        static AppNotificationManager()
        {
            currentNotification = null;
            otherNotification = new Queue<INotification>();
            WaitingNotifications = new List<INotification>();

            AppEventManager.SetCarInfo += SetCarInfo;
            AppEventManager.ReshowNotification += ReshowNotificationOnRoomId;
        }

        public static void CreateQueryNotification(Dialogue dialogue)
        {
            var newNotification = new QueryNotificationWindow(dialogue, RightMonitorBorder, BottomMoniorBorder);
            CreateNotification(newNotification);
        }
        public static void CreateMessageNotification(Dialogue dialogue)
        {
            var newNotification = new MessageNotificationWindow(dialogue, RightMonitorBorder, BottomMoniorBorder);
            CreateNotification(newNotification);
        }

        public static void NextNotification(bool reshowFlag)
        {
            if (reshowFlag)
                HideNotification(currentNotification);
            else
                CloseNotification(currentNotification);

            if (otherNotification.Count != 0)
            {
                currentNotification = otherNotification.Dequeue();
                ShowNotification(currentNotification);
            }
            else
            {
                currentNotification = null;
            }
        }
        public static void SetCarInfo(long id, string brand, string model, string vin, string year)
        {
            if (currentNotification != null)
                if (currentNotification.Dialogue.CarId == id)
                    currentNotification.SetCarInfo(brand, model, vin, year);

            var notificationInQueue = otherNotification.FirstOrDefault(n => n.Dialogue.CarId == id);
            if (notificationInQueue != null)
                //@TODO ---- проверить будет ли работать без вноса изменения в очередь ----
                notificationInQueue.SetCarInfo(brand, model, vin, year);
        }

        public static void CloseNotificationWithReshow(long roomId)
        {
            if (currentNotification != null)
                if (currentNotification.Dialogue.RoomId == roomId)
                {
                    currentNotification.ReshowTimer.Start();
                    WaitingNotifications.Add(currentNotification);
                    NextNotification(true);
                }
            var notificationInQueue = otherNotification.FirstOrDefault(n => n.Dialogue.RoomId == roomId);
            if (notificationInQueue != null)
            {
                notificationInQueue.ReshowTimer.Start();
                WaitingNotifications.Add(notificationInQueue);
                otherNotification = otherNotification.Remove(notificationInQueue);
            }

        }
        public static void CloseNotificationWithoutReshow(long roomId)
        {
            if (currentNotification != null)
                if (currentNotification.Dialogue.RoomId == roomId)
                {
                    currentNotification.ReshowTimer.Stop();
                    WaitingNotifications.Remove(currentNotification);
                    NextNotification(false);
                }
            var notificationInQueue = otherNotification.FirstOrDefault(n => n.Dialogue.RoomId == roomId);
            if (notificationInQueue != null)
            {
                otherNotification = otherNotification.Remove(notificationInQueue);
            }
            var notificationInWaiting = WaitingNotifications.FirstOrDefault(n => n.Dialogue.RoomId == roomId);
            if (notificationInWaiting != null)
            {
                notificationInWaiting.ReshowTimer.Stop();
                WaitingNotifications.Remove(notificationInWaiting);
            }

        }

        public static void ReshowNotificationOnRoomId(long roomId)
        {
            var reshowingNotification = WaitingNotifications.FirstOrDefault(n => n.Dialogue.RoomId == roomId);
            if (reshowingNotification != null)
                CreateNotification(reshowingNotification);
        }

        public static INotification GetNotificationOnRoomId(long roomId)
        {
            if (currentNotification != null && currentNotification.Dialogue.RoomId == roomId)
                return currentNotification;
            return otherNotification.FirstOrDefault(n => n.Dialogue.RoomId == roomId) ??
                   WaitingNotifications.FirstOrDefault(n => n.Dialogue.RoomId == roomId);
        }
        private static void CreateNotification(INotification newNotification)
        {
            if (currentNotification == null)
            {
                currentNotification = newNotification;
                ShowNotification(currentNotification);
            }
            else
            {
                if (currentNotification.Dialogue.Equals(newNotification.Dialogue))
                {
                    CloseNotification(currentNotification);
                    currentNotification = newNotification;
                    ShowNotification(newNotification);
                    return;
                }
                var notificationInQueue =
                    otherNotification.FirstOrDefault(n => n.Dialogue.Equals(newNotification.Dialogue));
                if (notificationInQueue == null)
                    otherNotification.Enqueue(newNotification);
                else
                    otherNotification = otherNotification.Change(notificationInQueue, newNotification);
            }
        }

        private static void ShowNotification(INotification notification)
        {
            var notificationWindow = notification as Window;
            if(notificationWindow != null)
                notificationWindow.Show();
        }

        private static void CloseNotification(INotification notification)
        {
            var notificationWindow = notification as Window;
            if (notificationWindow != null)
                notificationWindow.Close();
        }

        private static void HideNotification(INotification notification)
        {
            var notificationWindow = notification as Window;
            if (notificationWindow != null)
                notificationWindow.Hide();
        }
    }
    static class ExtensionQueue
    {
        public static Queue<INotification> Change(this Queue<INotification> queue, INotification changing, INotification changed)
        {
            if (changed == null) throw new NullReferenceException("Ссылка не ссылается на объект");
            if (!queue.Contains(changing, Support.DialogueComparer)) throw new ArgumentException("Переданного объекта нет в очереди");
            var newQueue = new Queue<INotification>(queue.Count);
            foreach (var notification in queue)
            {
                newQueue.Enqueue(notification.Equals(changing) ? changed : notification);
            }
            return newQueue;
        }
        public static Queue<INotification> Remove(this Queue<INotification> queue, INotification removable)
        {
            if (removable == null) throw new NullReferenceException("Ссылка не ссылается на объект");
            if (!queue.Contains(removable, Support.DialogueComparer)) throw new ArgumentException("Переданного объекта нет в очереди");
            return new Queue<INotification>(queue.Where(notification => !notification.Dialogue.Equals(removable.Dialogue)));
        }
    }
}
