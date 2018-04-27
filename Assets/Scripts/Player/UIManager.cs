using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    Dictionary<AbilityItem.AbilityItemType, Sprite> itemToSpriteMap;

    [SerializeField]
    Text zoneTimer;

    [SerializeField]
    Text zoneMoved;

    [SerializeField]
    RectTransform healthFill;

    [SerializeField]
    RectTransform manaFill;

    [SerializeField]
    Sprite noneSprite, visionFlareSprite, bubbleShieldSprite, crowSprite;

    [SerializeField]
    Image itemSlot1, itemSlot2, itemSlot3;

    [SerializeField]
    Image itemEquippedInd1, itemEquippedInd2, itemEquippedInd3;

    [SerializeField]
    Text[] itemAmt;

    [SerializeField]
    Image invisActiveImage;

    [SerializeField]
    RectTransform invisCooldown;

    [SerializeField]
    RectTransform steezCooldown;

    [SerializeField]
    Sprite[] activeIndicatorSprites;

    [SerializeField]
    Image notEnoughManaInd;

    [SerializeField]
    Text deathText;

    int currActiveSpriteIndex;
    bool invisActive;

    public bool zoneMovedEnabled;

    void Start()
    {
        invisActive = false;
        invisActiveImage.enabled = false;
        zoneMoved.enabled = false;
        zoneMovedEnabled = false;
        deathText.enabled = false;
        itemToSpriteMap = new Dictionary<AbilityItem.AbilityItemType, Sprite>()
        {
            {AbilityItem.AbilityItemType.None, noneSprite },
            {AbilityItem.AbilityItemType.Flare, visionFlareSprite },
            {AbilityItem.AbilityItemType.Bubble, bubbleShieldSprite },
            {AbilityItem.AbilityItemType.Crow, crowSprite }
        };
    }

    void Update()
    {
        UpdateZoneTimer(GameManager.instance.zoneMovement.timeBetweenMoves - GameManager.instance.zoneMovement.timeSinceMoved); 
    }

    public void UpdateInvisCooldownInd(float pct)
    {
        invisCooldown.sizeDelta = new Vector2(40, 40 * pct);
    }

    public void UpdateSteezCooldownInd(float pct)
    {
        steezCooldown.sizeDelta = new Vector2(40, 40 * pct);
    }

    public void ShowNotEnoughMana()
    {
        StartCoroutine(NotEnoughManaCoroutine(.2f));
    }

    IEnumerator NotEnoughManaCoroutine(float displayTime)
    {
        float time = 0;
        float currAlpha = 1f;
        while(time < displayTime)
        {
            currAlpha = Mathf.Lerp(currAlpha, 0, (time / displayTime));
            time += Time.deltaTime;
            notEnoughManaInd.color = new Color(1, 1, 1, currAlpha);
            yield return new WaitForSeconds(.02f);
        }
    }

    public void ToggleInvisActiveInd()
    {
        if (invisActive)
        {
            invisActive = false;
            invisActiveImage.enabled = false;
            CancelInvoke("SwapActiveInd");
        }
        else
        {
            invisActive = true;
            invisActiveImage.enabled = true;
            currActiveSpriteIndex = 0;
            SwapActiveInd();
        }
    }

    void SwapActiveInd()
    {
        invisActiveImage.sprite = activeIndicatorSprites[currActiveSpriteIndex];
        if (currActiveSpriteIndex == activeIndicatorSprites.Length - 1) currActiveSpriteIndex = 0;
        else currActiveSpriteIndex++;
        Invoke("SwapActiveInd", .02f);
    }

    public void UpdateItemBar(AbilityItem[] itemBar, int equippedSlot)
    {
        itemSlot1.sprite = itemToSpriteMap[itemBar[0].ItemType];
        itemSlot2.sprite = itemToSpriteMap[itemBar[1].ItemType];
        itemSlot3.sprite = itemToSpriteMap[itemBar[2].ItemType];
        for(int i = 0; i < 3; i++)
        {
            if (itemBar[i].ItemType != AbilityItem.AbilityItemType.None)
            {
                itemAmt[i].text = itemBar[i].count.ToString();
            }
            else itemAmt[i].text = "";
        }
        if(equippedSlot == 0)
        {
            itemEquippedInd1.enabled = true;
            itemEquippedInd2.enabled = false;
            itemEquippedInd3.enabled = false;
        }
        else if (equippedSlot == 1)
        {
            itemEquippedInd1.enabled = false;
            itemEquippedInd2.enabled = true;
            itemEquippedInd3.enabled = false;
        }
        else if (equippedSlot == 2)
        {
            itemEquippedInd1.enabled = false;
            itemEquippedInd2.enabled = false;
            itemEquippedInd3.enabled = true;
        }
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

    public void ShowDeathText()
    {
        deathText.enabled = true;
        Invoke("HideDeathText", GameManager.instance.matchSettings.respawnTime);
    }

    void HideDeathText()
    {
        deathText.enabled = false;
    }

    public void UpdateZoneTimer(float timeLeft)
    {
        zoneTimer.text = timeLeft.ToString();
    }

    public void UpdateHealthBar(float health)
    {
        healthFill.sizeDelta = new Vector2(health * 2, 20);
    }

    public void UpdateManaBar(float mana)
    {
        manaFill.sizeDelta = new Vector2(mana * 2, 20);
    }
}
