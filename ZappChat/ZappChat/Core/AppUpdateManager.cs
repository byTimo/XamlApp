using System;
using System.Collections.Generic;
using System.IO;
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
//UpdateManager.Instance.UpdateSource = new SimpleWebSource(url);
            var feed = File.ReadAllText(@"E:\Repositories\ZappChat\ZappChat\ZappChat\bin\DebugInSite\feed.xml");
            UpdateManager.Instance.UpdateSource = new MemorySource(feed);
            UpdateManager.Instance.Config.TempFolder = FileDispetcher.FullPathToUpdateFolder;
            UpdateManager.Instance.ReinstateIfRestarted();
        }

        public static void StartupCheckAndPrepareUpdateFeeds(Action callBack)
        {
            var updManager = UpdateManager.Instance;
            updManager.BeginCheckForUpdates(checkResult =>
            {
                if (checkResult.IsCompleted)
                {
//@TODO ------------------ изменить этот кусок, когда сервер будет выдавать информацию об обновлениях ----------
                    try
                    {
                        ((UpdateProcessAsyncResult) checkResult).EndInvoke();
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
                    updManager.BeginPrepareUpdates(OnPrepareUpdatesCompleted,null);
//                    callBack.Invoke();
                }
            }, null);

        }

        private static void OnPrepareUpdatesCompleted(IAsyncResult prepareResult)
        {
            try
            {
                ((UpdateProcessAsyncResult) prepareResult).EndInvoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Updates preperation failed. Check the feed and try again.{0}{1}",
                    Environment.NewLine, ex));
                Application.Current.Dispatcher.Invoke(Application.Current.Shutdown);
                return;
            }
            MessageBox.Show("Всё хорошо!");
            try
            {
                UpdateManager.Instance.ApplyUpdates(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error while trying to install software updates{0}{1}", Environment.NewLine, ex));
            }
        }

    }
}
