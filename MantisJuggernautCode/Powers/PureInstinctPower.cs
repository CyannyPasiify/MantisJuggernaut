using MantisJuggernaut.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Powers;

[RegisterPower]
public class PureInstinctPower : MantisJuggernautPower
{
    // 类型，Buff或Debuff
    public override PowerType Type => PowerType.Buff;

    // 叠加类型，Counter表示可叠加，Single表示不可叠加
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.None;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        InstinctSlash.MakeCardHoverTip()
    ];

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay
    )
    {
        if (Owner != dealer || cardSource == null) return 0m;

        if (!props.IsPoweredAttack()) return 0m;

        if (cardSource is not InstinctSlash) return 0m;

        return Amount;
    }

    // public override Task AfterCardEnteredCombat(CardModel card)
    // {
    //     if (card is not InstinctSlash)
    //     {
    //         return Task.CompletedTask;
    //     }
    //
    //     if (card.Owner != Owner.Player)
    //     {
    //         return Task.CompletedTask;
    //     }
    //
    //     CardCmd.ApplyKeyword(card, CardKeyword.Retain);
    //     return Task.CompletedTask;
    // }
    //
    // public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    // {
    //     if (Owner.Player?.PlayerCombatState is null)
    //     {
    //         return Task.CompletedTask;
    //     }
    //
    //     foreach (CardModel item in Owner.Player.PlayerCombatState.AllCards.Where(c => c is InstinctSlash))
    //     {
    //         CardCmd.ApplyKeyword(item, CardKeyword.Retain);
    //     }
    //
    //     return Task.CompletedTask;
    // }
}