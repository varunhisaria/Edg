using Edg.Common;
using Edg.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class TestPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public ObservableCollection<Event> obj;
        public int current;

        public TestPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

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
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            var group = await SampleDataSource.GetGroupAsync((string)e.NavigationParameter);
            
            
            obj = group.Events;
            this.DefaultViewModel["Events"] = obj;
            string cat = group.category_name;
            MyPivot.Title = cat;
            MyPivot.ItemsSource = obj;
            ////var obj = group.Events;
            //string cat = group.category_name;
            //MyPivot.Title = cat;

            //string temp = "TemplateGrid";
            //int count = 0;
            //foreach (Event ob in obj)
            //{
            //    //object templateHub;
                


            //    //PivotItem pivot = new PivotItem();
            //    //pivot.Header = ob.name;
            //    //pivot.Tag = count.ToString();

            //    //pivot.ContentTemplate = this.Resources[temp + count.ToString()] as DataTemplate;

            //    //MyPivot.Items.Add(pivot);
                
            //    count++;

            //}
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

        private void MyPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MyCommandBar.PrimaryCommands.Clear();
            MyCommandBar.SecondaryCommands.Clear();

            current = ((Pivot)sender).SelectedIndex;

            //MyCommandBar.PrimaryCommands.Insert(0, new AppBarSeparator());
            
            if (obj[current].file != "")
            {
                AppBarButton addButton = new AppBarButton();
                addButton.Icon = new SymbolIcon(Symbol.Attach);
                addButton.Label = "rules";
                addButton.Click += addButton_Click;
                MyCommandBar.PrimaryCommands.Insert(0, addButton);
            }
             

             int num = obj[current].Contacts.Count;
             if (num > 0)
             {
                 AppBarButton contactButton1 = new AppBarButton();
                 contactButton1.Label = "contact: " + obj[current].Contacts[0].name;
                 contactButton1.Click += contactButton1_Click;
                 MyCommandBar.SecondaryCommands.Insert(0, contactButton1);
             }
             if (num > 1)
             {
                 AppBarButton contactButton2 = new AppBarButton();
                 contactButton2.Label = "contact: " + obj[current].Contacts[1].name;
                 contactButton2.Click += contactButton2_Click;
                 MyCommandBar.SecondaryCommands.Insert(1, contactButton2);
             }

            
        }

        void contactButton2_Click(object sender, RoutedEventArgs e)
        {
           Windows.ApplicationModel.Calls.PhoneCallManager.ShowPhoneCallUI(obj[current].Contacts[1].phone, obj[current].Contacts[1].name);
        }

        void contactButton1_Click(object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine(obj[current].Contacts[0].name);
            //Debug.WriteLine(obj[current].Contacts[0].phone);
            Windows.ApplicationModel.Calls.PhoneCallManager.ShowPhoneCallUI(obj[current].Contacts[0].phone, obj[current].Contacts[0].name);
        }

        public async void ShowMessage(string msg)
        {
            MessageDialog messageDialog = new MessageDialog(msg);
            await messageDialog.ShowAsync();
        }

        async void addButton_Click(object sender, RoutedEventArgs e)
        {
            HttpClient http = new System.Net.Http.HttpClient();
            string fname="";
            LayoutRoot.Visibility = Visibility.Collapsed;
            MyProgressRing.IsActive = true;
            try
            {
                byte[] buffer = await http.GetByteArrayAsync(new Uri("http://edg.co.in/content/" + obj[current].file));
                fname = obj[current].file.Substring(5);
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
                ShowMessage("File cannot be downloaded! Check your data connection.");
                //Debug.WriteLine("mkkjhjjhll");

            }
        }
    }
}
