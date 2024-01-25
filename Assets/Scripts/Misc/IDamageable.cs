using Photon.Realtime;

public interface IDamageable
{
    void TakeDamage(float damage, Player shooter);
}