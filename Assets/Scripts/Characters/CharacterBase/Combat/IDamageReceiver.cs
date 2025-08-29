namespace Characters.Combat
{
    public interface IDamageReceiver
    {
        void ReceiveDamage(DamagePayload payload);
    }
}