using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameTxt;
    [SerializeField] private Button createLobbyBtn;
    [SerializeField] private Button listLobbyBtn;

    public event EventHandler OnListLobbyClick;

    private void Start()
    {
        createLobbyBtn.onClick.AddListener(CreateLobby);

        listLobbyBtn.onClick.AddListener(async () =>
        {
            await SteamLobbyManager.Instance.ListLobbies();
            OnListLobbyClick?.Invoke(this, EventArgs.Empty);
        });


        if (SteamLobbyManager.Instance.IsInitialized)
        {
            SetPlayerName();
        }
        else
        {
            SteamLobbyManager.Instance.OnSteamInitDone += SetPlayerName;
        }
    }

    private void OnDestroy()
    {
        createLobbyBtn.onClick.RemoveAllListeners();
        listLobbyBtn.onClick.RemoveAllListeners();
        if (SteamLobbyManager.Instance.IsInitialized)
        {
            SteamLobbyManager.Instance.OnSteamInitDone -= SetPlayerName;
        }
    }

    private async void CreateLobby()
    {
        await SteamLobbyManager.Instance.CreateLobby(4);
    }

    private void SetPlayerName()
    {
        Debug.Log("Player: " + SteamLobbyManager.Instance.GetPlayerName());
        playerNameTxt.text = "Player: " + SteamLobbyManager.Instance.GetPlayerName();
    }
}
