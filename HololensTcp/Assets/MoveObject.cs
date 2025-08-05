using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MoveObject : MonoBehaviour
{
    public GameObject objectToMove; // ��Ҫ�ƶ�������
    public Vector3 targetPosition; // ��Ҫ�ƶ�����Ŀ��λ��
    public float speed = 1f; // �����ƶ����ٶ�

    public void StartMoving()
    {
        StartCoroutine(SmoothMove(objectToMove.transform, targetPosition, speed));
    }

    // Э��ʵ��ƽ���ƶ�
    IEnumerator SmoothMove(Transform objectTransform, Vector3 target, float speed)
    {
        while (Vector3.Distance(objectTransform.position, target) > 0.01f)
        {
            objectTransform.position = Vector3.Lerp(objectTransform.position, target, speed * Time.deltaTime);
            yield return null;
        }
    }
}
