using UnityEngine;
using UnityEngine.UI;

public class WakeBar : MonoBehaviour
{
    public Image fullBar;

    private float maxTime = 60f;
    private float timer = 0f;
    private float interval = 1f; //1s interval

    public PlayerController playerController;

    static public WakeBar S; 

    private void Awake()
    {
        S = this; //setting Singleton
    }

    void Update()
    {
        if (playerController.Wakefullness)
        {
            //set timer
            timer += Time.deltaTime;

            //after one interval, reset timer
            if (timer >= interval)
            {
                timer -= interval; 

                //lower time left
                if (fullBar.fillAmount > 0)
                {
                    fullBar.fillAmount -= 1f / maxTime;
                }

                else
                {
                    //wakefullness bar depleted, now asleep
                    fullBar.fillAmount = 0;
                    playerController.Wakefullness = false;
                }
            }
        }        
    }

    //adding points to wakefullness bar
    public void WakeUp(float points)
    {

        fullBar.fillAmount += points;

        //if max exceeded
        if (fullBar.fillAmount >= 1.0f)
        {
            //set to max
            fullBar.fillAmount = 1.0f;

            //if not awake before but now maxed out the bar
            if(!playerController.Wakefullness)
            {
                //wake up
                playerController.Wakefullness = true;
            }
        }
    }
}
