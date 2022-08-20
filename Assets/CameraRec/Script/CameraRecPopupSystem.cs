using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraRecPopupSystem : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject popUpBox;
    public Animator animator;
    public float delayPop = 0.25f;

    public void Pop()
    {
        StartCoroutine(PopUp());
    }

    public void Down()
    {
        StartCoroutine(PopDown());
    }

    private IEnumerator PopUp()
    {
        yield return new WaitForSeconds(delayPop);
        animator.SetTrigger("pop");
    }

    private IEnumerator PopUp_trigger()
    {
        yield return new WaitForSeconds(delayPop);
        animator.SetTrigger("pop");
        StartCoroutine(Countdown());
    }

    private IEnumerator PopDown()
    {
        yield return new WaitForSeconds(delayPop);
        animator.ResetTrigger("pop");
    }

    private IEnumerator Countdown()
    {
        yield return new WaitForSeconds(0.2f);
        animator.ResetTrigger("pop");
    }
}
