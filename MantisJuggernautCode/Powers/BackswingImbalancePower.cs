using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Powers;

[RegisterPower]
public class BackswingImbalancePower : MantisJuggernautPower
{
    // 类型，Buff或Debuff
    public override PowerType Type => PowerType.Buff;

    // 叠加类型，Counter表示可叠加，Single表示不可叠加
    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerInstanceType InstanceType => PowerInstanceType.None;

    public const string DamageIncreaseVarKey = "DamageIncrease";

    public const string DamageDecreaseVarKey = "DamageDecrease";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(DamageIncreaseVarKey, 1.5m),
        new DynamicVar(DamageDecreaseVarKey, 0.5m),
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
        if (!props.IsPoweredAttack())
        {
            return 1m;
        }

        if (Owner == dealer)
        {
            return DynamicVars[DamageDecreaseVarKey].BaseValue;
        }

        if (Owner == target)
        {
            return DynamicVars[DamageIncreaseVarKey].BaseValue;
        }

        return 1m;
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (participants.Contains(Owner))
        {
            await PowerCmd.Remove(this);
            await PowerCmd.Apply<BackswingBalancePower>(
                new ThrowingPlayerChoiceContext(),
                Owner,
                Amount,
                Owner,
                null
            );
        }
    }
}