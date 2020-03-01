﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace SpaceXCoder {

    public class CONST
    {
        public static Dictionary<string, Sprite> ITEM_SPRITE = new Dictionary<string, Sprite>()
        {
            { "FogLight", Resources.Load<Sprite>("EngineeringCraftIcons/bg/addons/engeniring_06_b") },
            { "StopClock", Resources.Load<Sprite>("EngineeringCraftIcons/bg/addons/engeniring_33_b") }
        };
    }

    [System.Serializable]
    public class Save
    {
        public int unlocked = 0;
        public LvRecord[] lvRecords = new LvRecord[20];
        public Dictionary<string, int> myItemDict = new Dictionary<string, int>()
        {
            { "FogLight", 0 },
            { "StopClock", 0 }
        };

        public void ReceiveItem(string itemType, int itemAmount)
        {
            if (itemType == "FogLight")
            {
                myItemDict["FogLight"] += itemAmount;
                Debug.Log("Item received => Fog Light: " + myItemDict["FogLight"]);
            }
            else if (itemType == "StopClock")
            {
                myItemDict["StopClock"] += itemAmount;
                Debug.Log("Item received => Stop Clock: " + myItemDict["StopClock"]);
            }
        }

        public Dictionary<string, int> ListItemDict()
        {
            return myItemDict;
        }
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
