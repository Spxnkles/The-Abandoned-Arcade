using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class StoryManager : MonoBehaviour
{
    #region UTILITIES AND FUNCTIONS

    public static StoryManager Instance;

    [Header("Story")]
    public int objectiveStage = 0;
    public List<Objective> objectives;
    public Objective activeObj => objectives[objectiveStage];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        StartCoroutine(IntroSequence());
    }

    public void advanceTask(string taskID)
    {
        Task task = activeObj.tasks.FirstOrDefault(t => t.id == taskID);
        if (task == null)
        {
            Debug.LogError("ERROR: Couldn't find Task to complete in advanceTask();");
        }
        task.complete = true;
        ObjectiveManager.Instance.completeTask(task.text);

        if (checkTaskCompletion()) advanceObjective();
    }

    public void advanceObjective()
    {
        objectiveStage++;

        Objective obj = activeObj;
        Task[] tasks = activeObj.tasks.ToArray();

        ObjectiveManager.Instance.setObjective(obj.title, tasks);
    }

    public bool checkTaskCompletion()
    {
        bool tasksCompleted = false;
        foreach (Task t in activeObj.tasks)
        {
            tasksCompleted = t.complete;
            if (!tasksCompleted) break;
        }

        return tasksCompleted;
    }

    #endregion

    #region STORY SEQUENCES

    // START OF GAME - PLAYER ARRIVED HOME
    private IEnumerator IntroSequence()
    {
        // Wait 5 seconds
        yield return new WaitForSeconds(5f);
        // After 5 seconds play the monologue, player has arrived home.
        yield return PlayIntroMonologue();
    }

    // Player intro monologue
    IEnumerator PlayIntroMonologue()
    {
        Speech introMono = new Speech()
        {
            title = "You",
            color = Color.lightBlue,
            speeches = new string[] { "Finaly home...", "Today was such a loooong day.", "Man, I hate work, it's so boring.", "Well, at least I make a decent living.", "I am sooo hungry, I think there are some leftovers in the fridge.", "I should heat them up." }
        };

        Speech[] mono = new Speech[] { introMono };

        yield return DialogueManager.Instance.PlayDialogue(mono);
    }

    #endregion
}

#region CLASSES

[System.Serializable]
public class Task
{
    public string id;
    public string text;
    public bool complete;
}

[System.Serializable]
public class Objective
{
    public int stageID;
    public string title;
    public List<Task> tasks;
}

#endregion