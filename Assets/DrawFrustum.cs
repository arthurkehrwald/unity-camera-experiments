using UnityEngine;
using Vector3Extensions;

[RequireComponent(typeof(Camera))]
public class DrawFrustum : MonoBehaviour
{
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

    private void OnDrawGizmos()
    {
        // l = left, r = right
        // b = bottom, t = top
        // n = near, f = far
        var lbn = NdcToWorld(new(-1, -1, -1), Cam);
        var rbn = NdcToWorld(new(1, -1, -1), Cam);
        var ltn = NdcToWorld(new(-1, 1, -1), Cam);
        var rtn = NdcToWorld(new(1, 1, -1), Cam);
        var lbf = NdcToWorld(new(-1, -1, 1), Cam);
        var rbf = NdcToWorld(new(1, -1, 1), Cam);
        var ltf = NdcToWorld(new(-1, 1, 1), Cam);
        var rtf = NdcToWorld(new(1, 1, 1), Cam);
        Gizmos.DrawLine(lbn, rbn);
        Gizmos.DrawLine(rbn, rtn);
        Gizmos.DrawLine(rtn, ltn);
        Gizmos.DrawLine(ltn, lbn);
        Gizmos.DrawLine(lbf, rbf);
        Gizmos.DrawLine(rbf, rtf);
        Gizmos.DrawLine(rtf, ltf);
        Gizmos.DrawLine(ltf, lbf);
        Gizmos.DrawLine(lbn, lbf);
        Gizmos.DrawLine(rbn, rbf);
        Gizmos.DrawLine(ltn, ltf);
        Gizmos.DrawLine(rtn, rtf);
    }

    private static Vector3 NdcToWorld(Vector3 ndc, Camera cam)
    {
        var viewProjection = cam.projectionMatrix * cam.worldToCameraMatrix;
        var worldPos = viewProjection.inverse * ndc.AddWCoordinate(1f);
        worldPos /= worldPos.w;
        return worldPos;
    }
}