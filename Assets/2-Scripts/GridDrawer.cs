using UnityEngine;

public class GridDrawer : MonoBehaviour
{
    public float halfSize = 1.5f;      // grid outer border is -1.5..+1.5
    public float lineWidth = 0.05f;
    public Material lineMaterial;

    private void Start()
    {
        // Vertical lines: x = -1.5, -0.5, 0.5, 1.5
        DrawLine(new Vector3(-1.5f, -1.5f, 0), new Vector3(-1.5f,  1.5f, 0));
        DrawLine(new Vector3(-0.5f, -1.5f, 0), new Vector3(-0.5f,  1.5f, 0));
        DrawLine(new Vector3( 0.5f, -1.5f, 0), new Vector3( 0.5f,  1.5f, 0));
        DrawLine(new Vector3( 1.5f, -1.5f, 0), new Vector3( 1.5f,  1.5f, 0));

        // Horizontal lines: y = -1.5, -0.5, 0.5, 1.5
        DrawLine(new Vector3(-1.5f, -1.5f, 0), new Vector3( 1.5f, -1.5f, 0));
        DrawLine(new Vector3(-1.5f, -0.5f, 0), new Vector3( 1.5f, -0.5f, 0));
        DrawLine(new Vector3(-1.5f,  0.5f, 0), new Vector3( 1.5f,  0.5f, 0));
        DrawLine(new Vector3(-1.5f,  1.5f, 0), new Vector3( 1.5f,  1.5f, 0));
    }

    private void DrawLine(Vector3 a, Vector3 b)
    {
        GameObject go = new GameObject("GridLine");
        go.transform.parent = transform;

        var lr = go.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, a);
        lr.SetPosition(1, b);

        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;

        lr.useWorldSpace = true;

        lr.material = lineMaterial;
        lr.sortingOrder = -10; // behind sprites
    }
}