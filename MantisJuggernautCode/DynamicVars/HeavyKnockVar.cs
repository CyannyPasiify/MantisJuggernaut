namespace MantisJuggernaut.DynamicVars;

public class HeavyKnockVar : MantisJuggernautToolTipVar
{
    public const string Key = "HeavyKnock";

    public HeavyKnockVar(decimal baseValue = 1m) : base(Key, baseValue)
    {
    }
}