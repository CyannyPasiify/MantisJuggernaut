using STS2RitsuLib.Scaffolding.Content;

namespace MantisJuggernaut.Relics;

public abstract class MantisJuggernautRelic : ModRelicTemplate
{
    // 图片资源统一放在 AssetProfile 里配置。
    // 三个路径可以先指向同一张图。后续有高清图或轮廓图时再拆开。
    public override RelicAssetProfile AssetProfile => new(
        // 小图标（原版 85x85）。
        IconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        // 轮廓图标（原版 85x85）。
        IconOutlinePath: $"{Entry.ResPath}/images/relics/{GetType().Name}_outline.png",
        // 大图标（原版 256x256）。
        BigIconPath: $"{Entry.ResPath}/images/relics/big/{GetType().Name}.png");
}