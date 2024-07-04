namespace _Project.Scripts.Interface
{
    public interface IDamager
    {
        public float DamageAmount { get; }
        public void DealDamage(IDamageable damageable);
    }
}