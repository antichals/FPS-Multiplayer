using System.Collections.Generic;
using FishNet.Object;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour 
{
    public static UIManager _instance { get; private set; }
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private ScoreboardUI _scoreboard;
    public Dictionary<int, GameObject> playerEntries;

    private void OnEnable()
    {

    }

    private void Awake()
    {
        // Ensure only one instance exists
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        _instance.playerEntries = new();

        if (_scoreboard != null)
            _scoreboard.gameObject.SetActive(false);

        //DontDestroyOnLoad(gameObject); // Optional, if you want it across scenes
    }


    public void SubscribeToHealthChange(PlayerHealth playerHealth)
    {
        playerHealth.OnHealthChanged += UpdateHealth;
    }


    public void UpdateHealth(int currentHealth)
    {
        _instance._healthText.text = currentHealth.ToString();
    }

    public void NewPlayerScoreboard(int clinetID)
    {
        Debug.Log("New player scoreboard-----");
        _instance._scoreboard?.UpdatePlayerStats(clinetID, clinetID.ToString(), 0, 0);
    }

    public void UpdateScoreboard(int clientId, string nickname, int kills, int deaths)
    {
        _instance._scoreboard?.UpdatePlayerStats(clientId, nickname, kills, deaths);
    }

    public void RemoveScoreboardEntry(int clientId)
    {
        _instance._scoreboard?.RemovePlayerEntry(clientId);
    }
    public void ToggleScoreboard(bool show)
    {
        if (_scoreboard != null)
        {
            _scoreboard.gameObject.SetActive(show);
        }
    }

}
