using UnityEngine;
using UnityEngine.SceneManagement;

public class UIGame : MonoBehaviour
{
    public void GoHome()
    {
        SceneManager.LoadSceneAsync("Start");
    }
}
