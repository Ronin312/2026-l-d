# Pociski, hak i łańcuch (zimowy cyberpunk)

Wszystkie sprite'y są narysowane zwrócone w prawo (oś +X = kierunek lotu). Import jak reszta paczki: Filter Mode Point, Compression None, mipmapy wyłączone, PPU 32.

| Plik | Do czego | Klatki | Rozmiar klatki |
|---|---|---|---|
| arrow.png | strzała Cienistego Łucznika | 4 (pętla) | 24x8 |
| ice_bolt.png | lodowy pocisk (`EnemyProjectile`, Szaman, pająki) | 4 (pętla) | 16x16 |
| rune_bolt.png | magenta pocisk runiczny (Arcykapłan, Szaman) | 4 (pętla) | 16x16 |
| hook.png | głowa haka `ChainHook` (Łowca, Tyran) | 4 (pętla) | 16x16 |
| chain.png | kafel łańcucha dla `LineRenderer` | 1 | 16x8 |
| impact.png | efekt trafienia pocisku / haka | 5 (raz) | 32x32 |

## Pocisk, hak, trafienie (PixelSheetAnimator)
1. Na prefabie pocisku dodaj child ze `SpriteRenderer` i `PixelSheetAnimator`.
2. Przypisz arkusz, preset **Projectile** (arrow, ice_bolt, rune_bolt, hook) albo **Impact** (impact).
3. Pocisk obracaj w stronę lotu: `transform.right = kierunek;` (sprite jest narysowany w prawo).
4. Efekt trafienia: w miejscu uderzenia utwórz obiekt z animatorem (preset Impact), wywołaj `PlayOnce("idle")` i zniszcz obiekt po ok. 0,5 s (np. `Destroy(go, 0.5f)`).

## Łańcuch (LineRenderer)
`ChainHook` używa `LineRenderer` (2 punkty). Ustawienia:
- Material: nowy materiał, shader `Sprites/Default`, tekstura = chain.png
- Texture: Wrap Mode = **Repeat** (w imporcie tekstury), Filter Mode = Point
- Line Renderer: **Texture Mode = Tile**, Width = **0.25** (8 px / PPU 32), Alignment = View lub TransformZ
- Sorting Layer ustaw tak jak wrogowie

Przy szerokości 0,25 jedno powtórzenie kafla ma 0,5 jednostki (16 px), więc ogniwa nie rozciągają się z długością łańcucha.

## Regeneracja
`cd PixelArt && python3 props.py` (zapisuje arkusze do `Assets/Art/Enemies/Projectiles/` i podglądy GIF do `preview/prop_*.gif`).
