using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using MantisJuggernaut.Characters;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace MantisJuggernaut.Cards;

[RegisterCard(typeof(MantisJuggernautCardPool))]
public sealed class WhiffPunish()
    : MantisJuggernautAttackBackswingHeavyKnockCard(BaseEnergyCost, CardRarityValue, CardTarget, true)
{
    // 基础耗能。
    private const int BaseEnergyCost = 2;

    // 卡牌稀有度。
    private const CardRarity CardRarityValue = CardRarity.Common;

    // 目标类型。
    private const TargetType CardTarget = TargetType.AnyEnemy;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => base.AdditionalHoverTips.Concat(
    [
        InstinctSlash.MakeCardHoverTip(IsUpgraded, !SwingRight)
    ]);

    // 卡牌基础数值。
    protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat(
    [
        new DamageVar(6m, ValueProp.Move),
        new CardsVar(1)
    ]);

    // 打出时的效果逻辑。
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitCount(DynamicVars.Repeat.IntValue)
            .WithHitFx("vfx/vfx_heavy_blunt", null, "heavy_attack.mp3")
            .Execute(choiceContext);
        await ApplyHeavyKnock(choiceContext, cardPlay.Target, HeavyKnockAmount, Owner.Creature, this);

        if (CombatState is not null)
        {
            InstinctSlash card = CombatState.CreateCard<InstinctSlash>(Owner);
            card.SwingRight = !SwingRight;
            if (IsUpgraded)
            {
                CardCmd.Upgrade(card, CardPreviewStyle.None);
            }

            for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
            {
                await InstinctSlash.CreateInHand(Owner, CombatState, IsUpgraded, !SwingRight);
                await Cmd.CustomScaledWait(0.1f, 0.2f);
            }
        }

        await base.OnPlay(choiceContext, cardPlay);
    }

    // 升级后的效果逻辑。
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}