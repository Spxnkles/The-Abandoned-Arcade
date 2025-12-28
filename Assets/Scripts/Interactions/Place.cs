using UnityEngine;

public class Place : MonoBehaviour, IInteractable
{
    public string itemName;
    public GameObject placeSpot;
    public GameObject objectOverride;
    public string placePrompt = "Press E to Place";

    [Header("Story")]
    public bool completeTask = false;
    public string taskID;

    [Header("Story Based Toggle")]
    public bool storyBasedInteraction = false;
    public enum StoryState { Before, During, After };
    public StoryState storyState = StoryState.Before;
    public bool allowPlacement = true;
    public int stageID = -1;

    // Private
    private bool canPlace = true;

    public string GetPrompt()
    {
        if (itemName == Inventory.Instance.CheckEquippedItem() && canPlace)
        {
            return placePrompt;
        }
        else return "";
    }

    public void Interact()
    {
        if (itemName == Inventory.Instance.CheckEquippedItem() && canPlace)
        {
            GameObject itemObj = Inventory.Instance.GetGameObject();

            if (objectOverride == null) itemObj.transform.SetParent(placeSpot.transform, false);
            else
            {
                Instantiate(objectOverride, placeSpot.transform);
                Destroy(itemObj);
            }

            Inventory.Instance.RemoveItem(itemName);
            canPlace = false;
            AdvanceStory();
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
                    if (StoryManager.Instance.objectiveStage < stageID) canPlace = allowPlacement;
                    else canPlace = !allowPlacement;
                    break;

                case StoryState.During:
                    if (StoryManager.Instance.objectiveStage == stageID) canPlace = allowPlacement;
                    else canPlace = !allowPlacement;
                    break;

                case StoryState.After:
                    if (StoryManager.Instance.objectiveStage > stageID) canPlace = allowPlacement;
                    else canPlace = !allowPlacement;
                    break;
            }
        }
    }
}
