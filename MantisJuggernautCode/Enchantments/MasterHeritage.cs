namespace MantisJuggernaut.Enchantments;

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterEnchantment]
public class MasterHeritage : ModEnchantmentTemplate
{
    // 是否在卡牌上显示数值
    public override bool ShowAmount => false;

    // 重载这个以改变显示的数字
    // public override int DisplayAmount => DynamicVars.Cards.IntValue;

    // 是否会添加额外的卡牌描述文本
    public override bool HasExtraCardText => false;

    // 像卡牌、遗物、药水等一样，可以使用DynamicVars和ExtraHoverTips
    protected override IEnumerable<DynamicVar> CanonicalVars => [];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Sly)];

    // 图标位置。大小1:1就行，原版是64x64
    public override EnchantmentAssetProfile AssetProfile => new(
        // IconPath: "res://icon.svg"
    );

    public override bool CanEnchantCardType(CardType cardType)
    {
        HashSet<CardType> valid = [CardType.None, CardType.Attack, CardType.Skill, CardType.Power];
        return valid.Contains(cardType);
    }

    // 当附魔被应用时调用，这里我们给卡牌添加保留。
    protected override void OnEnchant()
    {
        Card.AddKeyword(CardKeyword.Sly);
    }
}