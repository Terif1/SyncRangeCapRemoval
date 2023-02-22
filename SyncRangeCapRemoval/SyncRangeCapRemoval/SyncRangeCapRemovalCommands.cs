using Sandbox.Game.World;
using Torch.Commands;
using Torch.Commands.Permissions;
using VRage.Game.ModAPI;

namespace SyncRangeCapRemoval
{
    public class SyncRangeCapRemovalCommands : CommandModule
    {

        public SyncRangeCapRemoval Plugin => (SyncRangeCapRemoval)Context.Plugin;

        [Command("syncrange", "Gets the current Sync range")]
        [Permission(MyPromoteLevel.Admin)]
        public void sync()
        {
            Context.Respond($"current sync range is {MySession.Static.Settings.SyncDistance}");
        }
    }
}
