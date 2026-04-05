using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI TextMeshProUGUI;

    public Material unhoverMaterial;
    public Material hoverMaterial;
    public AudioSource hoverSound;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer enter!");
        if (TextMeshProUGUI != null && hoverMaterial != null && hoverSound != null)
        {
            TextMeshProUGUI.fontMaterial = hoverMaterial;
            if (hoverSound.time == 0 || hoverSound.time > 0.15) hoverSound.Play();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TextMeshProUGUI != null && unhoverMaterial != null)
        {
            TextMeshProUGUI.fontMaterial = unhoverMaterial;
        }
    }
}
