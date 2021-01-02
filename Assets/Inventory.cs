﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using TMPro;

namespace SpaceXCoder
{
    public static class Inventory
    {
        public static void InitGameItemList(
            GameObject PrefabItemTpl,
            GameObject InventoryCellPrefab,
            GameObject GameInventoryOverlay,
            Action<GameObject, int> Callback
        )
        {
            int gridWidth = 8;
            int gridHeight = 1;
            int numberToCreate = gridWidth * gridHeight;
            int cellIndex = 0;

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
                Transform InventoryCell = GameItemList[cellIndex].transform;

                //if (kv.Value > 0)
                //{
                GameObject newObj = PopulateItem(InventoryCell, PrefabItemTpl, index);

                Button itemBtn = newObj.GetComponent<Button>();
                int i = index;
                itemBtn.onClick.AddListener(delegate {
                    int cd = newObj.transform.Find("Image/CD").gameObject.GetComponent<UnityEngine.UI.Extensions.UICircle>().FillPercent;

                    if (cd > 0)
                    {
                        Debug.Log("cd time => " + cd + " wait till it becomes zero");
                        return;
                    }
                    else
                    {
                        //GameItemClicked(newObj, i);
                        //UseGameItem();
                        if (Callback != null)
                        {
                            Callback(newObj, i);
                        }
                    }
                });

                cellIndex++;
                //}
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