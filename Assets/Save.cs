using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace SpaceXCoder
{
    [System.Serializable]
    public class DashConfig
    {
        public string name;
        public string type;
    }

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
                    { "Sprite", "engeniring_06_b" },
                    { "Name", "Fog Light" },
                    { "Stackable", "false" }
                }
            },
            {
                "StopClock",
                new Dictionary<string, string>() {
                    { "Sprite", "engeniring_33_b" },
                    { "Name", "Stop Clock" },
                    { "Stackable", "true" }
                }
            },
            {
                "BombShortRange",
                new Dictionary<string, string>() {
                    { "Sprite", "en_craft_98" },
                    { "Name", "Short Range Bomb" },
                    { "Stackable", "false" },
                    { "Life", "0.5" },
                    { "CD", "5" },
                }
            },
            {
                "RocketBomb",
                new Dictionary<string, string>() {
                    { "Sprite", "en_craft_95" },
                    { "Name", "Rocket Bomb" },
                    { "Stackable", "false" },
                    { "Life", "0.5" },
                    { "CD", "10" },
                }
            },
            {
                "ExtraStar",
                new Dictionary<string, string>() {
                    { "Sprite", "engeniring_09_b" },
                    { "Name", "Extra Star" },
                    { "Stackable", "false" },
                    { "SingleUse", "true" },//TODO
                }
            },
            {
                "Teleport",
                new Dictionary<string, string>() {
                    { "Sprite", "en_craft_51" },
                    { "Name", "Teleport" },
                    { "Stackable", "false" },
                    { "Life", "0.5" },
                    { "CD", "5" },
                }
            },
            {
                "PowerOverwhelming",
                new Dictionary<string, string>() {
                    { "Sprite", "en_craft_25" },
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
        public LvRecord[] lvRecords = new LvRecord[0];
        public MyItems myItems = new MyItems();
        public List<ClockIn> myClockIns = new List<ClockIn>();
        public DashConfig[] myDashConfig = new DashConfig[20];

        public DashConfig[] GetDashConfig()
        {
            if (myDashConfig == null)
            {
                myDashConfig = new DashConfig[20];
            }
            return myDashConfig;
        }

        public void UpdateDashConfig(int i, string name, string type)
        {
            myDashConfig[i].name = name;
            myDashConfig[i].type = type;
        }

        public LvRecord GetLvRecord(int i)
        {
            if (i > lvRecords.Length - 1)
            {
                Array.Resize(ref lvRecords, i + 1);
                for (int j = 0; j < lvRecords.Length; j++)
                {
                    if (lvRecords[j] == null)
                    {
                        lvRecords[j] = new LvRecord()
                        {
                            score = 0,
                            timeLeft = 0
                        };
                    }
                }
            }
            return lvRecords[i];
        }

        public void SetLvRecord(int i, LvRecord record)
        {
            if (i > lvRecords.Length - 1)
            {
                Array.Resize(ref lvRecords, i + 1);
                for (int j = 0; j < lvRecords.Length; j++)
                {
                    lvRecords[j] = new LvRecord()
                    {
                        score = 0,
                        timeLeft = 0
                    };
                }
            }
            lvRecords[i] = record;
        }

        public LvRecord[] GetAllRecords()
        {
            return lvRecords;
        }

        public void Unlock(int lv)
        {
            unlocked = lv;
        }

        public int Unlocked()
        {
            if(PlayerPrefs.GetInt("TesterUnlocked") > 0)
            {
                return PlayerPrefs.GetInt("TesterUnlocked");
            }
            else
            {
                return unlocked;
            }
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

            if (PlayerPrefs.GetInt("TesterUnlocked") > 0)
            {
                myClockIns.Add(bonus);
                return true;
            }
            else
            {
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
        }

        public bool HasClockedInThisWeek(DateTime date, string itemType, int itemAmount)
        {
            int ti = 0;
            if ((int)date.DayOfWeek == 0)
            {
                ti = 7;
            }
            else
            {
                ti = (int)date.DayOfWeek;
            }

            bool exist = false;

            for (int i = 0; i <= ti; i++)
            {
                if(myClockIns.Exists(x => x.date == date.AddDays(0 - i).ToString("yyyyMMdd") && x.itemType == itemType && x.itemAmount == itemAmount))
                {
                    exist = true;
                }
            }

            return exist;
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

    public static class GameService
    {
        public static string SavePath = Application.persistentDataPath + "/GameService.save";
        public static TextAsset Levels = Resources.Load("levels") as TextAsset;
        public static Save save = new Save();
        public static Save LoadSave()
        {
            Debug.Log("GameService path: " + SavePath);
            if (File.Exists(SavePath))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(SavePath, FileMode.Open);
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
            FileStream file = File.Create(SavePath);
            bf.Serialize(file, save);
            file.Close();
        }

        public static List<string> ReadStagesByChapter(string chapter)
        {
            List<string> list = new List<string>();
            string[] ln = Levels.text.Split('\n');

            for (int i = 1; i < ln.Length; i++)
            {
                string[] cfg = ln[i].Split(',');
                if (cfg[0] == chapter)
                {
                    list.Add(cfg[1]);
                }
            }

            IEnumerable<string> distinctStages = list.Distinct();
            return list.Distinct().ToList();
        }

        public static List<Dictionary<string, object>> ReadLevelsByStage(string stage)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            string[] ln = Levels.text.Split('\n');

            // e.g. {"GridWidth", 5}, {"GridHeight", 5}, {"RockQty", 5}, {"ResourceQty", 4}, {"FogGrowSpeed", 0.0003f}
            for (int i = 1; i < ln.Length; i++)
            {
                string[] cfg = ln[i].Split(',');
                int start = 2;
                if (cfg[1] == stage)
                {
                    Dictionary<string, object> level = new Dictionary<string, object>();
                    level = ParseLevelConfig(cfg, start);

                    list.Add(level);
                    Debug.Log("READ_LEVEL1 => " + stage + ", " + cfg[start + 0] + ", " + cfg[start + 1] + ", " + cfg[start + 2] + ", " + cfg[start + 3] + ", " + cfg[start + 4] + ", " + cfg[start + 5] + ", " + cfg[start + 6] + ", " + cfg[start + 7]);
                    Debug.Log("READ_LEVEL2 => " + stage + ", " + cfg[start + 8] + ", " + cfg[start + 9] + ", " + cfg[start + 10] + ", " + cfg[start + 11] + ", " + cfg[start + 12] + ", " + cfg[start + 13] + ", " + cfg[start + 14]);
                }
            }
            return list;
        }

        public static Dictionary<string, object> ReadLevelConfig(int level)
        {
            Dictionary<string, object> config = new Dictionary<string, object>();
            string[] ln = Levels.text.Split('\n');

            // e.g. {"GridWidth", 5}, {"GridHeight", 5}, {"RockQty", 5}, {"ResourceQty", 4}, {"FogGrowSpeed", 0.0003f}
            for (int i = 1; i < ln.Length; i++)
            {
                string[] cfg = ln[i].Split(',');
                int start = 2;

                if (int.Parse(cfg[start + 0]) == level)
                {
                    config = ParseLevelConfig(cfg, start);
                    Debug.Log("READ_LEVEL1 => " + level + ", " + cfg[start + 1] + ", " + cfg[start + 2] + ", " + cfg[start + 3] + ", " + cfg[start + 4] + ", " + cfg[start + 5] + ", " + cfg[start + 6] + ", " + cfg[start + 7]);
                    Debug.Log("READ_LEVEL2 => " + level + ", " + cfg[start + 8] + ", " + cfg[start + 9] + ", " + cfg[start + 10] + ", " + cfg[start + 11] + ", " + cfg[start + 12] + ", " + cfg[start + 13] + ", " + cfg[start + 14]);
                    break;
                }
            }
            return config;
        }

        private static Dictionary<string, object> ParseLevelConfig(string[] cfg, int start)
        {
            Dictionary<string, object> config = new Dictionary<string, object>();

            config["Level"] = int.Parse(cfg[start + 0]);
            config["GridWidth"] = int.Parse(cfg[start + 1]);
            config["GridHeight"] = int.Parse(cfg[start + 2]);
            config["RockQty"] = int.Parse(cfg[start + 3]);
            config["ResourceQty"] = int.Parse(cfg[start + 4]);
            config["FogGrowSpeed"] = float.Parse(cfg[start + 5]);
            config["RockGrowNumber"] = int.Parse(cfg[start + 6]);
            config["RockGrowTime"] = int.Parse(cfg[start + 7]);
            config["BlackholeNumber"] = int.Parse(cfg[start + 8]);
            config["BlackholeTimeMin"] = int.Parse(cfg[start + 9]);
            config["BlackholeTimeMax"] = int.Parse(cfg[start + 10]);
            config["BlackholeLifeMin"] = int.Parse(cfg[start + 11]);
            config["BlackholeLifeMax"] = int.Parse(cfg[start + 12]);
            config["BlackholeSizeMin"] = int.Parse(cfg[start + 13]);
            config["BlackholeSizeMax"] = int.Parse(cfg[start + 14]);

            return config;
        }
    }
}
