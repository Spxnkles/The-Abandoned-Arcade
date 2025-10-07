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
    public InventorySlot[] slots = new InventorySlot[3];
    public Transform equip;

    public Color slotColor = Color.black;
    public Color selectedSlotColor = Color.white;

    // Private variables
    public int currentSlot = -1;
    private GameObject currentItemObject;

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
                updateSelectorUI();
                return;
            }
            else currentSlot = slot;
        }

        if (equip != null && currentSlot != -1 && slots[slot].itemPrefab != null)
        {
            currentItemObject = Instantiate(slots[slot].itemPrefab, equip);
        }
        updateSelectorUI();
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

    public void updateSelectorUI()
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