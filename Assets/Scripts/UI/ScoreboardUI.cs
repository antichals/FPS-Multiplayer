using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreboardUI : MonoBehaviour
{
    [Header("References")]
    public GameObject playerEntryPrefab; 
    public Transform playerListParent;
    public Dictionary<int, GameObject> playerEntries = new();




    public void UpdatePlayerStats(int clientId, string nickname, int kills, int deaths)
    {
        // Logic to add/update UI entry...


        GameObject entry;

        if (!playerEntries.ContainsKey(clientId))
        {
            // No existe: instanciamos y lo guardamos
            entry = Instantiate(playerEntryPrefab, playerListParent);
            playerEntries[clientId] = entry;
        }
        else
        {
            // Ya existe: lo usamos
            entry = playerEntries[clientId];
        }

        TextMeshProUGUI[] texts = entry.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length >= 3)
        {
            texts[0].text = nickname;
            texts[1].text = kills.ToString();
            texts[2].text = deaths.ToString();
        }
    }

    public void RemovePlayerEntry(int clientId)
    {

        Debug.Log("[ScoreboardUI.RemovePlayerEntry] method called");

        if (playerEntries.TryGetValue(clientId, out GameObject entry))
        {
            Destroy(entry);
            playerEntries.Remove(clientId);
        }
    }
}

