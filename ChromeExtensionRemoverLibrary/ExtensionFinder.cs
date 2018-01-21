using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
/*********************************************************************************************************************************
Copyright and Licensing Message

This code is copyright 2018 Gary Cole Jr. 

This code is licensed by Gary Cole to others under the GPLv.3 https://opensource.org/licenses/GPL-3.0 
If you find the code useful or just feel generous a donation is appreciated.

Donate with this link: paypal.me/GColeJr
Please choose Friends and Family

Alternative Licensing Options

If you prefer to license under the LGPL for a project, https://opensource.org/licenses/LGPL-3.0
Single Developers working on their own project can do so with a donation of $20 or more. 
Small and medium companies can do so with a donation of $50 or more. 
Corporations can do so with a donation of $1000 or more.


If you prefer to license under the MS-RL for a project, https://opensource.org/licenses/MS-RL
Single Developers working on their own project can do so with a donation of $40 or more. 
Small and medium companies can do so with a donation of $100 or more.
Corporations can do so with a donation of $2000 or more.


if you prefer to license under the MS-PL for a project, https://opensource.org/licenses/MS-PL
Single Developers working on their own project can do so with a donation of $1000 or more. 
Small and medium companies can do so with a donation of $2000 or more.
Corporations can do so with a donation of $10000 or more.


If you use the code in more than one project, a separate license is required for each project.


Any modifications to this code must retain this message. 
*************************************************************************************************************************************/
namespace ChromeExtensionRemoverLibrary
{
    public class ExtensionFinder
    {
        public string localappdir = "";
        public string extensionsdir = "";
        private GoogleExtension temp = new GoogleExtension();
        bool found = false;
        public ExtensionFinder()
        {
            localappdir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            extensionsdir = $@"{localappdir}\Google\Chrome\User Data\Default\Extensions";
        }
        public List<GoogleExtension> GetExtensions()
        {
            List<GoogleExtension> temp = new List<GoogleExtension>();
            string[] fileEntries = Directory.GetDirectories(extensionsdir);
            foreach (var f in fileEntries)
            {
                GoogleExtension current = new GoogleExtension();
                current = GetExtension($"{f}");
                if (!(temp.Contains(current)))
                    if (!(current.isempty()))
                        temp.Add(current);
                found = false;
            }
            return temp;
        }
        private bool SearchFiles(string[] files, string originaldirectory)
        {
            foreach (var f in files)
            {
                if (f.Contains("manifest.json"))
                {                  
                    Item item = JsonConvert.DeserializeObject<Item>(File.ReadAllText(f));
                    if (item.Name == "__MSG_appName__")
                        item.Name = GetRealName(f, item.Name, "app");
                    else if (item.Name == "__MSG_APP_NAME__")
                        item.Name = GetRealName(f, item.Name, "app2");
                    else if (item.Name == "__MSG_extName__")
                        item.Name = GetRealName(f, item.Name, "ext");
                        temp = new GoogleExtension(item.Name, originaldirectory, item.Version, item.ManifestVersion);                    
                    return true;
                }                
            }
            return false;
        }
        private string checklocalesfile(string manifest, string type)
        {
            if (type.Equals("app"))
            {
                Item2 item = JsonConvert.DeserializeObject<Item2>(File.ReadAllText(manifest));
                return item.appName.message;
            }
            else if (type.Equals("app2"))
            {
                Item4 item = JsonConvert.DeserializeObject<Item4>(File.ReadAllText(manifest));
                return item.app_name.message;
            }
            else
            {
                Item3 item = JsonConvert.DeserializeObject<Item3>(File.ReadAllText(manifest));
                return item.extName.message;
            }
        }
        private string GetRealName(string manifest, string oldnm, string type)
        {
            manifest = manifest.Replace("manifest.json", @"_locales\en_US\messages.json");
            try
            {
                return checklocalesfile(manifest, type);
            }
            catch 
            {
                try
                {
                    manifest=manifest.Replace("en_US", "en");
                    return checklocalesfile(manifest, type);
                }
                catch
                {
                    return oldnm;
                }
            }           
        }
        private void SearchDirectories(string[] directory, string originaldirectory)
        {
            foreach (var d in directory)
            {
                if (!found)
                    found = SearchFiles(Directory.GetFiles(d), originaldirectory);
                if (!(found))
                    SearchDirectories(Directory.GetDirectories(d), originaldirectory);
                if (found)
                    break;
            }
        }
        private GoogleExtension GetExtension(string directory)
        {            
            MemoryStream stream = new MemoryStream();
            found = false;
            string[] fileEntries = Directory.GetDirectories(directory);
            foreach (var f in fileEntries)
            {
                if (!found)
                    found = SearchFiles(Directory.GetFiles(f), directory);
                if (!found)
                    SearchDirectories(Directory.GetDirectories(f), directory);
            }            
            return temp;
        }
    }
   
}
