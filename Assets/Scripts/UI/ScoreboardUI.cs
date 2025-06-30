using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreboardUI : MonoBehaviour
{
    public GameObject playerEntryPrefab; 
    public Transform playerListParent;

    //private Dictionary<ulong, GameObject> playerEntries = new();

    public void UpdatePlayerStats(ulong clientId, string nickname, int kills, int deaths)
    {
        // Logic to add/update UI entry...
    }
}

