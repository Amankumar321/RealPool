using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Canvas UI;
    public Button playButton8;
    public Button playButton9;


    // Start is called before the first frame update
    void Start()
    {
        playButton8.onClick.AddListener(Play8Ball);
        playButton9.onClick.AddListener(Play9Ball);
    }

    public void Play8Ball()
    {
        PlayerPrefs.SetString("GameType", "8Ball");
        SceneManager.LoadScene("SampleScene");  
        //SceneManager.U);
    }
    public void Play9Ball()
    {
        PlayerPrefs.SetString("GameType", "9Ball");
        SceneManager.LoadScene("SampleScene");  
        //SceneManager.U);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
