using UnityEngine;
using UnityEngine.UI;

public class Pickup : MonoBehaviour, IInteractable
{
    public string itemName;
    public GameObject itemPrefab;
    public Texture2D itemIcon;

    public string pickupPrompt = "Press E to Pick Up";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Interact()
    {
        Inventory inventory = FindFirstObjectByType<Inventory>();

        if (inventory != null)
        {
            bool pickedUp = inventory.AddItem(itemName, itemPrefab, itemIcon);

            // Check if player had enough space and managed to pick up the item
            if (pickedUp)
            {
                Destroy(gameObject);
            }
        }
    }

    public string GetPrompt()
    {
        return pickupPrompt;
    }
}
