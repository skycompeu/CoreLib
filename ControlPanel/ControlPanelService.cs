using CoreLib.Extensions;
using CoreLib.WinNative;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//https://github.com/CoenraadS/Windows-Control-Panel-Items
namespace CoreLib.ControlPanel {
    public class ControlPanelService : IControlPanelService {
        public RegistryKey ControlPanelRegistryPath { get; set; }
        public RegistryKey ClsId { get; set; }

        private const string CONTROL = @"%SystemRoot%\System32\control.exe";
        static IntPtr DefaultIconPtr;
        private const uint GROUP_ICON = 14;

        public ControlPanelService() {
            ControlPanelRegistryPath = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\ControlPanel\\NameSpace");
            ClsId = Registry.ClassesRoot.OpenSubKey("CLSID");
        }

        public List<ControlPanelItem> GetControlPanelItems(int iconSize = 48) {
            List<ControlPanelItem> controlPanelItems = new List<ControlPanelItem>();

            var localizedString = String.Empty;
            var infoTip = String.Empty;
            Icon icon = null;


            var keys = ControlPanelRegistryPath.GetSubKeyNames();
            foreach (var key in keys) {

                var currentKey = ClsId.OpenSubKey(key);
                if (currentKey.IsNotNull()) {
                    var executablePath = GetProcessStartInfo(currentKey);
                    if (executablePath.IsNotNull()) {

                        localizedString = GetLocalizedString(currentKey);

                        if (!localizedString.StringIsNullOrEmpty()) {

                            infoTip = GetInfoTip(currentKey);
                            icon = GetIcon(currentKey, iconSize);

                            controlPanelItems.Add(new ControlPanelItem(localizedString, infoTip, key, executablePath, icon));
                        }
                    }
                }
            }

            return controlPanelItems;
        }

        private ProcessStartInfo GetProcessStartInfo(RegistryKey regKey) {
            var exePath = new ProcessStartInfo();
            var appName = String.Empty;

            var systemApplicationName = regKey.GetValue("System.ApplicationName");
            if (systemApplicationName.IsNotNull()) {
                appName = systemApplicationName.ToString();
                var envVar = Environment.ExpandEnvironmentVariables(CONTROL);

                exePath.FileName = envVar;
                exePath.Arguments = $"-name " + appName;

            } else if (regKey.OpenSubKey("Shell\\Open\\Command").IsNotNull() && regKey.OpenSubKey("Shell\\Open\\Command").GetValue(null).IsNotNull()) {
                
                string input = "\"" + Environment.ExpandEnvironmentVariables(regKey.OpenSubKey("Shell\\Open\\Command").GetValue(null).ToString()) + "\"";
                exePath.FileName = "cmd.exe";
                exePath.Arguments = "/C " + input;
                exePath.WindowStyle = ProcessWindowStyle.Hidden;

            } else {
                exePath = null;
            }

            return exePath;
        }

        private string GetLocalizedString(RegistryKey currentKey) {
            IntPtr dataFilePointer;
            string[] localizedStringRaw;
            uint stringTableIndex;
            StringBuilder resource;
            string localizedString;

            if (currentKey.GetValue("LocalizedString") != null) {
                localizedStringRaw = currentKey.GetValue("LocalizedString").ToString().Split(new[] { ",-" }, StringSplitOptions.None);

                if (localizedStringRaw.Length > 1) {
                    if (localizedStringRaw[0][0] == '@') {
                        localizedStringRaw[0] = localizedStringRaw[0].Substring(1);
                    }

                    localizedStringRaw[0] = Environment.ExpandEnvironmentVariables(localizedStringRaw[0]);

                    //Load file with strings
                    dataFilePointer = Kernel32.LoadLibraryEx(localizedStringRaw[0], IntPtr.Zero, Kernel32.LOAD_LIBRARY_AS_DATAFILE);

                    stringTableIndex = SanitizeUint(localizedStringRaw[1]);

                    resource = new StringBuilder(255);
                    Kernel32.LoadString(dataFilePointer, stringTableIndex, resource, resource.Capacity + 1); //Extract needed string
                    Kernel32.FreeLibrary(dataFilePointer);

                    localizedString = resource.ToString();

                    //Some apps don't return a string, although they do have a stringIndex. Use Default value.

                    if (String.IsNullOrEmpty(localizedString)) {
                        if (currentKey.GetValue(null) != null) {
                            localizedString = currentKey.GetValue(null).ToString();
                        } else {
                            return null; //Cannot have item without title.
                        }
                    }
                } else {
                    localizedString = localizedStringRaw[0];
                }
            } else if (currentKey.GetValue(null) != null) {
                localizedString = currentKey.GetValue(null).ToString();
            } else {
                return null; //Cannot have item without title.
            }
            return localizedString;
        }

        private static uint SanitizeUint(string args) {
            int x = 0;

            while (x < args.Length && !Char.IsDigit(args[x])) {
                args = args.Substring(1);
            }

            x = 0;

            while (x < args.Length && Char.IsDigit(args[x])) {
                x++;
            }

            if (x < args.Length) {
                args = args.Remove(x);
            }

            /*If the logic is correct, this should never through an exception.
             * If there is an exception, then need to analyze what the input is.
             * Returning the wrong number will cause more errors */
            return Convert.ToUInt32(args);
        }


        private static string GetInfoTip(RegistryKey currentKey) {
            IntPtr dataFilePointer;
            string[] infoTipRaw;
            uint stringTableIndex;
            StringBuilder resource;
            string infoTip = "";

            if (currentKey.GetValue("InfoTip") != null) {
                infoTipRaw = currentKey.GetValue("InfoTip").ToString().Split(new[] { ",-" }, StringSplitOptions.None);

                if (infoTipRaw.Length == 2) {
                    if (infoTipRaw[0][0] == '@') {
                        infoTipRaw[0] = infoTipRaw[0].Substring(1);
                    }
                    infoTipRaw[0] = Environment.ExpandEnvironmentVariables(infoTipRaw[0]);

                    dataFilePointer = Kernel32.LoadLibraryEx(infoTipRaw[0], IntPtr.Zero, Kernel32.LOAD_LIBRARY_AS_DATAFILE); //Load file with strings

                    stringTableIndex = SanitizeUint(infoTipRaw[1]);

                    resource = new StringBuilder(255);
                    Kernel32.LoadString(dataFilePointer, stringTableIndex, resource, resource.Capacity + 1); //Extract needed string
                    Kernel32.FreeLibrary(dataFilePointer);

                    infoTip = resource.ToString();
                } else {
                    infoTip = currentKey.GetValue("InfoTip").ToString();
                }
            } else {
                infoTip = "";
            }

            return infoTip;
        }

        private static Icon GetIcon(RegistryKey currentKey, int iconSize) {
            IntPtr iconPtr = IntPtr.Zero;
            List<string> iconString;
            IntPtr dataFilePointer;
            IntPtr iconIndex;
            Icon myIcon = null;

            if (currentKey.OpenSubKey("DefaultIcon") != null) {
                if (currentKey.OpenSubKey("DefaultIcon").GetValue(null) != null) {
                    iconString = new List<string>(currentKey.OpenSubKey("DefaultIcon").GetValue(null).ToString().Split(new[] { ',' }, 2));
                    if (string.IsNullOrEmpty(iconString[0])) {
                        // fallback to default icon
                        return null;
                    }

                    if (iconString[0][0] == '@') {
                        iconString[0] = iconString[0].Substring(1);
                    }

                    dataFilePointer = Kernel32.LoadLibraryEx(iconString[0], IntPtr.Zero, Kernel32.LOAD_LIBRARY_AS_DATAFILE);

                    if (iconString.Count == 2) {
                        iconIndex = (IntPtr)SanitizeUint(iconString[1]);

                        iconPtr = Kernel32.LoadImage(dataFilePointer, iconIndex, 1, iconSize, iconSize, 0);
                    }

                    if (iconPtr == IntPtr.Zero) {
                        DefaultIconPtr = IntPtr.Zero;
                        Kernel32.EnumResourceNamesWithID(dataFilePointer, GROUP_ICON, EnumRes, IntPtr.Zero); //Iterate through resources. 

                        iconPtr = Kernel32.LoadImage(dataFilePointer, DefaultIconPtr, 1, iconSize, iconSize, 0);
                    }

                    Kernel32.FreeLibrary(dataFilePointer);

                    if (iconPtr != IntPtr.Zero) {
                        try {
                            myIcon = Icon.FromHandle(iconPtr);
                            myIcon = (Icon)myIcon.Clone(); //Remove pointer dependancy.
                        } catch {
                            //Silently fail for now.
                        }
                    }
                }
            }

            if (iconPtr != IntPtr.Zero) {
                Kernel32.DestroyIcon(iconPtr);
            }
            return myIcon;
        }

        private static bool EnumRes(IntPtr hModule, IntPtr lpszType, IntPtr lpszName, IntPtr lParam) {
            DefaultIconPtr = lpszName;
            return false;
        }


    }
}
