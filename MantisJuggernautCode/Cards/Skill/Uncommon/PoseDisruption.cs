using MantisJuggernaut.Characters;
using MantisJuggernaut.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Cards;

[RegisterCard(typeof(MantisJuggernautCardPool))]
public sealed class PoseDisruption() : MantisJuggernautSkillCard(BaseEnergyCost, CardRarityValue, CardTarget)
{
    // 基础耗能。
    private const int BaseEnergyCost = 1;

    // 卡牌稀有度。
    private const CardRarity CardRarityValue = CardRarity.Uncommon;

    // 目标类型（Self 表示自己）。
    private const TargetType CardTarget = TargetType.AnyEnemy;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips
    {
        get
        {
            var hoverTips = base.AdditionalHoverTips.ToList();
            hoverTips.Add(HoverTipFactory.FromPower<HeavyKnockDownPower>());
            hoverTips.Add(HoverTipFactory.FromPower<BackswingBalancePower>());

            return hoverTips;
        }
    }

    // 卡牌基础数值。
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];

    protected override bool ShouldGlowGoldInternal => IsBalance;

    // 打出时的效果逻辑。
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        if (cardPlay.Target.Monster?.IntendsToAttack == true)
        {
            var oblique = cardPlay.Target.GetPower<HeavyKnockObliquePower>();
            if (oblique is not null) await PowerCmd.Remove(oblique);

            await PowerCmd.Apply<HeavyKnockDownPower>(
                choiceContext,
                cardPlay.Target,
                1,
                Owner.Creature,
                this
            );
        }

        if (IsBalance) await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
    }

    // 升级后的效果逻辑。
    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}