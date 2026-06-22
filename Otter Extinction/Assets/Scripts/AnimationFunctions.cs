using UnityEngine;

public class AnimationFunctions : MonoBehaviour
{
    public Animator anim;
    public GameObject SleepingOtters;
    public GameObject BlackScreen;
    public Animator BlackScreenAnim;
    public GameManager Manager;

    [Tooltip("The last otter GameObject — activated before EndDay() so its Animator is ready on day 4.")]
    public GameObject lastOtterObject;
    public Animator lastOtterAnimator;
    public string lastOtterAnimationState;

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
        if (Manager.currentDay == 3 && lastOtterObject != null)
        {
            lastOtterObject.SetActive(true);
            if (lastOtterAnimator != null && !string.IsNullOrEmpty(lastOtterAnimationState))
                lastOtterAnimator.Play(lastOtterAnimationState, 0, 0f);
        }

        Manager.EndDay();
    }
}