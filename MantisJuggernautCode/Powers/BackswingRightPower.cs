using MantisJuggernaut.Cards;
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
public class BackswingRightPower : MantisJuggernautPower
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
        new DynamicVar(DamageDecreaseVarKey, 0.5m)
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
        if (Owner != dealer || cardSource == null)
        {
            return 1m;
        }

        if (!props.IsPoweredAttack())
        {
            return 1m;
        }

        if (cardSource is not MantisJuggernautAttackBackswingCard bsCard)
        {
            return 1m;
        }

        return bsCard.SwingRight
            ? DynamicVars[DamageDecreaseVarKey].BaseValue
            : DynamicVars[DamageIncreaseVarKey].BaseValue;
    }

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature? dealer,
        DamageResult result,
        ValueProp props,
        Creature? target,
        CardModel? cardSource
    )
    {
        if (!CombatManager.Instance.IsInProgress)
        {
            return;
        }

        if (!props.IsPoweredAttack() || target != Owner || Owner.Player is null)
        {
            return;
        }

        if (result.WasFullyBlocked)
        {
            return;
        }

        bool toBalance = Owner.Player.RunState.Rng.CombatTargets.NextBool();
        await PowerCmd.Remove(this);
        if (toBalance)
        {
            await PowerCmd.Apply<BackswingBalancePower>(choiceContext, Owner, 1m, Owner, null);
        }
        else
        {
            await PowerCmd.Apply<BackswingImbalancePower>(choiceContext, Owner, 1m, Owner, null);
        }
    }
}