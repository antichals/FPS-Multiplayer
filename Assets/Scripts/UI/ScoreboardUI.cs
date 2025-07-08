using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreboardUI : MonoBehaviour
{
    [Header("References")]
    public GameObject playerEntryPrefab; 
    public Transform playerListParent;

    



    public void UpdatePlayerStats(int clientId, string nickname, int kills, int deaths)
    {
        // Logic to add/update UI entry...


        GameObject entry;

        if (!UIManager._instance.playerEntries.ContainsKey(clientId))
        {
            // No existe: instanciamos y lo guardamos
            entry = Instantiate(playerEntryPrefab, playerListParent);
            UIManager._instance.playerEntries[clientId] = entry;
        }
        else
        {
            // Ya existe: lo usamos
            entry = UIManager._instance.playerEntries[clientId];
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
        if (UIManager._instance.playerEntries.TryGetValue(clientId, out GameObject entry))
        {
            Destroy(entry);
            UIManager._instance.playerEntries.Remove(clientId);
        }
    }
}

