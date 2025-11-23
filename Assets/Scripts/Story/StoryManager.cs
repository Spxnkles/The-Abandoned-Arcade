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
    public int objectiveStage = -1;
    public List<Objective> objectives;
    public Objective activeObj => objectives[objectiveStage];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        CreateObjectives();

        StartCoroutine(Storyline());
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

    #region OBJECTIVES

    private void CreateObjectives()
    {
        objectives.Add(new Objective
        {
            stageID = 0,
            title = "Have some food",
            tasks = new Task[]
            {
                new Task {id = "fridge", text = "Open the fridge"},
                new Task {id = "food", text = "Pick up the leftovers"},
                new Task {id = "heat", text = "Heat up the leftovers"}
            }
        });
    }

    #endregion


    #region STORYLINE
    // THE STORYLINE CONTROLS THE FLOW OF THE STORY AND ORDER OF SEQUENCES 
    private IEnumerator Storyline()
    {
        // 5 Second delay for player to prepare
        yield return new WaitForSeconds(1f);

        // Intro monologue, food objective
        yield return IntroSequence();

        // Wait 30 seconds while the food is heating up
        yield return new WaitForSeconds(30f);

        // Phone starts ringing
        Debug.Log("RING RING RING NIGGA");
    }

    #endregion

    #region STORY SEQUENCES & DIALOGUES/MONOLOGUES

    // START OF GAME - PLAYER ARRIVED HOME
    private IEnumerator IntroSequence()
    {
        // Play the monologue, player has arrived home, hungry
        yield return PlayIntroMonologue();

        // Advance objective, starts the obhjective
        advanceObjective();

        // Await till the objective has been completed
        yield return new WaitUntil(() => checkTaskCompletion());

        // Start microwave sound
        GameObject.Find("Microwave").GetComponent<ObjectAudio>().PlaySound();
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


    //

    #endregion
}

#region CLASSES

[System.Serializable]
public class Task
{
    public string id;
    public string text;
    public bool complete = false;
}

[System.Serializable]
public class Objective
{
    public int stageID;
    public string title;
    public Task[] tasks;
}

#endregion