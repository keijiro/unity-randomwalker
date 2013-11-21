using UnityEngine;
using System.Collections;

public class WebCamHandler : MonoBehaviour
{
    public int requestWidth = 320;
    public int requestHeight = 240;
    WebCamTexture webcam;
    Texture2D texture;

    public bool Ready
    {
        get {
            return webcam.isPlaying && webcam.width > 16;
        }
    }

    IEnumerator Start ()
    {
        webcam = new WebCamTexture (requestWidth, requestHeight);
        webcam.Play ();

        while (!Ready)
        {
            yield return null;
        }

        transform.localScale = new Vector3 (1.0f * webcam.width / webcam.height, 1, 1);

        texture = new Texture2D (webcam.width, webcam.height);
        renderer.material.mainTexture = texture;
    }

    void Update()
    {
        if (texture != null)
        {
            var colors = webcam.GetPixels();
            var levels = new float [colors.Length];

            for (var i = 0; i < colors.Length; i++)
            {
                levels[i] = colors[i].grayscale;
            }

            var gx = new float [colors.Length];
            var gy = new float [colors.Length];

            var o1 = 0;
            var o2 = texture.width;
            var o3 = texture.width * 2;

            for (var y = 1; y < texture.height - 1; y++)
            {
                o1++;
                o2++;
                o3++;

                for (var x = 1; x < texture.width - 1; x++)
                {
                    gx[o2] =
                            levels[o1 - 1] * 1 - levels[o1 + 1] * 1 +
                            levels[o2 - 1] * 2 - levels[o2 + 1] * 2 +
                            levels[o3 - 1] * 1 - levels[o3 + 1] * 1;
                    gy[o2] =
                            levels[o1 - 1] * 1 - levels[o3 - 1] * 1 +
                            levels[o1 + 0] * 2 - levels[o3 + 0] * 2 +
                            levels[o1 + 1] * 1 - levels[o3 + 1] * 1;
                    o1++;
                    o2++;
                    o3++;
                }

                o1++;
                o2++;
                o3++;
            }

            for (var i = 0; i < levels.Length; i++)
            {
                var l = Mathf.Sqrt (gx[i] * gx[i] + gy[i] * gy[i]);
                colors[i] = new Color(l, l, l);
            }

            texture.SetPixels(colors);
            texture.Apply();
        }
    }

    public float GetLevel(Vector3 position)
    {
		if (texture == null)
						return 0;

        var x = (int)(position.x * webcam.height) + webcam.width / 2;
        var y = (int)((0.5f - position.y) * webcam.height);
        if (x < 0 || y < 0 || x >= webcam.width || y >= webcam.height)
        {
            return 0.0f;
        }
        else
        {
            return texture.GetPixel(x, y).grayscale;
        }
    }
}
