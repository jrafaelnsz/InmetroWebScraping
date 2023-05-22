using CertifiqInmetroWebScrapping.Scrap;
using System.Configuration;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace CertifiqInmetroWebScrapping.Util
{
    public class JsonFileManager
    {
        public static void Write(object data, string path)
        {
            try
            {
                string CertificadosPasta = ConfigurationManager.AppSettings["certificadosPasta"];

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(CertificadosPasta);
                }
                if (!File.Exists(path))
                {
                    Console.WriteLine("Criando novo arquivo");
                   // FileStream fs = File.Create(path);
                    
                }

                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true // optional: to format the JSON with indentation
                };

                string jsonString = JsonSerializer.Serialize(data, options);
                File.WriteAllText(path, jsonString);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public static T Read<T>(string path)
        {
            string jsonString = File.ReadAllText(path);

            // Deserialize the JSON string into an instance of the specified type
            return JsonSerializer.Deserialize<T>(jsonString);
        }
    }
}
