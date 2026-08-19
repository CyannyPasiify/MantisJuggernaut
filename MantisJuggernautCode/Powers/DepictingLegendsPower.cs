using MantisJuggernaut.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Powers;

[RegisterPower]
public class DepictingLegendsPower : MantisJuggernautPower
{
    // 类型，Buff或Debuff
    public override PowerType Type => PowerType.Buff;

    // 叠加类型，Counter表示可叠加，Single表示不可叠加
    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
    ];

    public bool IsUpgraded
    {
        get => ((BoolVar)DynamicVars["IsUpgraded"]).BoolVal;
        set => ((BoolVar)DynamicVars["IsUpgraded"]).BoolVal = value;
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BoolVar("IsUpgraded", false)
    ];

    private List<CardModel> LegendTechniques =>
    [
        ModelDb.Card<Omnislash>(),
        ModelDb.Card<Guillotine>(),
        ModelDb.Card<TurboWhirl>(),
        ModelDb.Card<DominateForce>(),
        ModelDb.Card<MazeStep>()
    ];

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState state)
    {
        if (player == Owner.Player && AmountOnTurnStart >= 1)
        {
            CardModel? cardItem = Owner.Player.RunState.Rng.CombatCardGeneration.NextItem(LegendTechniques);
            if (cardItem != null)
            {
                CardModel card = CombatState.CreateCard(cardItem, Owner.Player);
                if (IsUpgraded) CardCmd.Upgrade(card);

                Flash();
                await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, player);
            }
        }
    }
}