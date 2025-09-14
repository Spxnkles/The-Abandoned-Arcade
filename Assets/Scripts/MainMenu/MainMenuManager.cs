using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public Animator creditsAnimator;
    public AudioSource click;

    public void startGame()
    {
        playClick();
        SceneManager.LoadScene("Home");
    }

    public void showSettings()
    {

    }

    public void showCredits()
    {
        if (creditsAnimator != null)
        {
            creditsAnimator.SetBool("isOpen", true);
            playClick();
        }
    }

    public void hideCredits()
    {
        if (creditsAnimator != null)
        {
            creditsAnimator.SetBool("isOpen", false);
            playClick();
        }
    }

    public void exitGame()
    {
        playClick();
        Application.Quit();
    }

    public void playClick()
    {
        if (click.time == 0 || click.time > 0.5) click.Play();
    }
}
