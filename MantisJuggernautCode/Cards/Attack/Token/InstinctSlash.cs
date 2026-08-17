using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using MantisJuggernaut.Characters;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace MantisJuggernaut.Cards;

[RegisterCard(typeof(MantisJuggernautCardPool))]
public sealed class InstinctSlash()
    : MantisJuggernautAttackBackswingCard(BaseEnergyCost, CardRarityValue, CardTarget, false, false)
{
    // 基础耗能。
    private const int BaseEnergyCost = 0;

    // 卡牌稀有度。
    private const CardRarity CardRarityValue = CardRarity.Token;

    // 目标类型。
    private const TargetType CardTarget = TargetType.AnyEnemy;

    // 卡牌基础数值。
    protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat(
    [
        new DamageVar(7m, ValueProp.Move)
    ]);

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Retain,
        CardKeyword.Exhaust
    ];

    // 打出时的效果逻辑。
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await base.OnPlay(choiceContext, cardPlay);
    }

    // 升级后的效果逻辑。
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }

    public static async Task<CardModel?> CreateInHand(
        Player owner,
        ICombatState combatState,
        bool upgrade = false,
        bool swingRight = false,
        bool randomize = false,
        Player? creator = null
    )
    {
        return (await CreateInHand(owner, 1, combatState, upgrade, swingRight, randomize, creator)).FirstOrDefault();
    }

    public static async Task<IEnumerable<CardModel>> CreateInHand(
        Player owner,
        int count,
        ICombatState combatState,
        bool upgrade = false,
        bool swingRight = false,
        bool randomize = false,
        Player? creator = null
    )
    {
        if (count == 0)
        {
            return Array.Empty<CardModel>();
        }

        if (CombatManager.Instance.IsOverOrEnding)
        {
            return Array.Empty<CardModel>();
        }

        List<CardModel> cards = new List<CardModel>();
        for (int i = 0; i < count; i++)
        {
            InstinctSlash card = combatState.CreateCard<InstinctSlash>(owner);
            if (randomize)
            {
                card.SwingRight = (creator ?? owner).RunState.Rng.CombatCardGeneration.NextBool();
            }
            else
            {
                card.SwingRight = swingRight;
            }

            if (upgrade)
            {
                CardCmd.Upgrade(card);
            }

            cards.Add(card);
        }

        await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, creator ?? owner);
        return cards;
    }

    public static IHoverTip MakeCardHoverTip(
        bool upgrade = false,
        bool swingRight = false
    )
    {
        InstinctSlash card = (InstinctSlash)ModelDb.Card<InstinctSlash>().MutableClone();
        card.SwingRight = swingRight;
        if (upgrade)
        {
            card.UpgradeInternal();
            card.FinalizeUpgradeInternal();
        }

        return new CardHoverTip(card);
    }
}