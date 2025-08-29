using Characters.Combat;
using UnityEngine;

namespace Characters.Weapons
{
    public class DamageOnHit : MonoBehaviour, IDamageWriter
    {
        [SerializeField] private LayerMask _hurtLayers;
        private DamagePayload _payload;

        public void SetPayload(DamagePayload payload) => _payload = payload;

        private void OnTriggerEnter2D(Collider2D other) => TryDamage(other);
        private void OnCollisionEnter2D(Collision2D other) => TryDamage(other.collider);

        private void TryDamage(Collider2D col)
        {
            if (((1 << col.gameObject.layer) & _hurtLayers.value) == 0) return;

            // 1) Prefer a custom receiver if present
            var receiver = col.GetComponentInParent<IDamageReceiver>();
            if (receiver != null)
            {
                receiver.ReceiveDamage(_payload);
                return;
            }

            // 2) Fallback to HealthComponent directly
            float amount = _payload.Amount;

            var health = col.GetComponentInParent<HealthComponent>();
            health?.TakeDamage(amount);
        }
    }
}