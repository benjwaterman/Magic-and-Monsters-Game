using UnityEngine;
using System.Collections;

public class AssignTerrainDetail : MonoBehaviour
{
    public Terrain terrain;

    private TerrainData terrainData;


    void Start()
    {
        if (!terrain)
            Debug.LogError(gameObject.name + " has NO TERRAIN assigned in the Inspector");

        terrainData = terrain.terrainData;

        GenerateTerrainDetail();
    }


    void GenerateTerrainDetail()
    {
        int detailWidth = terrainData.detailWidth;
        int detailHeight = terrainData.detailHeight;

        int[,] details0 = new int[detailWidth, detailHeight];
        int[,] details1 = new int[detailWidth, detailHeight];

        int x, y, strength;

        for (x = 0; x < detailWidth; x++) // divided by 4 just to show a test patch
        {
            for (y = 0; y < detailHeight; y++) // test patch
            {
                if (x % 1 == 0 && y % 1 == 0)
                {
                    strength = 0;// (x / 2) % 17;
                }
                else
                    strength = 0;
                //strength = (x % 2 == 0 ? (x / 2) % 17 : 0); // just to spread the grass out a bit to see the difference

                //if (y % 4 == 0) // set detail layer 0 for every first row in 4
                    details0[y, x] = strength;
               // else if (y % 4 == 2) // set detail layer 1 for every third row in 4
                    details1[y, x] = strength;
            }
        }

        terrainData.SetDetailLayer(0, 0, 0, details0);
        terrainData.SetDetailLayer(0, 0, 1, details1);
    }
}