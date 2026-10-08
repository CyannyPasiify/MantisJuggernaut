using MantisJuggernaut.Characters;
using MantisJuggernaut.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Cards;

[RegisterCard(typeof(MantisJuggernautCardPool))]
public sealed class FullSpinSlash()
    : MantisJuggernautAttackBackswingCard(BaseEnergyCost, CardRarityValue, CardTarget)
{
    // 基础耗能。
    private const int BaseEnergyCost = 1;

    // 卡牌稀有度。
    private const CardRarity CardRarityValue = CardRarity.Uncommon;

    // 目标类型。
    private const TargetType CardTarget = TargetType.AnyEnemy;

    protected override bool ShouldGlowGoldInternal => IsBalance;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => base.AdditionalHoverTips.Concat(
    [
        HoverTipFactory.FromPower<BackswingBalancePower>()
    ]);

    // 卡牌基础数值。
    protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat(
    [
        new DamageVar(9m, ValueProp.Move),
        new CardsVar(2)
    ]);

    // 打出时的效果逻辑。
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        if (IsBalance)
        {
            var drawCards = PileType.Draw.GetPile(Owner).Cards;
            var nonXCards = drawCards.Where(e => !e.EnergyCost.CostsX).ToList();
            var xCards = drawCards.Where(e => e.EnergyCost.CostsX).ToList();
            List<CardModel> cardsToDraw = [];
            for (var i = 0; i < DynamicVars.Cards.IntValue; i++)
            {
                var card = nonXCards.MaxBy(e => e.EnergyCost.GetResolved());
                if (card != null)
                {
                    cardsToDraw.Add(card);
                    nonXCards.Remove(card);
                    continue;
                }

                card = xCards.FirstOrDefault();
                if (card != null)
                {
                    cardsToDraw.Add(card);
                    xCards.Remove(card);
                    continue;
                }

                break;
            }

            await CardPileCmd.Add(cardsToDraw, PileType.Hand);
        }

        await base.OnPlay(choiceContext, cardPlay);
    }

    // 升级后的效果逻辑。
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}