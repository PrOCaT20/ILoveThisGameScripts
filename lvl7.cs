using UnityEngine;

public class lvl7 : MonoBehaviour
{
    private const bool allowCarrierDataNetwork = false; 
    private const string pingAddress = "8.8.8.8"; // adreça de www.google.com
    private const float waitingTime = 2.0f;
    public Animator anim; // component per obrir porta/ignora

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
    }

    private void InternetIsNotAvailable()
    {
        anim.SetBool("Close", true); // obrir porta en cas que no arriba a pinguejar www.google.com
        Debug.Log("Internet is not available!");
    }

    private void InternetAvailable()
    {
        Debug.Log("Internet is available!");
    }
}
