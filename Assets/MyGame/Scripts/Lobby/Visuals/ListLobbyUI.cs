using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
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
        LobbyManager.Instance.OnListLobbiesChanged += LobbyManager_OnListLobbiesChanged;

        Hide();
        backBtn.onClick.AddListener(() =>
        {
            Hide();
        });
        reloadBtn.onClick.AddListener(async () =>
        {
            Debug.Log("Reload");
            await LobbyManager.Instance.ListLobbies();
        });
    }

    private void LobbyManager_OnListLobbiesChanged(object sender, LobbyManager.OnListLobbiesChangedEventArgs e)
    {
        UpdateListLobby();
    }

    private void MenuUI_OnListLobbyClick(object sender, System.EventArgs e)
    {
        Show();
        UpdateListLobby();
    }

    public void UpdateListLobby()
    {
        List<Lobby> lobbies = LobbyManager.Instance.GetCurrentLobbies();

        foreach (Transform chil in container.transform)
        {
            chil.gameObject.SetActive(false);
        }

        for (int i = 0; i < lobbies.Count; i++)
        {
            GameObject lobbyInfoOb = Instantiate(lobbySingleUI, container.transform);
            lobbyInfoOb.SetActive(true);
            lobbyInfoOb.GetComponent<LobbySingleUI>().UpdateLobby(lobbies[i]);
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
