using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using NAppUpdate.Framework;
using NAppUpdate.Framework.Common;
using NAppUpdate.Framework.Sources;

namespace ZappChat.Core
{
    static class AppUpdateManager
    {
        public static void SetUrlRemoteServer(string url)
        {
            UpdateManager.Instance.UpdateSource = new SimpleWebSource(url);
            UpdateManager.Instance.ReinstateIfRestarted();
        }

        public static void StartupCheckAndPrepareUpdateFeeds(Action callBack)
        {
            var updManager = UpdateManager.Instance;
            updManager.BeginCheckForUpdates(asyncResult =>
            {
                if (asyncResult.IsCompleted)
                {
//@TODO ------------------ изменить этот кусок, когда сервер будет выдавать информацию об обновлениях ----------
                    try
                    {
                        ((UpdateProcessAsyncResult) asyncResult).EndInvoke();
                    }
                    catch
                    {
                        callBack.Invoke();
                        return;
                    }
                    if (updManager.UpdatesAvailable == 0)
                    {
                        callBack.Invoke();
                        return;
                    }
                    ApplayUpdates();
                    callBack.Invoke();
                }
            }, null);

        }

        private static void ApplayUpdates()
        {
            var updManager = UpdateManager.Instance;
            updManager.BeginPrepareUpdates(asyncResult =>
            {
                ((UpdateProcessAsyncResult)asyncResult).EndInvoke();
                try
                {
                    updManager.ApplyUpdates(true);
                }
                catch
                {
                    throw new Exception("Ошибка в обновлении!");
                    return;
                }

            }, null);
        }
    }
}
