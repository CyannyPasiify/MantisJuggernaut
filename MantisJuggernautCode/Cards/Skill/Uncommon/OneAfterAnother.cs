using MantisJuggernaut.Characters;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Cards;

[RegisterCard(typeof(MantisJuggernautCardPool))]
public sealed class OneAfterAnother() : MantisJuggernautSkillCard(BaseEnergyCost, CardRarityValue, CardTarget)
{
    // 基础耗能。
    private const int BaseEnergyCost = 1;

    // 卡牌稀有度。
    private const CardRarity CardRarityValue = CardRarity.Uncommon;

    // 目标类型（Self 表示自己）。
    private const TargetType CardTarget = TargetType.Self;

    public const string SelCardsVarKey = "PreparedCards";

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => base.AdditionalHoverTips.Concat(
    [
    ]);

    // 卡牌基础数值。
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2),
        new CardsVar(SelCardsVarKey, 2)
    ];

    // 打出时的效果逻辑。
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);

        var cards = (await CardSelectCmd.FromHand(
            choiceContext,
            Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 0, DynamicVars[SelCardsVarKey].IntValue),
            null,
            this
        )).ToList();
        cards.Reverse();
        foreach (var card in cards)
        {
            await CardPileCmd.Add(card, PileType.Hand, CardPilePosition.Top);
            var nPlayerHand = NPlayerHand.Instance;
            if (nPlayerHand?.GetCardHolder(card) is NHandCardHolder holder)
            {
                nPlayerHand.CardHolderContainer.MoveChildSafely(holder, 0);
                holder.SetDefaultTargets();
            }
        }
    }

    // 升级后的效果逻辑。
    protected override void OnUpgrade()
    {
        DynamicVars[SelCardsVarKey].UpgradeValueBy(2m);
    }
}