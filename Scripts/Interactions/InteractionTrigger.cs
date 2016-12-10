using UnityEngine;
using System.Collections;

public abstract class InteractionTrigger : MonoBehaviour
{
    public int ConversationIndex = 0;
    public abstract void Play(int index);
}
