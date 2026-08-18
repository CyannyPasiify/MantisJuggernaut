using MantisJuggernaut.Characters;
using MantisJuggernaut.HoverTips;
using MantisJuggernaut.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Relics;

// RegisterRelic 会把遗物注册进指定遗物池。
[RegisterRelic(typeof(MantisJuggernautRelicPool))]
[RegisterCharacterStarterRelic(typeof(MantisJuggernautCharacter))]
[RegisterTouchOfOrobasRefinement(typeof(MasterCloak))]
public sealed class WingCloak : MantisJuggernautRelic
{
    // 稀有度。
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => base.AdditionalHoverTips.Concat(
    [
        ExtHoverTipFactory.Static(ExtStaticHoverTip.Stance),
        HoverTipFactory.FromPower<BackswingBalancePower>(),
        HoverTipFactory.FromPower<BackswingLeftPower>(),
        HoverTipFactory.FromPower<BackswingRightPower>(),
        HoverTipFactory.FromPower<BackswingImbalancePower>()
    ]);

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom)
        {
            Flash();
            await PowerCmd.Apply<BackswingBalancePower>(
                new ThrowingPlayerChoiceContext(),
                Owner.Creature,
                1,
                Owner.Creature,
                null
            );
        }
    }
}