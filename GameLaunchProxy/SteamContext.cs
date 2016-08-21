using Steam4NET;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
//using HSteamPipe = System.Int32;
//using HSteamUser = System.Int32;

namespace GameLaunchProxy
{
    class SteamException : Exception
    {
        public SteamException(string msg)
            : base(msg)
        {
        }
    }
    static class SteamContext
    {
        static bool libraryLoaded = false;

        //public static HSteamPipe Pipe { get; private set; }
        public static Int32 Pipe { get; private set; }
        //public static HSteamUser User { get; private set; }
        public static Int32 User { get; private set; }

        public static IClientShortcuts ClientShortcuts { get; private set; }
        public static IClientEngine ClientEngine { get; private set; }
        public static ISteamClient017 SteamClient { get; private set; }

        public static void Init()
        {

            libraryLoaded = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .Where(a => a.FullName.Contains("Steam4NET"))
                .Select(a => String.Format(
                    CultureInfo.InvariantCulture,
                    "[{0}] {1}",
                    a.GlobalAssemblyCache,
                    a.FullName
                )).ToArray().Count() > 0;

            if(!libraryLoaded)
            {
                throw new SteamException("Unable to load Steam4NET library.");
            }

            if (!Steamworks.Load())
            {
                throw new SteamException("Unable to load steamclient library.");
            }

            if (SteamClient == null)
            {
                SteamClient = Steamworks.CreateInterface<ISteamClient017>();

                if (SteamClient == null)
                {
                    throw new SteamException("Unable to get ISteamClient interface.");
                }
            }

            //if (Pipe == null)
            if (Pipe == 0)
            {
                Pipe = SteamClient.CreateSteamPipe();

                //if (Pipe == null)
                if (Pipe == 0)
                {
                    throw new SteamException("Unable to create steam pipe.");
                }
            }

            if (ClientEngine == null)
            {
                ClientEngine = Steamworks.CreateInterface<IClientEngine>();
                if (ClientEngine == null)
                {
                    throw new SteamException("Unable to get IClientEngine interface.");
                }
            }

            //if (User == null)
            if (User == 0 || User == -1)
            {
                User = SteamClient.ConnectToGlobalUser(Pipe);

                //if (User == null)
                if (User == 0 || User == -1)
                {
                    throw new SteamException("Unable to connect to global user.");
                }
            }

            if (ClientShortcuts == null)
            {
                ClientShortcuts = ClientEngine.GetIClientShortcuts<IClientShortcuts>(User, Pipe);

                if (ClientShortcuts == null)
                {
                    throw new SteamException("Unable to get GetIClientShortcuts interface.");
                }
            }
        }

        public static void SetShortcutName(UInt64 shortcutID, string steamShortcutID, string newName)
        {
            if (!libraryLoaded)
            {
                throw new SteamException("Unable to load Steam4NET library.");
            }

            try
            {
                uint shortCutCount = ClientShortcuts.GetShortcutCount();
                for (uint index = 0; index < shortCutCount; index++)
                {
                    uint appID = ClientShortcuts.GetShortcutAppIDByIndex(index);

                    CGameID gameID = ClientShortcuts.GetGameIDForAppID(appID);
                    if (shortcutID == gameID.ConvertToUint64()
                      || ClientShortcuts.GetShortcutExeByIndex(index).Contains($"-steamproxyactivate {steamShortcutID}"))
                    {
                        string name = ClientShortcuts.GetShortcutAppNameByAppID(appID);
                        ClientShortcuts.SetShortcutAppName(appID, newName);
                        return;
                    }
                }
            }
            catch { }
        }

        public static string GetShortcutName(UInt64 shortcutID, string steamShortcutID)
        {
            if (!libraryLoaded)
            {
                throw new SteamException("Unable to load Steam4NET library.");
            }

            try
            {
                uint shortCutCount = ClientShortcuts.GetShortcutCount();
                for (uint index = 0; index < shortCutCount; index++)
                {
                    uint appID = ClientShortcuts.GetShortcutAppIDByIndex(index);

                    CGameID gameID = ClientShortcuts.GetGameIDForAppID(appID);
                    if (shortcutID == gameID.ConvertToUint64()
                      || ClientShortcuts.GetShortcutExeByIndex(index).EndsWith($"-steamproxyactivate {steamShortcutID}"))
                    {
                        return ClientShortcuts.GetShortcutAppNameByAppID(appID);
                    }
                }
            }
            catch { }
            return null;
        }

        public static UInt64 FindShortcut(UInt64 shortcutID, string steamShortcutID)
        {
            if (!libraryLoaded)
            {
                throw new SteamException("Unable to load Steam4NET library.");
            }

            try
            {
                uint shortCutCount = ClientShortcuts.GetShortcutCount();
                for (uint index = 0; index < shortCutCount; index++)
                {
                    uint appID = ClientShortcuts.GetShortcutAppIDByIndex(index);

                    CGameID gameID = ClientShortcuts.GetGameIDForAppID(appID);
                    if (shortcutID == gameID.ConvertToUint64()
                      || ClientShortcuts.GetShortcutExeByIndex(index).EndsWith($"-steamproxyactivate {steamShortcutID}"))
                    {
                        return gameID.ConvertToUint64();
                    }
                }
            }
            catch { }
            return 0;
        }

        public static void Shutdown()
        {

            //CallbackDispatcher.StopDispatchThread(Pipe);

            if (SteamClient != null && User != 0 && User != -1)
            //if(User != null)
            {
                SteamClient.ReleaseUser(Pipe, User);
            }

            if (SteamClient != null && Pipe != 0)
            //if (Pipe != null)
            {
                //SteamClient.ReleaseSteamPipe(Pipe);
                SteamClient.BReleaseSteamPipe(Pipe);
            }

        }
    }
}
