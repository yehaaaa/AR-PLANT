using UnityEngine;

public class FPS : MonoBehaviour
{
    public TextMesh fpsText;  // �� TextMesh ������ק�����ֶ�
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

            // ��������������λ�ã�ʹ����������Ͻ�
            Vector3 offset = new Vector3(0.5f, 0.5f, 2.0f); // �������ƫ�����Կ���TextMesh��λ��
            fpsText.transform.position = mainCamera.transform.position + mainCamera.transform.rotation * offset;

            // ʹTextMeshʼ���������
            fpsText.transform.LookAt(mainCamera.transform);
            fpsText.transform.Rotate(0, 180, 0); // ��ת180���Ա��ı���ȷ��ʾ
        }
    }
}