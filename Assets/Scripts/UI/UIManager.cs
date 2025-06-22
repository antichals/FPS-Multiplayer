using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager _instance { get; private set; }
    [SerializeField] private TextMeshProUGUI _healthText;


    private void Awake()
    {
        // Ensure only one instance exists
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
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
}
