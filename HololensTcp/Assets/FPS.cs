using UnityEngine;

public class FPS : MonoBehaviour
{
    public TextMesh fpsText;  // 将 TextMesh 对象拖拽到此字段
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (fpsText == null)
        {
            Debug.LogError("TextMesh component is not assigned.");
            return;
        }
    }

    void Update()
    {
        if (fpsText != null)
        {
            float fps = 1.0f / Time.deltaTime;
            fpsText.text = "FPS: " + Mathf.Ceil(fps).ToString();

            // 计算相对于相机的位置，使其出现在右上角
            Vector3 offset = new Vector3(0.5f, 0.5f, 2.0f); // 调整这个偏移量以控制TextMesh的位置
            fpsText.transform.position = mainCamera.transform.position + mainCamera.transform.rotation * offset;

            // 使TextMesh始终面向相机
            fpsText.transform.LookAt(mainCamera.transform);
            fpsText.transform.Rotate(0, 180, 0); // 旋转180度以便文本正确显示
        }
    }
}