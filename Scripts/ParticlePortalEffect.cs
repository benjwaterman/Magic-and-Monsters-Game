using UnityEngine;
using System.Collections;

public class ParticlePortalEffect : MonoBehaviour
{
    public float RotationSpeed = 5f;
    public GameObject SpiralArm;
    public int NumberOfArms;
    public float Radius;
    public Color SpiralColor;

    void Start()
    {
        CreateSpiral();
    }

    void Update()
    {
        transform.Rotate(new Vector3(0,1,0), RotationSpeed * Time.deltaTime);
    }

    void CreateSpiral()
    {
        for (int armNum = 0; armNum < NumberOfArms; armNum++)
        {
            float i = (armNum * 1.0f) / NumberOfArms;
            float angle = i * Mathf.PI * 2.0f;
            var x = Mathf.Sin(angle) * Radius;
            var z = Mathf.Cos(angle) * Radius;
            var pos = new Vector3(x, 0, z) + transform.position;

            GameObject go = InstantiateArm(pos, Quaternion.identity);
            go.GetComponent<ParticleSystem>().startColor = SpiralColor;
            go.transform.localEulerAngles = new Vector3 (0, 360 / NumberOfArms * armNum);
        }
    }

    GameObject InstantiateArm(Vector3 armPosition, Quaternion armRotation)
    {
        GameObject temp = (GameObject)Instantiate(SpiralArm, armPosition, armRotation);
        temp.transform.SetParent(this.transform);
        return temp;
    }
}
