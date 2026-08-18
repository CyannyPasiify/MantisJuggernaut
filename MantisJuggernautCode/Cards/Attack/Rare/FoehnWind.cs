using Godot;
using MantisJuggernaut.Characters;
using MantisJuggernaut.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Cards;

[RegisterCard(typeof(MantisJuggernautCardPool))]
public sealed class FoehnWind()
    : MantisJuggernautAttackCard(BaseEnergyCost, CardRarityValue, CardTarget)
{
    // 基础耗能。
    private const int BaseEnergyCost = 2;

    // 卡牌稀有度。
    private const CardRarity CardRarityValue = CardRarity.Rare;

    // 目标类型。
    private const TargetType CardTarget = TargetType.AllEnemies;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => base.AdditionalHoverTips.Concat(
    [
        HoverTipFactory.FromPower<RoutPower>()
    ]);

    // 卡牌基础数值。
    protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat(
    [
        new DamageVar(12m, ValueProp.Move),
        new PowerVar<RoutPower>(6m)
    ]);

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    // 打出时的效果逻辑。
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState is null) return;

        SfxCmd.Play("event:/sfx/characters/silent/silent_dagger_spray");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .TargetingAllOpponents(CombatState)
            .WithAttackerFx(() => NDaggerSprayFlurryVfx.Create(Owner.Creature, new Color("#ddccb1"), true))
            .BeforeDamage(delegate
            {
                var hittableEnemies = CombatState.HittableEnemies;
                foreach (var item in hittableEnemies)
                {
                    var child = NDaggerSprayImpactVfx.Create(item, new Color("#ddccb1"), true);
                    NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(child);
                }

                return Task.CompletedTask;
            })
            .WithHitFx("vfx/vfx_fire_burning")
            .Execute(choiceContext);

        await PowerCmd.Apply<RoutPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(RoutPower)].BaseValue,
            Owner.Creature,
            this
        );
    }

    // 升级后的效果逻辑。
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}