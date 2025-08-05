using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MoveObject : MonoBehaviour
{
    public GameObject objectToMove; // 需要移动的物体
    public Vector3 targetPosition; // 需要移动到的目标位置
    public float speed = 1f; // 物体移动的速度

    public void StartMoving()
    {
        StartCoroutine(SmoothMove(objectToMove.transform, targetPosition, speed));
    }

    // 协程实现平滑移动
    IEnumerator SmoothMove(Transform objectTransform, Vector3 target, float speed)
    {
        while (Vector3.Distance(objectTransform.position, target) > 0.01f)
        {
            objectTransform.position = Vector3.Lerp(objectTransform.position, target, speed * Time.deltaTime);
            yield return null;
        }
    }
}
