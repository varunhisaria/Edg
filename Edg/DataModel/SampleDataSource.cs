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

    public class Contact
    {
        public Contact(String Name, String Email, String Phone, String Facebook)
        {
            
            this.name = Name;
            this.email = Email;
            this.phone = Phone;
            this.facebook = Facebook;
        }

        
        public string name { get; private set; }
        public string email { get; private set; }
        public string phone { get; private set; }
        public string facebook { get; private set; }

        public override string ToString()
        {
            return this.name;
        }
    }



    /// <summary>
    /// Generic event data model.
    /// </summary>
    /// 

    public class Event
    {
        public Event(String Name, String Description, String File)
        {
            this.name = Name;
            this.description = Description;
            this.file = File;
            
            this.Contacts = new ObservableCollection<Contact>();
        }

        public string name { get; private set; }
        public string description { get; private set; }
        public string file { get; private set; }

        public ObservableCollection<Contact> Contacts { get; private set; }


        public override string ToString()
        {
            return this.name;
        }
    }

    /// <summary>
    /// Generic category data model.
    /// </summary>
    public class Category
    {
        public Category(String Category_name)
        {
            this.category_name = Category_name;
            
            this.Events = new ObservableCollection<Event>();
        }

        public string category_name { get; private set; }
        public ObservableCollection<Event> Events { get; private set; }

        public override string ToString()
        {
            return this.category_name;
        }
    }

    
    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// SampleDataSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<Category> _groups = new ObservableCollection<Category>();
        public ObservableCollection<Category> Groups
        {
            get { return this._groups; }
        }

        public static async Task<IEnumerable<Category>> GetGroupsAsync()
        {
            await _sampleDataSource.GetSampleDataAsync();

            return _sampleDataSource.Groups;
        }

        public async void ShowMessage(string msg)
        {
            MessageDialog messageDialog = new MessageDialog(msg);
            await messageDialog.ShowAsync();
        }
        public static async Task<Category> GetGroupAsync(string Category_name)
        {
            await _sampleDataSource.GetSampleDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.Groups.Where((group) => group.category_name.Equals(Category_name));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static async Task<Event> GetItemAsync(string Name)
        {
            await _sampleDataSource.GetSampleDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.Groups.SelectMany(group => group.Events).Where((item) => item.name.Equals(Name));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        /*
        public static async Task<Contact> GetContactAsync(string Name)
        {
            await _sampleDataSource.GetSampleDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.Groups.SelectMany(group => group.Events).Where((item) => item.name.Equals(Name));
            if (matches.Count() == 1) return matches.First();
            return null;
        }
        */

        private async Task GetSampleDataAsync()
        {
            if (this._groups.Count != 0)
                return;
            string jsonText="";
            if (GlobalVars.flag == 0)
            {
                try
                {
                    //Uri dataUri = new Uri("ms-appx:///DataModel/SampleData.json");
                    //StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
                    //jsonText = await FileIO.ReadTextAsync(file);

                    var applicationFolder = ApplicationData.Current.LocalFolder;
                    var storageFile = await applicationFolder.GetFileAsync("json.txt");
                    jsonText = await Windows.Storage.FileIO.ReadTextAsync(storageFile);
                }
                catch(Exception e)
                {
                    //not able to open json.txt. Critical error.
                }
                
            }
            else
            {
                jsonText = GlobalVars.jso;
                try
                {
                    var applicationFolder2 = ApplicationData.Current.LocalFolder;
                    var storageFile2 = await applicationFolder2.CreateFileAsync("json.txt", CreationCollisionOption.ReplaceExisting);
                    await Windows.Storage.FileIO.WriteTextAsync(storageFile2, jsonText);
                    
                    //byte[] bytes= new byte[jsonText.Length * sizeof(char)];
                    //System.Buffer.BlockCopy(jsonText.ToCharArray(), 0, bytes, 0, bytes.Length);
                    
                    //var applicationFolder = ApplicationData.Current.LocalFolder;
                    //var storageFile = await applicationFolder.CreateFileAsync("json.txt", CreationCollisionOption.ReplaceExisting);
                    
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
            
            JsonArray jsonArray = jsonObject["categories"].GetArray();
            
            foreach (JsonValue groupValue in jsonArray)
            {
                JsonObject groupObject = groupValue.GetObject();
                Category group = new Category(groupObject["category_name"].GetString());

                foreach (JsonValue itemValue in groupObject["category_details"].GetArray())
                {

                    JsonObject itemObject = itemValue.GetObject();

                    
                    
                    Event tempEvent =  new Event(itemObject["name"].GetString(),
                                                       itemObject["description"].GetString(),
                                                       itemObject["file"].GetString());
                    foreach (JsonValue contactValue in itemObject["contacts"].GetArray())
                    {
                        
                         JsonObject contactObject = contactValue.GetObject();
                         //var lii = contactObject.Keys;
                         //foreach (var ss in lii)
                         //{
                         //    Debug.WriteLine(ss.ToString());
                         //}
                         //var ss = contactObject.GetNamedValue("id");
                         //Debug.WriteLine(ss.GetString()); 
                         //Debug.WriteLine(contactObject["id"].GetString().ToString()); 
                         Contact tempContact = new Contact(contactObject["name"].GetString(),
                                                       contactObject["email"].GetString(),
                                                       "+91"+contactObject["phone"].GetString(),
                                                       contactObject["facebook"].GetString());
                         tempEvent.Contacts.Add(tempContact);
                    }
                    group.Events.Add(tempEvent);
                }
                this.Groups.Add(group);
            }
        }
    }
}