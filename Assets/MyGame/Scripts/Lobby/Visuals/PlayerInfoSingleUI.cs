using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoSingleUI : MonoBehaviour
{
    [SerializeField] private Image playerAvatar;
    [SerializeField] private TextMeshProUGUI playerNameTxt;

    public void UpdatePlayerInfo(string playerName)
    {
        playerNameTxt.text = playerName;
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
