﻿using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace SpaceXCoder {

    [System.Serializable]
    public class Save
    {
        public int unlocked = 0;
        public LvRecord[] lvRecords = new LvRecord[20];
    }

    [System.Serializable]
    public class LvRecord
    {
        public int score = 0;
        public int timeLeft = 0;
    }

    public class GameSave
    {
        public static Save Load()
        {
            SpaceXCoder.Save save = new Save();
            Debug.Log("GameSave path: " + Application.persistentDataPath + "/gamesave.save");
            if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
                save = (Save)bf.Deserialize(file);
                file.Close();
            }

            string json = JsonUtility.ToJson(save);
            Debug.Log("Loading...");
            Debug.Log(json);

            return save;
        }

        public static void Write(Save save)
        {
            string json = JsonUtility.ToJson(save);
            Debug.Log("Saving...");
            Debug.Log(json);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
            bf.Serialize(file, save);
            file.Close();
        }
    }

}
