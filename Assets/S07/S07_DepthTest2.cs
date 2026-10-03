using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class S07_DepthTest2 : MonoBehaviour
{
    [SerializeField] private int canvasWidth = 256;
    [SerializeField] private int canvasHeight = 256;

    // 삼각형 1
    [SerializeField] private Vector3 vertexA1 = new Vector3(60, 60, 0.4f);
    [SerializeField] private Vector3 vertexB1 = new Vector3(190, 70, 0.4f);
    [SerializeField] private Vector3 vertexC1 = new Vector3(125, 210, 0.4f);

    // 삼각형 2
    // z를 서로 다르게 해서 겹치는 영역에서 앞뒤가 위치에 따라 바뀌도록 설계
    [SerializeField] private Vector3 vertexA2 = new Vector3(60, 60, 0.2f);
    [SerializeField] private Vector3 vertexB2 = new Vector3(190, 70, 0.8f);
    [SerializeField] private Vector3 vertexC2 = new Vector3(125, 210, 0.8f);

    // 삼각형 3
    [SerializeField] private Vector3 vertexA3 = new Vector3(80, 80, 0.9f);
    [SerializeField] private Vector3 vertexB3 = new Vector3(170, 80, 0.9f);
    [SerializeField] private Vector3 vertexC3 = new Vector3(125, 170, 0.9f);

    [SerializeField] private Color color1 = new Color(1f, 0.2f, 0.2f, 1f);
    [SerializeField] private Color color2 = new Color(0.2f, 0.8f, 0.2f, 1f);
    [SerializeField] private Color color3 = new Color(0.2f, 0.4f, 1f, 1f);

    private Texture2D canvasTexture;
    private RawImage targetImage;
    private float[,] depthBuffer;

    void OnEnable()
    {
        RedrawAll();
    }

    void OnValidate()
    {
        RedrawAll();
    }

    private void RedrawAll()
    {
        targetImage = GetComponent<RawImage>();

        if (targetImage == null)
            return;

        canvasTexture = new Texture2D(canvasWidth, canvasHeight);
        canvasTexture.filterMode = FilterMode.Point;

        depthBuffer = new float[canvasWidth, canvasHeight];

        // depthBuffer를 가장 먼 값으로 초기화
        for (int x = 0; x < canvasWidth; x++)
        {
            for (int y = 0; y < canvasHeight; y++)
            {
                canvasTexture.SetPixel(x, y, Color.black);
                depthBuffer[x, y] = float.MaxValue;
            }
        }

        // TODO 0
        DrawTriangle(vertexA1, vertexB1, vertexC1, color1);
        DrawTriangle(vertexA2, vertexB2, vertexC2, color2);
        DrawTriangle(vertexA3, vertexB3, vertexC3, color3);

        canvasTexture.Apply();
        targetImage.texture = canvasTexture;
    }

    private void DrawTriangle(Vector3 a, Vector3 b, Vector3 c, Color color)
    {
        for (int x = 0; x < canvasWidth; x++)
        {
            for (int y = 0; y < canvasHeight; y++)
            {
                Vector2 p = new Vector2(x + 0.5f, y + 0.5f);

                float denom =
                    a.x * (b.y - c.y) +
                    b.x * (c.y - a.y) +
                    c.x * (a.y - b.y);

                float w1 =
                    (p.x * (b.y - c.y) +
                    b.x * (c.y - p.y) +
                    c.x * (p.y - b.y)) / denom;

                float w2 =
                    (a.x * (p.y - c.y) +
                    p.x * (c.y - a.y) +
                    c.x * (a.y - p.y)) / denom;

                float w3 = 1f - w1 - w2;

                bool isInside =
                    w1 >= 0f &&
                    w2 >= 0f &&
                    w3 >= 0f;

                if (isInside)
                {
                    // TODO 1
                    float interpolatedZ =
                        w1 * a.z +
                        w2 * b.z +
                        w3 * c.z;

                    // TODO 2
                    if (interpolatedZ < depthBuffer[x, y])
                    {
                        canvasTexture.SetPixel(x, y, color);
                        depthBuffer[x, y] = interpolatedZ;
                    }
                }
            }
        }
    }
}