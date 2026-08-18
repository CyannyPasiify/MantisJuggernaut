using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Powers;

[RegisterPower]
public class AmbushPower : MantisJuggernautPower
{
    // 类型，Buff或Debuff
    public override PowerType Type => PowerType.Buff;

    // 叠加类型，Counter表示可叠加，Single表示不可叠加
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.None;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Sly)
    ];

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner == Owner.Player && card.Keywords.Contains(CardKeyword.Sly))
        {
            var num = CombatManager.Instance.History.Entries.OfType<CardDrawnEntry>()
                .Count(e => e.HappenedThisTurn(CombatState) &&
                            e.Actor == Owner && e.Card.Keywords.Contains(CardKeyword.Sly));
            if (num <= 1)
            {
                Flash();
                await CardCmd.DiscardAndDraw(choiceContext, [card], Amount);
            }
        }
    }
}