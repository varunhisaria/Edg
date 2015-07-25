using Edg.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Edg
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public MainPage()
        {
            this.InitializeComponent();
            //HideStatusBar();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        //async public void HideStatusBar()
        //{
        //    await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();
        //} 

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            GlobalVars.flag2 = 0;
            GetJson();
        }


        async public void GetJson()
        {
            var result = "";
            try
            {
                var client = new HttpClient(); // Add: using System.Net.Http;
                var response = await client.GetAsync(new Uri("http://edg.co.in/getsponsor.php"));
                result = await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                //Debug.WriteLine("Exception pakra gaya!");
                //ShowMessage("No internet connection found! Contents could not be updated.");
            }
            if (result == "")
            {
                //Debug.WriteLine("Exception yahan pakra gaya!");
                //ShowMessage("No internet connection found! Contents could not be updated.");
            }
            else
            {
                result = "{ \"sponsors\": " + result + "}";
                GlobalVars.flag2 = 1;
                GlobalVars.jso2 = result;
            }
        }

        public async void ShowMessage(string msg)
        {
            MessageDialog messageDialog = new MessageDialog(msg);
            await messageDialog.ShowAsync();
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

       

        async private void twiiter_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://twitter.com/GeekonixEdge"));
        }

        async private void facebook_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://facebook.com/Gx.Edg"));
        }

        async private void youtube_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://www.youtube.com/channel/UCSwFemGqe1XRmVlg1jRNJYw"));
        }

        
        private void Events_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(CategoriesPage));
        }

        private void SpecialEvents_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(SpecialEvents));
        }

        private void Sponsors_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(SponsorsPage));
        }

        private void Workshops_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(WorkshopsPage));
        }

        private async void Schedule_Tapped(object sender, TappedRoutedEventArgs e)
        {
            HttpClient http = new System.Net.Http.HttpClient();
            string fname = "";
            LayoutRoot.Visibility = Visibility.Collapsed;
            MyProgressRing.IsActive = true;
            try
            {
                byte[] buffer = await http.GetByteArrayAsync(new Uri("http://edg.co.in/content/pdfs/schedule.pdf"));
                fname = "schedule.pdf";
                Debug.WriteLine("dhj: " + fname);
                try
                {
                    IStorageFolder applicationFolder = ApplicationData.Current.LocalFolder;
                    IStorageFile storageFile = await applicationFolder.CreateFileAsync(fname, CreationCollisionOption.ReplaceExisting);
                    using (Stream stream = await storageFile.OpenStreamForWriteAsync())
                    {
                        await stream.WriteAsync(buffer, 0, buffer.Length);
                    }
                }
                catch (Exception ee)
                {
                    Debug.WriteLine("mkll");
                }
            }
            catch
            {
                //no net connection. pdf cannot be downloaded.

            }
            //// Access the file.
            try
            {
                var local = Windows.Storage.ApplicationData.Current.LocalFolder;
                StorageFile pdffile = await local.GetFileAsync(fname);
                // Launch the pdf file.
                LayoutRoot.Visibility = Visibility.Visible;
                MyProgressRing.IsActive = false;
                await Launcher.LaunchFileAsync(pdffile);
            }
            catch (Exception ee)
            {
                LayoutRoot.Visibility = Visibility.Visible;
                MyProgressRing.IsActive = false;
                ShowMessage("Edge 2015 is from 3rd to 5th April. Detailed schedule to be updated soon. Check bak later.");
                //Debug.WriteLine("mkkjhjjhll");

            }
        }

       

        private void AboutUs_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutUs));
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AppCreatorPage));
        }

        private void StarAttractions_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(StarPage));
        }

       
    }
}
