using DG.Tweening;
using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class  UIAnimationInfo
{
    public RectTransform UI;
    public Vector3 start;
    public Vector3 end;
    public float duration;
    public Ease type;
}

public class SkillTextAnimation : MonoBehaviour
{
    [SerializeField] private UIAnimationInfo info;

    private void OnEnable()
    {
        Sequence seq = DOTween.Sequence();

        ((RectTransform)transform).localPosition = info.start;
        gameObject.GetComponent<TMP_Text>().alpha = 1f;

        seq.Append(info.UI.DOAnchorPos(info.end, info.duration).SetEase(info.type))
            .Join(info.UI.GetComponent<TMP_Text>().DOFade(0.0f, info.duration).SetEase(info.type)).OnComplete(() => { gameObject.SetActive(false); });

    }
}
