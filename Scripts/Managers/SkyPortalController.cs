using UnityEngine;
using System.Collections;

public class SkyPortalController : MonoBehaviour
{
    public static SkyPortalController Current;

    public GameObject RedPortal;
    public GameObject BluePortal;
    public GameObject GreenPortal;

    public SkyPortalController()
    {
        Current = this;
    }

    void Awake()
    {
        DisableAllPortals();
    }

    public void DisableAllPortals()
    {
        RedPortal.SetActive(false);
        BluePortal.SetActive(false);
        GreenPortal.SetActive(false);
    }

    public void EnableAllPortals()
    {
        RedPortal.SetActive(true);
        BluePortal.SetActive(true);
        GreenPortal.SetActive(true);
    }
}
