using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIChoseLVL : MonoBehaviour
{
    #region scoreText

    [SerializeField] private Text animalText;
    [SerializeField] private Text techText;
    [SerializeField] private Text artText;
    [SerializeField] private Text architectureText;
    #endregion
    private void Awake()
    {
        //PlayerPrefs.DeleteAll();
        int anim = PlayerPrefs.GetInt("Animals", 0);
        int tech = PlayerPrefs.GetInt("Tech", 0);
        int art = PlayerPrefs.GetInt("Arts", 0);
        int arch = PlayerPrefs.GetInt("Architecture", 0);
        int all = anim + tech + art + arch;
        PlayerPrefs.SetInt("Stars", all);

        animalText.text = $"{anim}/24 ★";
        techText.text = $"{tech}/24 ★";
        artText.text = $"{art}/24 ★";
        architectureText.text = $"{arch}/24 ★";
    }
    public void Home()
    {
        SceneManager.LoadSceneAsync("Start");
    }
    public void GoToAnimals()
    {
        SceneManager.LoadSceneAsync("Animals");
    }
    public void GoToTech()
    {
        SceneManager.LoadSceneAsync("Tech");
    }
    public void GoToArchitecture()
    {
        SceneManager.LoadSceneAsync("Architecture");
    }
    public void GoToArts()
    {
        SceneManager.LoadSceneAsync("Architecture");
    }
}
