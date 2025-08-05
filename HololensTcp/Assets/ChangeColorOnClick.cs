using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.EventSystems;
using static NewBehaviourScript2;

public class ChangeColorOnClick : MonoBehaviour, IMixedRealityPointerHandler
{
    // 引用另一个脚本的实例
    private NewBehaviourScript2 NewBehaviourScript2;

    // 当指针点击时调用的方法
    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        ChangeColor(); // 改变颜色
    }

    // 当指针按下时调用的方法（未实现）
    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        //ChangeColor();
    }

    // 当指针拖动时调用的方法（未实现）
    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
    }

    // 当指针抬起时调用的方法（未实现）
    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
    }

    // 改变颜色的方法
    void ChangeColor()
    {
        // 获取另一个脚本的实例
        NewBehaviourScript2 = GameObject.Find("GameObject").GetComponent<NewBehaviourScript2>();

        // 获取当前对象的Renderer组件
        Renderer renderer = gameObject.GetComponent<Renderer>();

        // 从另一个脚本中获取一个随机颜色
        Color newColor = NewBehaviourScript2.acolor[NewBehaviourScript2.colornum];

        // 将新颜色应用到Renderer组件
        renderer.material.color = newColor;

        // 获取对象的位置、半径和大小
        Vector3 xyz_l = Printlocation();
        float r_l = this.GetComponent<Transform>().localScale.x;
        Vector3 qiu_size_l = this.GetComponent<Transform>().localScale;

        // 调用另一个脚本中的Tagging方法
        NewBehaviourScript2.Tagging(xyz_l, r_l, qiu_size_l);

        // 打印调试信息
        Debug.Log(xyz_l);
        Debug.Log(r_l);
        Debug.Log(qiu_size_l);
    }

    // 计算并返回相对位置的方法
    private Vector3 Printlocation()
    {
        // 获取空对象和当前对象的Transform组件
        Transform targetTransform = GameObject.Find("EmptyObject").GetComponent<Transform>();
        Transform referenceTransform = this.GetComponent<Transform>();

        // 计算两个对象之间的距离
        Vector3 distance = referenceTransform.transform.position - targetTransform.transform.position;

        // 计算相对位置
        Vector3 relativePosition = Vector3.zero;
        relativePosition.x = Vector3.Dot(distance, targetTransform.transform.right.normalized);
        relativePosition.z = Vector3.Dot(distance, targetTransform.transform.up.normalized);
        relativePosition.y = Vector3.Dot(distance, targetTransform.transform.forward.normalized);

        // 返回相对位置
        return relativePosition;
    }
}