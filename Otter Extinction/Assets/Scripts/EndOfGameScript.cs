using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfGameScript : MonoBehaviour
{
    public GameObject mainmenuButton;
    private void Start()
    {
        mainmenuButton.SetActive(false);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("Main Screen");
    }
    public void CanLeave()
    {
        mainmenuButton.SetActive(true);
    }
}
