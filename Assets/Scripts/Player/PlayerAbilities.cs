using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerInventory))]
public class PlayerAbilities : NetworkBehaviour {

    public static int CROW =    1;
    public static int FLARE =   2;

    int equipped = CROW;

    [SerializeField]
    PlayerInventory inventory;

    [SerializeField]
    Transform shootPosition;

    [SerializeField]
    PlayerController controller;

    [SerializeField]
    float invisManaCostPS;

    [SerializeField]
    Player player;

    public bool invisToggled;

    void Start () {
        player = GetComponent<Player>();
        controller = GetComponent<PlayerController>();
        inventory = GetComponent<PlayerInventory>();
        invisToggled = false;
        equipped = CROW;
    }
	
	void Update () {
        HandleScrollEquip();
        HandleSwapEquip();
        HandleAbilityInput();
        HandleInvisToggleInput();
        if (invisToggled)
        {
            if(!inventory.SpendMana(invisManaCostPS * Time.deltaTime))
            {
                invisToggled = false;
                controller.EnableStateChanging();
            }
        }
    }

    void HandleScrollEquip()
    {
        float f = Input.GetAxis("Mouse ScrollWheel");
        if (f < 0)
        {
            ShiftAbility(-1);
        }
        else if (f > 0)
        {
            ShiftAbility(1);
        }
    }

    void HandleSwapEquip()
    {
        if (Input.GetKeyDown("1"))
        {
            SwapAbility(CROW);
        }
        else if (Input.GetKeyDown("2"))
        {
            SwapAbility(FLARE);
        }
    }


    void ShiftAbility(int dir)
    {
        if (dir == 1)
        {
            if (equipped == FLARE) equipped = CROW;
            else equipped++;
        }
        else
        {
            if (equipped == CROW) equipped = FLARE;
            else equipped--;
        }
        //UIController.instance.ChangeEquipIndicator(equipped);
    }

    void SwapAbility(int val)
    {
        if (val == CROW || val == FLARE)
        {
            equipped = val;
            //UIController.instance.ChangeEquipIndicator(equipped);
        }
    }

    void HandleAbilityInput()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            if(equipped == CROW)
            {
                SpawnCrow();
            }else if(equipped == FLARE)
            {
                Flare();
            }
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
        if (!invisToggled)
        {
            invisToggled = true;
            controller.DisableStateChanging();
            controller.CmdUpdateState(PlayerController.PlayerState.Stealth);
        }else
        {
            invisToggled = false;
            controller.EnableStateChanging();
        }
    }

    [Client]
    void Flare()
    {
        if (inventory.UseFlare())
        {
            CmdSpawnFlare(shootPosition.transform.position, shootPosition.transform.rotation);
        }
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
        if (inventory.UseCrow())
        {
            CmdSpawnCrow(shootPosition.transform.position, shootPosition.transform.rotation);
        }
    }

    [Command]
    void CmdSpawnCrow(Vector3 position, Quaternion rotation)
    {
        Transform crow = Instantiate(ResourceManager.instance.crowPrefab, position, rotation);
        crow.GetComponent<Crow>().SetPlayer(player);
        crow.gameObject.SetActive(true);
        NetworkServer.Spawn(crow.gameObject);
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
}
