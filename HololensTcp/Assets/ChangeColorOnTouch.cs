using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.EventSystems;
using static NewBehaviourScript2;

public class ChangeColorOnTouch : MonoBehaviour, IMixedRealityTouchHandler
{
    private NewBehaviourScript2 newBehaviourScript2;

    private void Start()
    {
        
    }

    public void OnTouchCompleted(HandTrackingInputEventData eventData)
    {
        ChangeColor();
    }

    public void OnTouchStarted(HandTrackingInputEventData eventData)
    {
        // Do nothing
    }

    public void OnTouchUpdated(HandTrackingInputEventData eventData)
    {
        // Do nothing
    }

    public void ChangeColor()
    {
        // ��ȡ��һ���ű���ʵ��
        newBehaviourScript2 = GameObject.Find("GameObject").GetComponent<NewBehaviourScript2>();

        // ��ȡ��ǰ�����Renderer���
        Renderer renderer = gameObject.GetComponent<Renderer>();

        // ����һ���ű��л�ȡһ�������ɫ
        Color newColor = NewBehaviourScript2.acolor[NewBehaviourScript2.colornum];

        // ������ɫӦ�õ�Renderer���
        renderer.material.color = newColor;

        // ��ȡ�����λ�á��뾶�ʹ�С
        Vector3 xyz_l = Printlocation();
        float r_l = this.GetComponent<Transform>().localScale.x;
        Vector3 qiu_size_l = this.GetComponent<Transform>().localScale;

        // ������һ���ű��е�Tagging����
        newBehaviourScript2.Tagging(xyz_l, r_l, qiu_size_l);

        // ��ӡ������Ϣ
        Debug.Log(xyz_l);
        Debug.Log(r_l);
        Debug.Log(qiu_size_l);
    }

    // ���㲢�������λ�õķ���
    private Vector3 Printlocation()
    {
        // ��ȡ�ն���͵�ǰ�����Transform���
        Transform targetTransform = GameObject.Find("EmptyObject").GetComponent<Transform>();
        Transform referenceTransform = this.GetComponent<Transform>();

        // ������������֮��ľ���
        Vector3 distance = referenceTransform.transform.position - targetTransform.transform.position;

        // �������λ��
        Vector3 relativePosition = Vector3.zero;
        relativePosition.x = Vector3.Dot(distance, targetTransform.transform.right.normalized);
        relativePosition.z = Vector3.Dot(distance, targetTransform.transform.up.normalized);
        relativePosition.y = Vector3.Dot(distance, targetTransform.transform.forward.normalized);

        // �������λ��
        return relativePosition;
    }
}