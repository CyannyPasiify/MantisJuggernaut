using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MantisJuggernaut.HoverTips;

public enum ExtStaticHoverTip
{
    Prepared,
    Stance,
    BackswingLeft,
    BackswingRight,
    HeavyKonck
}

public static class ExtHoverTipFactory
{
    public static IHoverTip Static(ExtStaticHoverTip tip, params DynamicVar[] vars)
    {
        var text = StringHelper.Slugify(Entry.ModId) + "_" + StringHelper.Slugify(tip.ToString());
        var title = HoverTipFactory.L10NStatic(text + ".title");
        var description = HoverTipFactory.L10NStatic(text + ".description");
        foreach (var dynamicVar in vars)
        {
            title.Add(dynamicVar);
            description.Add(dynamicVar);
        }

        return new HoverTip(title, description);
    }
}