using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

namespace MJ.Item.ItemBox
{
    public class ItemBox : MonoBehaviour
    {
        [SerializeField] ItemData[] items; //아이템 전체 목록

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                int idx = Random.Range(0, items.Length);
                other.GetComponent<Inventory>().GetItem(items[idx]);

                Destroy(gameObject);
            }
        }
    }
}
