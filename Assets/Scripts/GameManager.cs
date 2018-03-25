using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    private static Dictionary<string, Player> players = new Dictionary<string, Player>();

    public MatchSettings matchSettings;

    public ZoneMovement zoneMovement;

    public NetworkManager networkManager;

    public Text zoneTimer;
    public Text zoneMoved;

    void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("More than 1 GameManager instantiated");
        } else
        {
            instance = this;
        }
    }

    #region Player tracking

    private const string PLAYER_ID_PREFIX = "Player ";

    public static void RegisterPlayer(string netID, Player player)
    {
        string playerID = PLAYER_ID_PREFIX + netID;
        players.Add(playerID, player);
        player.transform.name = playerID;
    }

    public static void UnRegisterPlayer(string netId)
    {
        players.Remove(netId);
    }

    public static Player GetPlayer(string playerId)
    {
        return players[playerId];
    }

    #endregion

    public void NotifyPlayersZoneMoved(Vector3 previousZonePos, float radius)
    {
        foreach(Player p in players.Values)
        {
            p.RpcNotifyZoneMoved(previousZonePos, radius);
        }
    }
}
