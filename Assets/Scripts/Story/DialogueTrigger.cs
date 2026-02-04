using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AppUI.UI;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public string[] lines;

    [Header("Flags")]
    public bool addFlag = false;
    public StoryFlag flag;

    [Header("Story")]
    public bool completeTask = false;
    public string taskID;

    [Header("Required Flags")]
    public List<StoryFlag> requiredFlags = new List<StoryFlag> { };

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

        foreach (StoryFlag flag in requiredFlags)
        {
            if (!StoryManager.Instance.HasFlag(flag))
            {
                canTrigger = false;
                return;
            }
        }

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
        Advance();
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
}
