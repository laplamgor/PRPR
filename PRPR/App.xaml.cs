using PRPR.Common;
using PRPR.ExReader.Views;
using PRPR.BooruViewer.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using PRPR.BooruViewer.Services;
using System.Diagnostics;
using Windows.ApplicationModel.AppService;
using PRPR.BooruViewer.Tasks;
using Windows.UI.Popups;
using PRPR.Common.Services;
using Windows.Networking.BackgroundTransfer;
using Windows.Graphics.Display;
using PRPR.Common.Models.Global;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;
using Microsoft.QueryStringDotNET;

namespace PRPR
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();

            this.EnteredBackground += App_EnteredBackground;
            this.LeavingBackground += App_LeavingBackground;
            
           
            this.Suspending += OnSuspending;
            this.Resuming += App_Resuming;
            this.UnhandledException += App_UnhandledException;
        }




        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.Handled = false;

            ToastService.ToastDebug("應用閃退StackTrace", e.Exception.StackTrace);
            ToastService.ToastDebug("應用閃退Exception", e.Exception.Message);

        }

        private async void App_LeavingBackground(object sender, LeavingBackgroundEventArgs e)
        {
            var d = e.GetDeferral();

            AppShell shell = Window.Current.Content as AppShell;
            if (shell != null && shell.AppFrame.CanGoBack)
            {
                try
                {
                    await SuspensionManager.RestoreAsync();
                }
                catch (SuspensionManagerException ex)
                {
                    // Something went wrong restoring state.
                    // Assume there is no state and continue
                }
            }

            
            d.Complete();
        }

        private async void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            var deferral = e.GetDeferral();


            await SuspensionManager.SaveAsync();


            deferral.Complete();
        }


        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            base.OnBackgroundActivated(args);

            
            // Run the background task accordingly
            if (args.TaskInstance.Task.Name.CompareTo(nameof(TileUpdateTask)) == 0)
            {
                var activity = new TileUpdateTask();
                activity.Run(args.TaskInstance);
            }
            else if (args.TaskInstance.Task.Name.CompareTo(nameof(WallpaperUpdateTask)) == 0)
            {
                var activity = new WallpaperUpdateTask();
                activity.Run(args.TaskInstance);
            }
            else if (args.TaskInstance.Task.Name.CompareTo(nameof(LockscreenUpdateTask)) == 0)
            {
                var activity = new LockscreenUpdateTask();
                activity.Run(args.TaskInstance);
            }
            else if (args.TaskInstance.Task.Name.CompareTo("ToastActionTask") == 0)
            {
                ToastNotificationActionTriggerDetail d = (ToastNotificationActionTriggerDetail)args.TaskInstance.TriggerDetails;
                QueryString queryString = QueryString.Parse(d.Argument);

                switch (queryString["action"])
                {
                    case "favorite":
                        var favoriteTask = new FavoriteTask();
                        favoriteTask.Run(args.TaskInstance);
                        break;
                    case "undoLockscreen":
                        var wallpaperUndoTask = new LockscreenUndoTask();
                        wallpaperUndoTask.Run(args.TaskInstance);
                        break;
                    case "undoWallpaper":
                        var lockscreenUndoTask = new WallpaperUndoTask();
                        lockscreenUndoTask.Run(args.TaskInstance);
                        break;
                    default:
                        break;
                }
                
            }
            else if (args.TaskInstance.Task.Name.StartsWith(nameof(FavoriteTask)))
            {
                var activity = new FavoriteTask();
                activity.Run(args.TaskInstance);
            }
            else if (args.TaskInstance.Task.Name.StartsWith(nameof(WallpaperUndoTask)))
            {
                var activity = new WallpaperUndoTask();
                activity.Run(args.TaskInstance);
            }
            else
            {
                var x = args.TaskInstance.Task.Trigger;
                var details = args.TaskInstance.TriggerDetails as BackgroundTransferCompletionGroupTriggerDetails;
                var downloads = details.Downloads.ToList();
                // TODO: validate the download
                ToastService.ToastDebug("Download Finished", $"{downloads.Where(o => o.Progress.Status == BackgroundTransferStatus.Completed).Count()}/{downloads.Count} images downloaded successfully.");
            }
        }




        private void App_Resuming(object sender, object e)
        {

        }


        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            AppShell shell = Window.Current.Content as AppShell;

            

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (shell == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                shell = new AppShell();

                //Associate the frame with a SuspensionManager key
                SuspensionManager.RegisterFrame(shell.AppFrame, "AppFrame");

                shell.AppFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        // Something went wrong restoring state.
                        // Assume there is no state and continue
                    }
                }

                // Place the frame in the current Window
                Window.Current.Content = shell;
            }


            // Back to the first(home) page if the app already have pages
            while (shell.AppFrame.CanGoBack)
            {
                shell.AppFrame.GoBack();
            }

            foreach (var item in shell.AppFrame.BackStack)
            {
                Debug.WriteLine(item.SourcePageType);
            }
            

            if (shell.AppFrame.Content == null)
            {
                shell.AppFrame.Navigate(typeof(BooruViewer.Views.HomePage), e.Arguments);
                //shell.AppFrame.Navigate(typeof(BooruViewer.Views.TestPage), e.Arguments);
            }


            // Handle the launch from a toast
            var queryString = QueryString.Parse(e.Arguments);
            string action = null;
            if (queryString.TryGetValue("action", out action))
            {
                switch (action)
                {
                    case "viewPost":

                        break;
                    default:
                        break;
                }
            }

            

            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

            // Ensure the current window is active
            Window.Current.Activate();
            
            AppSettings.Current.ScreenHeight = DisplayInformation.GetForCurrentView().ScreenHeightInRawPixels;
            AppSettings.Current.ScreenWidth = DisplayInformation.GetForCurrentView().ScreenWidthInRawPixels;
            

            // Update Tags
            if (TagDataBase.AllTags.Count == 0)
            {
                try
                {

                    await TagDataBase.DownloadLatestTagDBAsync();
                }
                catch (Exception ex)
                {

                    //new MessageDialog(ex.Message).ShowAsync();
                }
            }



            BackgroundAccessStatus status = await BackgroundExecutionManager.RequestAccessAsync();
            BackgroundTaskBuilder builder = new BackgroundTaskBuilder()
            {
                Name = "ToastActionTask",
                //TaskEntryPoint = "RuntimeComponent1.NotificationActionBackgroundTask"
            };
            builder.SetTrigger(new ToastNotificationActionTrigger());
            BackgroundTaskRegistration registration = builder.Register();



            await new MessageDialog("這是內測版應用，所有內容和功能不代表最終成品。內測者有義務不定期使用ＱＱ或電郵提出反饋和改善建議。請勿在應用商店就內測版內容作出評分或評論。如果無法理解或者無法同意此守則，請卸載本應用。電郵：laplamgor2@gmail.com", "使用須知").ShowAsync();


        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {

        }

    }
}
