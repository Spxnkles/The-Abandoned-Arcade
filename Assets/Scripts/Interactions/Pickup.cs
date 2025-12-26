using UnityEngine;
using UnityEngine.UI;

public class Pickup : MonoBehaviour, IInteractable
{
    public string itemName;
    public GameObject itemPrefab;
    public Texture2D itemIcon;

    public string pickupPrompt = "Press E to Pick Up";

    public AudioSource pickupSound;

    [Header("Story")]
    public bool completeTask = false;
    public string taskID;

    [Header("Story Based Toggle")]
    public bool storyBasedInteraction = false;
    public enum StoryState { Before, During, After};
    public StoryState storyState = StoryState.Before;
    public bool allowInteraction = true;
    public int stageID = -1;

    // Private
    private bool canInteract = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (storyBasedInteraction)
        {
            switch (storyState)
            {
                case StoryState.Before:
                    if (StoryManager.Instance.objectiveStage < stageID) canInteract = allowInteraction;
                    else canInteract = !allowInteraction;
                    break;

                case StoryState.During:
                    if (StoryManager.Instance.objectiveStage == stageID) canInteract = allowInteraction;
                    else canInteract = !allowInteraction;
                    break;

                case StoryState.After:
                    if (StoryManager.Instance.objectiveStage > stageID) canInteract = allowInteraction;
                    else canInteract = !allowInteraction;
                    break;
            }
        }
    }

    public void Interact()
    {
        if (!canInteract) return;

        if (Inventory.Instance != null)
        {
            bool pickedUp = Inventory.Instance.AddItem(itemName, itemPrefab, itemIcon);

            // Check if player had enough space and managed to pick up the item
            if (pickedUp)
            {
                if (pickupSound != null) pickupSound.Play();
                AdvanceStory();
                Destroy(gameObject);
            }
        }
    }

    public void AdvanceStory()
    {
        if (completeTask)
        {
            completeTask = false;
            StoryManager.Instance.advanceTask(taskID);
        }
    }

    public string GetPrompt()
    {
        if (!canInteract) return "";
        return pickupPrompt;
    }
}
