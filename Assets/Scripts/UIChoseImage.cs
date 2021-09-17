using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIChoseImage : MonoBehaviour
{
    [SerializeField] private GameObject panelComplexity;
    [SerializeField] private Text starsCount;
    [SerializeField] private Text[] imageStarsCount;
    private void Start()
    {
        string L = SceneManager.GetActiveScene().name;
        int stars = 0;
        for (int i = 0; i < imageStarsCount.Length; i++)
        {
            int currentStars = PlayerPrefs.GetInt($"{L}-{i+1}", 0);
            stars += currentStars;
            imageStarsCount[i].text = $"{currentStars} <color=yellow>★</color>";
        }
        starsCount.text =$"{stars}/24 <color=yellow>★</color>";
    }
    public void ChoseImage(int imageCount)
    {
        panelComplexity.SetActive(true);
        PlayerPrefs.SetInt("CurrentImage", imageCount);
    }
    public void GetComplexity(int complexity)//1-matrix 3x3, 2-matrix 4x4
    {
        PlayerPrefs.SetInt("CurrentComplexity", complexity);
        PlayerPrefs.SetString("CurrentCategories", SceneManager.GetActiveScene().name);
        Debug.Log(PlayerPrefs.GetInt("CurrentImage", 0) + "   " + PlayerPrefs.GetInt("CurrentComplexity", 0));
        SceneManager.LoadSceneAsync("MainScene");

    }
}
