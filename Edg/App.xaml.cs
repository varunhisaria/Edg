using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Edg
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        private TransitionCollection transitions;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        /// 
        public async void CreateFile()
        {
            
            string jsonText;
            try
            {
                Uri dataUri = new Uri("ms-appx:///DataModel/SampleData.json");
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
                jsonText = await FileIO.ReadTextAsync(file);
                
                //Debug.WriteLine(jsonText);
                
                var applicationFolder = ApplicationData.Current.LocalFolder;
                var storageFile = await applicationFolder.CreateFileAsync("json.txt", CreationCollisionOption.ReplaceExisting);
                await Windows.Storage.FileIO.WriteTextAsync(storageFile, jsonText);
                
                
                //Debug.WriteLine("creating file for the first time");

                //jsonText = await Windows.Storage.FileIO.ReadTextAsync(storageFile);
                //Debug.WriteLine(jsonText);
            }
            catch (Exception e)
            {
                //not able to create json.txt for the first time. Critical Error
            }
        }

        public async void CreateFileSponsor()
        {
            //Debug.WriteLine("in func");
            string jsonText;
            try
            {
                Uri dataUri2 = new Uri("ms-appx:///SponsorInit.txt");
                
                    StorageFile file2 = await StorageFile.GetFileFromApplicationUriAsync(dataUri2);
                    jsonText = await FileIO.ReadTextAsync(file2);
                    //Debug.WriteLine("yahi mila");
                
                
                Debug.WriteLine(jsonText);



                var applicationFolder2 = ApplicationData.Current.LocalFolder;
                var storageFile2 = await applicationFolder2.CreateFileAsync("sponsors.txt", CreationCollisionOption.ReplaceExisting);
                await Windows.Storage.FileIO.WriteTextAsync(storageFile2, jsonText);


                Debug.WriteLine("creating file for the first time");

                jsonText = await Windows.Storage.FileIO.ReadTextAsync(storageFile2);
                Debug.WriteLine(jsonText);
            }
            catch (Exception e)
            {
                Debug.WriteLine("not created");
                //not able to create json.txt for the first time. Critical Error
            }
        }
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Set the background color of the status bar, and DON'T FORGET to set the opacity!
            var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            //statusBar.BackgroundColor = ((SolidColorBrush)App.Current.Resources["PhoneAccentBrush"]).Color;
            statusBar.BackgroundColor = Colors.Black;
            statusBar.BackgroundOpacity = 1;
 
            // Set the text on the ProgressIndicator, and show it.
            statusBar.ProgressIndicator.Text = "Edge";
            await statusBar.ProgressIndicator.ShowAsync();
 
            // If the progress value is null (which is the default value), the progress indicator is in an 
            //indeterminate state (dots moving from left to right).
            // Set it to 0 if you don't wish to show the progress bar.
            statusBar.ProgressIndicator.ProgressValue = 0;

            // Set the desired bounds on the application view to use the core window, i.e., the entire screen (including app bar and status bar

            var applicationView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            applicationView.SetDesiredBoundsMode(Windows.UI.ViewManagement.ApplicationViewBoundsMode.UseCoreWindow);        



            try{
                var applicationFolder = ApplicationData.Current.LocalFolder;
                var storageFile = await applicationFolder.GetFileAsync("json.txt");
            }
            catch{
                CreateFile();                
            }

            try
            {
                var applicationFolder2 = ApplicationData.Current.LocalFolder;
                var storageFile2 = await applicationFolder2.GetFileAsync("sponsors.txt");
                //Debug.WriteLine("found");
            }
            catch
            {
               // Debug.WriteLine("not found");
                CreateFileSponsor();
            }


            for (int i = 0; i < 100000000; i++)
            {
                
            }   
                
    







            //GlobalVars.flag = 0;
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // TODO: change this value to a cache size that is appropriate for your application
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(MainPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
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
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}