using System.Reflection;
using Godot;
using HarmonyLib;
using MantisJuggernaut.Cards;
using MantisJuggernaut.Powers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using STS2RitsuLib;
using STS2RitsuLib.Interop;
using STS2RitsuLib.Scaffolding.Cards.HandOutline;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace MantisJuggernaut;

[ModInitializer(nameof(Initialize))]
public class Entry
{
    // ModId 需要和 MantisJuggernaut.json 里的 id 保持一致。
    // res://MantisJuggernaut/... 里的 MantisJuggernaut 是 PCK 资源目录，不是 C# namespace。
    public const string ModId = "MantisJuggernaut";
    public const string ResPath = $"res://{ModId}";

    public static Logger Logger { get; } = new(ModId, LogType.Generic);

    public static void Initialize()
    {
        var assembly = Assembly.GetExecutingAssembly();

        // 以下示例默认已经在 Entry.Initialize() 中调用了
        // RitsuLibFramework.EnsureGodotScriptsRegistered(...) 和
        // ModTypeDiscoveryHub.RegisterModAssembly(...)，否则自动注册不会生效。
        //
        // Godot C# 脚本注册只负责让 pck 中的脚本类型能被 Godot 找到。
        // 这一步和 RitsuLib 的内容自动注册不是同一件事，两个都需要保留。
        RitsuLibFramework.EnsureGodotScriptsRegistered(assembly, Logger);

        // 自动注册扫描会读取当前程序集里的 RegisterCard/RegisterRelic 等 attribute。
        // 新增内容类后，只要 attribute 写对，通常不需要在入口里手动逐个注册。
        ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);

        Harmony harmony = new(ModId);
        harmony.PatchAll();

        // 添加卡牌泛光规则
        ModCardHandOutlineRegistry.Register<MantisJuggernautCard>(
            ModCardHandOutlineRules.Fixed( // 特定种类卡牌。可以设置为你的卡牌基类，让所有子类发光。
                card =>
                {
                    var swiftPower = card.Owner.Creature.GetPower<SwiftPower>();
                    return swiftPower?.IsSwiftMarked(card) ?? false;
                }, // 发光条件
                Colors.Purple, // 发光颜色
                1, // （可选）优先级。更高的才会展示。
                true // 不可打出时仍显示边框
            ));

        Logger.Info("MantisJuggernaut initialized.");
    }
}