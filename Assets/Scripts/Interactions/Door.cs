using Unity.VisualScripting;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Header("Door")]
    public float openAngle = 90f;
    public float openingSpeed = 2.5f;
    public float interactCooldown = 2f;

    [Header("Locking")]
    public bool isLocked = false;
    public int key = 0;

    [Header("Prompts")]
    public string openPrompt = "Press E to Open Door";
    public string closePrompt = "Press E to Close Door";
    public string lockedPrompt = "Door is Locked";

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip openAudio;
    public AudioClip closeAudio;
    public AudioClip lockedAudio;


    private bool isOpen = false;
    private bool canInteract = true;
    private Quaternion openRotation;
    private Quaternion closedRotation;
    private float cooldown = 0f;



    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, openAngle, 0));
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
    }

    public string GetPrompt()
    {
        if (!canInteract) return "";

        if (isLocked) return lockedPrompt;
        else if (isOpen) return closePrompt;
        else return openPrompt;
    }
}
