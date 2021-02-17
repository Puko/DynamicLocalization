using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DynamicResources.Localization
{
    public class LocalizedResources : INotifyPropertyChanged
    {
        private const string DEFAULT_LANGUAGE = "en-US";
        private const string BASE_FILE_NAME = "AppResources";

        private string ResourcesFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Resources");
        private Dictionary<string, string> _dictionary = new Dictionary<string, string>();

        public event PropertyChangedEventHandler PropertyChanged;

        public string CurrentLanguage { get; private set; }
        public async Task SetLanguageAsync(string language, bool forceDownload = false)
        {
            _dictionary.Clear();

            await CreateResourcesAsync(language, forceDownload);
            foreach (var item in _dictionary)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs($"Item[{item.Key}]"));
            }
        }

        private async Task CreateResourcesAsync(string language = DEFAULT_LANGUAGE, bool forceDownload = false)
        {
            CurrentLanguage = language;

            var resourceStream = GetResourceStream(language);

            if (forceDownload || resourceStream == null)
            {
                switch (CurrentLanguage)
                {
                    case "nl-NL":
                        resourceStream = await DownloadAsync("https://drive.google.com/uc?export=download&id=1VNazRyvCxmcbZ_4XdGfv3wV4pychA-iv", "nl-NL");
                        break;
                    case "fr-FR":
                        resourceStream = await DownloadAsync("https://drive.google.com/uc?export=download&id=1Wqbm8RB10rpC0Y8w4hFnRzHG-FJSmke5", "fr-FR");
                        break;
                    default:
                        resourceStream = await DownloadAsync("https://drive.google.com/uc?export=download&id=19Q9eW8BzJ-psTPa-_PiUFRT15Uf1iBVK", DEFAULT_LANGUAGE);
                        break;
                }
            }

            if(resourceStream == null)
            {
                throw new InvalidOperationException("No resources available.");
            }

            var document = XDocument.Load(resourceStream);
            var elements = document.Root.Elements("data");
            foreach(var item in elements)
            {
                _dictionary.Add(item.FirstAttribute.Value, item.Value.TrimStart().TrimEnd());
            }
        }

        [IndexerName("Item")]
        public string this[string key]
        {
            get
            {
                return _dictionary[key];
            }
        }

        private Stream GetResourceStream(string language)
        {
            var filePath = Path.Combine(ResourcesFolder, $"{BASE_FILE_NAME}.{language}.resx");
            if(File.Exists(filePath))
            {
                return File.OpenRead(filePath);
            }

            return null;
        }

        private async Task<Stream> DownloadAsync(string url, string language)
        {
            if (!Directory.Exists(ResourcesFolder))
            {
                Directory.CreateDirectory(ResourcesFolder);
            }

            var filePath = Path.Combine(ResourcesFolder, $"{BASE_FILE_NAME}.{language}.resx");

            using (HttpClient client = new HttpClient())
            {
                using (var response = await client.GetAsync(url))
                {
                    var file = await response.Content.ReadAsStreamAsync();
               
                    using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        await file.CopyToAsync(fs);
                        await file.FlushAsync();
                    }
                }
            }

            return GetResourceStream(language);
        }
    }
}
