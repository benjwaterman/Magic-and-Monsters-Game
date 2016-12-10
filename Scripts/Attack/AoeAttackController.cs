using UnityEngine;
using System.Collections;

public class AoeAttackController : MonoBehaviour {

    private ParticleSystem particleSys;
    private Collider attackTrigger;
    private Renderer bubbleRenderer;

    void Start()
    {
        particleSys = GetComponent<ParticleSystem>();

        attackTrigger = GetComponentInChildren<Collider>();
        attackTrigger.enabled = false;

        bubbleRenderer = attackTrigger.gameObject.GetComponent<Renderer>();//.material.color = particleSys.startColor;
        bubbleRenderer.enabled = false;
    }

    void Update()
    {
        if (particleSys.isPlaying)
        {
            attackTrigger.enabled = true;
            bubbleRenderer.material.color -= new Color(0,0,0,2 * Time.deltaTime);
            bubbleRenderer.enabled = true;
            //MagicBallOrbit.Current.ScaleTo(5);
        }
        else
        {
            attackTrigger.enabled = false;
            bubbleRenderer.enabled = false;
            bubbleRenderer.material.color = particleSys.startColor;
            //MagicBallOrbit.Current.ScaleTo(1);
        }
    }

    public void SetColor(Color color)
    {
        particleSys.startColor = color;
        bubbleRenderer.material.color = particleSys.startColor;
    }
}
