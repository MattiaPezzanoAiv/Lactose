using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;
using GooglePlayGames.BasicApi;

public class AndroidSocialImplementation : ISocialImplementation
{

    const string androidLeaderBoardKey = "CgkIq96do8IdEAIQDA";

    public void Authenticate(System.Action<bool> callback)
    {
        MLog.Info("Android authentication process started");
        Social.localUser.Authenticate((success) =>
        {

            MLog.Info("Android authentication completed");

            if (success)
            {
                MLog.Info("Android authentication success");
                MLog.Info("Android username --> " + Social.localUser.userName);
                MLog.Info("Android starting achievments sync");
                SyncAchievements();
            }
            else
            {
                MLog.Info("Android authentication failed");
            }

            if (callback != null)
                callback.Invoke(success);
        });
    }

    public void Initialize(bool enableDebug)
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        // recommended for debugging:
        PlayGamesPlatform.DebugLogEnabled = true;
        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();
    }

    public void ShowNativeAchievPage()
    {
        if (!Social.localUser.authenticated)
        {
            this.Authenticate((success) => { if (success) Social.ShowAchievementsUI(); });
            return;
        }
        Social.ShowAchievementsUI();
    }
    public bool IsAuthenticated()
    {
        return Social.localUser.authenticated;
    }
    public void SyncAchievements()
    {
        Social.LoadAchievements((achievs) =>
        {
            MLog.Info("Iterating android achievements");

            var localAchievements = AchievmentsManager.Instance.GetAchievementsBySocialId();
            foreach (var achi in achievs)
            {
                MLog.Info("     achi: " + achi.id + " completed -> " + achi.completed);
                if (achi.completed) //social completed, unlock local
                {
                    if (!localAchievements[achi.id].unlocked)
                    {
                        AchievmentsManager.Instance.Unlock(localAchievements[achi.id].GetType(), false, false);
                        MLog.Info("Sync achiev id " + achi.id + " name: " + localAchievements[achi.id]);
                    }
                }
                else
                {
                    //social not completed, check if local completed, unlock social
                    bool localCompleted = localAchievements[achi.id].unlocked;
                    if (localCompleted)
                    {
                        //unlock social
                        Social.ReportProgress(achi.id, 100.0f, (success) => { });
                    }
                }
            }
        });
    }

    public void UnlockAchievement(string key)
    {
        Social.ReportProgress(key, 100.0, (success) =>
        {
            if (success)
            {
                MLog.Info("Achievments reported succesfully!");
            }
            else
            {
                MLog.Info("Achievments report failed!");
            }
        });
    }

    public void AddScoreToLeaderboard(int score)
    {
        Social.ReportScore(score, androidLeaderBoardKey, (success) =>
        {
            if (success)
            {
                MLog.Info("Score reported succesfully: " + score);
            }
            else
            {
                MLog.Info("Failed to report score on leaderboard");
            }
        });
    }

    public void ShowNativeLeaderboardPage()
    {
        if (!Social.localUser.authenticated)
        {
            this.Authenticate((success) => { if (success) Social.ShowLeaderboardUI(); });
            return;
        }
        Social.ShowLeaderboardUI();
    }
}
