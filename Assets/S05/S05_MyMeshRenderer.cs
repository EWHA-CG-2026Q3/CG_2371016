using UnityEngine;
using UnityEngine.UI;

public class S05_MyMeshRenderer : MonoBehaviour
{
    [SerializeField] private int canvasWidth = 256;
    [SerializeField] private int canvasHeight = 256;
    [SerializeField] private Color backgroundColor = new Color(0f, 0f, 0f, 1f);

    [Header("무늬 실습 (줄무늬·체스판 공용)")]
    [SerializeField] private int patternSize = 16;
    [SerializeField] private Color colorA = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color colorB = new Color(0.3f, 0.5f, 0.8f, 1f);

    private Texture2D canvasTexture;
    private RawImage targetImage;

    void Start()
    {
        targetImage = GetComponent<RawImage>();

        // 1. 빈 텍스처 생성
        canvasTexture = new Texture2D(canvasWidth, canvasHeight);

        // 2. 픽셀 경계를 흐리지 않게 설정
        canvasTexture.filterMode = FilterMode.Point;

        // 3. 픽셀 채우기
       FillCheckerboard(patternSize, colorA, colorB);

        // 4. 변경 사항 반영
        canvasTexture.Apply();

        // 5. RawImage에 텍스처 연결
        targetImage.texture = canvasTexture;
    }

    private void FillBackground(Color color)
    {
        for (int x = 0; x < canvasWidth; x++)
        {
            for (int y = 0; y < canvasHeight; y++)
            {
                canvasTexture.SetPixel(x, y, color);
            }
        }
    }

    private void FillVerticalStripes(int width, Color colorA, Color colorB)
    {
        for (int x = 0; x < canvasWidth; x++)
        {
            bool isColorA = (x / width) % 2 == 0;

            Color stripeColor = isColorA ? colorA : colorB;

            for (int y = 0; y < canvasHeight; y++)
            {
                canvasTexture.SetPixel(x, y, stripeColor);
            }
        }
    }

    private void FillCheckerboard(int size, Color colorA, Color colorB)
    {
        for (int x = 0; x < canvasWidth; x++)
        {
            for (int y = 0; y < canvasHeight; y++)
            {
                bool isColorA = ((x / size) + (y / size)) % 2 == 0;

                Color pixelColor = isColorA ? colorA : colorB;

                canvasTexture.SetPixel(x, y, pixelColor);
            }
        }
    }
}