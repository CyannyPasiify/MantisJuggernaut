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

[RegisterCard(typeof(MantisJuggernautCardPool))]
public sealed class Swarm() : MantisJuggernautSkillCard(BaseEnergyCost, CardRarityValue, CardTarget)
{
    // 基础耗能。
    private const int BaseEnergyCost = 3;

    // 卡牌稀有度。
    private const CardRarity CardRarityValue = CardRarity.Rare;

    // 目标类型。
    private const TargetType CardTarget = TargetType.RandomEnemy;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => base.AdditionalHoverTips.Concat(
    [
        HoverTipFactory.FromPower<RoutPower>()
    ]);

    // 卡牌基础数值。
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new RepeatVar(3),
        new DamageVar(1m, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move),
        new PowerVar<RoutPower>(1m)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Sly
    ];

    // 打出时的效果逻辑。
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        if (CombatState is null) return;

        for (var i = 0; i < DynamicVars.Repeat.IntValue; i++)
        {
            var enemy = Owner.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
            if (enemy == null) continue;

            await CreatureCmd.Damage(choiceContext, enemy, DynamicVars.Damage, this, cardPlay);
            await PowerCmd.Apply<RoutPower>(
                choiceContext,
                enemy,
                DynamicVars[nameof(RoutPower)].BaseValue,
                Owner.Creature,
                this
            );
        }
    }

    protected override CardLocation GetResultLocationForCardPlay()
    {
        var resultLocationForCardPlay = base.GetResultLocationForCardPlay();
        if (resultLocationForCardPlay.pileType == PileType.Discard) resultLocationForCardPlay.pileType = PileType.Hand;

        return resultLocationForCardPlay;
    }

    // 升级后的效果逻辑。
    protected override void OnUpgrade()
    {
        DynamicVars.Repeat.UpgradeValueBy(1m);
    }
}