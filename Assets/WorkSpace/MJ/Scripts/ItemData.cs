using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public Sprite icon; //인벤토리에 표시될 아이템의 아이콘
    public ITEMTYPE itemType; //아이템 사용 효과명
}
