using System.Collections;
using System.Collections.Generic;
using FishNet.Managing.Timing;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager _instances { get; private set; }
    [SerializeField] private TextMeshProUGUI _healthText;

    private void Awake()
    {
        // Ensure only one instance exists
        if (_instances != null && _instances != this)
        {
            Destroy(gameObject);
            return;
        }

        _instances = this;
        DontDestroyOnLoad(gameObject); // Optional, if you want it across scenes
    }

    public static void SetHealthText(string health)
    {
        _instances._healthText.text = health;
    }
}
