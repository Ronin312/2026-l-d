# Wrogowie 2D dla Unity: zestaw 9 jednostek

Dziewięć uzupełniających się wrogów dla gry 2D top-down. Wszystkie skrypty są w `Assets/Scripts/Enemies/`. Kod napisano pod Unity 2021 LTS lub nowsze (fizyka 2D, `Rigidbody2D`). Skrypty nie były kompilowane w edytorze Unity, więc pierwszy test na scenie może wymagać drobnych poprawek.

## 1. Przegląd

| Jednostka | Skrypt | Rola | Wymusza na graczu |
|---|---|---|---|
| Kamienny Strażnik | `StoneGuardian` | tank, kontrola przestrzeni | cierpliwość, wybór momentu ataku |
| Cienisty Łucznik | `ShadowArcher` | dystans, nękanie | ciągły ruch, gonienie celu |
| Rojowy Pająk | `SwarmSpider` + `SwarmManager` | liczebność, chaos | obrażenia obszarowe, priorytety |
| Szaman Run | `RuneShaman` | wsparcie | przebijanie się do kruchego celu |
| Rogaty Byk | `HornedBull` | szarżownik | używanie otoczenia, unikanie linii |
| Kret Zasadzkowiec | `BurrowMole` | zasadzka, strefa | ciągły ruch, uwaga na ziemię |
| Echo | `EchoPhantom` | kara za schematy | nie chodzić po własnych śladach |
| Łowca z Łańcuchem | `ChainHunter` + `ChainHook` | kontrola ruchu | przerywanie haka, unikanie linii |
| Ognik Wybuchowy | `ExplosiveWisp` | kamikaze, reakcje łańcuchowe | kontrola pozycji, użycie jako broń |

## 2. Opisy jednostek

### 2.1 Kamienny Strażnik
- Wolny, duży, dużo HP. Szeroki zamach z zapowiedzią (zmiana koloru) zostawia ślad na ziemi (`DamageZone`).
- Pancerz to osobny licznik HP. Po jego rozbiciu: ogłuszenie, potem faza osłabienia (szybszy, ale otrzymuje podwójne obrażenia), po której pancerz odrasta.
- Stany: Patrol → Chase → Windup → Recovery → Stunned.

### 2.2 Cienisty Łucznik
- Szybki, kruchy, utrzymuje dystans. Strzela z zapowiedzią (linia celowania), która blokuje się tuż przed strzałem.
- Gdy gracz podejdzie za blisko: unik w tył i teleport w najdalszy punkt (cooldown 5 s).
- Strzela tylko z linią widzenia (`Linecast`).

### 2.3 Rojowy Pająk
- Mały, 1-2 trafienia, grupa 4-6. Cykl: okrążanie gracza → szarża wszystkich naraz.
- `SwarmManager` rozdziela kąty okrążania, więc pająki się nie nakładają.
- Po śmierci zostawia kokon, który po 3 s wykluwa nowego pająka, o ile gracz go nie zniszczy.

### 2.4 Szaman Run
- Kruchy, trzyma dystans od gracza. Co kilka sekund nakłada tarczę (`ShieldBuff`, pochłania kilka trafień) na sojusznika, który jest najbliżej gracza. Między nimi widać wiązkę.
- Gdy nie ma kogo osłaniać, przywołuje duchy (`SpiritMinion`), które znikają po kilku sekundach. Liczba duchów jest ograniczona.
- Śmierć szamana natychmiast zdejmuje wszystkie nałożone przez niego tarcze.

### 2.5 Rogaty Byk
- Gdy ma linię widzenia i gracz jest w zasięgu, grzebie kopytem (zapowiedź), po czym szarżuje w prostej linii. Kierunek blokuje się tuż przed szarżą.
- Uderzenie w ścianę ogłusza go, a w tym czasie otrzymuje podwójne obrażenia. Trafienie gracza kończy szarżę.
- Stany: Idle → Windup → Charge → Recover lub Stunned.

### 2.6 Kret Zasadzkowiec
- Pod ziemią jest nietykalny i niewidoczny (widać tylko kopiec), przenika ściany i podąża za graczem.
- Zatrzymuje się pod graczem, na ziemi pojawia się okrąg zapowiedzi, po czym wynurza się, zadaje obrażenia obszarowe i przez kilka sekund zostaje na powierzchni, gdzie można go bić.
- Stany: Underground → Telegraph → Surfaced.

### 2.7 Echo
- Duch, który zapisuje trasę gracza i odtwarza ją z opóźnieniem (domyślnie 2 s). Przenika ściany (collider jako trigger) i rani przy dotknięciu.
- Trafienie go rozprasza na chwilę (przezroczysty, nieszkodliwy), po czym zaczyna zapisywać trasę od nowa.
- Można go zaprowadzić pod strzały Łucznika lub zamach Strażnika.

### 2.8 Łowca z Łańcuchem
- Utrzymuje dystans. Zapowiada rzut cienką linią, potem wyrzuca hak (`ChainHook`).
- Hak trafiony w gracza zadaje obrażenia i przyciąga go do łowcy. Hak jest `IDamageable`, więc atak gracza może go przeciąć.
- Gdy hak trafi w przeszkodę, łowca jest ogłuszony i otrzymuje zwiększone obrażenia.

### 2.9 Ognik Wybuchowy
- Leci falistym torem do gracza. W zasięgu zaczyna migać (lont 0,8 s) i wybucha.
- Wybuch rani gracza i innych wrogów. Zabity z daleka eksploduje od razu, a sąsiednie ogniki wybuchają w reakcji łańcuchowej.
- Można go odciągnąć w tłum i odstrzelić.

## 3. Kombinacje

| Duet | Efekt |
|---|---|
| Szaman + Strażnik | tarcza na tanku, trzeba najpierw dopaść szamana |
| Byk + Pająki | szarża rozbija formację roju i odsłania gracza |
| Kret + Łucznik | unikanie strzałów prowadzi prosto w pułapkę |
| Łowca + Strażnik | przyciągnięcie gracza pod zamach |
| Echo + Łucznik | ruch jest konieczny, ale nie po własnym śladzie |
| Ognik + Pająki | wybuch w środku roju to nagroda za dobre ustawienie |

## 4. Instalacja

1. Skopiuj folder `Assets/Scripts/Enemies/` do swojego projektu.
2. **Tag:** gracz musi mieć tag `Player`.
3. **Zdrowie gracza:** obiekt gracza musi mieć komponent implementujący `IDamageable`. Przykład: `PlayerHealth`.
4. **Przyciąganie:** dla Łowcy gracz potrzebuje komponentu `IPullable`. Przykład: `PlayerPullReceiver`. Twój kontroler ruchu powinien ignorować wejście, gdy `IsBeingPulled` jest `true`.
5. **Fizyka 2D:** wrogowie mają `Rigidbody2D` typu **Dynamic**. Skrypt ustawia Gravity Scale 0 i blokuje rotację.
6. **Warstwy:** utwórz warstwy `Player`, `Enemy` i `Obstacle`. Ściany i przeszkody muszą mieć zwykłe collidery (nie triggery) na warstwie `Obstacle`.
7. **Dane:** `Create > Enemies > Enemy Data`, po jednym assecie na typ wroga. Każdy typ używa tylko swojej sekcji.

### Maski warstw w Inspectorze

| Skrypt | Pole | Co ustawić |
|---|---|---|
| `StoneGuardian` | Player Layer | `Player` |
| `ShadowArcher` | Obstacle Mask | `Obstacle` |
| `RuneShaman` | Enemy Layer | `Enemy` |
| `HornedBull` | Obstacle Mask, Player Layer | `Obstacle`, `Player` |
| `BurrowMole` | Player Layer | `Player` |
| `ChainHunter` | Obstacle Mask | `Obstacle` |
| `ExplosiveWisp` | Explosion Mask | `Player` + `Enemy` |

Warstwa samego Byka nie może wchodzić w jego `Obstacle Mask`, bo będzie wykrywał sam siebie.

## 5. Prefaby

Każdy wróg ma: `SpriteRenderer`, `Rigidbody2D`, `Collider2D` i swój skrypt z przypisanym `EnemyData`.

- **Strażnik:** `StoneGuardian`. Opcjonalnie punkty patrolu i prefab `DamageZone`.
- **Ślad na ziemi:** `SpriteRenderer`, `Collider2D` (Is Trigger), `DamageZone`.
- **Łucznik:** `ShadowArcher`, dziecko `FirePoint`, `LineRenderer` (2 punkty, World Space) na linię celowania, prefab pocisku i puste `Transform`y jako punkty teleportu.
- **Pocisk Łucznika:** `Collider2D`, `Rigidbody2D`, `EnemyProjectile`.
- **Pająk:** `SwarmSpider` z prefabem kokonu.
- **Kokon:** `Collider2D`, `SpiderCocoon`.
- **Rój:** pusty obiekt z `SwarmManager` i prefabem pająka.
- **Szaman:** `RuneShaman`, `LineRenderer` na wiązkę (opcjonalnie) i prefab `SpiritMinion`. `ShieldBuff` dodaje się sam w locie.
- **Duch:** `SpiritMinion` z własnym `EnemyData` (np. HP 1, obrażenia kontaktowe 1, prędkość 3,5).
- **Byk:** `HornedBull`.
- **Kret:** `BurrowMole`. Opcjonalnie dziecko z kopcem (`Mound Effect`) i dziecko z okręgiem zapowiedzi (`Telegraph Marker`, sprite 1 jednostki, skalowany do promienia wybuchu).
- **Echo:** `EchoPhantom`. Collider zostanie ustawiony jako trigger. Opcjonalnie `TrailRenderer` na ślad.
- **Łowca:** `ChainHunter`, dziecko `FirePoint`, `LineRenderer` na linię zapowiedzi i prefab haka.
- **Hak:** `Collider2D`, `Rigidbody2D` (Kinematic), `ChainHook` i `LineRenderer` (2 punkty, World Space) przypisany w polu Chain.
- **Ognik:** `ExplosiveWisp`. Opcjonalnie prefab efektu wybuchu, który sam się niszczy.

## 6. Balans (wartości startowe)

| Wróg | HP | Prędkość | Obrażenia | Uwagi |
|---|---|---|---|---|
| Strażnik | 10 + 15 pancerza | 1,5 | 2 (zamach) | stun 2 s, osłabienie 4 s |
| Łucznik | 4 | 3 | 1 (pocisk) | strzał co 2,5 s, teleport co 5 s |
| Pająk | 1-2 | 2,5 (szarża ×2,2) | 1 | kokon 3 s, max 8 w roju |
| Szaman | 5 | 2 | 0 | tarcza 3 trafienia co 4 s |
| Byk | 12 | 2 | 3 (szarża) | szarża 10, stun 2,5 s (×2 obrażeń) |
| Kret | 6 | 4 pod ziemią | 2 (wybuch) | na powierzchni 3 s |
| Echo | 3 | 6 | 1 | opóźnienie 2 s, rozproszenie 1,2 s |
| Łowca | 6 | 2,5 | 1 (hak) | zasięg 10, stun 2 s (×2 obrażeń) |
| Ognik | 1 | 3,5 | 3 (wybuch) | lont 0,8 s, promień 2,2 |

## 7. Pliki w paczce

| Plik | Opis |
|---|---|
| `EnemyBase.cs` | baza: HP, śmierć, tarcze, ruch, kontakt |
| `EnemyData.cs` | parametry wszystkich wrogów (ScriptableObject) |
| `IDamageable.cs`, `IPullable.cs` | interfejsy obrażeń i przyciągania |
| `StoneGuardian.cs`, `DamageZone.cs` | Strażnik i jego ślad na ziemi |
| `ShadowArcher.cs`, `EnemyProjectile.cs` | Łucznik i jego pocisk |
| `SwarmSpider.cs`, `SwarmManager.cs`, `SpiderCocoon.cs` | rój pająków |
| `RuneShaman.cs`, `ShieldBuff.cs`, `SpiritMinion.cs` | Szaman, tarcza i duchy |
| `HornedBull.cs` | Byk |
| `BurrowMole.cs` | Kret |
| `EchoPhantom.cs` | Echo |
| `ChainHunter.cs`, `ChainHook.cs` | Łowca i hak |
| `ExplosiveWisp.cs` | Ognik |
| `PlayerHealth.cs`, `PlayerPullReceiver.cs` | przykładowe komponenty gracza do testów |

## 8. Rozbudowa i ograniczenia

- Nowy wróg: dziedzicz po `EnemyBase`, zaimplementuj `Tick()`, ruch zlecaj przez `Move()`, a obrażenia przechwytuj przez `TakeDamage()`.
- Sprite wroga jest szukany jako pierwszy `SpriteRenderer` w hierarchii. Kret go ukrywa pod ziemią, a Ognik faluje nim tylko wtedy, gdy leży na obiekcie podrzędnym.
- Tarcza rysuje pierścień przez `LineRenderer` z shaderem `Sprites/Default`, więc działa bez własnych assetów.
- Gdy `EnemyData` urośnie, rozdziel je na osobne ScriptableObjecty dla każdego typu.
- Kolejne pomysły: artyleria obszarowa, elita lub mini-boss łączący 2-3 mechaniki, wróg odporny na część strategii.
