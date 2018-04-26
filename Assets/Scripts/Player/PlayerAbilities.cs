using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerInventory))]
public class PlayerAbilities : NetworkBehaviour {

    public enum SecondaryAbility
    {
        Steez
    }

    public AbilityItem[] itemBar;
    int numEquipped;
    int equipIndex;

    [SerializeField]
    PlayerInventory inventory;

    [SerializeField]
    Transform shootPosition;

    [SerializeField]
    PlayerController controller;

    [SerializeField]
    float invisManaCostPS;

    [SerializeField]
    float throwForce;

    [SerializeField]
    Player player;

    UIManager _uiManager;
    public UIManager uiManager
    {
        get { return _uiManager; }
        set { _uiManager = value; }
    }

    public bool invisToggled;

    [SerializeField]
    float invisCooldown;
    float timeUntilCanInvis;

    [SerializeField]
    float steezManaCost;

    [SerializeField]
    float steezCooldown;
    float timeUntilCanSteez;

    void Awake()
    {
        timeUntilCanInvis = 0;
        timeUntilCanSteez = 0;
        player = GetComponent<Player>();
        controller = GetComponent<PlayerController>();
        inventory = GetComponent<PlayerInventory>();
        invisToggled = false;
        equipIndex = 0;
        itemBar = new AbilityItem[3];
        for (int i = 0; i < 3; i++)
        {
            itemBar[i] = AbilityItem.NoneItem;
        }
        numEquipped = 0;
    }
	
	void Update () {
        HandleSecondaryAbilityInput();
        HandleScrollEquip();
        HandleSwapEquip();
        HandleItemInput();
        HandleInvisToggleInput();

        timeUntilCanInvis = Mathf.Max(0, timeUntilCanInvis - Time.deltaTime);
        if(timeUntilCanInvis > 0)
        {
            uiManager.UpdateInvisCooldownInd(timeUntilCanInvis / invisCooldown);
        }
        if (invisToggled)
        {
            if(!inventory.SpendMana(invisManaCostPS * Time.deltaTime))
            {
                //invisToggled = false;
                //controller.EnableStateChanging();
                ToggleInvis();
            }
        }

        timeUntilCanSteez = Mathf.Max(0, timeUntilCanSteez - Time.deltaTime);
        if(timeUntilCanSteez > 0)
        {
            uiManager.UpdateSteezCooldownInd(timeUntilCanSteez / steezCooldown);
        }
    }

    #region itembar
    public bool PickupItem(AbilityItem.AbilityItemType type)
    {
        if (IncrementItem(type))
        {
            Debug.Log("Slot 1 :" + itemBar[0].ItemType + ", " + itemBar[0].count + ". " + "Slot 2 :" + itemBar[1].ItemType + ", " + itemBar[1].count + ". " + "Slot 3 :" + itemBar[2].ItemType + ", " + itemBar[2].count + ". ");
            uiManager.UpdateItemBar(itemBar, equipIndex);
            return true;
        }
        if(numEquipped < 3)
        {
            for(int i = 0; i < 3; i++)
            {
                if(itemBar[i].ItemType == AbilityItem.AbilityItemType.None)
                {
                    itemBar[i] = new AbilityItem(type);
                    numEquipped++;
                    Debug.Log("Slot 1 :" + itemBar[0].ItemType + ", " + itemBar[0].count + ". " + "Slot 2 :" + itemBar[1].ItemType + ", " + itemBar[1].count + ". " + "Slot 3 :" + itemBar[2].ItemType + ", " + itemBar[2].count + ". ");
                    uiManager.UpdateItemBar(itemBar, equipIndex);
                    return true;
                }
            }
            return false;
        }
        else
        {
            //some UI output indicating inventory is full
            return false;
        }
    }

    bool IncrementItem(AbilityItem.AbilityItemType type)
    {
        for(int i = 0; i < 3; i++)
        {
            if(itemBar[i].ItemType != AbilityItem.AbilityItemType.None && itemBar[i].ItemType == type)
            {
                itemBar[i].Add();
                return true;
            }
        }
        return false;
    }

    bool UseItem()
    {
        AbilityItem equippedItem = itemBar[equipIndex];
        if(equippedItem.ItemType != AbilityItem.AbilityItemType.None)
        {
            int numLeft = equippedItem.Use();
            switch (equippedItem.ItemType)
            {
                case AbilityItem.AbilityItemType.Crow:
                    SpawnCrow();
                    break;
                case AbilityItem.AbilityItemType.Flare:
                    Flare();
                    break;
                case AbilityItem.AbilityItemType.Bubble:
                    SpawnBubble();
                    break;
                default:
                    break;
            }
            if (numLeft == 0)
            {
                itemBar[equipIndex] = AbilityItem.NoneItem;
                numEquipped--;
                ShiftItem(1);
            }
            uiManager.UpdateItemBar(itemBar, equipIndex);
            return true;
        }
        return false;
    }
    #endregion itembar

    #region input


    void HandleScrollEquip()
    {
        float f = Input.GetAxis("Mouse ScrollWheel");
        if (f < 0)
        {
            ShiftItem(1);
        }
        else if (f > 0)
        {
            ShiftItem(-1);
        }
    }

    void HandleSwapEquip()
    {
        if (Input.GetKeyDown("1"))
        {
            SwapItem(0);
        }
        else if (Input.GetKeyDown("2"))
        {
            SwapItem(1);
        }
        else if (Input.GetKeyDown("3"))
        {
            SwapItem(2);
        }
    }


    void ShiftItem(int dir)
    {
        if (dir == 1)
        {
            if (equipIndex == 2) equipIndex = 0;
            else equipIndex++;
        }
        else
        {
            if (equipIndex == 0) equipIndex = 2;
            else equipIndex--;
        }
        uiManager.UpdateItemBar(itemBar, equipIndex);
    }

    void SwapItem(int val)
    {
        if (val == 0 || val == 1 || val == 2)
        {
            equipIndex = val;
            uiManager.UpdateItemBar(itemBar, equipIndex);
        }
    }

    void HandleItemInput()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            UseItem();
        }
    }

    void HandleInvisToggleInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleInvis();
        }
    }

    public void ToggleInvis()
    {
        if (!invisToggled && timeUntilCanInvis == 0)
        {
            invisToggled = true;
            controller.DisableStateChanging();
            controller.CmdUpdateState(PlayerController.PlayerState.Stealth);
            uiManager.ToggleInvisActiveInd();
        }
        else if(invisToggled)
        {
            timeUntilCanInvis = invisCooldown;
            invisToggled = false;
            controller.EnableStateChanging();
            uiManager.ToggleInvisActiveInd();
        }
    }

    void HandleSecondaryAbilityInput()
    {
        if (Input.GetKeyDown("f"))
        {
            if(timeUntilCanSteez == 0)
            {
                if (inventory.SpendMana(steezManaCost))
                {
                    timeUntilCanSteez = steezCooldown;
                    SpawnSteez();
                }
            }
        }
    }

    #endregion input

    #region instantiation
    [Client]
    void Flare()
    {
        CmdSpawnFlare(shootPosition.transform.position, shootPosition.transform.rotation);
    }

    [Command]
    void CmdSpawnFlare(Vector3 position, Quaternion rotation)
    {
        Transform flare = Instantiate(ResourceManager.instance.visionFlarePrefab, position, rotation);
        NetworkServer.Spawn(flare.gameObject);
    }


    [Client]
    void SpawnCrow()
    {
        CmdSpawnCrow(shootPosition.transform.position, shootPosition.transform.rotation);
    }

    [Command]
    void CmdSpawnCrow(Vector3 position, Quaternion rotation)
    {
        Transform crow = Instantiate(ResourceManager.instance.crowPrefab, position, rotation);
        crow.GetComponent<Crow>().SetPlayer(player);
        crow.gameObject.SetActive(true);
        NetworkServer.Spawn(crow.gameObject);
    }

    [Client]
    void SpawnBubble()
    {
        CmdSpawnBubble(shootPosition.transform.position, shootPosition.transform.rotation);
    }

    [Command]
    void CmdSpawnBubble(Vector3 position, Quaternion rotation)
    {
        Transform bubble = Instantiate(ResourceManager.instance.bubbleShieldPrefab, position, rotation);
        bubble.GetComponent<Rigidbody>().AddForce(shootPosition.transform.forward * throwForce, ForceMode.Impulse); // newobj.Init();
        NetworkServer.Spawn(bubble.gameObject);
    }

    [Client]
    void SpawnSteez()
    {
        CmdSpawnSteez(shootPosition.transform.position, shootPosition.transform.rotation, shootPosition.transform.forward);
    }

    [Command]
    void CmdSpawnSteez(Vector3 position, Quaternion rotation, Vector3 forward)
    {
        Transform steez = Instantiate(ResourceManager.instance.steezPrefab, position, rotation);
        steez.GetComponent<Steez>().Forward = forward;
        NetworkServer.Spawn(steez.gameObject);
    }

    /*
    [Client]
    void DistortionPlane()
    {
        CmdSpawnDistortionPlane(shootPosition.transform.position, shootPosition.transform.rotation);
    }

    [Command]
    void CmdSpawnDistortionPlane(Vector3 position, Quaternion rotation)
    {
        Vector3 flip = rotation.eulerAngles;
        flip.y -= 180;
        flip.x = 0;
        Instantiate(ResourceManager.instance.distortionPlanePrefab, position, Quaternion.Euler(flip));
    }*/
    #endregion instantiation
}
