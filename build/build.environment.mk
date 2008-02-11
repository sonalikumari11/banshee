# Initializers
MONO_BASE_PATH = 
MONO_ADDINS_PATH =

# Install Paths
DEFAULT_INSTALL_DIR = $(pkglibdir)
ADDINS_INSTALL_DIR = $(DEFAULT_INSTALL_DIR)/addins

# External libraries to link against, generated from configure
LINK_SYSTEM = -r:System
LINK_SQLITE = -r:System.Data -r:Mono.Data.Sqlite
LINK_CAIRO = -r:Mono.Cairo
LINK_MONO_POSIX = -r:Mono.Posix
LINK_GLIB = $(GLIBSHARP_LIBS)
LINK_GTK = $(GTKSHARP_LIBS)
LINK_GCONF = $(GCONFSHARP_LIBS)
LINK_DBUS = $(NDESK_DBUS_LIBS)
LINK_TAGLIB = $(TAGLIB_SHARP_LIBS)

LINK_MONO_ADDINS_DEPS = $(MONO_ADDINS_LIBS)
LINK_MONO_ADDINS_SETUP_DEPS = $(MONO_ADDINS_SETUP_LIBS)
LINK_MONO_ADDINS_GUI_DEPS = $(MONO_ADDINS_GUI_LIBS)

# Internal directories/libraries

DIR_BIN = $(top_builddir)/bin

# Libraries
#
#DIR_BOO = $(DIR_LIBRARIES)/Boo
#if EXTERNAL_BOO
#LINK_BOO = $(BOO_LIBS)
#else
#LINK_BOO = \
#	-r:$(DIR_BOO)/Boo.Lang.dll \
#	-r:$(DIR_BOO)/Boo.Lang.Compiler.dll \
#	-r:$(DIR_BOO)/Boo.Lang.Interpreter.dll
#endif
#
#DIR_BOOBUDDY = $(DIR_LIBRARIES)/BooBuddy
#MONO_BASE_PATH += $(DIR_BOOBUDDY)
#REF_BOOBUDDY = $(LINK_GTK) $(LINK_BOO)
#LINK_BOOBUDDY = -r:$(DIR_BOOBUDDY)/BooBuddy.dll
#LINK_BOOBUDDY_DEPS = $(REF_BOOBUDDY) $(LINK_BOOBUDDY)
#
#DIR_GNOME_KEYRING = $(DIR_LIBRARIES)/Gnome.Keyring
#MONO_BASE_PATH += $(DIR_GNOME_KEYRING)
#REF_GNOME_KEYRING = $(LINK_DBUS) $(LINK_MONO_POSIX)
#LINK_GNOME_KEYRING = -r:$(DIR_GNOME_KEYRING)/Gnome.Keyring.dll
#LINK_GNOME_KEYRING_DEPS = $(REF_GNOME_KEYRING) $(LINK_GNOME_KEYRING)
#
REF_LASTFM = $(LINK_SYSTEM)
LINK_LASTFM = -r:$(DIR_BIN)/Lastfm.dll
LINK_LASTFM_DEPS = $(REF_LASTFM) $(LINK_LASTFM)

REF_LASTFM_GUI = $(LINK_MONO_POSIX) $(LINK_GLIB) $(LINK_GTK) $(LINK_LASTFM_DEPS)
LINK_LASTFM_GUI = -r:$(DIR_BIN)/Lastfm.Gui.dll
LINK_LASTFM_GUI_DEPS = $(REF_LASTFM_GUI) $(LINK_LASTFM_GUI)
#
#DIR_MUSICBRAINZ = $(DIR_LIBRARIES)/MusicBrainz
#MONO_BASE_PATH += $(DIR_MUSICBRAINZ)
#REF_MUSICBRAINZ = $(LINK_SYSTEM)
#LINK_MUSICBRAINZ = -r:$(DIR_MUSICBRAINZ)/MusicBrainz.dll
#LINK_MUSICBRAINZ_DEPS = $(REF_MUSICBRAINZ) $(LINK_MUSICBRAINZ)


# Core
REF_HYENA = $(LINK_SYSTEM) $(LINK_SQLITE)
LINK_HYENA = -r:$(DIR_BIN)/Hyena.dll
LINK_HYENA_DEPS = $(REF_HYENA) $(LINK_HYENA)

REF_HYENA_GUI = $(LINK_HYENA_DEPS) $(LINK_MONO_POSIX) $(LINK_CAIRO) $(LINK_GTK)
LINK_HYENA_GUI = -r:$(DIR_BIN)/Hyena.Gui.dll
LINK_HYENA_GUI_DEPS = $(REF_HYENA_GUI) $(LINK_HYENA_GUI)

REF_BANSHEE_CORE = $(LINK_HYENA_DEPS) $(LINK_MONO_POSIX) $(LINK_GLIB) \
	$(LINK_DBUS) $(LINK_TAGLIB) $(LINK_MONO_ADDINS_DEPS)
LINK_BANSHEE_CORE = -r:$(DIR_BIN)/Banshee.Core.dll
LINK_BANSHEE_CORE_DEPS = $(REF_BANSHEE_CORE) $(LINK_BANSHEE_CORE)

REF_BANSHEE_SERVICES = $(LINK_SQLITE) $(LINK_BANSHEE_CORE_DEPS)
LINK_BANSHEE_SERVICES = -r:$(DIR_BIN)/Banshee.Services.dll
LINK_BANSHEE_SERVICES_DEPS = $(REF_BANSHEE_SERVICES) $(LINK_BANSHEE_SERVICES)

REF_BANSHEE_WIDGETS = $(LINK_MONO_POSIX) $(LINK_CAIRO) $(LINK_GTK)
LINK_BANSHEE_WIDGETS = -r:$(DIR_BIN)/Banshee.Widgets.dll
LINK_BANSHEE_WIDGETS_DEPS = $(REF_BANSHEE_WIDGETS) $(LINK_BANSHEE_WIDGETS)

REF_BANSHEE_THICKCLIENT = $(LINK_BANSHEE_WIDGETS_DEPS) \
	$(LINK_BANSHEE_SERVICES_DEPS) $(LINK_HYENA_GUI_DEPS) $(LINK_MONO_ADDINS_SETUP_DEPS) $(LINK_MONO_ADDINS_GUI_DEPS)
LINK_BANSHEE_THICKCLIENT = -r:$(DIR_BIN)/Banshee.ThickClient.dll
LINK_BANSHEE_THICKCLIENT_DEPS = $(REF_BANSHEE_THICKCLIENT) \
	$(LINK_BANSHEE_THICKCLIENT)

REF_NEREID = $(LINK_BANSHEE_THICKCLIENT_DEPS)

# Dap
REF_DAP_DAPCORE = $(LINK_BANSHEE_SERVICES_DEPS)
LINK_DAP_DAPCORE = -r:$(DIR_BIN)/Banshee.DapCore.dll
LINK_DAP_DAPCORE_DEPS = $(REF_DAP_DAPCORE) $(LINK_DAP_DAPCORE)

# Backends
REF_BACKEND_GNOME = $(LINK_BANSHEE_CORE_DEPS) $(LINK_GCONF)
REF_BACKEND_GSTREAMER = $(LINK_BANSHEE_SERVICES_DEPS) $(LINK_GLIB)
REF_BACKEND_UNIX = $(LINK_BANSHEE_CORE_DEPS) $(LINK_MONO_POSIX)

# Extensions
REF_EXTENSION_AUDIOSCROBBLER = $(LINK_BANSHEE_SERVICES_DEPS)
REF_EXTENSION_MULTIMEDIAKEYS = $(LINK_BANSHEE_SERVICES_DEPS)
REF_EXTENSION_NOTIFICATIONAREA = $(LINK_BANSHEE_THICKCLIENT_DEPS)
REF_EXTENSION_PLAYQUEUE = $(LINK_BANSHEE_THICKCLIENT_DEPS)
REF_EXTENSION_LASTFM = $(LINK_BANSHEE_THICKCLIENT_DEPS) $(LINK_LAST_FM) -r:System.Data -r:System.Web -r:System.Xml

# Build rules
# Ignoring 0278 due to a bug in gmcs: 
# http://bugzilla.ximian.com/show_bug.cgi?id=79998
BUILD_FLAGS = -debug -nowarn:0278 $(ASSEMBLY_BUILD_FLAGS)
BUILD = $(MCS) $(BUILD_FLAGS)
BUILD_LIB = $(BUILD) -target:library

# Cute hack to replace a space with something
colon:= :
empty:=
space:= $(empty) $(empty)

# Build path to allow running uninstalled
RUN_PATH = $(subst $(space),$(colon), $(MONO_BASE_PATH))

