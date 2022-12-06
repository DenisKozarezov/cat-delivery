namespace Core.Player.Buffs
{
    public interface IBuff
    {
        Cooldown Duration { get; }
        void Execute();
        void Reset();
    }
}
