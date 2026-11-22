namespace FrostGame.Health
{
    /// <summary>
    /// Interfejs dla obiektów, które mogą otrzymywać obrażenia.
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(int amount);
    }
}
