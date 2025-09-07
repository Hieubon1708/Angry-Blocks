using UnityEngine;

public class Test : MonoBehaviour
{
    // Các tham số cho hình hộp va chạm của bạn
    public Vector3 boxCenterOffset = Vector3.zero; // Độ lệch tâm của hộp so với GameObject
    public Vector3 boxHalfExtents = new Vector3(2f, 2f, 2f); // Nửa kích thước của hộp
    public Quaternion boxOrientation = Quaternion.identity; // Góc xoay của hộp

    // Mảng để lưu trữ kết quả va chạm (cần cho hàm OverlapBoxNonAlloc)
    private Collider[] hitColliders = new Collider[10];

    // Lớp mà bạn muốn kiểm tra va chạm (ví dụ: "Enemy")
    public LayerMask targetLayer;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 actualBoxCenter = transform.position + boxCenterOffset;

        Matrix4x4 originalMatrix = Gizmos.matrix;

        Gizmos.matrix = Matrix4x4.TRS(actualBoxCenter, boxOrientation, Vector3.one);

        Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2);

        Gizmos.matrix = originalMatrix;
    }

    void Update()
    {
        Vector3 actualBoxCenter = transform.position + boxCenterOffset;
        int numColliders = Physics.OverlapBoxNonAlloc(actualBoxCenter, boxHalfExtents, hitColliders, boxOrientation, targetLayer);

        if (numColliders > 0)
        {
            for (int i = 0; i < numColliders; i++)
            {
                Debug.Log("Found: " + hitColliders[i].name + " at " + Time.time);
            }
        }
    }
}