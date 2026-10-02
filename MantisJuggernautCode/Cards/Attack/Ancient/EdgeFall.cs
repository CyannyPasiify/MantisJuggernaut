using MantisJuggernaut.Characters;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Cards;

[RegisterCard(typeof(MantisJuggernautCardPool))]
public sealed class EdgeFall()
    : MantisJuggernautAttackBackswingHeavyKnockCard(BaseEnergyCost, CardRarityValue, CardTarget)
{
    // 基础耗能。
    private const int BaseEnergyCost = 1;

    // 卡牌稀有度。
    private const CardRarity CardRarityValue = CardRarity.Ancient;

    // 目标类型。
    private const TargetType CardTarget = TargetType.AnyEnemy;

    public override bool GainsBlock => true;

    // 卡牌基础数值。
    protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat(
    [
        new DamageVar(6m, ValueProp.Move),
        new RepeatVar(2),
        new BlockVar(6m, ValueProp.Move)
    ]);

    // 打出时的效果逻辑。
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        bool applyHeavyKnock = false;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(DynamicVars.Repeat.IntValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .BeforeDamage(async delegate
            {
                if (applyHeavyKnock)
                {
                    await ApplyHeavyKnock(choiceContext, cardPlay.Target, HeavyKnockAmount, Owner.Creature, this);
                }
                else
                {
                    applyHeavyKnock = true;
                }
            })
            .WithHitFx("vfx/vfx_heavy_blunt", null, "heavy_attack.mp3")
            .Execute(choiceContext);

        if (DynamicVars.Repeat.IntValue > 0)
            await ApplyHeavyKnock(choiceContext, cardPlay.Target, HeavyKnockAmount, Owner.Creature, this);

        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await base.OnPlay(choiceContext, cardPlay);
    }

    // 升级后的效果逻辑。
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVars.Repeat.UpgradeValueBy(1m);
        DynamicVars.Block.UpgradeValueBy(1m);
    }
}