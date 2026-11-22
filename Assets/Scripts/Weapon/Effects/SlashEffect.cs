using UnityEngine;

namespace FrostGame.Weapon
{
    /// <summary>Krótki błysk / cięcie w miejscu ciosu mieczem (animacja lub kółko awaryjne).</summary>
    public class SlashEffect : MonoBehaviour
    {
        private float Life = 0.14f;

        private static Sprite sprite;

        private SpriteRenderer sr;
        private Animator anim;
        private Color baseColor;
        private float t;
        private bool hasAnimation;

        public void SetLifeTime(float seconds) => Life = seconds;

        public static GameObject Spawn(
            float timer,
            Transform par,
            float radius,
            Color color,
            RuntimeAnimatorController slashAnimator = null,
            float angleDegrees = 0f,
            bool mirrorVertical = false)
        {
            var go = new GameObject("Slash");
            go.transform.SetParent(par);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.Euler(0f, 0f, angleDegrees);
            go.transform.localScale = new Vector3(radius * 2f, radius * 2f, 1f);

            return SetupEffect(go, timer, color, slashAnimator, mirrorVertical);
        }

        public static GameObject Spawn(
            Vector2 position,
            float radius,
            Color color,
            RuntimeAnimatorController slashAnimator = null,
            float angleDegrees = 0f,
            float timer = 0.14f,
            bool mirrorVertical = false)
        {
            var go = new GameObject("Slash");
            go.transform.position = position;
            go.transform.rotation = Quaternion.Euler(0f, 0f, angleDegrees);
            go.transform.localScale = new Vector3(radius * 2f, radius * 2f, 1f);

            return SetupEffect(go, timer, color, slashAnimator, mirrorVertical);
        }

        private static GameObject SetupEffect(
            GameObject go,
            float timer,
            Color color,
            RuntimeAnimatorController slashAnimator,
            bool mirrorVertical)
        {
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.color = color;
            renderer.sortingOrder = 11;
            renderer.flipY = mirrorVertical;

            var fx = go.AddComponent<SlashEffect>();
            fx.sr = renderer;
            fx.baseColor = color;
            fx.SetLifeTime(timer);

            if (slashAnimator != null)
            {
                fx.hasAnimation = true;
                var animator = go.AddComponent<Animator>();
                animator.runtimeAnimatorController = slashAnimator;
                fx.anim = animator;
            }
            else
            {
                fx.hasAnimation = false;
                if (sprite == null) sprite = CreateCircleSprite();
                renderer.sprite = sprite;
            }

            return go;
        }

        private void Update()
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / Life);

            if (!hasAnimation)
            {
                sr.color = new Color(baseColor.r, baseColor.g, baseColor.b, baseColor.a * (1f - k));
            }

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
