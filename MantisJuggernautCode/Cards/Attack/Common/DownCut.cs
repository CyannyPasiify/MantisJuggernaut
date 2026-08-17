using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using MantisJuggernaut.Characters;
using MantisJuggernaut.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace MantisJuggernaut.Cards;

[RegisterCard(typeof(MantisJuggernautCardPool))]
public sealed class DownCut()
    : MantisJuggernautAttackHeavyKnockCard(BaseEnergyCost, CardRarityValue, CardTarget)
{
    // 基础耗能。
    private const int BaseEnergyCost = 1;

    // 卡牌稀有度。
    private const CardRarity CardRarityValue = CardRarity.Common;

    // 目标类型。
    private const TargetType CardTarget = TargetType.AnyEnemy;

    protected override bool ShouldGlowGoldInternal => IsBalance;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        new List<IHoverTip>
        {
            HoverTipFactory.FromPower<BackswingBalancePower>()
        }.Concat(base.AdditionalHoverTips);

    // 卡牌基础数值。
    protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat(
    [
        new DamageVar(9m, ValueProp.Move)
    ]);

    // 打出时的效果逻辑。
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var atkCmd = DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target);

        bool isBalance = IsBalance;
        if (isBalance)
        {
            atkCmd.WithHitFx("vfx/vfx_heavy_blunt", null, "heavy_attack.mp3");
        }
        else
        {
            atkCmd.WithHitFx("vfx/vfx_attack_slash");
        }

        await atkCmd.Execute(choiceContext);

        if (isBalance)
        {
            await ApplyHeavyKnock(choiceContext, cardPlay.Target, HeavyKnockAmount, Owner.Creature, this);
        }
    }

    // 升级后的效果逻辑。
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}