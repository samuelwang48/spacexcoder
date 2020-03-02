using System;
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
            { "StopClock", Resources.Load<Sprite>("EngineeringCraftIcons/bg/addons/engeniring_33_b") },
            { "BombShortRange", Resources.Load<Sprite>("EngineeringCraftIcons/bg/en_craft_98") }
        };
    }

    [System.Serializable]
    public class Save
    {
        public int unlocked = 0;
        public LvRecord[] lvRecords = new LvRecord[20];
        public MyItem myItemDict = new MyItem();

        public void ReceiveItem(string itemType, int itemAmount)
        {
            Debug.Log("Receive item: " + itemType + ", " + myItemDict.ContainsKey(itemType));
            if (myItemDict.ContainsKey(itemType) == true)
            {
                myItemDict[itemType] += itemAmount;
                Debug.Log("Item received => Fog Light: " + myItemDict[itemType]);
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

    [System.Serializable]
    public class MyItem : Dictionary<string, int>
    {
        public int FogLight = 0;
        public int StopClock = 0;
        public int BombShortRange = 0;
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
            Debug.Log(save.myItemDict);

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
