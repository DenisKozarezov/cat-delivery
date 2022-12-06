namespace Core.Player.Buffs
{
    public class MovementSpeedBonus : IBuff
    {
        private readonly float _movementBonus;
        private readonly PlayerModel _playerModel;
        private float _oldMovementSpeed;
        public Cooldown Duration { get; private set; }

        public MovementSpeedBonus(float speedBonus, PlayerModel model)
        {
            _movementBonus = speedBonus;
            _playerModel = model;
            Duration = new Cooldown();
        }

        public void Execute()
        {
            _oldMovementSpeed = _playerModel.MovementSpeed;
            _playerModel.MovementSpeed += _movementBonus;
        }
        public void Reset()
        {
            _playerModel.MovementSpeed = _oldMovementSpeed;
        }
    }
}
