using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public void Button()
    {
        SceneManager.LoadScene("Level1");
    }
}