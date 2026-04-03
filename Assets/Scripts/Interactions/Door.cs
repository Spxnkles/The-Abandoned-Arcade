using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Header("Door")]
    public float openAngle = 90f;
    public float openingSpeed = 2.5f;
    public float interactCooldown = 2f;
    public bool openUpwards = false;

    [Header("Locking")]
    public bool isLocked = false;
    public int key = 0;

    [Header("Prompts")]
    public string openPrompt = "Press E to Open Door";
    public string closePrompt = "Press E to Close Door";
    public string lockedPrompt = "Door is Locked";

    [Header("Story")]
    public bool completeTask = false;
    public string taskID;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip openAudio;
    public AudioClip closeAudio;
    public AudioClip lockedAudio;

    [Header("Story Based Toggle")]
    public bool storyBasedInteraction = false;
    public enum StoryState { Before, During, After };
    public StoryState storyState = StoryState.Before;
    public bool allowInteraction = true;
    public int stageID = -1;

    [Header("Control")]
    public bool isOpen = false;
    private bool canInteract = true;
    private Quaternion openRotation;
    private Quaternion closedRotation;
    private float cooldown = 0f;



    void Start()
    {
        closedRotation = transform.rotation;
        
        if (!openUpwards) openRotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, openAngle, 0));
        else openRotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(openAngle, 0, 0));
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpen)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, openRotation, Time.deltaTime * openingSpeed);
        } else
        {

            transform.rotation = Quaternion.Slerp(transform.rotation, closedRotation, Time.deltaTime * openingSpeed);
        }

        if (!canInteract)
        {
            cooldown -= Time.deltaTime;
            if (cooldown <= 0f) canInteract = true;
        }

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

        if (isLocked) {
            if (Inventory.Instance.keys.Contains(key)) isLocked = false;
        }
    }

    public void Interact()
    {
        if (!canInteract || isLocked) return;

        if (isOpen)
        {
            if (closeAudio != null) audioSource.PlayOneShot(closeAudio);
        } else
        {
            if (openAudio != null) audioSource.PlayOneShot(openAudio);
        }
        
        isOpen = !isOpen;

        cooldown = interactCooldown;
        canInteract = false;
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

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Killer")) return;

        if (!isOpen)
        {
            isOpen = true;
            if (openAudio != null) audioSource.PlayOneShot(openAudio);
        }

        StopAllCoroutines();
        StartCoroutine(CloseAfterDelay());
    }

    private IEnumerator CloseAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        isOpen = false;
        if (closeAudio != null) audioSource.PlayOneShot(closeAudio);
    }

    public string GetPrompt()
    {
        if (!canInteract) return "";

        if (isLocked) return lockedPrompt;
        else if (isOpen) return closePrompt;
        else return openPrompt;
    }
}
