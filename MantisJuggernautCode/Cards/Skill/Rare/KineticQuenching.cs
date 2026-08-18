using MantisJuggernaut.Characters;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Cards;

[RegisterCard(typeof(MantisJuggernautCardPool))]
public sealed class KineticQuenching() : MantisJuggernautSkillCard(BaseEnergyCost, CardRarityValue, CardTarget)
{
    // 基础耗能。
    private const int BaseEnergyCost = 1;

    // 卡牌稀有度。
    private const CardRarity CardRarityValue = CardRarity.Rare;

    // 目标类型（Self 表示自己）。
    private const TargetType CardTarget = TargetType.Self;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => base.AdditionalHoverTips.Concat(
        HoverTipFactory.FromEnchantment<Momentum>(DynamicVars[nameof(Momentum)].IntValue)
    );

    // 卡牌基础数值。
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(nameof(Momentum), 5m)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    // 打出时的效果逻辑。
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        if (CombatState is not null && Owner.PlayerCombatState is not null)
        {
            EnchantmentModel enchant = ModelDb.Enchantment<Momentum>();
            var handEnchant = Owner.PlayerCombatState.Hand.Cards.Where(enchant.CanEnchant);
            foreach (var card in handEnchant) CardCmd.Enchant<Momentum>(card, DynamicVars[nameof(Momentum)].IntValue);
        }
    }

    // 升级后的效果逻辑。
    protected override void OnUpgrade()
    {
        DynamicVars[nameof(Momentum)].UpgradeValueBy(3m);
    }
}