using TMPro;
using UnityEngine;

public class ResultPanel : MonoBehaviour
{
    public TMP_Text resultText;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void ShowThinking()
    {
        gameObject.SetActive(true);
        resultText.text = "Thinking...";
        resultText.color = Color.white;
    }

    public void ShowFail()
    {
        gameObject.SetActive(true);
        resultText.text = "NO\nSOLUTION";
        resultText.color = Color.white;
    }

    public void ShowSuccess()
    {
        gameObject.SetActive(true);
        resultText.text = "WIN!";
        resultText.color = Color.softRed;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}