using System.Collections;
using Unity.AppUI.UI;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public string[] lines;

    [Header("Story")]
    public bool completeTask = false;
    public string taskID;

    [Header("Story Based Toggle")]
    public bool storyBasedInteraction = false;
    public enum StoryState { Before, During, After };
    public StoryState storyState = StoryState.Before;
    public bool allowTrigger = true;
    public int stageID = -1;

    private bool canTrigger = true;

    private bool wasTrigerred = false;

    // Update is called once per frame
    void Update()
    {
        if (wasTrigerred) return;

        if (storyBasedInteraction)
        {
            switch (storyState)
            {
                case StoryState.Before:
                    if (StoryManager.Instance.objectiveStage < stageID) canTrigger = allowTrigger;
                    else canTrigger = !allowTrigger;
                    break;

                case StoryState.During:
                    if (StoryManager.Instance.objectiveStage == stageID) canTrigger = allowTrigger;
                    else canTrigger = !allowTrigger;
                    break;

                case StoryState.After:
                    if (StoryManager.Instance.objectiveStage > stageID) canTrigger = allowTrigger;
                    else canTrigger = !allowTrigger;
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canTrigger || wasTrigerred) return;

        if (!other.CompareTag("Player")) return;

        wasTrigerred = true;
        StartCoroutine(PlaySpeech());
    }

    IEnumerator PlaySpeech()
    {
        Speech speech = new Speech()
        {
            title = StoryManager.Instance.mainCharacter.name,
            color = StoryManager.Instance.mainCharacter.speechColor,
            icon = StoryManager.Instance.mainCharacter.icon,
            speeches = lines
        };

        Speech[] mono = new Speech[] { speech };

        yield return DialogueManager.Instance.PlayDialogue(mono);
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
}
