using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.EventSystems;
using static NewBehaviourScript2;

public class ChangeColorOnClick : MonoBehaviour, IMixedRealityPointerHandler
{
    // ������һ���ű���ʵ��
    private NewBehaviourScript2 NewBehaviourScript2;

    // ��ָ����ʱ���õķ���
    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        ChangeColor(); // �ı���ɫ
    }

    // ��ָ�밴��ʱ���õķ�����δʵ�֣�
    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        //ChangeColor();
    }

    // ��ָ���϶�ʱ���õķ�����δʵ�֣�
    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
    }

    // ��ָ��̧��ʱ���õķ�����δʵ�֣�
    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
    }

    // �ı���ɫ�ķ���
    void ChangeColor()
    {
        // ��ȡ��һ���ű���ʵ��
        NewBehaviourScript2 = GameObject.Find("GameObject").GetComponent<NewBehaviourScript2>();

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
        NewBehaviourScript2.Tagging(xyz_l, r_l, qiu_size_l);

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