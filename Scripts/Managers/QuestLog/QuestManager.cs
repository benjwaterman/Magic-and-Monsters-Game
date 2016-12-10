using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Current;

    [Header("List of Quests")]
    public List<Quest> QuestList = new List<Quest>(); //overeall list of quests

    [Header("Quest Log Related Props")]
    public GameObject QuestLog;
    public GameObject Quest;
    public GameObject Task;
    public GameObject Divider;
    public GameObject EmptySpace;

    [Header("Screen Overlay Props")]
    public Text SmallText;
    public Text LargeText;

    [HideInInspector]
    public GameObject QuestLogContainer;

    private bool fadeInSmallText = false;
    private bool fadeInLargeText = false;
    private float fadeInTime = 1;
    private float fadeOutTime = 0.5f;
    private float displayTime = 1f;
    private float currentDisplayTimeSmall = 0;
    private float currentDisplayTimeLarge = 0;
    private CanvasGroup smallTextCanvasGroup;
    private CanvasGroup largeTextCanvasGroup;
    private List<FadeText> queuedTextSmall= new List<FadeText>();
    private List<FadeText> queuedTextLarge = new List<FadeText>();

    public QuestManager()
    {
        Current = this;
    }

    void Start()
    {
        QuestLogContainer = QuestLog.transform.GetChild(0).gameObject;

        smallTextCanvasGroup = SmallText.GetComponent<CanvasGroup>();
        largeTextCanvasGroup = LargeText.GetComponent<CanvasGroup>();
        smallTextCanvasGroup.alpha = 0;
        largeTextCanvasGroup.alpha = 0;

        Quest StartQuest = CreateQuest("The Beggining");
        StartQuest.AddTask(0, "Talk to the Arch Mage", 0);
        QuestList.Add(StartQuest);

        Quest NeighbourQuest = CreateQuest("Friendly Neighbour");
        NeighbourQuest.AddTask(0, "Talk to the locals", 5);
        QuestList.Add(NeighbourQuest);
    }

    void Update()
    {
        HandleQueue(queuedTextSmall, SmallText);
        HandleQueue(queuedTextLarge, LargeText);
    }

    public Quest CreateQuest(string title)
    {
        GameObject newGo = Instantiate(Quest);
        newGo.transform.SetParent(QuestLogContainer.gameObject.transform);
        newGo.name = "Quest: " + title;
        Quest newQuest = newGo.GetComponent<Quest>();
        newQuest.InitialiseQuest(title);

        DisplayAddQuest(newQuest);

        return newQuest;
    }

    public void CompleteQuest(Quest quest)
    {
        DisplayCompleteQuest(quest);

        Destroy(quest.gameObject);
        //QuestList.Remove(quest);
    }

    void DisplayCompleteQuest(Quest quest)
    {
        queuedTextLarge.Add(new FadeText("Quest Completed: " + quest.Title));
        GameManager.Current.PlayCompletionSound();
    }

    public void DisplayCompleteTask(Task task)
    {
        if(task.TargetAmount == 0)
            queuedTextSmall.Add(new FadeText("Task Completed: " + task.Title));
        else 
            queuedTextSmall.Add(new FadeText("Task Completed: " + task.Title + " (" + task.CurrentAmount + "/" + task.TargetAmount + ")"));
    }

    public void DisplayAddQuest(Quest quest)
    {
        queuedTextLarge.Add(new FadeText("New Quest: " + quest.Title));
    }

    public void DisplayAddTask(Task task)
    {
        queuedTextSmall.Add(new FadeText("New Task: " + task.Title));
        GameManager.Current.PlayAchievementSound();
    }

    public void DisplayUpdateTask(Task task)
    {
        queuedTextSmall.Add(new FadeText("Task Updated: " + task.Title + " (" + task.CurrentAmount + "/" + task.TargetAmount + ")"));
        GameManager.Current.PlayAchievementSound();
    }

    public void HandleQueue(List<FadeText> list, Text uiText)
    {
        if (list.Count > 0) //if there is text in the queue to be displayed
        {
            uiText.text = list[0].Text;

            if (!list[0].HasFaded)
            {
                uiText.GetComponent<CanvasGroup>().alpha += fadeInTime * Time.deltaTime;
                if (uiText.GetComponent<CanvasGroup>().alpha == 1)
                {
                    if (list[0].CurrentDisplayTime < displayTime)
                        list[0].CurrentDisplayTime += Time.deltaTime;
                    else
                    {
                        list[0].CurrentDisplayTime = 0;
                        list[0].HasFaded = true;
                    }
                }
            }
            else
            {
                uiText.GetComponent<CanvasGroup>().alpha -= fadeOutTime * Time.deltaTime;
                if (uiText.GetComponent<CanvasGroup>().alpha == 0) //once it is invisible, remove it from queue
                {
                    list.RemoveAt(0);
                }
            }
        }
    }
}

public class FadeText
{
    public string Text;
    public bool HasFaded;
    public float CurrentDisplayTime;

    public FadeText(string text)
    {
        Text = text;
        HasFaded = false;
        CurrentDisplayTime = 0;
    }
}