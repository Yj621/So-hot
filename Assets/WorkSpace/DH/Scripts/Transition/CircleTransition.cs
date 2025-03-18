using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CircleTransition : MonoBehaviour
{
    private Material material;
    [SerializeField] private float duration;
    [SerializeField] private Ease type;
    private void Awake()
    {
        material = GetComponent<Image>().material;
    }

    //밝아지는거
    public void FadeIn()
    {
        Vector2 curCircleSize = Vector2.zero;
        Vector2 endCircleSize = new Vector2(2.5f, 2.0f);

        DOTween.To(() => curCircleSize, x =>
        {
            curCircleSize = x;
            material.SetVector("_Circle_Size", curCircleSize);
        }, endCircleSize, duration).SetEase(type).OnComplete(() => { material.SetVector("_Circle_Size", Vector2.zero); gameObject.SetActive(false); });
    }

    //어두워지는거
    public void FadeOut(Action action)
    {
        Vector2 curCircleSize = new Vector2(2.5f, 2.0f);
        Vector2 endCircleSize = Vector2.zero;

        //어두워질때는 씬 이동이라 SetActive안하는게 맞음
        DOTween.To(() => curCircleSize, x =>
        {
            curCircleSize = x;
            material.SetVector("_Circle_Size", curCircleSize);
        }, endCircleSize, duration).SetEase(type).OnComplete(() => { action.Invoke(); });
    }
}
