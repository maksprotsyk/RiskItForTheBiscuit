using System.Collections.Generic;
using Characters.Inventory;
using Characters.Weapons;
using UnityEngine;

namespace Characters
{
    public class CharacterBase : MonoBehaviour
    {
        [SerializeField] private MovementComponent _movementComponent;
        [SerializeField] private WeaponController _weaponController;
        [SerializeField] private HealthComponent _healthComponent;
        [SerializeField] private CharacterStatsHub _statsHubComponent;
        [SerializeField] private InventoryComponent _inventoryComponent;

        private CharacterAnimationController _animationController;
        private List<ICharacterComponent> _characterComponents;

        public MovementComponent Movement => _movementComponent;
        public WeaponController Weapon => _weaponController;
        public HealthComponent Health => _healthComponent;
        public CharacterStatsHub StatsHub => _statsHubComponent;
        public InventoryComponent Inventory => _inventoryComponent;

        public CharacterAnimationController AnimationController => _animationController;

        public void OnAttackAnimationStarted() { _weaponController.HandleAnimatioStart(); }

        public void OnDeathAnimationFinished()
        {
            Destroy(gameObject);
        }

        private void Awake()
        {
            Animator animator = GetComponent<Animator>();
            if (!animator)
            {
                Debug.LogError("Animator component is missing on the character.");
                return;
            }
            _animationController = new CharacterAnimationController(animator);

            _characterComponents = new List<ICharacterComponent> {_statsHubComponent, _inventoryComponent, _movementComponent, _weaponController, _healthComponent };
            foreach (ICharacterComponent comp in _characterComponents)
            {
                comp.Init(this);
            }
        }

        private void Start()
        {
            foreach (ICharacterComponent comp in _characterComponents)
            {
                comp.OnStart();
            }
        }

        public void OnDestroy()
        {
            foreach (ICharacterComponent comp in _characterComponents)
            {
                comp.OnDestroy();
            }
        }

        private void FixedUpdate()
        {
            foreach (ICharacterComponent comp in _characterComponents)
            {
                comp.FixedUpdateComponent(Time.fixedDeltaTime);
            }
        }
        private void Update()
        {
            foreach (ICharacterComponent comp in _characterComponents)
            {
                comp.UpdateComponent(Time.deltaTime);
            }
        }
    }
}
