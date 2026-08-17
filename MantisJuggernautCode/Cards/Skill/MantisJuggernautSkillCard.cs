using MegaCrit.Sts2.Core.Entities.Cards;

namespace MantisJuggernaut.Cards;

public abstract class MantisJuggernautSkillCard(
    int baseCost,
    CardRarity rarity,
    TargetType target,
    bool showInCardLibrary = true)
    : MantisJuggernautCard(baseCost, CardType.Skill, rarity, target, showInCardLibrary)
{
}