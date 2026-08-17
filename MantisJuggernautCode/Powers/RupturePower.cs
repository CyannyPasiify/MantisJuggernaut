using MantisJuggernaut.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Powers;

[RegisterPower]
public class RupturePower : MantisJuggernautPower
{
    // 类型，Buff或Debuff
    public override PowerType Type => PowerType.Debuff;

    // 叠加类型，Counter表示可叠加，Single表示不可叠加
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.None;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
    ];

    public const string RuptureIncreaseVarKey = "RuptureIncrease";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar(RuptureIncreaseVarKey, 1)
    ];

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource
    )
    {
        if (target != Owner || !props.IsPoweredAttack())
        {
            return;
        }

        await CreatureCmd.Damage(
            choiceContext,
            Owner,
            Amount,
            ValueProp.Unpowered,
            null,
            null
        );

        await PowerCmd.Apply<RupturePower>(
            choiceContext,
            Owner,
            DynamicVars[RuptureIncreaseVarKey].BaseValue,
            Owner,
            null
        );
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (participants.Contains(Owner))
        {
            if (Amount > 1)
            {
                await PowerCmd.ModifyAmount(
                    choiceContext,
                    this,
                    1 - Amount,
                    Owner,
                    null
                );
            }
        }
    }
}