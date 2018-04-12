using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {

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
}
