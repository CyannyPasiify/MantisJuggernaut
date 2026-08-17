using Godot;
using MantisJuggernaut.Characters;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Cards;

[RegisterCard(typeof(MantisJuggernautCardPool))]
public sealed class Weeding()
    : MantisJuggernautAttackHeavyKnockCard(BaseEnergyCost, CardRarityValue, CardTarget)
{
    // 基础耗能。
    private const int BaseEnergyCost = 0;

    // 卡牌稀有度。
    private const CardRarity CardRarityValue = CardRarity.Rare;

    // 目标类型。
    private const TargetType CardTarget = TargetType.AllEnemies;

    protected override bool HasEnergyCostX => true;

    // 卡牌基础数值。
    protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat(
    [
        new DamageVar(7m, ValueProp.Move)
    ]);

    // 打出时的效果逻辑。
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState is null) return;

        var num = ResolveEnergyXValue();
        if (IsUpgraded) num += 1;

        if (num > 0)
        {
            var color = new Color("FF99FF80");
            var num2 = SaveManager.Instance.PrefsSave.FastMode == FastModeType.Fast ? 0.2 : 0.3;
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(
                NHorizontalLinesVfx.Create(color, 0.8 + Mathf.Min(8, num) * num2));
            SfxCmd.Play("event:/sfx/characters/ironclad/ironclad_whirlwind");
            NRun.Instance?.GlobalUi.AddChildSafely(NSmokyVignetteVfx.Create(color, color));
        }

        for (var i = 0; i < num; i++)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this, cardPlay)
                .TargetingAllOpponents(CombatState)
                .WithHitFx("vfx/vfx_giant_horizontal_slash")
                .Execute(choiceContext);

            foreach (var enemy in CombatState.HittableEnemies)
                await ApplyHeavyKnock(choiceContext, enemy, HeavyKnockAmount, Owner.Creature, this);
        }
    }
}