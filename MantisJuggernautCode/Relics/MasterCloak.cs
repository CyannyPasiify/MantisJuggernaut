using MantisJuggernaut.Characters;
using MantisJuggernaut.Enchantments;
using MantisJuggernaut.HoverTips;
using MantisJuggernaut.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Relics;

// RegisterRelic 会把遗物注册进指定遗物池。
[RegisterRelic(typeof(MantisJuggernautRelicPool))]
public sealed class MasterCloak : MantisJuggernautRelic
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
        ])
        .Concat(HoverTipFactory.FromEnchantment<MasterHeritage>());

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3)
    ];

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

    public override async Task AfterObtained()
    {
        EnchantmentModel enchant = ModelDb.Enchantment<MasterHeritage>();
        List<CardModel> list = PileType.Deck.GetPile(Owner).Cards.Where(enchant.CanEnchant).ToList();
        List<CardModel> cards;
        // 处理初始遗物被替换的特殊情况，附魔末端的若干张牌
        if (LocalContext.NetId is null)
        {
            cards = list.TakeLast(DynamicVars.Cards.IntValue).ToList();
        }
        else
        {
            cards = (await CardSelectCmd.FromDeckForEnchantment(
                    list.UnstableShuffle(Owner.RunState.Rng.Niche).ToList(),
                    enchant,
                    1,
                    new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, DynamicVars.Cards.IntValue))
                ).ToList();
        }

        foreach (var card in cards)
        {
            CardCmd.Enchant<MasterHeritage>(card, 1m);
            NCardEnchantVfx? nCardEnchantVfx = NCardEnchantVfx.Create(card);
            if (nCardEnchantVfx != null)
            {
                NRun.Instance?.GlobalUi.CardPreviewContainer.AddChildSafely(nCardEnchantVfx);
            }
        }
    }
}