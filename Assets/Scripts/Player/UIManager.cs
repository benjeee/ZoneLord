using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [SerializeField]
    Text zoneTimer;

    [SerializeField]
    Text zoneMoved;

    public bool zoneMovedEnabled;

    void Awake()
    {
        zoneTimer = GameManager.instance.zoneTimer;
        zoneMoved = GameManager.instance.zoneMoved;
        zoneMoved.enabled = false;
        zoneMovedEnabled = false;
    }

    void Update()
    {
        UpdateZoneTimer(GameManager.instance.zoneMovement.timeBetweenMoves - GameManager.instance.zoneMovement.timeSinceMoved);
    }

    public void ShowZoneMoved()
    {
        zoneTimer.enabled = false;
        zoneMoved.enabled = true;
        Invoke("ShowZoneTimer", 2f);
    }

    public void ShowZoneTimer()
    {
        zoneMoved.enabled = false;
        zoneTimer.enabled = true;
    }


    public void UpdateZoneTimer(float timeLeft)
    {
        zoneTimer.text = timeLeft.ToString();
    }
}
