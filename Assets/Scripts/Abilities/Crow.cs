using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Crow : NetworkBehaviour {

    enum dir
    {
        left,
        right,
        forward,
        back
    }

    dir currDir;

    Transform target;

    public float rotateSpeed;

    public float moveSpeed;

    private bool canMove = true;
    private bool canRotate = true;

    private int iVal;
    private int jVal;

    Player player;

    bool chasingPlayer;

    [SerializeField]
    SphereCollider playerDetectCollider;

    [SerializeField]
    SphereCollider playerHitCollider;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    void Start()
    {
        if (isServer)
        {
            chasingPlayer = false;
            iVal = 0;
            jVal = 0;
            SetInitialTarget();
            currDir = dir.forward;
        }
    }

    void EnableColliders()
    {
        playerDetectCollider.enabled = true;
        playerHitCollider.enabled = true;
    }

    void Update()
    {
        if (isServer)
        {
            RotateTowardsTarget();
            MoveTowardsTarget();
        }
    }

    public void SetPlayer(Player p)
    {
        player = p;
        EnableColliders();
    }

    void RotateTowardsTarget()
    {
        if (canRotate)
        {
            Vector3 targetDir = target.position - transform.position;
            float step = rotateSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
            transform.rotation = Quaternion.LookRotation(newDir);
        }
    }

    void MoveTowardsTarget()
    {
        if (canMove)
        {
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(target.position.x, target.position.y, target.position.z), step);
        }
    }

    public void StartMoving()
    {
        canMove = true;
    }

    public void StartRotating()
    {
        canRotate = true;
    }

    public void StopMoving()
    {
        canMove = false;
    }

    public void StopRotating()
    {
        canRotate = false;
    }

    public void HitTarget()
    {
        Invoke("FindNextTarget", 0f);
    }

    void FindNextTarget()
    {
        if (chasingPlayer) return;
        int newDir = Random.Range(0, 15);
        dir modDir;
        if(newDir < 12)
        {
            modDir = dir.forward;
        }else if(newDir == 13)
        {
            modDir = dir.left;
        }else
        {
            modDir = dir.right;
        }
        currDir = getRelativeDir(currDir, modDir);
        PickTargetInDirection();
    }

    void PickTargetInDirection()
    {
        bool pickedValidTarget = false;
        if(currDir == dir.forward)
        {
            if(iVal < ResourceManager.instance.targetListSize - 1)
            {
                iVal++;
                pickedValidTarget = true;
            }
        }else if(currDir == dir.left)
        {
            if(jVal > 0)
            {
                jVal--;
                pickedValidTarget = true;
            }
        }else if(currDir == dir.right)
        {
            if(jVal < ResourceManager.instance.targetListSize - 1)
            {
                jVal++;
                pickedValidTarget = true;
            }
        }else if(currDir == dir.back)
        {
            if(iVal > 0)
            {
                iVal--;
                pickedValidTarget = true;
            }
        }
        if (pickedValidTarget)
        {
            target = ResourceManager.instance.crowTargets[iVal, jVal];
        }else
        {
            FindNextTarget();
        }
    }

    void SetInitialTarget()
    {
        Vector3 position = transform.position;
        iVal = (int)Mathf.Floor((position.x) / ResourceManager.instance.spaceBetweenTargets) + ResourceManager.instance.targetListSize/2;
        jVal = (int)Mathf.Floor((position.z) / ResourceManager.instance.spaceBetweenTargets) + ResourceManager.instance.targetListSize/2;
        
        target = ResourceManager.instance.crowTargets[iVal, jVal];
    }

    dir getRelativeDir(dir current, dir mod)
    {
        if (mod == dir.forward) return current;
        if(current == dir.forward)
        {
            return mod;
        }else if(current == dir.left)
        {
            if(mod == dir.left) return dir.back;
            if(mod == dir.right) return dir.forward;
            if (mod == dir.back) return dir.right;
        }else if(current == dir.right)
        {
            if (mod == dir.left) return dir.forward;
            if (mod == dir.right) return dir.back;
            if (mod == dir.back) return dir.left;
        }else if (current == dir.back)
        {
            if (mod == dir.left) return dir.right;
            if (mod == dir.right) return dir.left;
            if (mod == dir.back) return dir.forward;
        }
        return current;
    }

    void OnTriggerEnter(Collider col)
    {
        if (player != null && col.CompareTag("Player") && col.name != player.name)
        {
            target = col.transform;
            chasingPlayer = true;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (player != null && col.gameObject.CompareTag("Player") && col.gameObject.name != player.name)
        {
            PlayerController controller = col.gameObject.GetComponent<PlayerController>();
            PlayerAbilities abilities = col.gameObject.GetComponent<PlayerAbilities>();
            if (abilities.invisToggled)
            {
                abilities.ToggleInvis();
            }
            controller.CmdUpdateState(PlayerController.PlayerState.Combat);
            controller.DisableStateChanging(15);
            Destroy(this.gameObject, 15);
        }
    }
}
