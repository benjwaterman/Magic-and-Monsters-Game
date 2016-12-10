using UnityEngine;
using System.Collections;

public class CopyParticleColor : MonoBehaviour
{
    private ParticleSystem particleSys;
    private Light thisLight;
    
    void Start()
    {
        particleSys = GetComponent<ParticleSystem>();
        thisLight = GetComponent<Light>();
    }

    void Update()
    {
        if(thisLight.color != particleSys.startColor)
            thisLight.color = particleSys.startColor;
    }
}
