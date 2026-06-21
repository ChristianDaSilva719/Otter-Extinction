using UnityEngine;

public class AnimationFunctions : MonoBehaviour
{
    public Animator anim;            
    public GameObject SleepingOtters;
    public GameObject BlackScreen;
    public Animator BlackScreenAnim; 
    public GameManager Manager;

    public void SleepingStart()
    {
        SleepingOtters.SetActive(true);
    }

    public void BlackScreenBack()
    {
        BlackScreen.SetActive(true);
        BlackScreenAnim.Play("BlackScreenLeave", 0, 0f); 
    }

    public void BlackScreenAway()
    {
        BlackScreenAnim.Play("BlackScreen", 0, 0f);
        anim.SetBool("OnOrOff", true);
        
    }

    public void HideBlackScreen()
    {
        BlackScreen.SetActive(false);
    }

    public void SleepingStop()
    {
        SleepingOtters.SetActive(false);
        Manager.EndDay();
    }
}