using MantisJuggernaut.Characters;
using MantisJuggernaut.HoverTips;
using MantisJuggernaut.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Cards;

[RegisterCard(typeof(MantisJuggernautCardPool))]
public sealed class WindAxe()
    : MantisJuggernautAttackHeavyKnockCard(BaseEnergyCost, CardRarityValue, CardTarget)
{
    // 基础耗能。
    private const int BaseEnergyCost = 2;

    // 卡牌稀有度。
    private const CardRarity CardRarityValue = CardRarity.Rare;

    // 目标类型。
    private const TargetType CardTarget = TargetType.AnyEnemy;

    protected override bool ShouldGlowGoldInternal => IsPrepared;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => base.AdditionalHoverTips.Concat(
    [
        HoverTipFactory.FromPower<SwiftPower>(),
        ExtHoverTipFactory.Static(ExtStaticHoverTip.Prepared)
    ]);

    // 卡牌基础数值。
    protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat(
    [
        new DamageVar(6m, ValueProp.Move),
        new PowerVar<SwiftPower>(1m)
    ]);

    // 打出时的效果逻辑。
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Creature? target = cardPlay.Target;
        if (target is null)
        {
            if (CombatState is not null)
            {
                target = Owner.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
            }

            ArgumentNullException.ThrowIfNull(target);
        }

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(target)
            .WithHitFx("vfx/vfx_heavy_blunt", null, "heavy_attack.mp3")
            .Execute(choiceContext);
        if (IsUpgraded) await ApplyHeavyKnock(choiceContext, target, HeavyKnockAmount, Owner.Creature, this);

        await PowerCmd.Apply<SwiftPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(SwiftPower)].BaseValue,
            Owner.Creature,
            this
        );
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Player != Owner || !IsPrepared || cardPlay.Card == this) return;
        await OnPlay(choiceContext, cardPlay);
    }

    // 升级后的效果逻辑。
    protected override void OnUpgrade()
    {
    }
}