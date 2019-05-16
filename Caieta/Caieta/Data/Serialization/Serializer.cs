using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Caieta.Data.Serialization
{
    public static class Serializer
    {
        // Notes: Add Json.Net to serialize XML and JSON
        public enum Mode {  XML, JSON, BINARY };

        private static BinaryFormatter formatter;

        public static void Initialize()
        {
            formatter = new BinaryFormatter();
        }

        #region Save

        public static bool Save<T>(T obj, string filepath, Mode mode)
        {
            try
            {
                Serialize<T>(obj, filepath, mode);
                return true;
            }
            catch
            {
                Debug.ErrorLog("[Serializer]: Couldnt serialize file '" + filepath + "' for game object '" + obj + "'.");
                return false;
            }
        }

        private static void Serialize<T>(T obj, string filepath, Mode mode)
        {
            // Create a FileStream that will write data to file.
            FileStream file = new FileStream(filepath, FileMode.Create);
            /*
            MemoryStream stream = new MemoryStream();
            using (BsonWriter writer = new BsonWriter(stream))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, obj);
                stream.CopyTo(file);
            }

            string data = Convert.ToBase64String(stream.ToArray());
            */

            // Save our object to file
            switch (mode)
            {
                case Mode.XML:
                    break;
                case Mode.JSON:
                    //string data = JsonConvert.SerializeObject(obj);
                    //File.WriteAllText(Path.Combine(Engine.Instance.ContentDirectory, "Data\\" + filepath + ".json"), data);
                    break;
                case Mode.BINARY:
                    //var bf = new BinaryFormatter();
                    //bf.Serialize(file, obj);
                    formatter.Serialize(file, obj);
                    break;
                default:
                    Debug.ErrorLog("[Serializer]: Invalid mode '" + mode + "' while serializing.");
                    break;
            }

            // Close the FileStream when we are done.
            file.Close();
        }

        #endregion

        #region Load

        public static T Load<T>(string filepath, Mode mode)
        {
            if(File.Exists(filepath))
            {
                try
                {
                    return Deserialize<T>(filepath, mode);
                }
                catch
                {
                    Debug.ErrorLog("[Serializer]: Couldnt deserialize file '" + filepath + "'. File invalid or corrupted.");
                    return default(T);
                }
            }
            else
            {
                Debug.ErrorLog("[Serializer]: Couldnt deserialize file '" + filepath + "'. File doesnt exist.");
                return default(T);
            }
        }

        private static T Deserialize<T>(string filepath, Mode mode)
        {
            T data;
            /// Create a FileStream will gain read access to the data file.
            FileStream file = File.OpenRead(filepath);//= new FileStream(filepath, FileMode.Open, FileAccess.Read);

            // Reconstruct information from file.
            switch (mode)
            {
                case Mode.XML:
                    /// TODO Not yet implemented
                    data = default(T);
                    break;
                case Mode.JSON:
                    /// TODO Not yet implemented
                    //string jsonString = File.ReadAllText(Path.Combine(Engine.Instance.ContentDirectory, "Data\\" + filepath + ".json"));
                    //data = JsonConvert.DeserializeObject<T>(jsonString); ;
                    data = default(T);
                    break;
                case Mode.BINARY:
                    //var bf = new BinaryFormatter();
                    //data = (T)bf.Deserialize(file);
                    data = (T)formatter.Deserialize(file);
                    break;
                default:
                    Debug.ErrorLog("[Serializer]: Invalid mode '" + mode + "' while deserializing.");
                    data = default(T);
                    break;
            }

            // Close the readerFileStream when we are done
            file.Close();

            return data;
        }

        #endregion

    }
}
