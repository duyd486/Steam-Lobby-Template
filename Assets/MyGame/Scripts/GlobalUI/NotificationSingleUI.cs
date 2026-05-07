using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private Outline notificationOutline;

    public void SetNotification(string message, bool isError)
    {
        notificationText.text = message;
        if (isError)
        {
            notificationOutline.effectColor = Color.red;
        }
        else
        {
            notificationOutline.effectColor = Color.green;
        }
        gameObject.SetActive(true);
        StartCoroutine(ShowNotification());
    }

    private IEnumerator ShowNotification()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}
