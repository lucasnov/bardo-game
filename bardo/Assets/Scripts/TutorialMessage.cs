using UnityEngine;
using TMPro;
using System.Collections;

public class TutorialMessage : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public float displayTime = 15f;
    public float fadeDuration = 1f;

    void Start()
    {
        StartCoroutine(ShowMessage());
    }

    private IEnumerator ShowMessage()
    {
        messageText.gameObject.SetActive(true);

        // Exibir por X segundos
        yield return new WaitForSeconds(displayTime);

        // Fade-out
        float t = 0;
        Color c = messageText.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            messageText.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }

        messageText.gameObject.SetActive(false);
    }
}
