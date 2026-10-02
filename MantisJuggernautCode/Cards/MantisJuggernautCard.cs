using MantisJuggernaut.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Scaffolding.Content;

namespace MantisJuggernaut.Cards;

public abstract class MantisJuggernautCard(
    int baseCost,
    CardType cardType,
    CardRarity rarity,
    TargetType target,
    bool showInCardLibrary = true)
    : ModCardTemplate(baseCost, cardType, rarity, target, showInCardLibrary)
{
    public enum StanceSate
    {
        Balance,
        Imbalance,
        Left,
        Right
    }

    // 卡图资源。
    // 这里的 res://MantisJuggernaut/... 是 Godot 资源路径，对应的是你的资源文件夹名字。
    public override CardAssetProfile AssetProfile => new(
        // TODO RitsuLib analyzer: 资源路径 'res://MantisJuggernaut/images/cards/MantisJuggernautCard.png' 在项目资源索引中未找到。
        $"{Entry.ResPath}/images/cards/{GetType().Name}.png");

    public virtual bool IsPrepared => PileType.Hand.GetPile(Owner).Cards.FirstOrDefault() == this;

    public bool IsBalance => Owner.Creature.HasPower<BackswingBalancePower>();
    public bool IsImbalance => Owner.Creature.HasPower<BackswingImbalancePower>();
    public bool IsLeft => Owner.Creature.HasPower<BackswingLeftPower>();
    public bool IsRight => Owner.Creature.HasPower<BackswingRightPower>();

    // 切换状态
    public virtual async Task ToState(PlayerChoiceContext choiceContext, StanceSate state)
    {
        var creature = Owner.Creature;
        Dictionary<StanceSate, MantisJuggernautPower?> powers = new()
        {
            { StanceSate.Balance, creature.GetPower<BackswingBalancePower>() },
            { StanceSate.Imbalance, creature.GetPower<BackswingImbalancePower>() },
            { StanceSate.Left, creature.GetPower<BackswingLeftPower>() },
            { StanceSate.Right, creature.GetPower<BackswingRightPower>() }
        };
        foreach (var (st, power) in powers)
            if (st != state && power is not null)
                await PowerCmd.Remove(power);

        switch (state)
        {
            case StanceSate.Balance:
                if (!IsBalance)
                    await PowerCmd.Apply<BackswingBalancePower>(
                        choiceContext,
                        Owner.Creature,
                        1,
                        Owner.Creature,
                        this
                    );
                break;
            case StanceSate.Imbalance:
                if (!IsImbalance)
                    await PowerCmd.Apply<BackswingImbalancePower>(
                        choiceContext,
                        Owner.Creature,
                        1,
                        Owner.Creature,
                        this
                    );
                break;
            case StanceSate.Left:
                if (!IsLeft)
                    await PowerCmd.Apply<BackswingLeftPower>(
                        choiceContext,
                        Owner.Creature,
                        1,
                        Owner.Creature,
                        this
                    );
                break;
            case StanceSate.Right:
                if (!IsRight)
                    await PowerCmd.Apply<BackswingRightPower>(
                        choiceContext,
                        Owner.Creature,
                        1,
                        Owner.Creature,
                        this
                    );
                break;
        }
    }
}