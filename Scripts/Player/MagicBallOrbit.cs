using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicBallOrbit : MonoBehaviour
{
    public static MagicBallOrbit Current;

    [Header("Prefabs and GameObjects")]
    public GameObject OrbPrefab;
    public GameObject PlayerPivot;

    [Header("Parameters")]
    public int NumberOfBalls;
    public float OrbitRadius;
    public float OrbitSpeed = 200;

    [Header("Lists")]
    public List<GameObject> BallList = new List<GameObject>();
    public List<GameObject> FiredBallList = new List<GameObject>();

    private GameObject yPivot;
    private GameObject xPivot;

    private Animator playerAnimator;
    private Vector3 pivotOriginalPos;
    private Vector3 pivotOriginalPosLocal;
    private float orbitSpeed;
    private float orbitSpeedMultiplier;

    private Quaternion startRotation;
    private Quaternion targetRotation;
    private Vector3 targetPosition;
    private float targetScale;
    private bool canSpin = true;

    public MagicBallOrbit()
    {
        Current = this;
    }

    void Start()
    {
        xPivot = transform.GetChild(0).gameObject;
        yPivot = xPivot.transform.GetChild(0).gameObject;

        transform.position = PlayerController.Current.transform.position;
        transform.position += new Vector3(0, 1);

        pivotOriginalPos = PlayerPivot.transform.position;
        pivotOriginalPosLocal = PlayerPivot.transform.localPosition;

        playerAnimator = PlayerController.Current.animator;

        orbitSpeed = OrbitSpeed;

        startRotation = transform.rotation;
        targetRotation = Quaternion.Euler(0, 0, 0);
        targetPosition = Vector3.zero;
        targetScale = 1;
        orbitSpeedMultiplier = 1;

        CreateBalls();
    }

    void Update()
    {

        if (PlayerController.Current.InCombat)
        {
            PlayerPivot.transform.localPosition = Vector3.MoveTowards(PlayerPivot.transform.localPosition,
                new Vector3(pivotOriginalPosLocal.x + 0.15f, pivotOriginalPosLocal.y + 0.7f, pivotOriginalPosLocal.z) + targetPosition,
                Time.deltaTime);

            orbitSpeed = Mathf.SmoothStep(OrbitSpeed, OrbitSpeed * 2 * orbitSpeedMultiplier, 0.8f);
        }
        else
        {
            PlayerPivot.transform.localPosition = Vector3.MoveTowards(PlayerPivot.transform.localPosition,
                new Vector3(pivotOriginalPosLocal.x, pivotOriginalPosLocal.y, pivotOriginalPosLocal.z),
                Time.deltaTime);

            orbitSpeed = OrbitSpeed;
        }

        transform.forward = PlayerController.Current.transform.forward; //z pivot, facing left and right
        if (canSpin)
            yPivot.transform.Rotate(Vector3.up, orbitSpeed * Time.deltaTime); //y pivot, 
        xPivot.transform.localRotation = Quaternion.RotateTowards(xPivot.transform.localRotation, targetRotation, Time.deltaTime * 100); //x pivot, facing up and 
        yPivot.transform.localScale = Vector3.Lerp(yPivot.transform.localScale, new Vector3(targetScale, targetScale, targetScale), Time.deltaTime * 2); //scale on y pivot
    }

    void LateUpdate()
    {
        transform.position = PlayerPivot.transform.position;
    }

    void CreateBalls()
    {
        for (int ballNum = 0; ballNum < NumberOfBalls; ballNum++)
        {
            float i = (ballNum * 1.0f) / NumberOfBalls;
            float angle = i * Mathf.PI * 2.0f;
            var x = Mathf.Sin(angle) * OrbitRadius;
            var z = Mathf.Cos(angle) * OrbitRadius;
            var pos = new Vector3(x, 0, z) + PlayerPivot.transform.position;

            GameObject go = InstantiateBall(pos, Quaternion.identity);
            BallList.Add(go);
        }
    }

    GameObject InstantiateBall(Vector3 ballPosition, Quaternion ballRotation)
    {
        GameObject temp = (GameObject)Instantiate(OrbPrefab, ballPosition, ballRotation);
        temp.transform.SetParent(yPivot.transform);
        return temp;
    }

    public void PlayParticles()
    {
        foreach (GameObject orb in BallList)
        {
            orb.GetComponent<ParticleSystem>().Play();
        }
    }

    public void StopParticles()
    {
        foreach (GameObject orb in BallList)
        {
            orb.GetComponent<ParticleSystem>().Stop();
        }
    }

    public void RotateTo(Vector3 degrees)
    {
        targetRotation = Quaternion.Euler(degrees);
    }

    public void MoveBy(Vector3 location)
    {
        targetPosition = location;
    }

    public void ScaleTo(float scale)
    {
        targetScale = scale;
    }

    public void MultiplyOrbitSpeedBy(float multiplier)
    {
        orbitSpeedMultiplier = multiplier;
    }

    public void DisableAllOrbs()
    {
        foreach(GameObject go in BallList)
        {
            go.SetActive(false);

            FiredBallList.Add(go);
        }

        BallList.Clear();
    }
}
