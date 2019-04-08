using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameOverPage : MonoBehaviour
{
    public Text tfNewRecord;
    public Image bg;
    public Sprite bgLose, bgNewRecord;
    public GameObject loseGo, newRecordGo;

    public void SetupNewRecord(int score)
    {
        tfNewRecord.text = "New Record! " + score;
        bg.sprite = bgNewRecord;
        loseGo.SetActive(false);
        newRecordGo.SetActive(true);
    }
    public void SetupLose()
    {
        bg.sprite = bgLose;
        loseGo.SetActive(true);
        newRecordGo.SetActive(false);
    }

    public void OnBackButton()
    {
        GameManager.Instance.NotGameOver();
        UIManager.Instance.Show("UIMainPage");
    }
}
