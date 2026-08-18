using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Powers;

[RegisterPower]
public class FakeFlawPower : MantisJuggernautPower
{
    // 类型，Buff或Debuff
    public override PowerType Type => PowerType.Buff;

    // 叠加类型，Counter表示可叠加，Single表示不可叠加
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.None;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<BackswingImbalancePower>()
    ];

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource
    )
    {
        if (power.Owner != Owner ||
            power is not BackswingImbalancePower ||
            amount <= 0)
            return;

        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
    }
}