﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace SpaceXCoder
{
    [System.Serializable]
    public class ClockIn
    {
        public string date;
        public string itemType;
        public int itemAmount;
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
        public int RocketBomb = 0;
        public int ExtraStar = 0;
        public int Teleport = 0;
        public int PowerOverwhelming = 0;
    }

    public class CONST
    {
        public static Dictionary<string, Dictionary<string, object>> DAILY_BONUS = new Dictionary<string, Dictionary<string, object>>()
        {
            {
                "Monday",
                new Dictionary<string, object>() {
                    { "itemType", "FogLight" },
                    { "itemAmount", 10 },
                    { "weekIndex", 1 }
                }
            },
            {
                "Tuesday",
                new Dictionary<string, object>() {
                    { "itemType", "StopClock" },
                    { "itemAmount", 10 },
                    { "weekIndex", 2 }
                }
            },
            {
                "Wednesday",
                new Dictionary<string, object>() {
                    { "itemType", "BombShortRange" },
                    { "itemAmount", 10 },
                    { "weekIndex", 3 }
                }
            },
            {
                "Thursday",
                new Dictionary<string, object>() {
                    { "itemType", "RocketBomb" },
                    { "itemAmount", 10 },
                    { "weekIndex", 4 }
                }
            },
            {
                "Friday",
                new Dictionary<string, object>() {
                    { "itemType", "Teleport" },
                    { "itemAmount", 10 },
                    { "weekIndex", 5 }
                }
            },
            {
                "Saturday",
                new Dictionary<string, object>() {
                    { "itemType", "ExtraStar" },
                    { "itemAmount", 10 },
                    { "weekIndex", 6 }
                }
            },
            {
                "Sunday",
                new Dictionary<string, object>() {
                    { "itemType", "PowerOverwhelming" },
                    { "itemAmount", 10 },
                    { "weekIndex", 7 }
                }
            }
        };

        public static Dictionary<string, Dictionary<string, string>> ITEM_INFO = new Dictionary<string, Dictionary<string, string>>() {
            {
                "FogLight",
                new Dictionary<string, string>() {
                    { "Sprite", "EngineeringCraftIcons/bg/addons/engeniring_06_b" },
                    { "Name", "Fog Light" },
                    { "Stackable", "false" }
                }
            },
            {
                "StopClock",
                new Dictionary<string, string>() {
                    { "Sprite", "EngineeringCraftIcons/bg/addons/engeniring_33_b" },
                    { "Name", "Stop Clock" },
                    { "Stackable", "true" }
                }
            },
            {
                "BombShortRange",
                new Dictionary<string, string>() {
                    { "Sprite", "EngineeringCraftIcons/bg/en_craft_98" },
                    { "Name", "Short Range Bomb" },
                    { "Stackable", "false" },
                    { "Life", "0.5" },
                    { "CD", "5" },
                }
            },
            {
                "RocketBomb",
                new Dictionary<string, string>() {
                    { "Sprite", "EngineeringCraftIcons/bg/en_craft_95" },
                    { "Name", "Rocket Bomb" },
                    { "Stackable", "false" },
                    { "Life", "0.5" },
                    { "CD", "10" },
                }
            },
            {
                "ExtraStar",
                new Dictionary<string, string>() {
                    { "Sprite", "EngineeringCraftIcons/bg/addons/engeniring_09_b" },
                    { "Name", "Extra Star" },
                    { "Stackable", "false" },
                    { "SingleUse", "true" },//TODO
                }
            },
            {
                "Teleport",
                new Dictionary<string, string>() {
                    { "Sprite", "EngineeringCraftIcons/bg/en_craft_51" },
                    { "Name", "Teleport" },
                    { "Stackable", "false" },
                    { "Life", "0.5" },
                    { "CD", "5" },
                }
            },
            {
                "PowerOverwhelming",
                new Dictionary<string, string>() {
                    { "Sprite", "EngineeringCraftIcons/bg/en_craft_25" },
                    { "Name", "Power Overwhelming" },
                    { "Stackable", "false" },
                    { "Life", "10" },
                    { "CD", "20" },
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
        public List<ClockIn> myClockIns = new List<ClockIn>();

        public void Unlock(int lv)
        {
            unlocked = lv;
        }

        public int Unlocked()
        {
            return unlocked;
        }

        public void SetRecord(int index, LvRecord record)
        {
            lvRecords[index] = record;
        }

        public LvRecord GetRecord(int index)
        {
            return lvRecords[index];
        }

        public LvRecord[] GetAllRecords()
        {
            return lvRecords;
        }

        public bool AppendClockIn(string itemType, int itemAmount)
        {
            ClockIn bonus = new ClockIn()
            {
                date = System.DateTime.Now.ToString("yyyyMMdd"),
                itemType = itemType,
                itemAmount = itemAmount
            };
            Debug.Log("Append ClockIn => " + bonus.date);
            if (myClockIns.Exists(x => x.date == bonus.date && x.itemType == itemType && x.itemAmount == itemAmount) == false)
            {
                myClockIns.Add(bonus);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool HasClockedIn(DateTime date, string itemType, int itemAmount)
        {
            string datestring = date.ToString("yyyyMMdd");
            Debug.Log("HasClockedIn => " + datestring + ", " + itemType + ", " + itemAmount);
            Debug.Log("HasClockedIn => " + myClockIns);

            return myClockIns.Exists(x => x.date == datestring && x.itemType == itemType && x.itemAmount == itemAmount);
        }

        public void ReceiveItem(string itemType, int itemAmount)
        {
            FieldInfo field = myItems.GetType().GetField(itemType);

            Debug.Log("Receive item: " + itemType + ", " + itemAmount);
            if (field != null && AppendClockIn(itemType, itemAmount) == true)
            {
                int value = (int)field.GetValue(myItems);
                field.SetValue(myItems, value + itemAmount);

                Debug.Log("Item received => " + itemType + ": " + field.GetValue(myItems));
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

                Debug.Log("Item left => " + itemType + ": " + field.GetValue(myItems));
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

    public class GameService
    {
        public static Save save = new Save();
        public static Save LoadSave()
        {
            Debug.Log("GameService path: " + Application.persistentDataPath + "/GameService.save");
            if (File.Exists(Application.persistentDataPath + "/GameService.save"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/GameService.save", FileMode.Open);
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
            FileStream file = File.Create(Application.persistentDataPath + "/GameService.save");
            bf.Serialize(file, save);
            file.Close();
        }

        public static Dictionary<string, object> ReadLevelConfig(int level)
        {
            // e.g. {"GridWidth", 5}, {"GridHeight", 5}, {"RockQty", 5}, {"ResourceQty", 4}, {"FogGrowSpeed", 0.0003f}
            FileStream file = File.Open(Application.dataPath + "/Resources/levels.csv", FileMode.Open);
            StreamReader inp_stm = new StreamReader(file);
            int ln = 0;
            Dictionary<string, object> config = new Dictionary<string, object>();

            while (!inp_stm.EndOfStream)
            {
                string inp_ln = inp_stm.ReadLine();
                if (ln > 0)
                {
                    string[] cfg = inp_ln.Split(',');
                    int start = 2;
                    int index = int.Parse(cfg[start + 0]);

                    if (index == level)
                    {
                        config["GridWidth"] = int.Parse(cfg[start + 1]);
                        config["GridHeight"] = int.Parse(cfg[start + 2]);
                        config["RockQty"] = int.Parse(cfg[start + 3]);
                        config["ResourceQty"] = int.Parse(cfg[start + 4]);
                        config["FogGrowSpeed"] = float.Parse(cfg[start + 5]);
                        Debug.Log("READ_LEVEL => " + index + ", " + cfg[start + 1] + ", " + cfg[start + 2] + ", " + cfg[start + 3] + ", " + cfg[start + 4] + ", " + cfg[start + 5]);
                        break;
                    }
                }

                ln++;
            }
            inp_stm.Close();
            return config;
        }
    }

}
