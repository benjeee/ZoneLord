using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead;  }
        protected set { _isDead = value;  }
    }

    [SerializeField]
    private float maxHealth = 100;

    [SyncVar]
    private float currHealth;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    UIManager _uiManager;
    public UIManager uiManager
    {
        get { return _uiManager; }
        set { _uiManager = value; }
    }

    public void Setup()
    {
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled;
        }
        SetDefaults();
    }

    private void Respawn()
    {
        SetDefaults();
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;
        Debug.Log(transform.name + " respawned");
    }

    public void SetDefaults()
    {
        isDead = false;
        currHealth = maxHealth;
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }
        if(_uiManager != null) _uiManager.UpdateHealthSlider(currHealth);
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = true;
    }

    public void TakeDamage(float damage)
    {
        CmdTakeDamage(damage);
    }

    [Command]
    public void CmdTakeDamage(float damage)
    {
        RpcTakeDamage(damage);
    }

    [ClientRpc]
    public void RpcTakeDamage(float damage)
    {
        if (isDead)
        {
            return;
        }
        currHealth -= damage;
        if(isLocalPlayer) _uiManager.UpdateHealthSlider(currHealth);
        if (currHealth <= 0)
        {
            Die();
        }
    }

    [ClientRpc]
    public void RpcNotifyZoneMoved(Vector3 previousZonePos, float radius)
    {
        float dist = Vector2.Distance(new Vector2(previousZonePos.x, previousZonePos.z), new Vector2(transform.position.x, transform.position.z));
        _uiManager.ShowZoneMoved();
        if(dist > radius)
        {
            RpcTakeDamage(1000);
        }
    }

    private void Die()
    {
        isDead = true;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        Debug.Log(transform.name + " IS DEAD!!!!");

        Invoke("Respawn", GameManager.instance.matchSettings.respawnTime);
    }


}
