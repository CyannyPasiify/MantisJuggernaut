using MantisJuggernaut.Characters;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Cards;

[RegisterCard(typeof(MantisJuggernautCardPool))]
public sealed class Improvisation() : MantisJuggernautSkillCard(BaseEnergyCost, CardRarityValue, CardTarget)
{
    // 基础耗能。
    private const int BaseEnergyCost = 2;

    // 卡牌稀有度。
    private const CardRarity CardRarityValue = CardRarity.Rare;

    // 目标类型（Self 表示自己）。
    private const TargetType CardTarget = TargetType.Self;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => base.AdditionalHoverTips.Concat(
    [
        InstinctSlash.MakeCardHoverTip()
    ]);

    // 卡牌基础数值。
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
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
            var num = CardPile.MaxCardsInHand - Owner.PlayerCombatState.Hand.Cards.Count;
            for (var i = 0; i < num; i++)
            {
                await InstinctSlash.CreateInHand(
                    Owner,
                    CombatState,
                    false,
                    false,
                    true
                );
                await Cmd.CustomScaledWait(0.1f, 0.2f);
            }
        }
    }

    // 升级后的效果逻辑。
    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}