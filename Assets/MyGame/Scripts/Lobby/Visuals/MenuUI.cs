using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Button createLobbyBtn;
    [SerializeField] private Button listLobbyBtn;


    [SerializeField] private GameObject createLobbyModal;
    [SerializeField] private Button cancelBtn;
    [SerializeField] private Button createBtn;
    [SerializeField] private TMP_InputField nameLobbyInput;

    [SerializeField] private TMP_InputField playerNameInput;


    public event EventHandler OnListLobbyClick;

    private void Awake()
    {
        createLobbyModal.SetActive(false);
    }

    private void Start()
    {
        playerNameInput.onValueChanged.AddListener((name) =>
        {
            LobbyManager.Instance.SetPlayerName(playerNameInput.text);
        });

        createLobbyBtn.onClick.AddListener(() => { createLobbyModal.SetActive(true); });

        cancelBtn.onClick.AddListener(() => { createLobbyModal.SetActive(false); });

        createBtn.onClick.AddListener(CreateLobby);



        listLobbyBtn.onClick.AddListener(async () =>
        {
            await LobbyManager.Instance.ListLobbies();
            OnListLobbyClick?.Invoke(this, EventArgs.Empty);
        });

        playerNameInput.text = LobbyManager.Instance.GetPlayer().Data["PlayerName"].Value;
        if (playerNameInput.text.IsNullOrEmpty())
        {
            playerNameInput.text = LobbyManager.Instance.GetPlayerName();
            LobbyManager.Instance.SetPlayerName(playerNameInput.text);
        }
    }

    private async void CreateLobby()
    {
        if (nameLobbyInput.text.IsNullOrEmpty()) return;
        //LobbyManager.Instance.CreateLobby(nameLobbyInput.text, 4);
        await LobbyManager.Instance.CreateLobbyWithRelay(nameLobbyInput.text, 4);
        createLobbyModal.SetActive(false);
    }
}
