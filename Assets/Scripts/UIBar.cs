using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBar : MonoBehaviour
{
    private float maxValue, maxValuePixel;

    [SerializeField]
    private Image bg, fg, border;
    [SerializeField]
    private Color bgColor, fgColorStart, fgColorEnd;
    [SerializeField]
    private Vector2 bgBorder;
    [SerializeField]
    Animation shakeAnimation;

    public float FORCED_VALUE = 10;
    public float FORCED_MAX = 100;
    public float FORCED_MAX_PIXELS = 200;
    public bool move;


    private void Start()
    {
        shakeAnimation.clip.legacy = true;
        shakeAnimation.clip.wrapMode = WrapMode.Once;
    }

    public void Shake()
    {
        shakeAnimation.Rewind();
        shakeAnimation.Play();
    }

    /// <summary>
    /// Values are not normalized
    /// </summary>
    /// <param name="minVal"></param>
    /// <param name="maxVal"></param>
    /// <param name="currentVal"></param>
    public void Setup(float maxVal, float currentVal, float maxValPixel)
    {
        //cache values
        maxValue = maxVal;
        maxValuePixel = maxValPixel;

        //setup bg
        bg.rectTransform.pivot = new Vector2(0, 0.5f);
        bg.rectTransform.sizeDelta = new Vector2(maxValPixel, bg.rectTransform.sizeDelta.y);
        bg.color = bgColor;

        //setup fg
        fg.rectTransform.pivot = new Vector2(0, 0.5f);
        fg.rectTransform.sizeDelta = new Vector2(maxValPixel * (currentVal / maxVal), fg.rectTransform.sizeDelta.y);
        //fg.rectTransform.anchoredPosition += new Vector2(bgBorder.x / 2f, 0);
        fg.color = fgColorStart;

        //setup border
        border.rectTransform.sizeDelta = new Vector2(maxValuePixel, bg.rectTransform.sizeDelta.y) + bgBorder;
    }
    public void Refresh(float currentVal, bool allowShake)
    {
        if (currentVal > maxValue)
            currentVal = maxValue;

        if (allowShake)
            Shake();

        fg.rectTransform.sizeDelta = new Vector2(maxValuePixel * (currentVal / maxValue), fg.rectTransform.sizeDelta.y);
        fg.color = Color.Lerp(fgColorStart, fgColorEnd, currentVal / maxValue);
    }

#if UNITY_EDITOR
    //private void Start()
    //{
    //    if (move)
    //        Setup(FORCED_MAX, FORCED_VALUE, FORCED_MAX_PIXELS);
    //}
    //private void Update()
    //{

    //    if (move)
    //    {
    //        FORCED_VALUE = Mathf.Abs(Mathf.Sin(Time.time)) * 30f;
    //    }
    //    Refresh(FORCED_VALUE);

    //}
#endif
}
