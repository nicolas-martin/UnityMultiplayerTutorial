using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class Health : NetworkBehaviour
{
    public const int MaxHealth = 100;
    [SyncVar(hook = "OnChangeHealth")]
    public int CurrentHealth = MaxHealth;
    public RectTransform HealthBar;
    private NetworkStartPosition[] _spawnPoints;

    public bool DestroyOnDeath;

    void Start()
    {
        if (isLocalPlayer)
        {
            _spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        }
    }

    public void TakeDamage(int amount)
    {
        if (!isServer)
        {
            return;
        }

        CurrentHealth -= amount;
        if (CurrentHealth <= 0)
        {
            if (DestroyOnDeath)
            {
                Destroy(gameObject);
            }
            else
            {
                CurrentHealth = MaxHealth;
                RpcRespawn();
            }
            
        }

        OnChangeHealth(CurrentHealth);
    }

    private void OnChangeHealth(int health)
    {
        HealthBar.sizeDelta = new Vector2(health, HealthBar.sizeDelta.y);
    }

    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            Vector3 spawnPoint = Vector3.zero;

            // If there is a spawn point array and the array is not empty, pick one at random
            if (_spawnPoints != null && _spawnPoints.Length > 0)
            {
                spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)].transform.position;
            }

            // Set the player’s position to the chosen spawn point
            transform.position = spawnPoint;
        }
    }
}