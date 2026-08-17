using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
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
        string text = StringHelper.Slugify(Entry.ModId) + "_" + StringHelper.Slugify(tip.ToString());
        LocString title = HoverTipFactory.L10NStatic(text + ".title");
        LocString description = HoverTipFactory.L10NStatic(text + ".description");
        foreach (DynamicVar dynamicVar in vars)
        {
            title.Add(dynamicVar);
            description.Add(dynamicVar);
        }

        return new HoverTip(title, description);
    }
}