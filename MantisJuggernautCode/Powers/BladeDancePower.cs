using MantisJuggernaut.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Powers;

[RegisterPower]
public class BladeDancePower : MantisJuggernautPower
{
    // 类型，Buff或Debuff
    public override PowerType Type => PowerType.Buff;

    // 叠加类型，Counter表示可叠加，Single表示不可叠加
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override int DisplayAmount =>
        DynamicVars.Cards.IntValue - GetInternalData<Data>().InstictPlayed % DynamicVars.Cards.IntValue;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        InstinctSlash.MakeCardHoverTip(IsUpgraded)
    ];

    public bool IsUpgraded
    {
        get => ((BoolVar)DynamicVars["IsUpgraded"]).BoolVal;
        set => ((BoolVar)DynamicVars["IsUpgraded"]).BoolVal = value;
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3),
        new BoolVar("IsUpgraded", false)
    ];

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player || cardPlay.Card is not InstinctSlash) return;

        var data = GetInternalData<Data>();
        data.InstictPlayed++;
        var triggers = data.InstictPlayed / DynamicVars.Cards.IntValue;
        await InstinctSlash.CreateInHand(Owner.Player, triggers, CombatState, IsUpgraded, false, true);
        data.InstictPlayed -= triggers * DynamicVars.Cards.IntValue;
        InvokeDisplayAmountChanged();
    }

    private class Data
    {
        public int InstictPlayed;
    }
}