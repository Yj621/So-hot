#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


[ExecuteInEditMode]
public class ObjectPlacement : MonoBehaviour
{
    //이 스크립트를 오브젝트에 붙이면 씬 뷰에서 이동할 때마다

    //자동으로 지면 위에 정렬
    void Update()
    {
        if (!Application.isPlaying) // 에디터 모드에서만 실행
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
            {
                transform.position = hit.point;
            }
        }
    }

}
