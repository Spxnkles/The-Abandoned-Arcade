using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIInteract : MonoBehaviour, IInteractable
{
    public string UIName;
    public string interactPrompt = "Press E to View";

    [Header("Flags")]
    public bool addFlag = false;
    public StoryFlag flag;

    [Header("Story")]
    public bool completeTask = false;
    public string taskID;

    [Header("Required Flags")]
    public List<StoryFlag> requiredFlags = new List<StoryFlag>();

    [Header("Story Based Toggle")]
    public bool storyBasedInteraction = false;
    public enum StoryState { Before, During, After }
    public StoryState storyState = StoryState.Before;
    public bool allowInteraction = true;
    public int stageID = -1;

    [Header("Content Movement")]
    public float moveSpeed = 400f;
    public float zoomSpeed = 2f;

    [Header("Movement Limits")]
    public Vector2 moveLimitX = new Vector2(-300f, 300f);
    public Vector2 moveLimitY = new Vector2(-200f, 200f);

    [Header("Zoom Limits")]
    public float minZoom = 0.7f;
    public float maxZoom = 2.5f;

    // Internal state
    private bool canInteract = true;
    private bool isOpen = false;

    // UI references
    private Transform panel;
    private Transform content;

    // Reset values
    private Vector3 initialContentPos;
    private Vector3 initialContentScale;

    void Update()
    {
        UpdateInteractionAvailability();

        if (!isOpen) return;

        HandleCloseInput();
        HandleContentMovement();
        HandleContentZoom();
    }

    void UpdateInteractionAvailability()
    {
        canInteract = true;

        foreach (StoryFlag flag in requiredFlags)
        {
            if (!StoryManager.Instance.HasFlag(flag))
            {
                canInteract = false;
                return;
            }
        }

        if (!storyBasedInteraction) return;

        switch (storyState)
        {
            case StoryState.Before:
                canInteract = StoryManager.Instance.objectiveStage < stageID ? allowInteraction : !allowInteraction;
                break;

            case StoryState.During:
                canInteract = StoryManager.Instance.objectiveStage == stageID ? allowInteraction : !allowInteraction;
                break;

            case StoryState.After:
                canInteract = StoryManager.Instance.objectiveStage > stageID ? allowInteraction : !allowInteraction;
                break;
        }
    }

    void HandleCloseInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isOpen = false;

            PlayerController.Instance.freeze = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            content.localPosition = initialContentPos;
            content.localScale = initialContentScale;

            panel.gameObject.SetActive(false);
        }
    }

    void HandleContentMovement()
    {
        float x = 0f;
        float y = 0f;

        if (Input.GetKey(KeyCode.A)) x -= 1f;
        if (Input.GetKey(KeyCode.D)) x += 1f;
        if (Input.GetKey(KeyCode.W)) y += 1f;
        if (Input.GetKey(KeyCode.S)) y -= 1f;

        if (x == 0f && y == 0f) return;

        Vector3 newPos = content.localPosition;
        newPos += new Vector3(x, y, 0f) * moveSpeed * Time.deltaTime;

        newPos.x = Mathf.Clamp(newPos.x, moveLimitX.x, moveLimitX.y);
        newPos.y = Mathf.Clamp(newPos.y, moveLimitY.x, moveLimitY.y);

        content.localPosition = newPos;
    }

    void HandleContentZoom()
    {
        float zoomInput = 0f;

        if (Input.GetKey(KeyCode.Z)) zoomInput += 1f;
        if (Input.GetKey(KeyCode.X)) zoomInput -= 1f;

        if (zoomInput == 0f) return;

        float newScaleValue =
            content.localScale.x +
            zoomInput * zoomSpeed * Time.deltaTime;

        newScaleValue = Mathf.Clamp(newScaleValue, minZoom, maxZoom);

        content.localScale = Vector3.one * newScaleValue;
    }

    public void Interact()
    {
        if (!canInteract || isOpen) return;

        PlayerController.Instance.freeze = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Transform uiElement = GameObject.Find(UIName).transform;

        panel = uiElement.GetChild(0);
        content = panel.GetChild(0);

        initialContentPos = content.localPosition;
        initialContentScale = content.localScale;

        panel.gameObject.SetActive(true);
        isOpen = true;
    }


    public void Advance()
    {
        if (completeTask)
        {
            completeTask = false;
            StoryManager.Instance.advanceTask(taskID);
        }

        if (addFlag)
        {
            StoryManager.Instance.AddFlag(flag);
        }
    }

    public string GetPrompt()
    {
        return canInteract && !isOpen ? interactPrompt : "";
    }
}
