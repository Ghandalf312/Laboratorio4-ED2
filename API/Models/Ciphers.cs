using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Text.Json;


namespace API.Models
{
    public class Ciphers
    {
        public string OriginalName { get; set; }
        public string CompressedName { get; set; }
        public string CompressedFilePath { get; set; }

        public Ciphers() { }

        public void SetAttributes(string path, string prevName, string newName)
        {
            var FileP = $"{path}/{newName}";

            OriginalName = prevName;
            CompressedName = newName;
            CompressedFilePath = FileP;

            LoadHistList(path);
            Singleton.Instance.HistoryList.Add(this);
            var file = new FileStream($"{path}/CompressionHist", FileMode.OpenOrCreate);
            var BytesToWrite = JsonSerializer.Serialize(Singleton.Instance.HistoryList, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            using var writer = new StreamWriter(file);
            writer.WriteLine(BytesToWrite);
            writer.Close();
            file.Close();
        }

        public static void LoadHistList(string path)
        {
            var file = new FileStream($"{path}/CompressionHist", FileMode.OpenOrCreate);
            if (file.Length != 0)
            {
                Singleton.Instance.HistoryList.Clear();
                using var reader = new StreamReader(file);
                var content = reader.ReadToEnd();
                var list = JsonSerializer.Deserialize<List<Ciphers>>(content, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                foreach (var item in list)
                {
                    Singleton.Instance.HistoryList.Add(item);
                }
            }
            file.Close();
        }
    }
}
