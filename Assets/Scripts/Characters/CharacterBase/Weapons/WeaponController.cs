using System;
using DataStorage;
using DataStorage.Generated;
using Items;
using Managers;
using UnityEngine;

namespace Characters.Weapons
{
    [Serializable]
    public class WeaponController : BaseCharacterComponent
    {
        [Header("Animation parameters")]
        [SerializeField] private AnimationParameters _attackTrigger = AnimationParameters.Attack; // reuse your constants

        private Vector2 _attackBaseOffset;

        private AudioManager _audioManager;
        private IWeaponInstance _instance;
        private WeaponDefinition _equipped;
        private Vector2 _aim = Vector2.up;

        // Cached stats
        private float _cooldownBase;
        private float _cooldownTimer;

        public WeaponDefinition Equipped => _equipped;

        public override void Init(CharacterBase characterBase)
        {
            base.Init(characterBase);
            _cooldownTimer = 0f;
            SpriteRenderer spriteRenderer = characterBase.GetComponent<SpriteRenderer>();
            _attackBaseOffset = spriteRenderer.bounds.center - characterBase.transform.position;
        }

        public override void OnStart()
        {
            base.OnStart();
            _audioManager = ManagersOwner.GetManager<AudioManager>();

            SyncEquippedFromInventory(); // initial
            RebuildCache();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Unequip();
        }

        public override void UpdateComponent(float dt)
        {
            if (_cooldownTimer > 0f)
                _cooldownTimer = Mathf.Max(0f, _cooldownTimer - dt);

            _instance?.SetAim(_aim);
            _instance?.Tick(dt);
        }

        public void SetAimDirection(Vector2 dir)
        {
            if (dir.sqrMagnitude > 0.0001f) _aim = dir.normalized;
        }

        public bool AttackIsReady()
        {
            if (_character.Health.IsDead) return false;
            if (_cooldownTimer > 0f) return false;
            return _instance?.CanFire() ?? false;
        }
        
        public bool IsInRange(CharacterBase other)
        {
            if (!other) return false;

            float range = GetEffectiveRange();
            if (range <= 0f) return false;

            var myCol = _character.GetComponent<Collider2D>();
            var otherCol = other.GetComponent<Collider2D>();
            if (myCol && otherCol)
            {
                Vector2 p1 = myCol.ClosestPoint(other.transform.position);
                Vector2 p2 = otherCol.ClosestPoint(_character.transform.position);
                return (p2 - p1).sqrMagnitude <= range * range;
            }

            return (other.transform.position - _character.transform.position).sqrMagnitude <= range * range;
        }
        
        public float GetEffectiveRange()
        {
            if (_instance != null)
                return Mathf.Max(0f, _instance.GetEffectiveRange());

            return _equipped ? Mathf.Max(0f, _equipped.Range) : 0.5f; // small default for melee
        }

        public void PerformAttack()
        {
            if (!AttackIsReady()) return;

            // Animation-driven by default; still allow logic to fire immediately if it wants.
            _character.AnimationController.SetTrigger(_attackTrigger);
            _audioManager.PlaySound(_equipped.AttackSound, _character.transform, (prevSound) => { return TableID.NONE; });
            _cooldownTimer = Mathf.Max(0f, _cooldownBase);
            _instance?.Fire();
        }

        public void HandleAnimatioStart() => _instance?.OnAnimationStart();

        public void Equip(WeaponDefinition def)
        {
            if (def == _equipped) return;
            Unequip();

            if (def == null || def.Logic == null)
            {
                Debug.LogWarning("WeaponController: null weapon or logic", _character);
                return;
            }

            _equipped = def;

            var ctx = new WeaponRuntimeContext(
                _character,
                _equipped,
                _character.StatsHub,
                _character.transform,
                _attackBaseOffset,
                () => _aim
            );

            _instance = _equipped.Logic.CreateInstance(ctx);
            _instance.OnEquip();

            RebuildCache();
        }

        public void Unequip()
        {
            _instance?.Dispose();
            _instance = null;
            _equipped = null;
        }

        // ---- internals ----
        protected override void AddListeners()
        {
            _character.StatsHub.Stats.OnStatChanged += OnStatChanged;
            _character.Inventory.OnChanged += SyncEquippedFromInventory;
        }

        protected override void RemoveListeners()
        {
            _character.StatsHub.Stats.OnStatChanged -= OnStatChanged;
            _character.Inventory.OnChanged -= SyncEquippedFromInventory;
        }

        private void RebuildCache()
        {
            // You can mix item stats and weapon base here.
            float cdStat = _character.StatsHub.Stats.Get(DataStorage.Generated.StatsDef.AttackCooldown);
            // Combine SO base and stats (simple example: take max)
            _cooldownBase = Mathf.Max(0f, (_equipped ? _equipped.Cooldown : 0f), cdStat);
        }

        private void OnStatChanged(DataStorage.Generated.StatsDef stat)
        {
            if (stat == DataStorage.Generated.StatsDef.AttackCooldown) RebuildCache();
        }

        private void SyncEquippedFromInventory()
        {
            // pick first weapon from top row, or null if none
            for (var c = 0; c < Inventory.InventoryGridRuntime.Cols; c++)
            {
                var item = _character.Inventory.UI_Get(0, c) as WeaponDefinition;
                if (item)
                {
                    Equip(item);
                    return;
                }
            }
            Equip(null);
        }
    }
}
