using UnityEngine;
using System.Collections;

public class HealthPickup : MonoBehaviour
{
    public int HealthRestoreAmount;
    
    void OnTriggerEnter(Collider coll)
    {
        if (coll.transform.CompareTag("Player"))
        {
            coll.GetComponent<PlayerHealth>().GainHealth(HealthRestoreAmount);
            Destroy(this.gameObject);
        }
    }
}
