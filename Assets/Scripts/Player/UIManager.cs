using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [SerializeField]
    Text zoneTimer;

    [SerializeField]
    Text zoneMoved;

    [SerializeField]
    Slider healthSlider;

    [SerializeField]
    Slider manaSlider;

    public bool zoneMovedEnabled;

    void Start()
    {
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

    public void UpdateHealthSlider(float health)
    {
        healthSlider.value = health;
    }

    public void UpdateManaSlider(float mana)
    {
        manaSlider.value = mana;
    }
}
