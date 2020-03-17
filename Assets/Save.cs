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

        public static Dictionary<string, string> ITEM_NAME = new Dictionary<string, string>()
        {
            { "FogLight", "Fog Light" },
            { "StopClock", "Stop Clock" },
            { "BombShortRange", "Short Range Bomb" }
        };

        public static Dictionary<string, Dictionary<string, dynamic>> ITEM_INFO = new Dictionary<string, Dictionary<string, dynamic>>() {
            {
                "FogLight",
                new Dictionary<string, dynamic>() {
                    { "Sprite", Resources.Load<Sprite>("EngineeringCraftIcons/bg/addons/engeniring_06_b") },
                    { "Name", "Fog Light" },
                    { "Stackable", false }
                }
            },
            {
                "StopClock",
                new Dictionary<string, dynamic>() {
                    { "Sprite", Resources.Load<Sprite>("EngineeringCraftIcons/bg/addons/engeniring_33_b") },
                    { "Name", "Stop Clock" },
                    { "Stackable", true }
                }
            },
            {
                "BombShortRange",
                new Dictionary<string, dynamic>() {
                    { "Sprite", Resources.Load<Sprite>("EngineeringCraftIcons/bg/en_craft_98") },
                    { "Name", "Short Range Bomb" },
                    { "Stackable", false }
                }
            }
        };
    }

    [System.Serializable]
    public class Save
    {
        public int unlocked = 0;
        public LvRecord[] lvRecords = new LvRecord[20];
        public MyItems myItems = new MyItems();

        public void ReceiveItem(string itemType, int itemAmount)
        {
            FieldInfo field = myItems.GetType().GetField(itemType);

            Debug.Log("Receive item: " + itemType + ", " + itemAmount);
            if (field != null)
            {
                int value = (int)field.GetValue(myItems);
                field.SetValue(myItems, value + itemAmount);
                
                Debug.Log("Item received => Fog Light: " + field.GetValue(myItems));
            }
        }

        public void ConsumeItem(string itemType, int itemAmount)
        {
            FieldInfo field = myItems.GetType().GetField(itemType);

            Debug.Log("Consume item: " + itemType + ", " + itemAmount);
            if (field != null)
            {
                int value = (int)field.GetValue(myItems);
                field.SetValue(myItems, value - itemAmount);

                Debug.Log("Item left => Fog Light: " + field.GetValue(myItems));
            }
        }

        public Dictionary<string, int> ListItemDict()
        {
            Dictionary<string, int> myItemDict = new Dictionary<string, int>();

            FieldInfo[] fi = myItems.GetType().GetFields();

            for (int i = 0; i < fi.Length; i++)
            {
                string key = fi[i].Name;
                int val = (int)fi[i].GetValue(myItems);
                myItemDict[key] = val;
                Debug.Log(fi[i].Name + ":" + fi[i].GetValue(myItems));
            }

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
    public class MyItems
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
            Debug.Log(save.myItems);

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
