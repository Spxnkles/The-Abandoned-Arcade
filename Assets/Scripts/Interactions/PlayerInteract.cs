using TMPro;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactionDistance = 2f;
    public KeyCode interactKey = KeyCode.E;

    // Specify which layer is interactable, don't forget to assign the correct layer to the object
    public LayerMask interactLayer;

    public TextMeshProUGUI promptUI;



    void Start()
    {
        
    }

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                // Functions to execute when an interactable object was found
                if (Input.GetKeyDown(interactKey))
                {
                    interact(interactable);
                }
                else
                {
                    showPrompt(interactable);
                }
            }
            else
            {
                // If the hit doesn't contain an interactable, hide the prompt (this shouldn't happen, unless a layer is misconfigured)
                hidePrompt();
            }
        }
        else
        {
            // If the raycast didn't hit anything, hide the prompt
            hidePrompt();
        }
    }

    public void showPrompt(IInteractable interactable)
    {
        string prompt = interactable.GetPrompt();

        promptUI.text = prompt;
        promptUI.gameObject.SetActive(true);
    }

    public void hidePrompt()
    {
        promptUI.gameObject.SetActive(false);
    }

    public void interact(IInteractable interactable)
    {
        interactable.Interact();
    }
}
