using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InventorySlot
{
    public string itemName;
    public GameObject itemPrefab;
    public GameObject slotIcon;
}

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    public InventorySlot[] slots = new InventorySlot[3];
    public List<int> keys;
    public Transform equip;

    public Color slotColor = Color.black;
    public Color selectedSlotColor = Color.white;

    // Private variables
    public int currentSlot = -1;
    private GameObject currentItemObject;

    private void Start()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) EquipSlot(2);
    }

    public void EquipSlot(int slot, bool newItem = false)
    {
        if (currentItemObject != null) Destroy(currentItemObject);

        if (!newItem)
        {
            if (slot == currentSlot)
            {
                currentSlot = -1;
                UpdateSelectorUI();
                return;
            }
            else currentSlot = slot;
        }

        if (equip != null && currentSlot != -1 && slots[slot].itemPrefab != null)
        {
            currentItemObject = Instantiate(slots[slot].itemPrefab, equip);
        }
        UpdateSelectorUI();
    }

    public bool AddItem(string newItemName, GameObject newItemPrefab, Texture2D newItemIcon)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.itemPrefab == null)
            {
                slot.itemName = newItemName;
                slot.itemPrefab = newItemPrefab;
                slot.slotIcon.transform.GetComponentInChildren<RawImage>().texture = newItemIcon;
                slot.slotIcon.transform.GetComponentInChildren<RawImage>().enabled = true;

                EquipSlot(currentSlot, true);

                return true;
            }
        }
        return false;
    }

    public GameObject GetGameObject()
    {
        if (currentSlot != -1)
        {
            return currentItemObject;
        }
        else return null;
    }

    public string CheckEquippedItem()
    {
        if (currentSlot == -1) return null;

        return slots[currentSlot].itemName;
    }

    public void RemoveItem(string itemName)
    {
        if (slots[currentSlot].itemName == itemName)
        {
            slots[currentSlot].itemName = null;
            slots[currentSlot].itemPrefab = null;
            slots[currentSlot].slotIcon.transform.GetComponentInChildren<RawImage>().texture = null;
            slots[currentSlot].slotIcon.transform.GetComponentInChildren<RawImage>().enabled = false;

            currentItemObject = null;
        }
    }

    public void UpdateSelectorUI()
    {
        foreach (InventorySlot slot in slots)
        {
            slot.slotIcon.GetComponent<Outline>().effectColor = slotColor;
        }
        if (currentSlot != -1)
        {
            slots[currentSlot].slotIcon.GetComponent<Outline>().effectColor = selectedSlotColor;
        }
    }



}