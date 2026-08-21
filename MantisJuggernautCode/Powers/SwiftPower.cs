using System.Diagnostics;
using MantisJuggernaut.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Powers;

[RegisterPower]
public class SwiftPower : MantisJuggernautPower
{
    public const string SwiftMaxVar = "SwiftMax";

    private CardModel? _leftCard;

    private CardModel? _rightCard;

    // 类型，Buff或Debuff
    public override PowerType Type => PowerType.Buff;

    // 叠加类型，Counter表示可叠加，Single表示不可叠加
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.None;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar(SwiftMaxVar, 3)
    ];

    public bool IsSwiftMarked(CardModel card)
    {
        return card == _leftCard || card == _rightCard;
    }

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        UpdateRecord();
        return Task.CompletedTask;
    }

    public void UpdateRecord()
    {
        if (Owner.Player is null) return;

        var hand = PileType.Hand.GetPile(Owner.Player);
        _leftCard = hand.Cards.FirstOrDefault();
        _rightCard = hand.Cards.LastOrDefault();
        if (_leftCard == _rightCard) _leftCard = _rightCard = null;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (cardPlay.Player != Owner.Player) return;

        if (card == playCard)
        {
            if (cardToDiscard is not null)
            {
                await CardCmd.DiscardAndDraw(
                    choiceContext,
                    [cardToDiscard],
                    1
                );
                await PowerCmd.Decrement(this);
                if (Amount > 0) UpdateRecord();
                cardToDiscard = null;
            }

            playCard = null;
        }

        // 确定卡牌是否为之前记录的左右端侧牌
        if (card == _leftCard)
        {
            if (_rightCard is not null)
            {
                await CardCmd.DiscardAndDraw(
                    choiceContext,
                    [_rightCard],
                    1
                );
                await PowerCmd.Decrement(this);
                if (Amount > 0) UpdateRecord();
            }
        }
        else if (card == _rightCard)
        {
            if (_leftCard is not null)
            {
                await ExtCardCmd.DiscardAndDraw(
                    choiceContext,
                    [_leftCard],
                    1
                );
                await PowerCmd.Decrement(this);
                if (Amount > 0) UpdateRecord();
            }
        }
    }

    private CardModel? playCard;
    private CardModel? cardToDiscard;

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (card.Owner != Owner.Player)
        {
            return Task.CompletedTask;
        }

        // 有卡牌离开手牌
        if (oldPileType == PileType.Hand)
        {
            // 这是一张从手牌中打出的牌进入结算队列
            if (card.Pile?.Type == PileType.Play)
            {
                playCard = card;
                // 确定卡牌是否为之前记录的左右端侧牌
                if (card == _leftCard)
                {
                    cardToDiscard = _rightCard;
                }
                else if (card == _rightCard)
                {
                    cardToDiscard = _leftCard;
                }
            }
        }

        // 有卡牌进入手牌
        else if (card.Pile?.Type == PileType.Hand)
        {
            UpdateRecord();
        }

        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (participants.Contains(Owner))
            if (Amount > DynamicVars[SwiftMaxVar].IntValue)
                await PowerCmd.ModifyAmount(
                    choiceContext,
                    this,
                    DynamicVars[SwiftMaxVar].IntValue - Amount,
                    Owner,
                    null
                );
    }
}