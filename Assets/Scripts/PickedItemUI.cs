using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickedItemUI : MonoBehaviour
{
    [SerializeField] GameObject[] all_Items;
    [SerializeField] GameObject itemsPanel;
    [SerializeField] GameObject noItemText;

    private void Start()
    {
        HidePickedItem();
    }
    public void Setup(List<Item>pickedItemList)
    {
        if (!itemsPanel.activeInHierarchy)
        {
            if (pickedItemList.Count > 0) ShowPickedItem(pickedItemList);
            else ShowNoPickedItem();
        }
        else HidePickedItem();
    }

    void ShowNoPickedItem()
    {
        foreach (GameObject item in all_Items) item.SetActive(false);
        noItemText.SetActive(true);
        itemsPanel.SetActive(true);
    }

    void ShowPickedItem(List<Item> pickedIteList) 
    {
        foreach (GameObject item in all_Items) item.SetActive(false);
        for (int i = 0; i < pickedIteList.Count; i++)
        {
            // CALCULATE POSITION
            int startingPos = -75 * (pickedIteList.Count - 1);
            int Interval = 150;

            int xPos = startingPos + (Interval * i);
            int yPos = 25;

            // PICK ITEM
            GameObject pickedItem = all_Items[pickedIteList[i].order];
            pickedItem.SetActive(true);
            pickedItem.transform.localPosition = new Vector2(xPos, yPos);
        }
        noItemText.SetActive(false);
        itemsPanel.SetActive(true);
    }

    public void HidePickedItem() => itemsPanel.SetActive(false);

}

public class Item
{
    public string itemName { get; protected set; }
    public string info { get; protected set; }
    public int order { get; protected set; }
}

public class Item_Hydran : Item
{
    public Item_Hydran()
    {
        itemName = Keyword.ITEM_NAME_HYDRAN;
        info = "Info: Tahan \"KIRI MOUSE\" untuk menggunakan";
        order = 0;
    }
}

public class Item_Mask : Item
{
    public Item_Mask()
    {
        itemName = Keyword.ITEM_NAME_MASK;
        info = "Info: Memperlambat energi yang berkurang";
        order = 1;
    }
}

public class Item_Kayu : Item
{
    public Item_Kayu()
    {
        itemName = Keyword.ITEM_NAME_KAYU;
        info = "Info: Mungkin akan berguna";
        order = 2;
    }
}

public class Item_Kunci: Item
{
    public Item_Kunci()
    {
        itemName = Keyword.ITEM_NAME_KUNCI;
        info = "Info: Mungkin akan berguna";
        order = 3;
    }
}