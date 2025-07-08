using UnityEngine;
using UnityEngine.UI;
using FishNet.Managing;
using FishNet.Transporting.Tugboat;
/*
public class LobbyManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button startServerButton;
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button connectToGameButton;
    [SerializeField] private InputField ipInputField;
    [SerializeField] private GameObject lobbyPanel;

    [Header("Networking")]
    [SerializeField] private NetworkManager _networkManager;

    private void Start()
    {
        startServerButton.onClick.AddListener(StartServerOnly);
        startHostButton.onClick.AddListener(StartHost);
        connectToGameButton.onClick.AddListener(ConnectToGame);

        _networkManager = FindObjectOfType<NetworkManager>();
    }

    private void StartServerOnly()
    {
        Debug.Log("[LobbyManager] Starting Server Only...");
        _networkManager.StartServer();
        HideLobby();
    }

    private void StartHost()
    {
        Debug.Log("[LobbyManager] Starting Host (Server + Client)...");
        _networkManager.ServerManager.StartHost();
        HideLobby();
    }

    private void ConnectToGame()
    {
        string ip = ipInputField.text;
        if (string.IsNullOrWhiteSpace(ip))
        {
            Debug.LogWarning("[LobbyManager] IP is empty.");
            return;
        }

        Tugboat tugboat = (Tugboat)_networkManager.TransportManager.Transport;
        tugboat.SetClientAddress(ip);

        Debug.Log($"[LobbyManager] Connecting to {ip}...");
        _networkManager.ClientManager.StartConnection();
        HideLobby();
    }

    private void HideLobby()
    {
        if (lobbyPanel != null)
            lobbyPanel.SetActive(false);
    }
}
*/
