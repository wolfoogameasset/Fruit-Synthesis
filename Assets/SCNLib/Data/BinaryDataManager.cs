using SCN.Common;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace SCN.BinaryData
{
    [System.Serializable]
    public abstract class BinaryData
    {
        public abstract void SetupDefault();
    }

    public class BinaryDataManager
    {
        public static T LoadData<T>(string fileName) where T : BinaryData, new()
        {
            if (File.Exists(PathFile(fileName)))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(PathFile(fileName), FileMode.Open);
                var stData = (StringData)bf.Deserialize(file);
                file.Close();

                var tructData = JsonTool.DeserializeObject<T>(stData.St);

                return tructData;
            }
            else
            {
                return SetupDefaultData<T>(fileName);
            }
        }

        static T SetupDefaultData<T>(string fileName) where T : BinaryData, new()
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(PathFile(fileName));

            var dataStruct = new T();
            dataStruct.SetupDefault();

            var dataSt = new StringData(JsonTool.SerializeObject(dataStruct));

            bf.Serialize(file, dataSt);
            file.Close();

            return dataStruct;
        }

        public static void SaveData<T>(T data, string fileName) where T : BinaryData, new()
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(PathFile(fileName), FileMode.Open);
            bf.Serialize(file, new StringData(JsonTool.SerializeObject(data)));
            file.Close();
        }

        static string PathFile(string fileName)
		{
            return $"{Application.persistentDataPath}/{fileName}";
		}

        [System.Serializable]
        class StringData
        {
            public string St;

            public StringData(string st)
			{
                St = st;
			}
		}
    }
}