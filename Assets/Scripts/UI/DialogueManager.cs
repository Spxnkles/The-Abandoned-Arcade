using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    public float charDelay = 0.05f;
    public float sentenceDelay = 1f;

    public static DialogueManager Instance;
    public GameObject icon;
    public TextMeshProUGUI title;
    public TextMeshProUGUI speech;
    public AudioSource audioSource;
    public AudioClip[] typingAudio;

    private Coroutine typingCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public IEnumerator PlayDialogue(Speech[] dialogues)
    {
        showPanel();

        foreach (Speech s in dialogues)
        {
            title.text = s.title;
            title.color = s.color;

            if (s.icon != null)
            {
                icon.transform.GetComponentInChildren<RawImage>().texture = s.icon;
                icon.transform.GetComponentInChildren<RawImage>().enabled = true;
            } else icon.transform.GetComponentInChildren<RawImage>().enabled = false;

            foreach (string t in s.speeches)
            {
                speech.text = t;
                speech.maxVisibleCharacters = 0;

                for (int i = 0; i <= speech.text.Length; i++)
                {
                    speech.maxVisibleCharacters = i;
                    playRandomAudio();

                    yield return new WaitForSeconds(charDelay);
                }

                yield return new WaitForSeconds(sentenceDelay);
            }
        }

        hidePanel();
    }

    void showPanel()
    {
        title.transform.parent.gameObject.SetActive(true);
    }

    void hidePanel()
    {
        title.transform.parent.gameObject.SetActive(false);
    }

    void playRandomAudio()
    {
        audioSource.PlayOneShot(typingAudio[Random.Range(0, typingAudio.Length)]);
    }
}

public class Speech
{
    public string title;
    public Color color;
    public Texture2D icon;
    public string[] speeches;
}
