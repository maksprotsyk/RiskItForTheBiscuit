using UnityEngine;

namespace Characters.Combat
{
    public struct DamagePayload
    {
        public float Amount;
        public Vector2 Direction;
        public object Source;

        public DamagePayload(float amount, Vector2 direction, object source)
        {
            Amount = amount;
            Direction = direction;
            Source = source;
        }
    }
}