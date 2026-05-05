using Steamworks.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListLobbyUI : MonoBehaviour
{
    [SerializeField] private Button backBtn;
    [SerializeField] private Button reloadBtn;
    [SerializeField] private GameObject container;
    [SerializeField] private GameObject lobbySingleUI;
    [SerializeField] private MenuUI menuUI;

    private void Start()
    {
        menuUI.OnListLobbyClick += MenuUI_OnListLobbyClick;

        SteamLobbyManager.Instance.OnLobbyListUpdated += OnLobbyListUpdated;

        Hide();

        backBtn.onClick.AddListener(() =>
        {
            Hide();
        });

        reloadBtn.onClick.AddListener(async () =>
        {
            Debug.Log("Reload Steam Lobby");
            await SteamLobbyManager.Instance.ListLobbies();
        });
    }

    private void OnLobbyListUpdated(List<Lobby> lobbies)
    {
        UpdateListLobby(lobbies);
    }

    private void MenuUI_OnListLobbyClick(object sender, System.EventArgs e)
    {
        Show();
        _ = SteamLobbyManager.Instance.ListLobbies();
    }

    public void UpdateListLobby(List<Lobby> lobbies)
    {
        foreach (Transform child in container.transform)
        {
            // bỏ qua template
            if (child == lobbySingleUI.transform) continue;

            Destroy(child.gameObject);
        }

        foreach (var lobby in lobbies)
        {
            GameObject obj = Instantiate(lobbySingleUI, container.transform);
            obj.SetActive(true);

            obj.GetComponent<LobbySingleUI>().UpdateLobby(lobby);
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}