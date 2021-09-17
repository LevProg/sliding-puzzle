using UnityEngine;
using UnityEngine.UI;

public class LockPanel : MonoBehaviour
{
    [SerializeField] private Text allText;
    [SerializeField] private int stars;
    void Start()
    {
        allText.text = PlayerPrefs.GetInt("Stars", 0) + "★";
        ClosePanel();
    }
    private void ClosePanel()
    {
        if (PlayerPrefs.GetInt($"panel-{stars}", 0) == 1)
        {
            gameObject.SetActive(false);
        }
    }
    public void Unlock()
    {
        int currentStars = PlayerPrefs.GetInt("Stars");
        if (PlayerPrefs.GetInt("Stars") >= stars)
        {
            PlayerPrefs.SetInt($"panel-{stars}", 1);
            PlayerPrefs.SetInt("Stars", currentStars- stars);
            allText.text = PlayerPrefs.GetInt("Stars", 0) + "★";
            ClosePanel();
        }
    }
}
