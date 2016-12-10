using UnityEngine;
using System.Collections;

public class HouseLightController : MonoBehaviour
{
    public Color LightColor;
    public float BaseIntensity = 1;
    public float FlickerAmount;
    public float FlickerFrequency;

    private Light[] lightArray;

    void Start()
    {
        lightArray = GetComponentsInChildren<Light>();
    }

    void Update()
    {
        foreach (Light l in lightArray)
        {
            l.intensity = Mathf.Lerp(l.intensity, BaseIntensity + Random.Range(-FlickerAmount, FlickerAmount), FlickerFrequency * Time.deltaTime);
            l.color = LightColor;
        }
    }
}
