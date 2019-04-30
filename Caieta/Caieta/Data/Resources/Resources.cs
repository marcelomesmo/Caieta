using System;

namespace Caieta
{
    public static class Resources
    {   
        // Shortcut to grab Content Manager resources
        public static T Get<T>(string filename)
        {
            try
            {
                return Engine.Instance.Content.Load<T>(filename);
            }
            catch (Exception ex)
            {
                Debug.ErrorLog("[Resources]: Unable to load resource file '" + filename + "' type '" + typeof(T) +"'.");

                return Engine.Instance.Content.Load<T>(filename);
            }
        }


        // Peguei esse codigo de Adriano e sai correndo
        /*
        public static T LoadDeserializedJsonFile<T>(string fileName)
        {
            string jsonString = LoadJsonFile(fileName);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        private static string LoadJsonFile(string fileName)
        {
            return File.ReadAllText(Path.Combine(ContentFullPath, "Data\\" + fileName + ".json"));
        }

        private static object DeserializeJsonFile(string jsonString)
        {
            return JsonConvert.DeserializeObject<object>(jsonString);
        }

        public static void SaveJsonFile<T>(string fileName, T data)
        {
            SaveJsonFile(fileName, JsonConvert.SerializeObject(data));
        }

        private static void SaveJsonFile(string fileName, string jsonText)
        {
            File.WriteAllText(Path.Combine(ContentFullPath, "Data\\" + fileName + ".json"), jsonText);
        }
        */
    }
}
