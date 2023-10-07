using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using PixelerPerfect.GUI;


namespace PixelerPerfect;

public class Plugin : IDalamudPlugin
{
    public string Name => "Pixeler Perfect";
    public string SettingsCommand => "/pxpr";
    public string AssemblyLocation { get; set; } = System.Reflection.Assembly.GetExecutingAssembly().Location;
    public DalamudPluginInterface PluginInterface { get; }
    public IClientState ClientState { get; }
    public ICommandManager CommandManager { get; }
    public ICondition Condition { get; }
    public IGameGui GameGui { get; }
    private Config PluginConfig { get; }
    private WorldHelper WorldHelper { get; }
    private PluginGui PluginGui { get; }

    private bool _drawConfigWindow = false;

    public Plugin
    (
        DalamudPluginInterface pluginInterface,
        //BuddyList buddies,
        //ChatGui chat,
        //ChatHandlers chatHandlers,
        IClientState clientState,
        ICommandManager commands,
        ICondition condition,
        //DataManager data,
        //FateTable fates,
        //FlyTextGui flyText,
        //Framework framework,
        IGameGui gameGui
        //GameNetwork gameNetwork,
        //JobGauges gauges,
        //KeyState keyState,
        //LibcFunction libcFunction,
        //ObjectTable objects,
        //PartyFinderGui pfGui,
        //PartyList party,
        //SeStringManager seStringManager,
        //SigScanner sigScanner
        //TargetManager targets,
        //ToastGui toasts
    )
    {
        PluginInterface = pluginInterface;
        ClientState = clientState;
        CommandManager = commands;
        Condition = condition;
        GameGui = gameGui;

        PluginConfig = (Config) (pluginInterface.GetPluginConfig() ?? new Config());
        PluginConfig.Init(this);

        WorldHelper = new WorldHelper(this);
        PluginGui = new PluginGui(PluginConfig, WorldHelper);

        PluginInterface.UiBuilder.Draw += BuildUi;
        PluginInterface.UiBuilder.OpenConfigUi += () => _drawConfigWindow = true;
        SetupCommands();
    }

    public void Dispose()
    {
        PluginInterface.UiBuilder.Draw -= BuildUi;
        RemoveCommands();
    }

    private void OpenCommandWindow(string command, string args)
    {
        _drawConfigWindow = true;
    }

    private void SetupCommands()
    {
        CommandManager.AddHandler(
            SettingsCommand,
            new CommandInfo(OpenCommandWindow)
            {
                HelpMessage = $"Open config window for {Name}",
                ShowInHelp = true
            }
        );
    }

    private void RemoveCommands()
    {
        CommandManager.RemoveHandler(SettingsCommand);
    }

    private void BuildUi()
    {
        _drawConfigWindow = _drawConfigWindow && PluginGui.DrawPluginConfig();
        if (ClientState.IsLoggedIn)
        {
            PluginGui.DrawInWorld();
        }
    }
}
