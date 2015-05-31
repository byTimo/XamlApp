using System;
using System.Windows;
using NAppUpdate.Framework;
using NAppUpdate.Framework.Common;
using NAppUpdate.Framework.Sources;

namespace ZappChat.Core
{
    internal static class AppUpdateManager
    {
        public static void SetUrlRemoteServer(string url)
        {
            UpdateManager.Instance.UpdateSource = new SimpleWebSource(url);
            UpdateManager.Instance.ReinstateIfRestarted();
        }

        public static void StartupCheckAndPrepareUpdateFeeds(Action<bool> callBack)
        {
            var updManager = UpdateManager.Instance;
            updManager.CheckForUpdates();
            if (updManager.UpdatesAvailable == 0)
            {
                callBack.Invoke(true);
                return;
            }
            updManager.BeginPrepareUpdates(ar => OnPrepareUpdatesCompleted(ar, callBack), null);
        }



        private static void OnPrepareUpdatesCompleted(IAsyncResult prepareResult, Action<bool> callBack)
        {
            try
            {
                ((UpdateProcessAsyncResult) prepareResult).EndInvoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Updates preperation failed. Check the feed and try again.{0}{1}",
                    Environment.NewLine, ex));
                callBack.Invoke(false);
                return;
            }
            try
            {
                UpdateManager.Instance.ApplyUpdates(true);

            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error while trying to install software updates{0}{1}",
                    Environment.NewLine, ex));
                callBack.Invoke(false);
            }
            UpdateManager.Instance.CleanUp();
        }

    }
}
