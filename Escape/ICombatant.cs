namespace Escape
{
    interface ICombatant
    {
        int Health { get; set; }
        int MaxHealth { get; }
        int Magic { get; set; }
        int Power { get; }
        int Defense { get; }
    }
}
