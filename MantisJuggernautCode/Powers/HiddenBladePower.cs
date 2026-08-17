using MantisJuggernaut.HoverTips;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Powers;

[RegisterPower]
public class HiddenBladePower : MantisJuggernautPower
{
    // 类型，Buff或Debuff
    public override PowerType Type => PowerType.Buff;

    // 叠加类型，Counter表示可叠加，Single表示不可叠加
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.None;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        ExtHoverTipFactory.Static(ExtStaticHoverTip.Prepared)
    ];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == Owner.Player)
        {
            var cards = (await CardSelectCmd.FromHand(
                choiceContext,
                Owner.Player,
                new CardSelectorPrefs(SelectionScreenPrompt, 0, Amount),
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
    }
}