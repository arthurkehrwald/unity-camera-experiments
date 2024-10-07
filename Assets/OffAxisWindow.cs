using UnityEngine;

public class OffAxisWindow : MonoBehaviour
{
    public Vector3 BottomLeftCorner => transform.TransformPoint(new Vector3(-1f, -1f, 0f));
    public Vector3 BottomRightCorner => transform.TransformPoint(new Vector3(1f, -1f, 0f));
    public Vector3 TopLeftCorner => transform.TransformPoint(new Vector3(-1f, 1f, 0f));
    public Vector3 TopRightCorner => transform.TransformPoint(new Vector3(1f, 1f, 0f));

    public struct Corners
    {
        public Corners(OffAxisWindow window)
        {
            bottomLeft = window.BottomLeftCorner;
            bottomRight = window.BottomRightCorner;
            topLeft = window.TopLeftCorner;
        }

        public Vector3 BottomLeft => bottomLeft;
        public Vector3 BottomRight => bottomRight;
        public Vector3 TopLeft => topLeft;

        private readonly Vector3 bottomLeft;
        private readonly Vector3 bottomRight;
        private readonly Vector3 topLeft;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(BottomLeftCorner, TopLeftCorner);
        Gizmos.DrawLine(TopLeftCorner, TopRightCorner);
        Gizmos.DrawLine(TopRightCorner, BottomRightCorner);
        Gizmos.DrawLine(BottomRightCorner, BottomLeftCorner);
    }
}