using UnityEngine;
using Vector3Extensions;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class OffAxisCamera : MonoBehaviour
{
    [SerializeField]
    private OffAxisWindow window;

    private Camera _cam;
    private Camera Cam
    {
        get
        {
            if (_cam == null)
                _cam = GetComponent<Camera>();
            return _cam;
        }
    }

    private void LateUpdate()
    {
        Cam.worldToCameraMatrix = CalcViewMatrix(Cam.transform.position, window.BottomLeftCorner, window.BottomRightCorner, window.TopLeftCorner);
        Cam.projectionMatrix = CalcProjectionMatrix(Cam.worldToCameraMatrix, window.BottomLeftCorner, window.BottomRightCorner, window.TopLeftCorner, Cam.nearClipPlane, Cam.farClipPlane);
        Cam.cullingMatrix = Cam.projectionMatrix * Cam.worldToCameraMatrix;
    }

    private static Matrix4x4 CalcViewMatrix(Vector3 cameraPos, Vector3 bottomLeftWindowCorner, Vector3 bottomRightWindowCorner, Vector3 topLeftWindowCorner)
    {
        // Calculate left-handed basis vectors of view
        var right = Vector3.Normalize(bottomRightWindowCorner - bottomLeftWindowCorner);
        var up = Vector3.Normalize(topLeftWindowCorner - bottomLeftWindowCorner);
        // Fail-safe
        if (right == Vector3.zero)
            right = Vector3.right;
        if (up == Vector3.zero)
            up = Vector3.up;
        var forward = Vector3.Cross(right, up); // points through the window        

        var rotation = Matrix4x4.identity;
        rotation[0, 0] = right.x;
        rotation[1, 0] = right.y;
        rotation[2, 0] = right.z;
        rotation[0, 1] = up.x;
        rotation[1, 1] = up.y;
        rotation[2, 1] = up.z;
        rotation[0, 2] = forward.x;
        rotation[1, 2] = forward.y;
        rotation[2, 2] = forward.z;

        var translation = Matrix4x4.identity;
        translation[0, 3] = cameraPos.x;
        translation[1, 3] = cameraPos.y;
        translation[2, 3] = cameraPos.z;

        // Combine
        var camToWorld = translation * rotation;

        var windowToCam = cameraPos - bottomLeftWindowCorner;
        // If the forward vector is pointing from the window towards the camera
        bool IsWindowFacingCam = Vector3.Dot(forward, windowToCam) > 0f;
        if (IsWindowFacingCam)
        {
            // Rotate view 180° around y to face window
            camToWorld.SetColumn(0, -camToWorld.GetColumn(0));
            camToWorld.SetColumn(2, -camToWorld.GetColumn(2));
        }
        
        // The above matrix describes how to transform points from camera space to world space.
        // For example: camToWorld * (0, 0, 0) = cameraPos, and camToWorld * (0, 0, 1) = forward
        // The view matrix should do the opposite, that is: viewMatrix * cameraPos = (0, 0, 0) and
        // viewMatrix * forward = (0, 0, 1)
        var viewMatrix = camToWorld.inverse;
        // The third row is negated to convert from Unity world space, where forward is positive Z,
        // to Unity camera space, where forward is negative Z
        viewMatrix.SetRow(2, -viewMatrix.GetRow(2));
        return viewMatrix;
    }

    private static Matrix4x4 CalcProjectionMatrix(Matrix4x4 viewMatrix, Vector3 bottomLeftWindowCorner, Vector3 bottomRightWindowCorner, Vector3 topLeftWindowCorner, float near, float far)
    {
        var bottomLeft = viewMatrix * bottomLeftWindowCorner.AddWCoordinate(1f);
        var bottomRight = viewMatrix * bottomRightWindowCorner.AddWCoordinate(1f);
        var topLeft = viewMatrix * topLeftWindowCorner.AddWCoordinate(1f);
        var windowDist = Mathf.Abs(bottomLeft.z); // All corners should have the same z coordinate in camera space
        windowDist = Mathf.Max(windowDist, 10e-5f); // Don't divide by zero
        var left = Mathf.Min(bottomLeft.x, bottomRight.x) / windowDist * near;
        var right = Mathf.Max(bottomLeft.x, bottomRight.x) / windowDist * near;
        var bottom = Mathf.Min(bottomLeft.y, topLeft.y) / windowDist * near;
        var top = Mathf.Max(bottomLeft.y, topLeft.y) / windowDist * near;
        // Fail-safe
        if (Mathf.Approximately(left, right))
            right += 10e-5f;
        if (Mathf.Approximately(bottom, top))
            top += 10e-5f;
        var projection = Matrix4x4.Frustum(left, right, bottom, top, near, far);
        return projection;
    }
}