using UnityEngine;

namespace FrostGame.Enemies
{
    /// <summary>
    /// Steruje animacjami wroga (PixelSheetAnimator) bez ingerencji w skrypty wrogów:
    ///  - idle / move na podstawie faktycznego ruchu,
    ///  - odwraca sprite w stronę ruchu albo gracza,
    ///  - po śmierci zostawia animację "death" (rozbicie na lód, eksplozja...), bo EnemyBase niszczy obiekt od razu,
    ///  - atak odpala sam, gdy wróg zmieni kolor na kolor zapowiedzi (Strażnik, Tyran, Byk, Matka Roju, Arcykapłan)
    ///    albo gdy ukryty sprite pojawi się ponownie (Kret wynurza się z ziemi).
    /// Pozostali wrogowie (Łucznik, Łowca, Szaman, Ognik, pająki) nie mają koloru zapowiedzi: wywołaj dla nich
    /// TriggerAttack() ręcznie w momencie ataku (jedna linia, miejsca podaje README).
    ///
    /// Skrypt wieszasz na obiekcie ze SpriteRenderer i PixelSheetAnimator. Wróg (EnemyBase) może być na tym samym
    /// obiekcie albo wyżej w hierarchii. Sprite'y rysowane są zwrócone w prawo.
    /// </summary>
    [RequireComponent(typeof(PixelSheetAnimator))]
    public class EnemySpriteDriver : MonoBehaviour
    {
        public enum FaceMode { Movement, Player, None }

        [Header("Kierunek")]
        [SerializeField] FaceMode faceMode = FaceMode.Movement;
        [Tooltip("Pozostaw włączone: grafiki w paczce są narysowane zwrócone w prawo.")]
        [SerializeField] bool spriteFacesRight = true;
        [Tooltip("Prędkość (jednostki/s), od której wróg uznawany jest za poruszającego się.")]
        [SerializeField] float moveThreshold = 0.25f;

        [Header("Atak")]
        [SerializeField] string attackClip = "attack";
        [Tooltip("Odpalaj atak, gdy kolor sprite'a zmieni się na jeden z kolorów zapowiedzi.")]
        [SerializeField] bool attackFromTint = true;
        [SerializeField]
        Color[] windupColors =
        {
        new Color(1f, 0.55f, 0.3f),   // Strażnik, Tyran, Byk
        new Color(1f, 0.6f, 0.4f),    // Matka Roju
        new Color(0.8f, 0.6f, 1f),    // Arcykapłan
    };
        [SerializeField] float tintTolerance = 0.12f;
        [Tooltip("Jak długo kolor musi się utrzymać, żeby uznać go za zapowiedź (odsiewa błyski od trafień).")]
        [SerializeField] float tintHold = 0.08f;
        [Tooltip("Odpalaj atak, gdy sprite zostanie ponownie włączony (Kret wynurzający się spod ziemi).")]
        [SerializeField] bool attackOnReappear = true;

        [Header("Śmierć")]
        [SerializeField] string deathClip = "death";
        [SerializeField] bool leaveCorpse = true;

        PixelSheetAnimator anim;
        SpriteRenderer sr;
        EnemyBase enemy;
        Transform player;
        Vector3 lastPos;
        float tintTimer;
        float lastWindupTime = -10f;
        bool tintArmed = true;
        bool wasEnabled;
        bool attacking;
        bool attackFromTintActive;
        string lastSetName;

        void Awake()
        {
            anim = GetComponent<PixelSheetAnimator>();
            sr = GetComponent<SpriteRenderer>();
            enemy = GetComponent<EnemyBase>();
            lastPos = transform.position;
            wasEnabled = sr.enabled;
        }

        void Start()
        {
            if (enemy != null) enemy.DiedAction += OnEnemyDied;

            if (faceMode == FaceMode.Player)
            {
                var p = GameObject.FindGameObjectWithTag("Player");
                if (p != null) player = p.transform;
            }
        }

        void OnDestroy()
        {
            if (enemy != null) enemy.DiedAction -= OnEnemyDied;
        }

        /// <summary>Odpal animację ataku ręcznie (np. w momencie strzału, rzutu haka, zapalenia lontu).</summary>
        public void TriggerAttack()
        {
            if (!anim.HasClip(attackClip)) return;

            attacking = true;
            attackFromTintActive = false;
            anim.PlayOnce(attackClip);
        }

        void Update()
        {


            // Animacja ataku trzyma ostatnią klatkę, dopóki trwa zapowiedź (kolor), a potem wraca do ruchu.


            //string wanted = speed > moveThreshold && anim.HasClip("move") ? "move" : "idle";
            //anim.Play(wanted);
            //Debug.Log(wanted);
        }

        private void FixedUpdate()
        {
            float dt = Time.deltaTime;
            Vector3 pos = transform.position;
            Vector3 delta = pos - lastPos;
            lastPos = pos;
            float speed = dt > 0f ? delta.magnitude / dt : 0f;

            UpdateFacing(delta);
            DetectAttack(dt);

            if (attacking)
            {
                bool clipDone = anim.Finished;
                // 0,25 s marginesu: czerwony błysk po trafieniu nie może przerwać ani powtórzyć ataku.
                bool tintStillOn = attackFromTintActive && Time.time - lastWindupTime < 0.25f;
                if (clipDone && !tintStillOn) attacking = false;
                else return;
            }


            string wanted = speed > moveThreshold && anim.HasClip("move") ? "move" : "idle";
            if (wanted != lastSetName || wanted == "move")
            {
                anim.Play(wanted);
                lastSetName = wanted;
            }
            Debug.Log(wanted);
        }

        void UpdateFacing(Vector3 delta)
        {
            if (faceMode == FaceMode.None) return;

            float dx = 0f;
            if (faceMode == FaceMode.Movement) dx = delta.x;
            else if (player != null) dx = player.position.x - transform.position.x;

            float eps = faceMode == FaceMode.Movement ? 0.001f : 0.05f;
            if (Mathf.Abs(dx) < eps) return;

            bool faceLeft = dx < 0f;
            sr.flipX = spriteFacesRight ? faceLeft : !faceLeft;
        }

        void DetectAttack(float dt)
        {
            if (attackOnReappear)
            {
                if (!wasEnabled && sr.enabled) TriggerAttack();
                wasEnabled = sr.enabled;
            }

            if (!attackFromTint) return;

            if (IsWindupTint(sr.color))
            {
                lastWindupTime = Time.time;
                tintTimer += dt;
                if (tintArmed && tintTimer >= tintHold && !attacking)
                {
                    tintArmed = false;
                    TriggerAttack();
                    attackFromTintActive = true;
                }
            }
            else
            {
                tintTimer = 0f;
                tintArmed = true;
            }
        }

        bool IsWindupTint(Color c)
        {
            foreach (var w in windupColors)
            {
                float d = Mathf.Max(Mathf.Abs(c.r - w.r), Mathf.Abs(c.g - w.g), Mathf.Abs(c.b - w.b));
                if (d <= tintTolerance) return true;
            }
            return false;
        }

        void OnEnemyDied(EnemyBase enemy)
        {
            if (leaveCorpse) anim.SpawnCorpse(deathClip);
        }
    }
}
