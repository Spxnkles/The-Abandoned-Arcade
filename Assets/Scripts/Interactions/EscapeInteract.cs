using System.Collections.Generic;
using UnityEngine;

public class EscapeInteract : MonoBehaviour, IInteractable
{
    public string interactPrompt = "Press E to Escape";

    [Header("Required Flags")]
    public List<StoryFlag> requiredFlags = new List<StoryFlag> { };

    // Private
    private bool canInteract = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        foreach (StoryFlag flag in requiredFlags)
        {
            if (!StoryManager.Instance.HasFlag(flag))
            {
                canInteract = false;
                return;
            }
            else
            {
                canInteract = true;
            }
        }
    }

    public void Interact()
    {
        if (!canInteract) return;

        UIManager.Instance.ShowWinScreen();
    }

    public string GetPrompt()
    {
        if (!canInteract) return "";
        return interactPrompt;
    }


}
