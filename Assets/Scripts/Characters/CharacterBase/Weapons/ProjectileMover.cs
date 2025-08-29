using UnityEngine;

namespace Characters.Weapons
{
    public class ProjectileMover : MonoBehaviour
    {
        private Vector2 _dir;
        private float _speed;
        private float _maxDistance;
        private Vector3 _start;

        public void Init(Vector2 dir, float speed, float maxDistance)
        {
            _dir = dir.normalized; _speed = speed; _maxDistance = maxDistance; _start = transform.position;
            var rb = GetComponent<Rigidbody2D>();
            if (rb) rb.velocity = _dir * _speed;
        }

        private void Update()
        {
            if (_maxDistance > 0f && (transform.position - _start).sqrMagnitude >= _maxDistance * _maxDistance)
                Destroy(gameObject);
            else if (!GetComponent<Rigidbody2D>())
                transform.position += (Vector3)(_dir * _speed * Time.deltaTime);
        }
    }
}