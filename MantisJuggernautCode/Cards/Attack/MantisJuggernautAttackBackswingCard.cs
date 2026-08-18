using MantisJuggernaut.DynamicVars;
using MantisJuggernaut.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace MantisJuggernaut.Cards;

public abstract class MantisJuggernautAttackBackswingCard(
    int baseCost,
    CardRarity rarity,
    TargetType target,
    bool swingRight = false,
    bool showInCardLibrary = true
) : MantisJuggernautCard(baseCost, CardType.Attack, rarity, target, showInCardLibrary)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => base.AdditionalHoverTips.Concat(
    [
        // HoverTipFactory.FromPower<BackswingBalancePower>(),
        // HoverTipFactory.FromPower<BackswingLeftPower>(),
        // HoverTipFactory.FromPower<BackswingRightPower>(),
        // HoverTipFactory.FromPower<BackswingImbalancePower>()
    ]);

    // 卡牌基础数值。
    protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat(
    [
        new BackswingVar(swingRight)
    ]);

    // 何时发金色光
    protected override bool ShouldGlowGoldInternal =>
        SwingRight ? Owner.Creature.HasPower<BackswingLeftPower>() : Owner.Creature.HasPower<BackswingRightPower>();

    // 何时发红色光
    protected override bool ShouldGlowRedInternal =>
        SwingRight ? Owner.Creature.HasPower<BackswingRightPower>() : Owner.Creature.HasPower<BackswingLeftPower>();

    public bool SwingRight
    {
        get => ((BackswingVar)DynamicVars[BackswingVar.Key]).BoolVal;
        // 设置方向 false为向左，true为向右
        set
        {
            var swingVar = (BackswingVar)DynamicVars[BackswingVar.Key];
            swingVar.BoolVal = value;
        }
    }

    // 打出时的效果逻辑。
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await UpdateStateShiftSwing(choiceContext);
    }

    // 切换方向
    public virtual void ShiftSwing()
    {
        var swingVar = (BackswingVar)DynamicVars[BackswingVar.Key];
        swingVar.BoolVal = !swingVar.BoolVal;
        if (Pile is not null) NCard.FindOnTable(this)?.UpdateVisuals(Pile.Type, CardPreviewMode.Normal);
    }

    // 修正状态
    public virtual async Task UpdateState(PlayerChoiceContext choiceContext)
    {
        var swingR = SwingRight;
        var creature = Owner.Creature;
        if (creature.HasPower<BackswingBalancePower>())
        {
            await PowerCmd.Remove<BackswingBalancePower>(creature);
            if (swingR)
                await PowerCmd.Apply<BackswingRightPower>(choiceContext, creature, 1m, creature, this);
            else
                await PowerCmd.Apply<BackswingLeftPower>(choiceContext, creature, 1m, creature, this);
        }
        else if (creature.HasPower<BackswingLeftPower>())
        {
            await PowerCmd.Remove<BackswingLeftPower>(creature);
            if (swingR)
                await PowerCmd.Apply<BackswingBalancePower>(choiceContext, creature, 1m, creature, this);
            else
                await PowerCmd.Apply<BackswingImbalancePower>(choiceContext, creature, 1m, creature, this);
        }
        else if (creature.HasPower<BackswingRightPower>())
        {
            await PowerCmd.Remove<BackswingRightPower>(creature);
            if (swingR)
                await PowerCmd.Apply<BackswingImbalancePower>(choiceContext, creature, 1m, creature, this);
            else
                await PowerCmd.Apply<BackswingBalancePower>(choiceContext, creature, 1m, creature, this);
        }
    }

    public virtual async Task UpdateStateShiftSwing(PlayerChoiceContext choiceContext)
    {
        await UpdateState(choiceContext);
        ShiftSwing();
    }
}