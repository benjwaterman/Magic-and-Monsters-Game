using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Quest : MonoBehaviour
{
    public List<Task> TaskList = new List<Task>(); //list of tasks within this quest

    public string Title { get; private set; }
    public bool Completed { get; private set; }

    private GameObject GO_Task;

    void Start()
    {
        GO_Task = QuestManager.Current.Task;
    }

    public void InitialiseQuest(string title)
    {
        Title = title;

        ConstructUIElements();
    }

    public void AddTask(int id, string title, int targetAmount)
    {
        GameObject newGO = Instantiate(QuestManager.Current.Task);
        newGO.transform.SetParent(this.gameObject.transform);
        newGO.name = "Task: " + title;

        Task newTask = newGO.GetComponent<Task>();
        newTask.InitialiseTask(id, title, targetAmount);
        TaskList.Add(newTask);

        QuestManager.Current.DisplayAddTask(newTask);
    }

    public void AddProgressToTask(int id)
    {
        TaskList[id].AddProgress();

        bool allComplete = true;
        foreach (Task task in TaskList)
        {
            if(task.Completed != true)
            {
                allComplete = false;
            }
        }

        if (allComplete)
        {
            QuestManager.Current.CompleteQuest(this);
        }
    }

    public int GetProgressOnTask(int id)
    {
        return TaskList[id].GetProgress();

    }

    void ConstructUIElements()
    {
        AddSpace();
        AddHeader(Title);
        AddDivider();
    }

    void AddHeader(string title)
    {
        GameObject newHeader = Instantiate(QuestManager.Current.Task);
        newHeader.name = "Title: " + title;
        newHeader.transform.SetParent(this.gameObject.transform);
        newHeader.transform.GetChild(0).GetComponent<Text>().text = "<b>" + title + "</b>";
        newHeader.transform.GetChild(1).GetComponent<Text>().text = "";
    }

    void AddDivider()
    {
        GameObject divider = Instantiate(QuestManager.Current.Divider);
        divider.transform.SetParent(this.gameObject.transform);
        divider.name = "(Divider)";
    }

    void AddSpace()
    {
        GameObject space = Instantiate(QuestManager.Current.EmptySpace);
        space.transform.SetParent(this.gameObject.transform);
        space.name = "(Space)";
    }
}
