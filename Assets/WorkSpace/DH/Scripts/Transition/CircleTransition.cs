using DG.Tweening;
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
        float curCircleSize = 0.0f;

        DOTween.To(() => curCircleSize, x =>
        {
            curCircleSize = x;
            material.SetFloat("_Circle_Size", curCircleSize);
        }, 2.0f, duration).SetEase(type).OnComplete(() => { gameObject.SetActive(false); });
    }

    //어두워지는거
    public void FadeOut()
    {
        float curCircleSize = 2.0f;

        //어두워질때는 씬 이동이라 SetActive안하는게 맞음
        DOTween.To(() => curCircleSize, x =>
        {
            curCircleSize = x;
            material.SetFloat("_Circle_Size", curCircleSize);
        }, 0.0f, duration).SetEase(type);
    }
}
