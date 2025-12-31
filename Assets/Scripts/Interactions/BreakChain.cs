using System.Collections;
using UnityEngine;

public class BreakChain : MonoBehaviour, IInteractable
{
    public Animator animator;
    public GameObject position;
    public string interactPrompt = "Press E to Break chain";

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
        if ("Axe" == Inventory.Instance.CheckEquippedItem() && canInteract)
        {
            Destroy(Inventory.Instance.GetGameObject());
            Inventory.Instance.RemoveItem("Axe");

            PlayerController.Instance.freeze = true;
            PlayerController.Instance.transform.position = position.transform.position;
            PlayerController.Instance.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            PlayerController.Instance.playerCamera.transform.localRotation = Quaternion.Euler(22f, 0f, 0f);

            canInteract = false;

            StartCoroutine(Animation());
        }
    }

    IEnumerator Animation()
    {
        animator.SetBool("break", true);

        yield return new WaitForSeconds(4f);

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
        if ("Axe" == Inventory.Instance.CheckEquippedItem() && canInteract) return interactPrompt;
        return "";
    }
}
