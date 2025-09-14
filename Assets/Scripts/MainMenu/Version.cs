using TMPro;
using UnityEngine;

public class Version : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;

    void Start()
    {
        if (textMeshProUGUI != null)
        {
            textMeshProUGUI.text = $"version {Application.version}";
        }
    }
}
