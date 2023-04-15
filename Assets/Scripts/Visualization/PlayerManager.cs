using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using TMPro;
using System.Collections;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager instance;

    public Dictionary<int, Data.Player> players = new Dictionary<int, Data.Player>();
    [SerializeField] List<Transform> spawnPoints = new List<Transform>();

    [SerializeField] private MenuControl menuControl;

    public TextMeshProUGUI healthTMP, ammoTMP;
    private int maxHealth = 100;

    private void Awake()
    {
        instance = this;
    }

    public void DamagePlayer(int playerID, int damage, int shooterID)
    {

        if (!base.IsServer)
            return;

        players[playerID].health -= damage;

        if (players[playerID].health <= 0)
        {
            // If player dies, update UI with max health
            UpdateHealthUI(players[playerID].connection, players[playerID].playerObject, maxHealth);
        }
        else
        {
            UpdateHealthUI(players[playerID].connection, players[playerID].playerObject, players[playerID].health);
        }

        if (players[playerID].health <= 0)
        {
            players[playerID].playerObject.GetComponent<Collider>().enabled = false;
            PlayerKilled(playerID, shooterID);
        }
    }

    void PlayerKilled(int playerID, int attackerID)
    {
        print("Player " + playerID.ToString() + " was killed by " + attackerID.ToString());
        players[playerID].deaths++;
        players[playerID].health = maxHealth;
        players[attackerID].kills++;

        RespawnPlayer(players[playerID].connection, players[playerID].playerObject, Random.Range(0, spawnPoints.Count));
        players[playerID].health = maxHealth;
        UpdateHealthUI(players[playerID].connection, players[playerID].playerObject, players[playerID].health);
        players[playerID].playerObject.GetComponent<Collider>().enabled = true;

        StartCoroutine(MaxHealth(playerID));
    }

    IEnumerator MaxHealth(int playerID)
    {
        yield return new WaitForSeconds(1f);

        players[playerID].health = maxHealth;
        UpdateHealthUI(players[playerID].connection, players[playerID].playerObject, players[playerID].health);
    }

    [TargetRpc]
    void RespawnPlayer(NetworkConnection conn, GameObject player, int spawn)
    {
        player.transform.position = spawnPoints[spawn].position;

        player.GetComponent<PlayerEntity>().ammoLeft = player.GetComponent<PlayerEntity>().maxAmmo;
        player.GetComponent<PlayerEntity>().RespawnServer();
    }

    public void RestoreHealth(GameObject player)
    {
        int playerID = player.GetInstanceID();

        if (players[playerID].health < maxHealth)
        {
            players[playerID].health += 40;

            if (players[playerID].health > maxHealth)
            {
                players[playerID].health = maxHealth;

            }

            UpdateHealthUI(players[playerID].connection, players[playerID].playerObject, players[playerID].health);
        }

    }

    [TargetRpc]
    public void UpdateHealthUI(NetworkConnection conn, GameObject player, int health)
    {
        player.GetComponent<PlayerEntity>().healthTMP.text = "HP - " + health;
    }

    public void ChangeCursorLock()
    {
        Cursor.visible = !Cursor.visible;

        if (Cursor.visible)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}