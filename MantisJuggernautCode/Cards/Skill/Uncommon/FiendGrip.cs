using MantisJuggernaut.Characters;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Cards;

[RegisterCard(typeof(MantisJuggernautCardPool))]
public sealed class FiendGrip() : MantisJuggernautSkillBackswingCard(BaseEnergyCost, CardRarityValue, CardTarget, true)
{
    // 基础耗能。
    private const int BaseEnergyCost = 1;

    // 卡牌稀有度。
    private const CardRarity CardRarityValue = CardRarity.Uncommon;

    // 目标类型（Self 表示自己）。
    private const TargetType CardTarget = TargetType.Self;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => base.AdditionalHoverTips.Concat(
    [
        InstinctSlash.MakeCardHoverTip(IsUpgraded, SwingRight)
    ]);

    // 卡牌基础数值。
    protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat(
    [
        new CardsVar(3)
    ]);

    // 打出时的效果逻辑。
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState is not null)
            await InstinctSlash.CreateInHand(
                Owner,
                DynamicVars.Cards.IntValue,
                CombatState,
                IsUpgraded,
                SwingRight
            );

        await base.OnPlay(choiceContext, cardPlay);
    }

    // 升级后的效果逻辑。
    protected override void OnUpgrade()
    {
    }
}