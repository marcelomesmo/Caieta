using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace Caieta.Data.Serialization
{
    public static class Serializer
    {
        // Notes: Add Json.Net to serialize XML and JSON
        public enum Mode {  XML, JSON, BINARY };

        //private static BinaryFormatter formatter;
        private static JsonSerializer serializer;

        public static void Initialize()
        {
            //formatter = new BinaryFormatter();
            serializer = new JsonSerializer();
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
            string fullpath = Path.Combine(Engine.Instance.ContentDirectory, "Data/" + filepath);

            // Save our object to file
            switch (mode)
            {
                case Mode.XML:
                    break;
                case Mode.JSON:
                    string data = JsonConvert.SerializeObject(obj);
                    File.WriteAllText(fullpath + ".json", data);
                    break;
                case Mode.BINARY:
                    //Debug.Log("[Serializer]: Saving file..");
                    //Debug.Log("    Trying to save file to: " + fullpath + ".dat");
                    //Debug.Log("    Opening memory stream..");
                    using (MemoryStream stream = new MemoryStream())
                    using (BsonDataWriter datawriter = new BsonDataWriter(stream))
                    {
                        //Debug.Log("    Serializing data..");
                        try { serializer.Serialize(datawriter, obj); }
                        catch(Exception e) { Debug.Log("/n    Error while serializing: " + e); }

                        //Debug.Log("    Copying serialized data to file..");
                        try { File.WriteAllText(fullpath + ".dat", Convert.ToBase64String(stream.ToArray())); }
                        catch(Exception e) { Debug.Log("/n    Error while writing to file: " + e); }

                        //Debug.Log("    Successfully Serialized! Data: " + Convert.ToBase64String(stream.ToArray()));
                    }
                    break;
                default:
                    Debug.ErrorLog("[Serializer]: Invalid mode '" + mode + "' while serializing.");
                    break;
            }

        }

        #endregion

        #region Load

        public static T Load<T>(string filepath, Mode mode)
        {
            string fullpath = Path.Combine(Engine.Instance.ContentDirectory, "Data/" + filepath);

            switch (mode)
            {
                case Mode.XML:
                    fullpath += ".xml";
                    break;
                case Mode.JSON:
                    fullpath += ".json";
                    break;
                case Mode.BINARY:
                    fullpath += ".dat";
                    break;
                default:
                    break;
            }

            if (File.Exists(fullpath))
            {
                try
                {
                    return Deserialize<T>(fullpath, mode);
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

        private static T Deserialize<T>(string fullpath, Mode mode)
        {
            T data;

            // Reconstruct information from file.
            switch (mode)
            {
                case Mode.XML:
                    /// TODO Not yet implemented
                    data = default(T);
                    break;
                case Mode.JSON:
                    string jsonString = File.ReadAllText(fullpath);
                    data = JsonConvert.DeserializeObject<T>(jsonString);
                    break;
                case Mode.BINARY:
                    Debug.Log("[Serializer]: Loading file..");
                    Debug.Log("    Trying to load file from: " + fullpath);
                    string streamData = "";
                    try { streamData = File.ReadAllText(fullpath); }
                    catch(Exception e) { Debug.Log("    Error while reading from file: " + e); }

                    Debug.Log("    Converting data from stream: " + streamData);
                    byte[] fileData = Convert.FromBase64String(streamData);

                    Debug.Log("    Opening memory stream..");
                    using (MemoryStream stream = new MemoryStream(fileData))
                    using (BsonDataReader reader = new BsonDataReader(stream))
                    {
                        Debug.Log("    Deserializing data..");
                        data = serializer.Deserialize<T>(reader);
                        Debug.Log("    Successfully Deserialized! Data: " + Convert.ToBase64String(stream.ToArray()));
                    }

                    Debug.Log("    Deserialized to object: ");
                    Debug.Log(data);
                    break;
                default:
                    Debug.ErrorLog("[Serializer]: Invalid mode '" + mode + "' while deserializing.");
                    data = default(T);
                    break;
            }

            return data;
        }

        #endregion

    }
}
