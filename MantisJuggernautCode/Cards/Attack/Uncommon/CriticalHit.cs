using MantisJuggernaut.Characters;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Cards;

[RegisterCard(typeof(MantisJuggernautCardPool))]
public sealed class CriticalHit()
    : MantisJuggernautAttackBackswingCard(BaseEnergyCost, CardRarityValue, CardTarget, true)
{
    // 基础耗能。
    private const int BaseEnergyCost = 1;

    // 卡牌稀有度。
    private const CardRarity CardRarityValue = CardRarity.Uncommon;

    // 目标类型。
    private const TargetType CardTarget = TargetType.AnyEnemy;

    public const string DamageIncreaseVarKey = "DamageIncrease";

    // 卡牌基础数值。
    protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat(
    [
        new DamageVar(4m, ValueProp.Move),
        new DynamicVar(DamageIncreaseVarKey, 4)
    ]);

    // 打出时的效果逻辑。
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cards = (await CardSelectCmd.FromHandForDiscard(
            choiceContext,
            Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 0, 999999999),
            null,
            this
        )).ToList();
        var damage = DynamicVars.Damage.BaseValue * (1 + cards.Count);
        await CardCmd.Discard(choiceContext, cards);

        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(damage)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_dramatic_stab")
            .Execute(choiceContext);

        await base.OnPlay(choiceContext, cardPlay);
    }

    // 升级后的效果逻辑。
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVars[DamageIncreaseVarKey].UpgradeValueBy(1m);
    }
}