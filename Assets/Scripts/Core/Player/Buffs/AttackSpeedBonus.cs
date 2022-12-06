using UnityEngine;
using Core.Weapons;

namespace Core.Player.Buffs
{
    public class AttackSpeedBonus : IBuff
    {
        private float _oldReloadTime;
        private readonly float _attackBonus;
        private readonly IWeapon _weapon;
        public Cooldown Duration { get; private set; }

        public AttackSpeedBonus(float attackBonus, IWeapon weapon)
        {
            _attackBonus = attackBonus;
            _weapon = weapon;
            Duration = new Cooldown();
        }

        public void Execute()
        {
            _oldReloadTime = _weapon.ReloadTime;
            _weapon.ReloadTime = Mathf.Clamp(_weapon.ReloadTime, 0.3f, _weapon.ReloadTime - _attackBonus);
        }
        public void Reset()
        {
            _weapon.ReloadTime = _oldReloadTime;
        }
    }
}
