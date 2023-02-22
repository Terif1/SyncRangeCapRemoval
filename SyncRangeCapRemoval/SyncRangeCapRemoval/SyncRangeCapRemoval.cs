using NLog;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using HarmonyLib;
using Sandbox.Game.World;
using Torch;
using Torch.API;
using Torch.API.Managers;
using Torch.API.Plugins;
using Torch.API.Session;
using Torch.Session;
using VRage.Game;
using VRage.ObjectBuilders;
using VRageMath;

namespace SyncRangeCapRemoval
{
    public class SyncRangeCapRemoval : TorchPluginBase, IWpfPlugin
    {

        public static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private static readonly string CONFIG_FILE_NAME = "SyncRangeCapRemovalConfig.cfg";

        private SyncRangeCapRemovalControl _control;
        public UserControl GetControl() => _control ?? (_control = new SyncRangeCapRemovalControl(this));

        private Persistent<SyncRangeCapRemovalConfig> _config;
        public SyncRangeCapRemovalConfig Config => _config?.Data;

        public override void Init(ITorchBase torch)
        {
            base.Init(torch);

            SetupConfig();

            var sessionManager = Torch.Managers.GetManager<TorchSessionManager>();
            if (sessionManager != null)
                sessionManager.SessionStateChanged += SessionChanged;
            else
                Log.Warn("No session manager loaded!");

            Save();
            Patches.SyncMax = Config.SyncMaxRange;
            new Harmony("SyncRangeCapRemoval").PatchAll(Assembly.GetExecutingAssembly());
        }

        private void SessionChanged(ITorchSession session, TorchSessionState state)
        {

            switch (state)
            {

                case TorchSessionState.Loaded:
                    Log.Info("Session Loaded!");
                    break;

                case TorchSessionState.Unloading:
                    Log.Info("Session Unloading!");
                    break;
            }
        }

        private void SetupConfig()
        {

            var configFile = Path.Combine(StoragePath, CONFIG_FILE_NAME);

            try
            {

                _config = Persistent<SyncRangeCapRemovalConfig>.Load(configFile);

            }
            catch (Exception e)
            {
                Log.Warn(e);
            }

            if (_config?.Data == null)
            {

                Log.Info("Create Default Config, because none was found!");

                _config = new Persistent<SyncRangeCapRemovalConfig>(configFile, new SyncRangeCapRemovalConfig());
                _config.Save();
            }
        }

        public void Save()
        {
            try
            {
                _config.Save();
                Log.Info("Configuration Saved.");
            }
            catch (IOException e)
            {
                Log.Warn(e, "Configuration failed to save");
            }
        }
    }

    [HarmonyPatch]
    public static class Patches
    {
        public static int SyncMax;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MySession), "FixIncorrectSettings")]
        public static bool Prefix(MyObjectBuilder_SessionSettings settings)
        {
            MyObjectBuilder_SessionSettings newObject = MyObjectBuilderSerializer.CreateNewObject<MyObjectBuilder_SessionSettings>();
      if ((double) settings.RefinerySpeedMultiplier <= 0.0)
        settings.RefinerySpeedMultiplier = newObject.RefinerySpeedMultiplier;
      if ((double) settings.AssemblerSpeedMultiplier <= 0.0)
        settings.AssemblerSpeedMultiplier = newObject.AssemblerSpeedMultiplier;
      if ((double) settings.AssemblerEfficiencyMultiplier <= 0.0)
        settings.AssemblerEfficiencyMultiplier = newObject.AssemblerEfficiencyMultiplier;
      if ((double) settings.InventorySizeMultiplier <= 0.0)
        settings.InventorySizeMultiplier = newObject.InventorySizeMultiplier;
      if ((double) settings.WelderSpeedMultiplier <= 0.0)
        settings.WelderSpeedMultiplier = newObject.WelderSpeedMultiplier;
      if ((double) settings.GrinderSpeedMultiplier <= 0.0)
        settings.GrinderSpeedMultiplier = newObject.GrinderSpeedMultiplier;
      if ((double) settings.HackSpeedMultiplier <= 0.0)
        settings.HackSpeedMultiplier = newObject.HackSpeedMultiplier;
      if (!settings.PermanentDeath.HasValue)
        settings.PermanentDeath = new bool?(true);
      settings.ViewDistance = MathHelper.Clamp(settings.ViewDistance, 1000, 50000);
      settings.SyncDistance = MathHelper.Clamp(settings.SyncDistance, 1000, SyncMax);
      if (Sandbox.Engine.Platform.Game.IsDedicated)
      {
        settings.Scenario = false;
        settings.ScenarioEditMode = false;
      }
      if (MySession.Static != null && MySession.Static.Scenario != null)
        settings.WorldSizeKm = MySession.Static.Scenario.HasPlanets ? 0 : settings.WorldSizeKm;
      if (MySession.Static == null || MySession.Static.WorldBoundaries.HasValue || settings.WorldSizeKm <= 0)
        return false;
      double num = (double) (settings.WorldSizeKm * 500);
      if (num <= 0.0)
        return false;
      MySession.Static.WorldBoundaries = new BoundingBoxD?(new BoundingBoxD(new Vector3D(-num, -num, -num), new Vector3D(num, num, num)));
      return false;
        }
    }
}
