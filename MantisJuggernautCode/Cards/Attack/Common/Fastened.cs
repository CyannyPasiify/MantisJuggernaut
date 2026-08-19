using MantisJuggernaut.Characters;
using MantisJuggernaut.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Cards;

// RegisterCard 会把这张牌交给 RitsuLib 自动注册。
[RegisterCard(typeof(MantisJuggernautCardPool))]
public sealed class Fastened()
    : MantisJuggernautAttackBackswingCard(BaseEnergyCost, CardRarityValue, CardTarget)
{
    // 基础耗能。
    private const int BaseEnergyCost = 1;

    // 卡牌稀有度。
    private const CardRarity CardRarityValue = CardRarity.Common;

    // 目标类型（AnyEnemy 表示任意敌人）。
    private const TargetType CardTarget = TargetType.AnyEnemy;

    protected override bool ShouldGlowGoldInternal
    {
        get
        {
            if (CombatState == null) return false;

            return CombatState.HittableEnemies.Any(e =>
                e.HasPower<HeavyKnockObliquePower>() || e.HasPower<HeavyKnockDownPower>());
        }
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => base.AdditionalHoverTips.Concat(
    [
        HoverTipFactory.FromPower<HeavyKnockObliquePower>(),
        HoverTipFactory.FromPower<HeavyKnockDownPower>(),
        EnergyHoverTip
    ]);

    // 卡牌基础数值。
    protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat(
    [
        new DamageVar(8, ValueProp.Move),
        new CardsVar(1),
        new EnergyVar(1)
    ]);

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
    ];

    // 打出时的效果逻辑。
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        if (cardPlay.Target.HasPower<HeavyKnockObliquePower>() || cardPlay.Target.HasPower<HeavyKnockDownPower>())
        {
            var cards = PileType.Hand.GetPile(Owner).Cards
                .Where(e => !e.EnergyCost.CostsX && e.EnergyCost.GetResolved() > 0)
                .ToList();
            for (var i = 0; i < DynamicVars.Cards.IntValue && cards.Count > 0; i++)
            {
                var selIdx = Owner.RunState.Rng.CombatCardSelection.NextInt(0, cards.Count);
                cards[selIdx].EnergyCost.AddUntilPlayed(-DynamicVars.Energy.IntValue);
                cards.RemoveAt(selIdx);
            }
        }
        
        await base.OnPlay(choiceContext, cardPlay);
    }

    // 升级后的效果逻辑。
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}