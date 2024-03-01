using UnityEngine;

public class lvl7 : MonoBehaviour
{
    private const bool allowCarrierDataNetwork = false;
    private const string pingAddress = "8.8.8.8";
    private const float waitingTime = 2.0f;
    public Animator anim;
    bool f1, f2, f3, f4, f5; 

    private Ping ping;
    private float pingStartTime;

    public void Start()
    {
        InvokeRepeating("CheckInternetConnection", 0f, 3f);
    }

    private void CheckInternetConnection()
    {
        bool internetPossiblyAvailable;
        switch (Application.internetReachability)
        {
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                internetPossiblyAvailable = true;
                break;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                internetPossiblyAvailable = allowCarrierDataNetwork;
                break;
            default:
                internetPossiblyAvailable = false;
                break;
        }
        if (!internetPossiblyAvailable)
        {
            InternetIsNotAvailable();
            return;
        }

        ping = new Ping(pingAddress);
        pingStartTime = Time.time;
    }

    public void Update()
    {
        if (ping != null)
        {
            bool stopCheck = true;
            if (ping.isDone)
            {
                if (ping.time >= 0)
                    InternetAvailable();
                else
                    InternetIsNotAvailable();
            }
            else if (Time.time - pingStartTime < waitingTime)
                stopCheck = false;
            else
                InternetIsNotAvailable();
            if (stopCheck)
                ping = null;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3)) f1 = true;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            f2 = true;
            if (f3)
            {
                f4 = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) f3 = true;
        if (Input.GetKeyDown(KeyCode.Alpha6)) f5 = true;

        if (f1 && f2 && f3 && f4 && f5)
        {
            anim.SetBool("Close", true);
        }
    }

    private void InternetIsNotAvailable()
    {
        anim.SetBool("Close", true);
        Debug.Log("Internet is not available!");
    }

    private void InternetAvailable()
    {
        Debug.Log("Internet is available!");
    }
}