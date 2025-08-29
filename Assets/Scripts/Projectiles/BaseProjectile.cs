using System.Collections;
using System.Collections.Generic;
using Characters;
using UnityEngine;

namespace Projectiles
{
    public class BaseProjectile : MonoBehaviour
    {
        private float _damage = 10f;

        public void SetDamage(float value)
        {
            _damage = value;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            CharacterBase character = collision.GetComponent<CharacterBase>();
            if (character)
            {
                character.Health.TakeDamage(_damage);
                Debug.Log("Damaged someone for " + _damage);
            }
        }

    }
}
