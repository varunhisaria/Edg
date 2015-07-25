using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The data model defined by this file serves as a representative example of a strongly-typed
// model.  The property names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs. If using this model, you might improve app 
// responsiveness by initiating the data loading task in the code behind for App.xaml when the app 
// is first launched.

namespace Edg.Data
{
    /// <summary>
    /// Generic contact data model.
    /// </summary>
    /// 

    public class Sponsor
    {
        public Sponsor(String Url, String Title)
        {

            this.url = Url;
            this.title = Title;
            
        }


        public string url { get; private set; }
        public string title { get; private set; }
        

        public override string ToString()
        {
            return this.url;
        }
    }
    
    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// SampleDataSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource2
    {
        private static SampleDataSource2 _sampleDataSource = new SampleDataSource2();

        private ObservableCollection<Sponsor> _groups = new ObservableCollection<Sponsor>();
        public ObservableCollection<Sponsor> Groups
        {
            get { return this._groups; }
        }

        public static async Task<IEnumerable<Sponsor>> GetGroupsAsync()
        {
            await _sampleDataSource.GetSampleDataAsync();

            return _sampleDataSource.Groups;
        }

        public async void ShowMessage(string msg)
        {
            MessageDialog messageDialog = new MessageDialog(msg);
            await messageDialog.ShowAsync();
        }
        
        private async Task GetSampleDataAsync()
        {
            if (this._groups.Count != 0)
                return;
            string jsonText="";
            if (GlobalVars.flag2== 0)
            {
                try
                {
                    //Uri dataUri = new Uri("ms-appx:///DataModel/SampleData.json");
                    //StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
                    //jsonText = await FileIO.ReadTextAsync(file);

                    var applicationFolder = ApplicationData.Current.LocalFolder;
                    var storageFile = await applicationFolder.GetFileAsync("sponsors.txt");
                    jsonText = await Windows.Storage.FileIO.ReadTextAsync(storageFile);
                    Debug.WriteLine(jsonText);
                }
                catch(Exception e)
                {
                    Debug.WriteLine("not able to read");
                    //not able to open json.txt. Critical error.
                }
                
            }
            else
            {
                jsonText = GlobalVars.jso2;
                try
                {
                    var applicationFolder2 = ApplicationData.Current.LocalFolder;
                    var storageFile2 = await applicationFolder2.CreateFileAsync("sponsors.txt", CreationCollisionOption.ReplaceExisting);
                    await Windows.Storage.FileIO.WriteTextAsync(storageFile2, jsonText);
                    
                    //byte[] bytes= new byte[jsonText.Length * sizeof(char)];
                    //System.Buffer.BlockCopy(jsonText.ToCharArray(), 0, bytes, 0, bytes.Length);
                    
                    //var applicationFolder = ApplicationData.Current.LocalFolder;
                    //var storageFile = await applicationFolder.CreateFileAsync("sponsors.txt", CreationCollisionOption.ReplaceExisting);
                    
                    //using (var stream = await storageFile.OpenStreamForWriteAsync())
                    //{
                    //    await stream.WriteAsync(bytes, 0, bytes.Length);
                    //}
                }
                catch (Exception e)
                {
                    //not able to update json.txt. Critical Error!
                }
            }

            Debug.WriteLine(jsonText);
            JsonObject jsonObject = JsonObject.Parse(jsonText);
            
            JsonArray jsonArray = jsonObject["sponsors"].GetArray();
            
            foreach (JsonValue groupValue in jsonArray)
            {
                JsonObject groupObject = groupValue.GetObject();
                Sponsor group = new Sponsor("http://edg.co.in/crumbs/images/sponsors/2015/"+groupObject["logo_url"].GetString(), groupObject["title"].GetString());
                               
                this.Groups.Add(group);
            }
        }
    }
}