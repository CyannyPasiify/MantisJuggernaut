using MegaCrit.Sts2.Core.Entities.Cards;

namespace MantisJuggernaut.Cards;

public abstract class MantisJuggernautAttackCard(
    int baseCost,
    CardRarity rarity,
    TargetType target,
    bool showInCardLibrary = true)
    : MantisJuggernautCard(baseCost, CardType.Attack, rarity, target, showInCardLibrary)
{
}