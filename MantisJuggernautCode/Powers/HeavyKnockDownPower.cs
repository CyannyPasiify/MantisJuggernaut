using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Powers;

[RegisterPower]
public class HeavyKnockDownPower : MantisJuggernautPower
{
    public const string DamageIncreaseVarKey = "DamageIncrease";

    public const string DamageDecreaseVarKey = "DamageDecrease";

    // 类型，Buff或Debuff
    public override PowerType Type => PowerType.Debuff;

    // 叠加类型，Counter表示可叠加，Single表示不可叠加
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.None;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(DamageIncreaseVarKey, 1.5m),
        new(DamageDecreaseVarKey, 0.5m)
    ];

    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay
    )
    {
        if (!props.IsPoweredAttack()) return 1m;

        if (Owner == dealer) return (decimal)Math.Pow((double)DynamicVars[DamageDecreaseVarKey].BaseValue, Amount);

        if (Owner == target) return 1m + (DynamicVars[DamageIncreaseVarKey].BaseValue - 1m) * Amount;

        return 1m;
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Enemy)
            return;
        await PowerCmd.Remove(this);
    }
}