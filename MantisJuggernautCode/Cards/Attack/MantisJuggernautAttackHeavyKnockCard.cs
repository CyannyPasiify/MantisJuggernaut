using MantisJuggernaut.DynamicVars;
using MantisJuggernaut.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace MantisJuggernaut.Cards;

public abstract class MantisJuggernautAttackHeavyKnockCard(
    int baseCost,
    CardRarity rarity,
    TargetType target,
    bool showInCardLibrary = true
) : MantisJuggernautAttackCard(baseCost, rarity, target, showInCardLibrary), IHeavyKnock
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => base.AdditionalHoverTips.Concat(
    [
        HoverTipFactory.FromPower<HeavyKnockObliquePower>(),
        HoverTipFactory.FromPower<HeavyKnockDownPower>()
    ]);

    // 卡牌基础数值。
    protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat(
    [
        new HeavyKnockVar()
    ]);

    public decimal HeavyKnockAmount => DynamicVars[HeavyKnockVar.Key].BaseValue;

    public async Task ApplyHeavyKnock(PlayerChoiceContext choiceContext, Creature target,
        decimal amount,
        Creature? applier,
        CardModel? cardSource
    )
    {
        if (amount <= 0) return;

        if (target.HasPower<HeavyKnockDownPower>())
        {
            await PowerCmd.Apply<HeavyKnockDownPower>(
                choiceContext,
                target,
                amount,
                applier,
                cardSource
            );
        }
        else if (target.HasPower<HeavyKnockObliquePower>())
        {
            await PowerCmd.Remove<HeavyKnockObliquePower>(target);
            await PowerCmd.Apply<HeavyKnockDownPower>(
                choiceContext,
                target,
                amount,
                applier,
                cardSource
            );
        }
        else if (amount == 1)
        {
            await PowerCmd.Apply<HeavyKnockObliquePower>(
                choiceContext,
                target,
                amount,
                applier,
                cardSource
            );
        }
        else
        {
            await PowerCmd.Apply<HeavyKnockDownPower>(
                choiceContext,
                target,
                amount - 1m,
                applier,
                cardSource
            );
        }
    }
}