using UnityEngine;
using System.Collections;

public class RespondButton : MonoBehaviour
{
    public void Continue()
    {
        PlayerController.Current.TalkTarget.Continue();
    }
}
