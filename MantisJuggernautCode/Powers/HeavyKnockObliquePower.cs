using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Powers;

[RegisterPower]
public class HeavyKnockObliquePower : MantisJuggernautPower
{
    // 类型，Buff或Debuff
    public override PowerType Type => PowerType.Debuff;

    // 叠加类型，Counter表示可叠加，Single表示不可叠加
    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerInstanceType InstanceType => PowerInstanceType.None;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<HeavyKnockDownPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
    ];
}