using MegaCrit.Sts2.Core.Entities.Cards;

namespace MantisJuggernaut.Cards;

public abstract class MantisJuggernautPowerCard(
    int baseCost,
    CardRarity rarity,
    TargetType target,
    bool showInCardLibrary = true)
    : MantisJuggernautCard(baseCost, CardType.Power, rarity, target, showInCardLibrary)
{
}