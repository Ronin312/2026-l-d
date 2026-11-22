# Grafiki pixel-art wrogów i bossów (zimowy cyberpunk)

14 animowanych postaci + kilka dodatków. Wszystko narysowane proceduralnie (Python, źródła w `PixelArt/`), w jednym stylu: ciemne, zimne kolory, śnieg na górnych krawędziach, sople, futra, lodowe pancerze i neonowe akcenty (cyjan, magenta, bursztyn) jako cyberpunkowe implanty.

## Import tekstur (każdy PNG)
- Texture Type: Default lub Sprite (Single)
- **Filter Mode: Point (no filter)**, **Compression: None**, Generate Mip Maps: wyłączone
- Pixels Per Unit: 32 (zgodnie z polem w `PixelSheetAnimator`)

## Skrypty
- `PixelSheetAnimator.cs` – tnie arkusz w locie (bez Sprite Editora i Animator Controllera) i odtwarza animacje.
- `EnemySpriteDriver.cs` – idle/move wg ruchu, odwracanie w stronę ruchu/gracza, animacja ataku, animacja śmierci (zostawia „zwłoki", bo `EnemyBase.Die` niszczy obiekt od razu).

Na obiekcie wroga (obiekt ze SpriteRenderer): dodaj `PixelSheetAnimator` (przypisz arkusz i preset) + `EnemySpriteDriver`. Sprite'y są narysowane zwrócone w prawo.

## Arkusze
Jeden wiersz = jedna animacja. Rozmiar klatki wynika z rozmiaru tekstury.

| Plik | Postać | Preset | Klatka | Arkusz |
|---|---|---|---|---|
| guardian.png | Kamienny Strażnik | Standard | 32 | 192x128 |
| archer.png | Cienisty Łucznik | Standard | 32 | 192x128 |
| shaman.png | Szaman Run | Standard | 32 | 192x128 |
| hunter.png | Łowca z Łańcuchem | Standard | 32 | 192x128 |
| echo.png | Widmo Echa | Standard | 32 | 192x128 |
| spider.png | Rojowy Pająk | Standard | 16 | 96x64 (PPU 32 -> mały) |
| bull.png | Rogaty Byk | Standard | 40x32 | 240x128 |
| mole.png | Kret Drążyciel | StandardWithMound | 32 | 192x160 |
| wisp.png | Ognik Wybuchowy | Standard | 24 | 144x96 |
| spirit.png | Duch Szamana | Spirit | 16 | 64x48 |
| cocoon.png | Kokon Pająków | Cocoon | 16 | 80x48 |
| tyrant.png | BOSS Kamienny Tyran | Standard | 64 | 384x256 |
| brood.png | BOSS Matka Roju | Standard | 64 | 384x256 |
| archon.png | BOSS Arcykapłan Run | Standard | 64 | 384x256 |
| fx_circle.png | Znacznik zapowiedzi ataku | – | 64 | 64x64, PPU 64 |

Układy presetów: **Standard** 6x4 (idle 4, move 6, attack 5, death 6); **StandardWithMound** 6x5 (+ mound 4); **Spirit** 4x3 (idle, move, death); **Cocoon** 5x3 (idle, hatch, death 5).
Pliki i ich wymiary muszą zostać takie, jak w tabeli (preset dzieli teksturę na równe komórki).

## Atak – kiedy odpala animacja
- **Automatycznie** (po kolorze zapowiedzi): Strażnik, Tyran, Byk, Matka Roju, Arcykapłan.
- **Kret**: automatycznie, gdy ukryty sprite pojawia się ponownie. Kopiec (child) = ten sam arkusz, `startClip = mound`, preset StandardWithMound.
- **Ręcznie** – jedna linia `GetComponent<EnemySpriteDriver>().TriggerAttack();`:
  - Łucznik – w `AimAndShoot` przy strzale,
  - Łowca – w `TelegraphAndFire` przy rzucie haka,
  - Szaman – przy tarczy / przyzwaniu duchów,
  - Ognik – gdy zaczyna się lont,
  - Pająki – w `SetCharge`.
- Duch Szamana: preset Spirit. Kokon: preset Cocoon (`PlayOnce("hatch")` przy wykluciu).
- `fx_circle.png`: sprite znacznika w `TelegraphedStrike` (PPU 64; kolor ustawia skrypt).

## Podgląd i regeneracja
`PixelArt/preview/` – GIF-y każdej postaci, `overview.png` – zestawienie. Regeneracja: `cd PixelArt && python3 build.py` (wymaga Pythona + Pillow + numpy); pojedyncza postać: `python3 build.py brood`.

## Ograniczenia
- Grafika proceduralna, jakość prototypowa; do finalnej gry warto ją dopieścić ręcznie (źródłowe PNG można edytować w Aseprite, zachowując układ siatki).
- Skrypty C# nie były kompilowane w Unity (brak kompilatora w moim środowisku) – możliwe drobne poprawki.
- Brak sprite'ów pocisków, haka i łańcucha – mogę dorysować.
