using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Powers;

[RegisterPower]
public class AtemiWazaPower : MantisJuggernautPower
{
    // 类型，Buff或Debuff
    public override PowerType Type => PowerType.Buff;

    // 叠加类型，Counter表示可叠加，Single表示不可叠加
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.None;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Block),
        HoverTipFactory.FromPower<BackswingBalancePower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
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
        if (target != Owner || dealer is null || !result.WasFullyBlocked) return;

        if (Owner.GetPower<BackswingImbalancePower>() is not null)
            await PowerCmd.Remove<BackswingImbalancePower>(Owner);

        if (Owner.GetPower<BackswingLeftPower>() is not null) await PowerCmd.Remove<BackswingLeftPower>(Owner);

        if (Owner.GetPower<BackswingRightPower>() is not null) await PowerCmd.Remove<BackswingRightPower>(Owner);

        if (Owner.GetPower<BackswingBalancePower>() is null)
            await PowerCmd.Apply<BackswingBalancePower>(
                choiceContext,
                Owner,
                1m,
                Owner,
                null
            );

        await CreatureCmd.Damage(choiceContext, dealer, Amount, ValueProp.Unpowered, Owner, null, null);
    }
}