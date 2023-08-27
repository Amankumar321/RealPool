using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Canvas UI;

    void Start()
    {
        
    }

    public void Play8Ball()
    {
        Debug.Log(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("SampleScene");   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
