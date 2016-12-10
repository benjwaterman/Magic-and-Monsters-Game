using UnityEngine;
using System.Collections;

public class DisableLightAfterSeconds : MonoBehaviour
{
    private Light thisLight;
    private bool isFading = false;

    void Start()
    {
        thisLight = GetComponent<Light>();
    }

    void Update()
    {
        if (isFading == false)
            Invoke("DisableLight", 0.1f);
        else
        {
            thisLight.intensity = Mathf.Lerp(thisLight.intensity, 0, 0.1f);
        }
    }

    void DisableLight()
    {
        isFading = true;
    }
}
