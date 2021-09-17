using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIStart : MonoBehaviour
{
    public AudioMixerGroup Mixer;
    public Image Master;
    public GameObject musicSource;

    [SerializeField] private Sprite OffButtonSprite;
    [SerializeField] private Sprite OnButtonSprite;
    public void Exit()
    {
        Application.Quit();
    }
    public void Play()
    {
        SceneManager.LoadSceneAsync("ChoseLVL");
    }
    void Start()
    {
        DontDestroyOnLoad(musicSource);
        if (FindObjectsOfType<AudioSource>().Length>1)
        {
            Destroy(musicSource);
        }
        StartMaster();
    }
    public void StartMaster()
    {
        if (PlayerPrefs.GetInt(key: "Master", defaultValue: 0) == 0)
        {
            Off();
        }
        else
        {
            On();
        }
    }
    public void Chouse()
    {
        if (PlayerPrefs.GetInt(key: "Master", defaultValue: 0) == 1)
        {
            Off();
        }
        else
        {
            On();
        }
    }
    private void On()
    {
        Mixer.audioMixer.SetFloat("Master", 0);
        Master.sprite = OnButtonSprite;
        PlayerPrefs.SetInt("Master", 1);
        PlayerPrefs.Save();
    }
    private void Off()
    {
        Mixer.audioMixer.SetFloat("Master", -80);
        Master.sprite = OffButtonSprite;
        PlayerPrefs.SetInt("Master", 0);
        PlayerPrefs.Save();
    }
}
