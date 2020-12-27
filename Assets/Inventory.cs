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
        public static void InitGameInventoryOverlay(
            GameObject PrefabItemTpl,
            GameObject InventoryCellPrefab,
            GameObject GameInventoryOverlay,
            Action<GameObject, int> Callback
        )
        {
            List<GameObject> InventoryGridList = new List<GameObject>();
            int gridWidth = 10;
            int gridHeight = 1;
            int numberToCreate = gridWidth * gridHeight;
            int cellIndex = 0;

            Dictionary<string, Dictionary<string, string>> itemInfo = SpaceXCoder.CONST.ITEM_INFO;
            Dictionary<string, int> dict = GameService.LoadSave().ListItemDict();
            Transform containerTransform = GameInventoryOverlay.transform.Find("ItemGrid");
            GameObject newCell;

            for (int i = 0; i < numberToCreate; i++)
            {
                newCell = UnityEngine.Object.Instantiate(InventoryCellPrefab, containerTransform);
                InventoryGridList.Add(newCell);
            }

            for (int index = 0; index < dict.Count; index++)
            {
                var kv = dict.ElementAt(index);
                Debug.Log("key value pair: " + kv.Key + "=>" + kv.Value);
                Transform InventoryCell = InventoryGridList[cellIndex].transform;

                //if (kv.Value > 0)
                //{
                GameObject newObj = UnityEngine.Object.Instantiate(PrefabItemTpl, InventoryCell) as GameObject;
                newObj.transform.SetParent(InventoryCell);
                newObj.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>(itemInfo[kv.Key]["Sprite"]);
                newObj.transform.Find("Qty").GetComponent<TextMeshProUGUI>().SetText(kv.Value.ToString());
                newObj.transform.Find("Life").gameObject.SetActive(false);
                newObj.transform.Find("Image/CD").gameObject.SetActive(false);
                newObj.transform.Find("Image/CD").gameObject.GetComponent<UnityEngine.UI.Extensions.UICircle>().FillPercent = 0;

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
                        if(Callback != null)
                        {
                            Callback(newObj, i);
                        }
                    }
                });
                cellIndex++;
                //}
            }
        }
    }
}