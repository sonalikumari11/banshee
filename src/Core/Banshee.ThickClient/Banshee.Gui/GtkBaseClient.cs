// 
// GtkBaseClient.cs
//
// Author:
//   Aaron Bockover <abockover@novell.com>
//
// Copyright (C) 2007 Novell, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using Mono.Unix;

using Hyena;
using Hyena.CommandLine;

using Banshee.Base;
using Banshee.Database;
using Banshee.ServiceStack;
using Banshee.Gui.Dialogs;

namespace Banshee.Gui
{
    public abstract class GtkBaseClient : Client
    {
        private static Type client_type;

        private static string user_gtkrc = Path.Combine (Paths.ApplicationData, "gtkrc"); 
        public static void Startup<T> (string [] args) where T : GtkBaseClient
        {
            if (CheckHelpVersion ()) {
                return;
            }
        
            // Check for single instance
            DBusConnection.Connect ();
            if (DBusConnection.InstanceAlreadyRunning) {
                // Try running our friend Halie, the DBus command line client
                AppDomain.CurrentDomain.ExecuteAssembly (Path.Combine (Path.GetDirectoryName (
                    Assembly.GetEntryAssembly ().Location), "Halie.exe"));
                Gdk.Global.InitCheck (ref args);
                Gdk.Global.NotifyStartupComplete ();
                return;
            }
                    
            Hyena.Log.InformationFormat ("Running Banshee {0}", Application.Version);
            
            // This could go into GtkBaseClient, but it's probably something we
            // should really only support at each client level
            if (File.Exists (user_gtkrc) && !ApplicationContext.CommandLine.Contains ("no-gtkrc")) {
                Gtk.Rc.AddDefaultFile (user_gtkrc);
            } 
            
            // Ugly hack to avoid stupid themes that set this to 0, causing a huge
            // bug when constructing the "add to playlist" popup menu (BGO #524706)
            Gtk.Rc.ParseString ("gtk-menu-popup-delay = 225");

            // Boot the client
            Banshee.Gui.GtkBaseClient.Startup<T> ();
        }

        private static bool CheckHelpVersion ()
        {
            if (ApplicationContext.CommandLine.ContainsStart ("help")) {
                ShowHelp ();
                return true;
            } else if (ApplicationContext.CommandLine.Contains ("version")) {
                ShowVersion ();
                return true;
            }
            
            return false;
        }
        
        private static void ShowHelp ()
        {
            Console.WriteLine ("Usage: {0} [options...] [files|URIs...]", "banshee-1");
            Console.WriteLine ();
            
            Layout commands = new Layout (
                new LayoutGroup ("help", Catalog.GetString ("Help Options"),
                    new LayoutOption ("help", Catalog.GetString ("Show this help")),
                    new LayoutOption ("help-playback", Catalog.GetString ("Show options for controlling playback")),
                    new LayoutOption ("help-query-track", Catalog.GetString ("Show options for querying the playing track")),
                    new LayoutOption ("help-query-player", Catalog.GetString ("Show options for querying the playing engine")),
                    new LayoutOption ("help-ui", Catalog.GetString ("Show options for the user interface")),
                    new LayoutOption ("help-debug", Catalog.GetString ("Show options for developers and debugging")),
                    new LayoutOption ("help-all", Catalog.GetString ("Show all option groups")),
                    new LayoutOption ("version", Catalog.GetString ("Show version information"))
                ),
                
                new LayoutGroup ("playback", Catalog.GetString ("Playback Control Options"),
                    new LayoutOption ("next", Catalog.GetString ("Play the next track, optionally restarting if the 'restart' value is set")),
                    new LayoutOption ("previous", Catalog.GetString ("Play the previous track, optionally restarting if the 'restart value is set")),
                    new LayoutOption ("play-enqueued", Catalog.GetString ("Automatically start playing any tracks enqueued on the command line")),
                    new LayoutOption ("play", Catalog.GetString ("Start playback")),
                    new LayoutOption ("pause", Catalog.GetString ("Pause playback")),
                    new LayoutOption ("stop", Catalog.GetString ("Completely stop playback")),
                    new LayoutOption ("stop-when-finished", Catalog.GetString (
                        "Enable or disable playback stopping after the currently playing track (value should be either 'true' or 'false')")),
                    new LayoutOption ("set-volume=LEVEL", Catalog.GetString ("Set the playback volume (0-100)")),
                    new LayoutOption ("set-position=POS", Catalog.GetString ("Seek to a specific point (seconds, float)"))
                ),
                
                new LayoutGroup ("query-player", Catalog.GetString ("Player Engine Query Options"),
                    new LayoutOption ("query-current-state", Catalog.GetString ("Current player state")),
                    new LayoutOption ("query-last-state", Catalog.GetString ("Last player state")),
                    new LayoutOption ("query-can-pause", Catalog.GetString ("Query whether the player can be paused")),
                    new LayoutOption ("query-can-seek", Catalog.GetString ("Query whether the player can seek")),
                    new LayoutOption ("query-volume", Catalog.GetString ("Player volume")),
                    new LayoutOption ("query-position", Catalog.GetString ("Player position in currently playing track"))
                ),
                
                new LayoutGroup ("query-track", Catalog.GetString ("Playing Track Metadata Query Options"),
                    new LayoutOption ("query-uri", Catalog.GetString ("URI")),
                    new LayoutOption ("query-artist", Catalog.GetString ("Artist Name")),
                    new LayoutOption ("query-album", Catalog.GetString ("Album Title")),
                    new LayoutOption ("query-title", Catalog.GetString ("Track Title")),
                    new LayoutOption ("query-duration", Catalog.GetString ("Duration")),
                    new LayoutOption ("query-track-number", Catalog.GetString ("Track Number")),
                    new LayoutOption ("query-track-count", Catalog.GetString ("Track Count")),
                    new LayoutOption ("query-disc", Catalog.GetString ("Disc Number")),
                    new LayoutOption ("query-year", Catalog.GetString ("Year")),
                    new LayoutOption ("query-rating", Catalog.GetString ("Rating"))
                ),
                
                new LayoutGroup ("ui", Catalog.GetString ("User Interface Options"),
                    new LayoutOption ("show|--present", Catalog.GetString ("Present the user interface on the active workspace")),
                    new LayoutOption ("hide", Catalog.GetString ("Hide the user interface")),
                    new LayoutOption ("no-present", Catalog.GetString ("Do not present the user interface, regardless of any other options"))
                ),
                
                new LayoutGroup ("debugging", Catalog.GetString ("Debugging and Development Options"), 
                    new LayoutOption ("debug", Catalog.GetString ("Enable general debugging features")),
                    new LayoutOption ("debug-sql", Catalog.GetString ("Enable debugging output of SQL queries")),
                    new LayoutOption ("debug-addins", Catalog.GetString ("Enable debugging output of Mono.Addins")),
                    new LayoutOption ("db=FILE", Catalog.GetString ("Specify an alternate database to use")),
                    new LayoutOption ("uninstalled", Catalog.GetString ("Optimize instance for running uninstalled; " + 
                        "most notably, this will create an alternate Mono.Addins database in the working directory")),
                    new LayoutOption ("disable-dbus", Catalog.GetString ("Disable DBus support completely")),
                    new LayoutOption ("no-gtkrc", String.Format (Catalog.GetString (
                        "Skip loading a custom gtkrc file ({0}) if it exists"), 
                        user_gtkrc.Replace (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "~")))
                )
            );
            
            if (ApplicationContext.CommandLine.Contains ("help-all")) {
                Console.WriteLine (commands);
                return;
            }
            
            List<string> errors = null;
            
            foreach (KeyValuePair<string, string> argument in ApplicationContext.CommandLine.Arguments) {
                switch (argument.Key) {
                    case "help": Console.WriteLine (commands.ToString ("help")); break;
                    case "help-debug": Console.WriteLine (commands.ToString ("debugging")); break;
                    case "help-query-track": Console.WriteLine (commands.ToString ("query-track")); break;
                    case "help-query-player": Console.WriteLine (commands.ToString ("query-player")); break;
                    case "help-ui": Console.WriteLine (commands.ToString ("ui")); break;
                    case "help-playback": Console.WriteLine (commands.ToString ("playback")); break;
                    default:
                        if (argument.Key.StartsWith ("help")) {
                            (errors ?? errors = new List<string> ()).Add (argument.Key);
                        }
                        break;
                }
            }
            
            if (errors != null) {
                Console.WriteLine (commands.LayoutLine (String.Format (Catalog.GetString (
                    "The following help arguments are invalid: {0}"),
                    Hyena.Collections.CollectionExtensions.Join (errors, "--", null, ", "))));
            }
        }
        
        private static void ShowVersion ()
        {
            Console.WriteLine ("Banshee {0} ({1}) http://banshee-project.org", Application.DisplayVersion, Application.Version);
            Console.WriteLine ("Copyright 2005-{0} Novell, Inc. and Contributors.", DateTime.Now.Year);
        }

        public static void Startup<T> () where T : GtkBaseClient
        {
            if (client_type != null) {
                throw new ApplicationException ("Only a single GtkBaseClient can be initialized through Entry<T>");
            }
            
            client_type = typeof (T);
            Hyena.Gui.CleanRoomStartup.Startup (Startup);
        }
        
        private static void Startup ()
        {
            ((GtkBaseClient)Activator.CreateInstance (client_type)).Run ();
        }
        
        private string default_icon_name;
        
        protected GtkBaseClient () : this (true, Application.IconName)
        {
        }
        
        protected GtkBaseClient (bool initializeDefault, string defaultIconName)
        {
            this.default_icon_name = defaultIconName;
            if (initializeDefault) {
                Initialize (true);
            }
        }
        
        protected void Initialize (bool registerCommonServices)
        {
            // Set the process name so system process listings and commands are pretty
            PlatformHacks.TrySetProcessName (Application.InternalName);
            
            // Initialize GTK
            Gtk.Application.Init ();
            Gtk.Window.DefaultIconName = default_icon_name;

            ThreadAssist.InitializeMainThread ();
            
            Gdk.Global.ProgramClass = Application.InternalName;

            if (ApplicationContext.Debugging) {
                GLib.Log.SetLogHandler ("Gtk", GLib.LogLevelFlags.Critical, GLib.Log.PrintTraceLogFunction);
            }
            
            ServiceManager.ServiceStarted += OnServiceStarted;
            
            // Register specific services this client will care about
            if (registerCommonServices) {
                Banshee.Gui.CommonServices.Register ();
            }
            
            OnRegisterServices ();
            
            Application.ShutdownPromptHandler = OnShutdownPrompt;
            Application.TimeoutHandler = RunTimeout;
            Application.IdleHandler = RunIdle;
            Application.IdleTimeoutRemoveHandler = IdleTimeoutRemove;
            
            // Start the core boot process
            
            Application.PushClient (this);
            Application.Run ();
            
            Log.Notify += OnLogNotify;
        }
        
        public virtual void Run ()
        {
            RunIdle (delegate { OnStarted (); return false; });
            Gtk.Application.Run ();
        }
        
        protected virtual void OnRegisterServices ()
        {
        }

        private void OnServiceStarted (ServiceStartedArgs args)
        {
            if (args.Service is BansheeDbConnection) {
                ServiceManager.ServiceStarted -= OnServiceStarted;
                BansheeDbFormatMigrator migrator = ((BansheeDbConnection)args.Service).Migrator;
                if (migrator != null) {
                    migrator.Started += OnMigratorStarted;
                    migrator.Finished += OnMigratorFinished;
                }
            }
        }
        
        private void OnMigratorStarted (object o, EventArgs args)
        {
            BansheeDbFormatMigrator migrator = (BansheeDbFormatMigrator)o;
            new BansheeDbFormatMigratorMonitor (migrator);
        }

        private void OnMigratorFinished (object o, EventArgs args)
        {
            BansheeDbFormatMigrator migrator = (BansheeDbFormatMigrator)o;
            migrator.Started -= OnMigratorStarted;
            migrator.Finished -= OnMigratorFinished;
        }

        private void OnLogNotify (LogNotifyArgs args)
        {
            RunIdle (delegate {
                ShowLogCoreEntry (args.Entry);
                return false;
            });
        }
                
        private void ShowLogCoreEntry (LogEntry entry)
        {
            Gtk.Window window = null;
            Gtk.MessageType mtype;
            
            if (ServiceManager.Contains<GtkElementsService> ()) {
                window = ServiceManager.Get<GtkElementsService> ().PrimaryWindow;
            }
            
            switch (entry.Type) {
                case LogEntryType.Warning:
                    mtype = Gtk.MessageType.Warning;
                    break;
                case LogEntryType.Information:
                    mtype = Gtk.MessageType.Info;
                    break;
                case LogEntryType.Error:
                default:
                    mtype = Gtk.MessageType.Error;
                    break;
            }
              
            Banshee.Widgets.HigMessageDialog dialog = new Banshee.Widgets.HigMessageDialog (
                window, Gtk.DialogFlags.Modal, mtype, Gtk.ButtonsType.Close, entry.Message, entry.Details);
            
            dialog.Title = String.Empty;
            dialog.Run ();
            dialog.Destroy ();
        }
        
        private bool OnShutdownPrompt ()
        {
            ConfirmShutdownDialog dialog = new ConfirmShutdownDialog ();
            try {
                return dialog.Run () != Gtk.ResponseType.Cancel;
            } finally {
                dialog.Destroy ();
            }
        }
        
        protected uint RunTimeout (uint milliseconds, TimeoutHandler handler)
        {
            return GLib.Timeout.Add (milliseconds, delegate { return handler (); });
        }
        
        protected uint RunIdle (IdleHandler handler)
        {
            return GLib.Idle.Add (delegate { return handler (); });
        }
        
        protected bool IdleTimeoutRemove (uint id)
        {
            return GLib.Source.Remove (id);
        }
    }
}
