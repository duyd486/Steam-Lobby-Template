using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameTxt;
    private Image playerAvatar;

    private ulong currentSteamId;

    private void Awake()
    {
        playerAvatar = GetComponentInChildren<Image>();
    }

    public void UpdatePlayerInfo(string playerName)
    {
        playerNameTxt.text = playerName;
    }

    public async void UpdatePlayerInfo(ulong steamId)
    {
        currentSteamId = steamId;

        var data = await SteamLobbyManager.Instance.GetPlayerData(steamId);

        // chống race condition
        if (currentSteamId != steamId) return;

        playerNameTxt.text = data.Name;

        if (data.Avatar != null && playerAvatar != null)
        {
            playerAvatar.sprite = data.Avatar;
        }
    }


    public void UpdatePlayerInfo()
    {
        playerNameTxt.text = "Name";
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
