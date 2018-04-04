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

    public float mana;
    
    void Start()
    {
        mana = maxMana;
    }

    void Update()
    {
        mana = Mathf.Min(maxMana, mana + manaRegenPerSecond * Time.deltaTime);
    }

    public void PickupItem(Pickup.PickupType type)
    {
        Debug.Log("pickeder up");
    }

    public bool SpendMana(float amt)
    {
        if(mana - amt > 0)
        {
            mana -= amt;
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
