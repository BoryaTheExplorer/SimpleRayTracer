using UnityEngine;

public class CameraTest : MonoBehaviour
{
    [SerializeField] private Vector2 debugPointCount;
    private void CameraRayTest()
    {
        Camera cam = Camera.main;
        Transform camT = cam.transform;

        float planeHeight = cam.nearClipPlane * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * 2f;
        float planeWidth = planeHeight * cam.aspect;

        Vector3 bottomLeftLocal = new Vector3(-planeWidth / 2, -planeHeight / 2, cam.nearClipPlane);

        for (int i = 0; i < debugPointCount.x; i++)
        {
            for (int j = 0; j < debugPointCount.y; j++)
            {
                float tx = i / (debugPointCount.x - 1);
                float ty = j / (debugPointCount.y - 1);

                Vector3 pointLocal = bottomLeftLocal + new Vector3(planeWidth * tx, planeHeight * ty);
                Vector3 point = camT.position + camT.right * pointLocal.x + camT.up * pointLocal.y + camT.forward * pointLocal.z;

                Gizmos.DrawSphere(point, 0.05f);
                Gizmos.DrawLine(camT.position, point);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        CameraRayTest();
    }
}
