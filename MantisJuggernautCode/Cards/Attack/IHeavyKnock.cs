using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace MantisJuggernaut.Cards;

public interface IHeavyKnock
{
    public decimal HeavyKnockAmount { get; }

    public Task ApplyHeavyKnock(PlayerChoiceContext choiceContext, Creature target,
        decimal amount,
        Creature? applier,
        CardModel? cardSource
    );
}