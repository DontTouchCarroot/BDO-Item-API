using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ItemBase.Core.Services
{
    public sealed class JsonFile<TValue>
        where TValue : class
    {
        private static JsonSerializerOptions s_Options = new JsonSerializerOptions()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping

        };
        public static TValue? Load(string path)
        {
            if(!File.Exists(path)){
                throw new FileNotFoundException(path);
            }

            string file = File.ReadAllText(path);

            var result = JsonSerializer.Deserialize<TValue>(file);

            return result;
        }
        public static async Task<TValue?> LoadAsync(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            string file = await File.ReadAllTextAsync(path);

            var result = JsonSerializer.Deserialize<TValue>(file);


            return result;

        }
        public static void Save(string path, TValue value)
        {

            string str = JsonSerializer.Serialize(value, s_Options);

            File.WriteAllText(path, str);   
        }

        public static async Task SaveAsync(string path, TValue value)
        {
            string str = JsonSerializer.Serialize<TValue>(value, s_Options);

            await File.WriteAllTextAsync(path, str);
        }
    }
}
