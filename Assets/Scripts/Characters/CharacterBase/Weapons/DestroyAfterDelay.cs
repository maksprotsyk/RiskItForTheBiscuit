using System.Collections;
using Characters.Combat;
using UnityEngine;

namespace Characters.Weapons
{
    public class DestroyAfterDelay : MonoBehaviour
    {
        [SerializeField] private float _lifeDuration;

        private void Awake()
        {
            StartCoroutine(WaitAndDestroy(_lifeDuration));
        }
        private IEnumerator WaitAndDestroy(float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(gameObject);
        }
    }
}