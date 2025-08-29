using UnityEngine;

namespace Characters.Weapons
{
    public abstract class WeaponLogicBase : ScriptableObject
    {
        public abstract IWeaponInstance CreateInstance(WeaponRuntimeContext ctx);
    }
}