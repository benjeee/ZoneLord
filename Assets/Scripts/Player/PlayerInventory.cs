using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {

    [SerializeField]
    int numFlares;

    [SerializeField]
    int numCrows;

    [SerializeField]
    float maxMana;

    [SerializeField]
    float manaRegenPerSecond;

    [SerializeField]
    UIManager uiManager;

    public float mana;
    
    void Start()
    {
        mana = maxMana;
        uiManager = GetComponent<UIManager>();
    }

    void Update()
    {
        mana = Mathf.Min(maxMana, mana + manaRegenPerSecond * Time.deltaTime);
        uiManager.UpdateManaSlider(mana);
    }

    public void PickupItem(Pickup.PickupType type)
    {
        switch (type)
        {
            case Pickup.PickupType.Crow:
                numCrows++;
                break;
            case Pickup.PickupType.Flare:
                numFlares++;
                break;
            default:
                Debug.LogError("Unknown Pickup");
                break;
        }
    }

    public bool SpendMana(float amt)
    {
        if(mana - amt > 0)
        {
            mana -= amt;
            uiManager.UpdateManaSlider(mana);
            return true;
        }
        return false;
    }

    public bool UseFlare()
    {
        if(numFlares > 0)
        {
            numFlares--;
            return true;
        }
        return false;
    }

    public bool UseCrow()
    {
        if(numCrows > 0)
        {
            numCrows--;
            return true;
        }
        return false;
    }
}
