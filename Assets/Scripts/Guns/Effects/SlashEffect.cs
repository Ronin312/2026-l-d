using UnityEngine;

namespace FrostGame.Gun
{
    /// <summary>Krótki błysk w miejscu ciosu mieczem (sprite tworzony w kodzie, bez assetów).</summary>
    public class SlashEffect : MonoBehaviour
    {
        private const float Life = 0.14f;

        private static Sprite sprite;

        private SpriteRenderer sr;
        private Color baseColor;
        private float t;

        public static void Spawn(Vector2 position, float radius, Color color)
        {
            if (sprite == null) sprite = CreateCircleSprite();

            var go = new GameObject("Slash");
            go.transform.position = position;
            go.transform.localScale = new Vector3(radius * 2f, radius * 2f, 1f);

            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = 11;

            var fx = go.AddComponent<SlashEffect>();
            fx.sr = renderer;
            fx.baseColor = color;
        }

        private void Update()
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / Life);
            sr.color = new Color(baseColor.r, baseColor.g, baseColor.b, baseColor.a * (1f - k));
            if (t >= Life) Destroy(gameObject);
        }

        private static Sprite CreateCircleSprite()
        {
            const int size = 32;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            float c = (size - 1) * 0.5f;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), new Vector2(c, c));
                    float a = Mathf.Clamp01((c - d) / 2f);
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
                }
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        }
    }
}
