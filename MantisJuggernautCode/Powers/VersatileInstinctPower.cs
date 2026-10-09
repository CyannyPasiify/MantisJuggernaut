using MantisJuggernaut.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MantisJuggernaut.Powers;

[RegisterPower]
public class VersatileInstinctPower : MantisJuggernautPower
{
    // 类型，Buff或Debuff
    public override PowerType Type => PowerType.Buff;

    // 叠加类型，Counter表示可叠加，Single表示不可叠加
    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerInstanceType InstanceType => PowerInstanceType.None;

    // protected override IEnumerable<IHoverTip> AdditionalHoverTips
    // {
    //     get
    //     {
    //         var common = base.AdditionalHoverTips.ToList();
    //         if (!IsUpgraded)
    //             return common
    //                 .Concat(HoverTipFactory.FromEnchantment<Sharp>(3))
    //                 .Concat(HoverTipFactory.FromEnchantment<Adroit>(3))
    //                 .Concat(HoverTipFactory.FromEnchantment<Swift>(1))
    //                 .Concat(HoverTipFactory.FromEnchantment<Inky>());
    //
    //         return common
    //             .Concat(HoverTipFactory.FromEnchantment<Sharp>(5))
    //             .Concat(HoverTipFactory.FromEnchantment<Adroit>(5))
    //             .Concat(HoverTipFactory.FromEnchantment<Swift>(2))
    //             .Concat(HoverTipFactory.FromEnchantment<Inky>())
    //             .Concat(HoverTipFactory.FromEnchantment<Instinct>())
    //             .Concat(HoverTipFactory.FromEnchantment<Sown>());
    //     }
    // }

    public List<Tuple<EnchantmentModel, int>> EnchantmentsPool =>
        IsUpgraded
            ?
            [
                new Tuple<EnchantmentModel, int>(ModelDb.Enchantment<Sharp>(), 5),
                new Tuple<EnchantmentModel, int>(ModelDb.Enchantment<Adroit>(), 5),
                new Tuple<EnchantmentModel, int>(ModelDb.Enchantment<Swift>(), 2),
                new Tuple<EnchantmentModel, int>(ModelDb.Enchantment<Inky>(), 1),
                new Tuple<EnchantmentModel, int>(ModelDb.Enchantment<Instinct>(), 1),
                new Tuple<EnchantmentModel, int>(ModelDb.Enchantment<Sown>(), 1)
            ]
            :
            [
                new Tuple<EnchantmentModel, int>(ModelDb.Enchantment<Sharp>(), 3),
                new Tuple<EnchantmentModel, int>(ModelDb.Enchantment<Adroit>(), 3),
                new Tuple<EnchantmentModel, int>(ModelDb.Enchantment<Swift>(), 1),
                new Tuple<EnchantmentModel, int>(ModelDb.Enchantment<Inky>(), 1)
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

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card is not InstinctSlash) return Task.CompletedTask;

        if (card.Owner != Owner.Player) return Task.CompletedTask;

        RandomApplyEnchantment(card);
        return Task.CompletedTask;
    }

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (Owner.Player?.PlayerCombatState is null) return Task.CompletedTask;

        foreach (var item in Owner.Player.PlayerCombatState.AllCards.Where(c => c is InstinctSlash))
            RandomApplyEnchantment(item);

        return Task.CompletedTask;
    }

    private void RandomApplyEnchantment(CardModel card)
    {
        var validPool = EnchantmentsPool.Where(c => c.Item1.CanEnchant(card)).ToList();
        if (validPool.Count == 0) return;

        var enchantAmount = Owner.Player?.RunState.Rng.CombatCardGeneration.NextItem(validPool);
        if (enchantAmount is null) return;

        var (enchant, amount) = enchantAmount;
        CardCmd.Enchant(enchant.ToMutable(), card, amount);
    }
}