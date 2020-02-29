using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace SpaceXCoder {

    [System.Serializable]
    public class Save
    {
        public int unlocked = 0;
        public LvRecord[] lvRecords = new LvRecord[20];
        public MyItems myItems = new MyItems();

        public void ReceiveItem(string itemType, int itemAmount)
        {
            if (itemType == "FogLight")
            {
                myItems.FogLight += itemAmount;
                Debug.Log("Item received => Fog Light: " + myItems.FogLight);
            }
        }

        public void ListItems()
        {
            Debug.Log("List Items");

            FieldInfo[] fi = myItems.GetType().GetFields();

            for (int i = 0; i < fi.Length; i++)
            {
                Debug.Log(fi[i].Name + ":" + fi[i].GetValue(myItems));
            }
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
        public int FogLight;
        public int StopClock;
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
