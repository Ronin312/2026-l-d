using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrostGame.Enemies
{
    /// <summary>
    /// Odtwarza animacje z arkuszy sprite'ów (po jednym wierszu na animację). Nie wymaga ręcznego cięcia
    /// grafik w Sprite Editorze ani Animator Controllera: sprite'y składa w locie z tekstury.
    ///
    /// Ustawienia tekstury w Unity: Filter Mode = Point (no filter), Compression = None, Generate Mip Maps = off.
    /// Texture Type może być Default albo Sprite (Single). Pixels Per Unit ustaw tak samo tutaj (domyślnie 32).
    ///
    /// Układ arkusza zależy od presetu (rozmiar klatki wynika z rozmiaru tekstury, więc działa dla każdej postaci):
    ///   Standard          6 kolumn x 4 wiersze: idle, move, attack, death
    ///   StandardWithMound 6 x 5: jak wyżej + mound (Kret)
    ///   Spirit            4 x 3: idle, move, death (duch Szamana)
    ///   Cocoon            5 x 3: idle, hatch, death (kokon)
    ///   Projectile        4 x 1: idle (pętla; strzała, pociski, hak)
    ///   Impact            5 x 1: idle (raz; efekt trafienia, ustaw PlayOnce)
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class PixelSheetAnimator : MonoBehaviour
    {
        public enum Preset { Standard, StandardWithMound, Spirit, Cocoon, Projectile, Impact }

        struct ClipDef
        {
            public string name;
            public int row, frames;
            public float fps;
            public bool loop;

            public ClipDef(string name, int row, int frames, float fps, bool loop)
            {
                this.name = name;
                this.row = row;
                this.frames = frames;
                this.fps = fps;
                this.loop = loop;
            }
        }

        class Clip
        {
            public Sprite[] sprites;
            public float fps;
            public bool loop;
        }

        [SerializeField] Texture2D sheet;
        [SerializeField] Preset preset = Preset.Standard;
        [SerializeField] float pixelsPerUnit = 32f;
        [SerializeField] Vector2 pivot = new Vector2(0.5f, 0.5f);
        [Tooltip("Animacja odtwarzana na starcie. Dla kopca Kreta wpisz: mound.")]
        [SerializeField] string startClip = "idle";
        [SerializeField] float speed = 1f;

        static readonly Dictionary<(Texture2D, Preset, float, Vector2), Dictionary<string, Clip>> cache =
            new Dictionary<(Texture2D, Preset, float, Vector2), Dictionary<string, Clip>>();

        Dictionary<string, Clip> clips = new Dictionary<string, Clip>();
        SpriteRenderer sr;
        Clip current;
        string currentName;
        float time;
        bool looping;
        bool oneShot;
        bool finished;
        string thenPlay;
        bool destroyOnFinish;
        string lastClipName;

        public event Action<string> ClipFinished;

        public string Current => currentName;
        public bool Finished => finished;
        public bool IsOneShot => oneShot && !finished;
        public float Speed { get => speed; set => speed = value; }

        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            Build();
            if (!string.IsNullOrEmpty(startClip)) Play(startClip, true);
        }

        // ---------- API ----------

        public bool HasClip(string clipName) => clips.ContainsKey(clipName);

        /// <summary>Odtwarza animację w pętli (zgodnie z jej ustawieniem). Bez restart nic nie robi, gdy już gra.</summary>
        public void Play(string clipName, bool restart = false)
        {
            if (!clips.TryGetValue(clipName, out var clip)) return;
            if (!restart && currentName == clipName && !finished) return;

            Begin(clipName, clip, false);
        }

        /// <summary>Odtwarza animację raz i zatrzymuje się na ostatniej klatce. Opcjonalnie potem włącza inną.</summary>
        public void PlayOnce(string clipName, string then = null)
        {
            if (!clips.TryGetValue(clipName, out var clip)) return;

            thenPlay = then;
            Begin(clipName, clip, true);
        }

        void Begin(string clipName, Clip clip, bool once)
        {
            current = clip;
            currentName = clipName;
            oneShot = once;
            looping = clip.loop && !once;
            finished = false;
            time = 0f;
            if (!once) thenPlay = null;
            if (sr != null && clip.sprites.Length > 0) sr.sprite = clip.sprites[0];
        }

        /// <summary>
        /// Zostawia w tym miejscu samodzielny obiekt z animacją (np. death), który po jej końcu sam znika.
        /// Używaj, gdy oryginalny obiekt jest niszczony natychmiast (EnemyBase.Die).
        /// </summary>
        public GameObject SpawnCorpse(string clipName = "death")
        {
            if (!HasClip(clipName)) return null;

            var go = new GameObject(name + "_Corpse");
            go.transform.SetPositionAndRotation(transform.position, transform.rotation);
            go.transform.localScale = transform.lossyScale;

            var copy = go.AddComponent<SpriteRenderer>();
            copy.sortingLayerID = sr.sortingLayerID;
            copy.sortingOrder = sr.sortingOrder;
            copy.flipX = sr.flipX;
            copy.flipY = sr.flipY;
            copy.sharedMaterial = sr.sharedMaterial;

            var animator = go.AddComponent<PixelSheetAnimator>();
            animator.sheet = sheet;
            animator.preset = preset;
            animator.pixelsPerUnit = pixelsPerUnit;
            animator.pivot = pivot;
            animator.startClip = null;
            animator.Build();
            animator.destroyOnFinish = true;
            animator.PlayOnce(clipName);
            return go;
        }

        // ---------- odtwarzanie ----------

        void Update()
        {
            if (current == null || current.sprites.Length == 0) return;
            if (finished) return;

            time += Time.deltaTime * speed;
            float f = time * current.fps;
            int count = current.sprites.Length;
            int index;

            if (looping)
            {
                index = (int)f % count;
            }
            else
            {
                index = Mathf.Min((int)f, count - 1);
            }

            if (sr != null) sr.sprite = current.sprites[index];

            if (!looping && f >= count)
            {
                finished = true;
                string finishedName = currentName;
                ClipFinished?.Invoke(finishedName);

                if (destroyOnFinish)
                {
                    Destroy(gameObject, 0.25f);
                }
                else if (!string.IsNullOrEmpty(thenPlay) && finishedName == currentName)
                {
                    string next = thenPlay;
                    thenPlay = null;
                    Play(next, true);
                }
            }
        }

        // ---------- budowanie sprite'ów ----------

        void Build()
        {
            clips = new Dictionary<string, Clip>();
            if (sheet == null) return;

            var key = (sheet, preset, pixelsPerUnit, pivot);
            if (cache.TryGetValue(key, out var cached) && IsAlive(cached))
            {
                clips = cached;
                return;
            }

            GetLayout(preset, out int cols, out int rows, out ClipDef[] defs);
            int fw = sheet.width / cols;
            int fh = sheet.height / rows;

            foreach (var def in defs)
            {
                var sprites = new Sprite[def.frames];
                for (int c = 0; c < def.frames; c++)
                {
                    // Unity liczy y od dołu tekstury, a wiersze arkusza od góry.
                    var rect = new Rect(c * fw, sheet.height - (def.row + 1) * fh, fw, fh);
                    sprites[c] = Sprite.Create(sheet, rect, pivot, pixelsPerUnit, 0, SpriteMeshType.FullRect);
                    sprites[c].name = sheet.name + "_" + def.name + "_" + c;
                }

                clips[def.name] = new Clip { sprites = sprites, fps = def.fps, loop = def.loop };
            }

            cache[key] = clips;
        }

        static bool IsAlive(Dictionary<string, Clip> dict)
        {
            foreach (var clip in dict.Values)
                return clip.sprites.Length > 0 && clip.sprites[0] != null;
            return false;
        }

        static void GetLayout(Preset p, out int cols, out int rows, out ClipDef[] defs)
        {
            switch (p)
            {
                case Preset.StandardWithMound:
                    cols = 6; rows = 5;
                    defs = new[]
                    {
                    new ClipDef("idle", 0, 4, 5f, true), new ClipDef("move", 1, 6, 10f, true),
                    new ClipDef("attack", 2, 5, 8f, false), new ClipDef("death", 3, 6, 10f, false),
                    new ClipDef("mound", 4, 4, 6f, true),
                };
                    break;

                case Preset.Spirit:
                    cols = 4; rows = 3;
                    defs = new[]
                    {
                    new ClipDef("idle", 0, 4, 6f, true), new ClipDef("move", 1, 4, 10f, true),
                    new ClipDef("death", 2, 4, 10f, false),
                };
                    break;

                case Preset.Cocoon:
                    cols = 5; rows = 3;
                    defs = new[]
                    {
                    new ClipDef("idle", 0, 4, 4f, true), new ClipDef("hatch", 1, 4, 6f, false),
                    new ClipDef("death", 2, 5, 10f, false),
                };
                    break;

                case Preset.Projectile:
                    cols = 4; rows = 1;
                    defs = new[] { new ClipDef("idle", 0, 4, 14f, true) };
                    break;

                case Preset.Impact:
                    cols = 5; rows = 1;
                    defs = new[] { new ClipDef("idle", 0, 5, 14f, false) };
                    break;

                default:
                    cols = 6; rows = 4;
                    defs = new[]
                    {
                    new ClipDef("idle", 0, 4, 5f, true), new ClipDef("move", 1, 6, 10f, true),
                    new ClipDef("attack", 2, 5, 8f, false), new ClipDef("death", 3, 6, 10f, false),
                };
                    break;
            }
        }
    }
}
