using System;
using TMPro;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    [Header("UI")]
    public TextMeshProUGUI title;
    public TextMeshProUGUI[] tasks;

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

    public void setObjective(string objective_title, params Task[] objective_tasks)
    {
        hideObjective();

        title.enabled = true;
        title.text = objective_title;

        int i = 0;
        foreach (Task ot in objective_tasks)
        {
            tasks[i].enabled = true;
            tasks[i].text = ot.text;

            if (i >= tasks.Length - 1) break;
            i++;
        }

    }

    void hideObjective()
    {
        // Hide title
        title.enabled = false;

        // Hide each task 
        foreach (TextMeshProUGUI task in tasks)
        {
            resetTask(task);
            task.enabled = false;
        }
    }

    // Function to complete task + overload with TMP variable + overload to complete task with taskText
    void completeTask(int index)
    {
        tasks[index].fontStyle = FontStyles.Strikethrough;
        tasks[index].alpha = 0.2f;
    }
    void completeTask(TextMeshProUGUI task)
    {
        task.fontStyle = FontStyles.Strikethrough;
        task.alpha = 0.2f;
    }
    public void completeTask(string taskText)
    {
        TextMeshProUGUI task = null;

        foreach (TextMeshProUGUI t in tasks)
        {
            if (t.text == taskText)
            {
                task = t;
                break;
            }
        }
        if (task == null)
        {
            Debug.LogError("UI ERROR: Couldn't find Task in the UI!");
        }

        task.fontStyle = FontStyles.Strikethrough;
        task.alpha = 0.2f;
    }

    // Function to reset the tasks back to uncomplete + overload with TMP variable
    void resetTask(int index)
    {
        tasks[index].fontStyle &= ~FontStyles.Strikethrough;
        tasks[index].alpha = 1f;
    }
    void resetTask(TextMeshProUGUI task)
    {
        task.fontStyle &= ~FontStyles.Strikethrough;
        task.alpha = 1f;
    }


}
