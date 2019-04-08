using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISocialImplementation 
{

    void Initialize(bool enableDebug);
    void Authenticate(System.Action<bool> callback);

    void UnlockAchievement(string key);
    void SyncAchievements();
    void AddScoreToLeaderboard(int score);
    bool IsAuthenticated();

    void ShowNativeAchievPage();
    void ShowNativeLeaderboardPage();
}
