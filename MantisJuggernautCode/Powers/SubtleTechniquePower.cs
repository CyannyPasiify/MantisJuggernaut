using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Powers;

[RegisterPower]
public class SubtleTechniquePower : MantisJuggernautPower
{
    // 类型，Buff或Debuff
    public override PowerType Type => PowerType.Buff;

    // 叠加类型，Counter表示可叠加，Single表示不可叠加
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.None;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<BackswingImbalancePower>(),
        HoverTipFactory.FromPower<BackswingBalancePower>(),
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource
    )
    {
        if (power.Owner == Owner && power is BackswingImbalancePower)
        {
            int num = CombatManager.Instance.History.Entries.OfType<PowerReceivedEntry>()
                .Count(e => e.HappenedThisTurn(CombatState) && e.Power is BackswingImbalancePower && e.Actor == Owner);
            if (num <= 1)
            {
                Flash();
                await PowerCmd.Remove(power);
                await PowerCmd.Apply<BackswingBalancePower>(
                    choiceContext,
                    Owner,
                    1,
                    Owner,
                    null
                );

                await PowerCmd.Apply<SubtleTechniqueTempBoostPower>(
                    choiceContext,
                    Owner,
                    Amount,
                    Owner,
                    null
                );
            }
        }
    }
}