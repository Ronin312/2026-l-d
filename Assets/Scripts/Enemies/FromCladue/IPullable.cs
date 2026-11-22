using UnityEngine;

/// <summary>
/// Implementuje obiekt, który można przyciągnąć (gracz). Używa tego ChainHook Łowcy z Łańcuchem.
/// </summary>
public interface IPullable
{
    /// <summary>Przyciąga obiekt w stronę kotwicy z zadaną prędkością, maksymalnie przez duration sekund.</summary>
    void ApplyPull(Transform anchor, float speed, float duration, float stopDistance);

    void CancelPull();
}
