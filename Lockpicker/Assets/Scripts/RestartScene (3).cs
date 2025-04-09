using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartScene : MonoBehaviour
{
    // Start is called before the first frame update
    public string startScene;

    public string endScene;

    public void restartGame()
    {
        SceneManager.LoadScene($"{startScene}");
    }

    public void NextScene()
    {
        SceneManager.LoadScene($"{endScene}");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            restartGame();
        }
    }
}
