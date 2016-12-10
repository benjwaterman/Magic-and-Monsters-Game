using UnityEngine;
using System.Collections;

public class DestroyAfterSeconds : MonoBehaviour
{
    public float TimeToDestory = 0;

    void Start()
    {
        Destroy(this.gameObject, TimeToDestory);
    }
}
