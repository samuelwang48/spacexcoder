using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using TMPro;

namespace SpaceXCoder
{
    public static class Inventory
    {
        public static void InitDash(GameObject[] SkillSlot, GameObject PrefabItemTpl, Action<GameObject, DashConfig> Callback)
        {
            DashConfig[] myDashConfig = GameService.LoadSave().GetDashConfig();
            for (int i = 0; i < myDashConfig.Length; i++)
            {
                DashConfig c = myDashConfig[i];
                if (c != null)
                {
                    Debug.Log("Dash Config => " + i + ", " + c.gameItemIndex + ", " + c.type);
                    if (c.type == "GameItem" && c.gameItemIndex > -1)
                    {
                        Debug.Log("Dash Config GameItem => " + c.gameItemIndex);
                        GameObject newObj = SpaceXCoder.Inventory.PopulateItem(SkillSlot[i].transform.GetChild(0).transform, PrefabItemTpl, c.gameItemIndex);
                        newObj.AddComponent<LayoutElement>();

                        if (Callback != null)
                        {
                            Button itemBtn = newObj.GetComponent<Button>();
                            itemBtn.onClick.AddListener(delegate
                            {
                                int cd = newObj.transform.Find("Image/CD").gameObject.GetComponent<UnityEngine.UI.Extensions.UICircle>().FillPercent;

                                if (cd > 0)
                                {
                                    Debug.Log("cd time => " + cd + " wait till it becomes zero");
                                    return;
                                }
                                else
                                {
                                    Callback(newObj, c);
                                }
                            });
                        }
                    }
                    else if (c.type == "Skill" && c.skillName != "")
                    {
                        GameObject obj = GameObject.Find(c.skillName);
                        Transform parent = SkillSlot[i].transform.GetChild(0).transform;
                        GameObject newObj = UnityEngine.Object.Instantiate(obj, parent) as GameObject;
                        newObj.transform.SetParent(parent);
                        newObj.transform.SetSiblingIndex(0);
                        newObj.AddComponent<LayoutElement>();
                        if (Callback != null)
                        {
                            Callback(newObj, c);
                        }
                    }
                }
            }
        }

        public static void InitGameItemList(
            GameObject PrefabItemTpl,
            GameObject InventoryCellPrefab,
            GameObject GameInventoryOverlay
        )
        {
            int gridWidth = 8;
            int gridHeight = 1;
            int numberToCreate = gridWidth * gridHeight;

            Dictionary<string, int> dict = GameService.LoadSave().ListItemDict();
            List<GameObject> GameItemList = new List<GameObject>();
            GameObject newCell;

            for (int i = 0; i < numberToCreate; i++)
            {
                newCell = UnityEngine.Object.Instantiate(InventoryCellPrefab, GameInventoryOverlay.transform);
                GameItemList.Add(newCell);
            }

            for (int index = 0; index < dict.Count; index++)
            {
                Transform InventoryCell = GameItemList[index].transform;
                PopulateItem(InventoryCell, PrefabItemTpl, index);
            }
        }
        public static GameObject PopulateItem(Transform InventoryCell, GameObject PrefabItemTpl, int index)
        {
            Dictionary<string, int> dict = GameService.LoadSave().ListItemDict();
            KeyValuePair<string, int> kv = dict.ElementAt(index);
            Debug.Log("key value pair: " + kv.Key + "=>" + kv.Value + " | sprite " + SpaceXCoder.CONST.ITEM_INFO[kv.Key]["Sprite"]);
            GameObject newObj = UnityEngine.Object.Instantiate(PrefabItemTpl, InventoryCell) as GameObject;
            newObj.transform.SetParent(InventoryCell);
            newObj.transform.SetSiblingIndex(0);
            newObj.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>(SpaceXCoder.CONST.ITEM_INFO[kv.Key]["Sprite"]);
            newObj.transform.Find("Qty").GetComponent<TextMeshProUGUI>().SetText(kv.Value.ToString());
            newObj.transform.Find("Life").gameObject.SetActive(false);
            newObj.transform.Find("Image/CD").gameObject.SetActive(false);
            newObj.transform.Find("Image/CD").gameObject.GetComponent<UnityEngine.UI.Extensions.UICircle>().FillPercent = 0;
            InventoryCell.GetChild(0).name = index.ToString();
            InventoryCell.gameObject.AddComponent<LayoutElement>();

            return newObj;
        }
    }
}