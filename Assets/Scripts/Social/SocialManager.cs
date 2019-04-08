using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialManager : MonoBehaviour
{
    public static ISocialImplementation Implementation { get; private set; }

    private void Awake()
    {
#if UNITY_ANDROID
        Implementation = new AndroidSocialImplementation();
#else
        //implement ios
#endif

        //initialize system
        Implementation.Initialize(true);

        Implementation.Authenticate(null);
    }


    public void OnSocialAchievButtonPressed()
    {
        Implementation.ShowNativeAchievPage();
    }
    public void OnSocialLeaderboardButtonPressed()
    {
        Implementation.ShowNativeLeaderboardPage();
    }
}
