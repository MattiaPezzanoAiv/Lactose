using System;

public class Achievment
{
    public string name;
    public string socialKey;
    public string lockedLabel;
    public bool unlocked;
    public string message;
}




#region BEST_SCORES
public class Record10k : Achievment
{
    public Record10k()
    {
        socialKey = "CgkIq96do8IdEAIQAQ";
        message = "New Record! 10k";
        lockedLabel = "Reach 10k";
        name = "Suckling!";
    }
}
public class Record25k : Achievment
{
    public Record25k()
    {
        socialKey = "CgkIq96do8IdEAIQAg";
        message = "New Record! 25k";
        lockedLabel = "Reach 25k";
        name = "Newbie";
    }
}
public class Record50k : Achievment
{
    public Record50k()
    {
        socialKey = "CgkIq96do8IdEAIQAw";
        message = "New Record! 50k";
        lockedLabel = "Reach 50k";
        name = "Novice";
    }
}
#endregion

#region PROGRESSIVE_COLLECT
public class Collect100kPoints : Achievment
{
    public Collect100kPoints()
    {
        socialKey = "CgkIq96do8IdEAIQBA";
        message = "Collected 100k points!";
        lockedLabel = "Collect 100k points";
        name = "Collector";
    }
}
public class Collect200kPoints : Achievment
{
    public Collect200kPoints()
    {
        socialKey = "CgkIq96do8IdEAIQBQ";
        message = "200k points!";
        lockedLabel = "Collect 200k points";
        name = "Amateur Collector";
    }
}
public class Collect500kPoints : Achievment
{
    public Collect500kPoints()
    {
        socialKey = "CgkIq96do8IdEAIQBg";
        message = "500k points!";
        lockedLabel = "Collect 500k points";
        name = "Passionate Collector";
    }
}
#endregion

#region GENERIC
public class FirstMatchCompleted : Achievment {
    public FirstMatchCompleted()
    {
        socialKey = "CgkIq96do8IdEAIQBw";
        message = "First Match Completed";
        lockedLabel = "Complete 1 match";
        name = "First Blood";
    }
}
public class CreditsWatched : Achievment
{
    public CreditsWatched()
    {
        socialKey = "CgkIq96do8IdEAIQCA";
        message = "Credits Watched!";
        lockedLabel = "Watch credits until the end";
        name = "Spy";
    }
}
public class TrailEquipped : Achievment
{
    public TrailEquipped()
    {
        socialKey = "CgkIq96do8IdEAIQCQ";
        message = "New Trail Equipped!";
        lockedLabel = "Equip 1 new trail";
        name = "Trail Explorer";
    }
}
public class AllTrailsUnlocked : Achievment
{
    public AllTrailsUnlocked()
    {
        socialKey = "CgkIq96do8IdEAIQCg";
        message = "All trails unlocked!";
        lockedLabel = "Unlock all trails";
        name = "Trail Master";
    }
}
public class TutorialCompleted : Achievment
{
    public TutorialCompleted()
    {
        socialKey = "CgkIq96do8IdEAIQCw";
        message = "Tutorial completed!";
        lockedLabel = "Complete the tutorial";
        name = "Not a n00b";
    }
}
#endregion