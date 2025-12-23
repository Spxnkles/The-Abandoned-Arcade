using UnityEngine;

public class ObjectInteract : MonoBehaviour, IInteractable
{
    public string pickupPrompt = "Press E to Interact";

    [Header("Story")]
    public bool completeTask = false;
    public string taskID;

    [Header("Story Based Toggle")]
    public bool storyBasedInteraction = false;
    public enum StoryState { Before, During, After };
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

        AdvanceStory();
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
