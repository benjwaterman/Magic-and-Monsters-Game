using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]

public class Task : MonoBehaviour
{
    public int TaskID { get; private set; }
    public string Title { get; private set; }
    public int TargetAmount { get; private set; }
    public int CurrentAmount { get; private set; }
    public bool Completed { get; private set; }

    private Text titleText;
    private Text progressText;
    private CanvasGroup canvasGroup;

    void Start()
    {
        CurrentAmount = 0;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void InitialiseTask(int id, string title, int targetAmount)
    {
        TaskID = id;
        Title = title;
        TargetAmount = targetAmount;

        titleText = transform.GetChild(0).GetComponent<Text>();
        progressText = transform.GetChild(1).GetComponent<Text>();

        titleText.text = title;

        if (targetAmount != 0) //if target is 0 then there doesnt need to be a progress tracker
           progressText.text = CurrentAmount.ToString() + "/" + TargetAmount.ToString();
        else
            progressText.text = "";
    }

    public void AddProgress()
    {
        if(CurrentAmount < TargetAmount)
            CurrentAmount++;

        if (TargetAmount != 0)
            progressText.text = CurrentAmount.ToString() + "/" + TargetAmount.ToString();

        if (CurrentAmount == TargetAmount) //task is complete
        {
            QuestManager.Current.DisplayCompleteTask(this);
            Completed = true;
            canvasGroup.alpha = 0.5f;
        }

        else if (TargetAmount != 0) //if not complete, just show update
            QuestManager.Current.DisplayUpdateTask(this);
    }

    public int GetProgress()
    {
        return CurrentAmount;
    }
}
