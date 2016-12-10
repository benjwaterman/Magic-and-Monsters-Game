using UnityEngine;
using System.Collections;

public abstract class Interactive : MonoBehaviour
{
    public enum InteractionType
    {
        Talk
    };

    public InteractionType Interaction = InteractionType.Talk;
}
