using System;
using Gtk;
using GObject;
using MemcardRex.Core;

namespace MemcardRex.Linux
{
    public class PluginsDialog
    {
        // ASCII 31 (Unit Separator)
        private static readonly string Sep = ((char)31).ToString();

        //private readonly Builder _builder;
        private readonly Dialog _dialog;
        private readonly ColumnView _listView;

        private rexPluginSystem pluginSystem;
        private List<pluginMetadata> loadedMetadata = new List<pluginMetadata>();

        //Currently selected plugin
        int pluginIndex = 0;

        public PluginsDialog(Window parent, ref rexPluginSystem plgSys)
        {
            pluginSystem = plgSys;
            loadedMetadata = plgSys.assembliesMetadata;

            var _builder = new Builder("MemcardRex.Linux.GUI.PluginsDialog.ui")!;

            _dialog = (Dialog)_builder.GetObject("PluginsDialog")!;
            _dialog.SetTransientFor(parent);

            _listView = (ColumnView)_builder.GetObject("PluginsListView")!;
            var btnOk = (Button)_builder.GetObject("BtnOk")!;
            var btnConfig = (Button)_builder.GetObject("BtnConfig")!;
            var btnAbout = (Button)_builder.GetObject("BtnAbout")!;

            btnOk.OnClicked += (s, e) => _dialog.Destroy();
            
            btnConfig.OnClicked += (s, e) => {
                if (loadedMetadata.Count > 0)
                    pluginSystem.setWindowParent(pluginIndex, _dialog.Handle);
                    pluginSystem.showConfigDialog(pluginIndex);
            };

            btnAbout.OnClicked += (s, e) => {
                if (loadedMetadata.Count > 0){
                    pluginSystem.setWindowParent(pluginIndex, _dialog.Handle);
                    pluginSystem.showAboutDialog(pluginIndex);
                }
            };

            var store = Gio.ListStore.New(Gtk.StringObject.GetGType());

            //Check if there are any loaded assemblies
            if (loadedMetadata.Count > 0)
            {
                //Populate list with plugins
                for (int i = 0; i < loadedMetadata.Count; i++)
                {
                    store.Append(Gtk.StringObject.New($"{loadedMetadata[i].pluginName}{Sep}{loadedMetadata[i].pluginAuthor}{Sep}{loadedMetadata[i].pluginSupportedGames}"));
                }
            }

            SetupTable();

            _listView.SetModel(Gtk.SingleSelection.New(store));
        }

        //Draw columns and populate list
        private void SetupTable()
        {
            var nameFactory = Gtk.SignalListItemFactory.New();
            nameFactory.OnSetup += (sender, args) => {
                var listItem = (Gtk.ListItem)args.Object;
                listItem.SetChild(new Gtk.Label { Halign = Gtk.Align.Start, MarginStart = 2 });
            };
            nameFactory.OnBind += (sender, args) => {
                var stringObj = (Gtk.StringObject)((Gtk.ListItem)args.Object).GetItem()!;
                var label = (Gtk.Label)((Gtk.ListItem)args.Object).GetChild()!;
                var dijelovi = stringObj?.String?.Split(Sep?[0] ?? ' ') ?? Array.Empty<string>();
                if (label != null && dijelovi != null) label.SetText(dijelovi[0]);
            };
            var nameColumn = Gtk.ColumnViewColumn.New("Plugin name", nameFactory);
            nameColumn.FixedWidth = 150;
            _listView.AppendColumn(nameColumn);

            var versionFactory = Gtk.SignalListItemFactory.New();
            versionFactory.OnSetup += (sender, args) => {
                var listItem = (Gtk.ListItem)args.Object;
                listItem.SetChild(new Gtk.Label { Halign = Gtk.Align.Start, MarginStart = 2 });
            };
            versionFactory.OnBind += (sender, args) => {
                var stringObj = (Gtk.StringObject)((Gtk.ListItem)args.Object).GetItem()!;
                var label = (Gtk.Label)((Gtk.ListItem)args.Object).GetChild()!;
                
                var dijelovi = stringObj?.String?.Split(Sep?[0] ?? ' ') ?? Array.Empty<string>();
                if (label != null && dijelovi != null) label.SetText(dijelovi.Length > 1 ? dijelovi[1] : "");
            };
            var versionColumn = Gtk.ColumnViewColumn.New("Author", versionFactory);
            versionColumn.FixedWidth = 100;
            _listView.AppendColumn(versionColumn);

            var descFactory = Gtk.SignalListItemFactory.New();
            descFactory.OnSetup += (sender, args) => {
                var listItem = (Gtk.ListItem)args.Object;
                var label = new Gtk.Label { Halign = Gtk.Align.Start, MarginStart = 2 };
                label.Ellipsize = Pango.EllipsizeMode.End;
                listItem.SetChild(label);
            };
            descFactory.OnBind += (sender, args) => {
                var stringObj = (Gtk.StringObject)((Gtk.ListItem)args.Object).GetItem()!;
                var label = (Gtk.Label)((Gtk.ListItem)args.Object).GetChild()!;
                
                var dijelovi = stringObj?.String?.Split(Sep?[0] ?? ' ') ?? Array.Empty<string>();
                if (label != null && dijelovi != null) label.SetText(dijelovi.Length > 2 ? dijelovi[2] : "");
            };
            var descColumn = Gtk.ColumnViewColumn.New("Supported game(s)", descFactory);
            descColumn.Expand = true;
            _listView.AppendColumn(descColumn);
        }

        public void Show()
        {
            _dialog.Show();
        }
    }
}