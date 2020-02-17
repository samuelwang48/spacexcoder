using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace SpaceXCoder {

    [System.Serializable]
    public class Save
    {
        public int unlocked = 0;
    }

    public class GameSave
    {
        public static Save Load()
        {
            Debug.Log("GameSave path: " + Application.persistentDataPath + "/gamesave.save");
            if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
                Save save = (Save)bf.Deserialize(file);
                file.Close();
                return save;
            }
            else
            {
                return new Save();
            }
        }

        public static void Write(Save save)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
            bf.Serialize(file, save);
            file.Close();
        }
    }

}
