using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Powers;

[RegisterPower]
public class AnxietyPower : MantisJuggernautPower
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

    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (!participants.Contains(Owner) || Owner.Player is null)
        {
            return;
        }

        var slyCards = PileType.Hand.GetPile(Owner.Player).Cards
            .Where(e => e.Keywords.Contains(CardKeyword.Sly))
            .ToList();

        for (int i = 0; i < Amount && slyCards.Count > 0; i++)
        {
            CardModel? card = Owner.Player.RunState.Rng.Shuffle.NextItem(slyCards);
            if (card is not null)
            {
                CardCmd.ApplySingleTurnRetain(card);
                card.EnergyCost.AddThisCombat(1);
                slyCards.Remove(card);
            }
        }
    }
}