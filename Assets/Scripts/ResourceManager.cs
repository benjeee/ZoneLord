using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour {

    public static ResourceManager instance;

    #region prefabs
    public Terrain mapTerrain;

    public Transform missileImpactPrefab;
    public Transform missilePlayerImpactPrefab;
    public Transform shotPrefab;

    public Transform visionFieldPrefab;
    public Transform visionFlarePrefab;

    public Transform steezPrefab;

    public Transform crowPrefab;
    public Transform crowTargetPrefab;
    public Transform[,] crowTargets;
    public float spaceBetweenTargets = 10;
    public int targetListSize = 40;

    public Transform crowPickupPrefab;
    public Transform visionFlarePickupPrefab;
    public Transform bubbleShieldPickupPrefab;
    public Transform steezPickupPrefab;

    public Transform invisTrailPrefab;

    public Transform bubbleShieldPrefab;
    #endregion prefabs

    #region UI
    public Text zoneTimer;
    public Text zoneMoved;
    public Slider healthSlider;
    public Slider manaSlider;
    #endregion UI


    public Transform distortionPlanePrefab;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than 1 Resource Manager instantiated");
        }
        else
        {
            instance = this;
        }

        crowTargets = new Transform[targetListSize, targetListSize];
        int halfSize = targetListSize / 2;
        for (int i = -halfSize; i < halfSize; i++)
        {
            for(int j = -halfSize; j < halfSize; j++)
            {
                float xVal = spaceBetweenTargets * i;
                float zVal = spaceBetweenTargets * j;
                float yVal = mapTerrain.SampleHeight(new Vector3(xVal, 0, zVal));
                Vector3 pos = new Vector3(xVal, yVal + .4f, zVal);
                Transform crowTarget = Instantiate(crowTargetPrefab, pos, Quaternion.identity);
                crowTarget.parent = this.transform;
                crowTargets[i + halfSize, j + halfSize] = crowTarget;
            }
        }
    }
}
