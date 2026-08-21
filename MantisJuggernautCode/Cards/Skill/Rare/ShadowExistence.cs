using MantisJuggernaut.Characters;
using MantisJuggernaut.HoverTips;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Cards;

[RegisterCard(typeof(MantisJuggernautCardPool))]
public sealed class ShadowExistence()
    : MantisJuggernautSkillCard(BaseEnergyCost, CardRarityValue, CardTarget)
{
    // 基础耗能。
    private const int BaseEnergyCost = 4;

    // 卡牌稀有度。
    private const CardRarity CardRarityValue = CardRarity.Rare;

    // 目标类型。
    private const TargetType CardTarget = TargetType.Self;

    protected override bool ShouldGlowGoldInternal => IsPrepared;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => base.AdditionalHoverTips.Concat(
    [
        ExtHoverTipFactory.Static(ExtStaticHoverTip.Prepared),
        InstinctSlash.MakeCardHoverTip(IsUpgraded)
    ]);

    // 卡牌基础数值。
    protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat(
    [
        new CardsVar(1)
    ]);

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Unplayable
    ];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Player != Owner ||
            !IsPrepared ||
            cardPlay.Card is InstinctSlash itSlash && itSlash.IsUpgraded == IsUpgraded ||
            CombatState is null)
            return;

        await InstinctSlash.CreateInHand(
            Owner,
            DynamicVars.Cards.IntValue,
            CombatState,
            IsUpgraded,
            false,
            true
        );
    }

    // 升级后的效果逻辑。
    protected override void OnUpgrade()
    {
    }
}