using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {

    [SerializeField]
    float maxMana;

    [SerializeField]
    float manaRegenPerSecond;

    UIManager _uiManager;
    public UIManager uiManager
    {
        get { return _uiManager; }
        set { _uiManager = value; }
    }

    public float mana;
    
    void Start()
    {
        mana = maxMana;
    }

    void Update()
    {
        mana = Mathf.Min(maxMana, mana + manaRegenPerSecond * Time.deltaTime);
        if(uiManager != null)
            _uiManager.UpdateManaSlider(mana);
    }

    public bool SpendMana(float amt)
    {
        if(mana - amt > 0)
        {
            mana -= amt;
            _uiManager.UpdateManaSlider(mana);
            return true;
        }
        return false;
    }
}
