﻿using System.Collections;
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
    private int maxHealth = 100;

    [SyncVar]
    private int currHealth;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    UIManager uiManager;

    public void Setup()
    {
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled;
        }
        SetDefaults();
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);
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
        uiManager = GetComponent<UIManager>();
        uiManager.UpdateHealthSlider(currHealth);
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = true;
    }

    public void TakeDamage(int damage)
    {
        CmdTakeDamage(damage);
    }

    [Command]
    public void CmdTakeDamage(int damage)
    {
        RpcTakeDamage(damage);
    }

    [ClientRpc]
    public void RpcTakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }
        currHealth -= damage;
        if(isLocalPlayer) uiManager.UpdateHealthSlider(currHealth);
        if (currHealth <= 0)
        {
            Die();
        }
    }

    [ClientRpc]
    public void RpcNotifyZoneMoved(Vector3 previousZonePos, float radius)
    {
        float dist = Vector2.Distance(new Vector2(previousZonePos.x, previousZonePos.z), new Vector2(transform.position.x, transform.position.z));
        uiManager.ShowZoneMoved();
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

        StartCoroutine(Respawn());
    }


}
