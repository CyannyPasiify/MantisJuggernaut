using MantisJuggernaut.Characters;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Cards;

// RegisterCard 会把这张牌交给 RitsuLib 自动注册。
[RegisterCard(typeof(MantisJuggernautCardPool))]
public sealed class JointStrike() : MantisJuggernautAttackCard(BaseEnergyCost, CardRarityValue, CardTarget)
{
    // 基础耗能。
    private const int BaseEnergyCost = 1;

    // 卡牌稀有度。
    private const CardRarity CardRarityValue = CardRarity.Common;

    // 目标类型（AnyEnemy 表示任意敌人）。
    private const TargetType CardTarget = TargetType.AnyEnemy;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => base.AdditionalHoverTips.Concat(
    [
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<VulnerablePower>()
    ]);

    // 卡牌基础数值。
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7, ValueProp.Move),
        new PowerVar<WeakPower>(1m),
        new PowerVar<VulnerablePower>(1m)
    ];

    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];

    // 打出时的效果逻辑。
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        if (cardPlay.Target.Monster?.IntendsToAttack ?? false)
            await PowerCmd.Apply<WeakPower>(
                choiceContext,
                cardPlay.Target,
                DynamicVars.Weak.BaseValue,
                Owner.Creature,
                this
            );
        else
            await PowerCmd.Apply<VulnerablePower>(
                choiceContext,
                cardPlay.Target,
                DynamicVars.Vulnerable.BaseValue,
                Owner.Creature,
                this
            );
    }

    // 升级后的效果逻辑。
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars.Weak.UpgradeValueBy(1m);
        DynamicVars.Vulnerable.UpgradeValueBy(1m);
    }
}