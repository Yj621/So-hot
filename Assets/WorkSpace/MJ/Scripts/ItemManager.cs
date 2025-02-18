using UnityEngine;

public enum ITEMTYPE
{
    GaugeStop,
    NoDie,
    UnlimitRun

}
public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    private void Awake()
    {
        Instance = this;
    }


    public void GaugeStop()
    {
        //TO-DO: 뜨거움 게이지 멈추기 로직 추가
    }

    public void NoDie()
    {
        //TO-DO: 죽음 1회 면제 로직 추가
    }

    public void UnlimitRun()
    {
        //TO-DO: 일정 시간동안 달리기 무제한 로직 추가
    }

}
