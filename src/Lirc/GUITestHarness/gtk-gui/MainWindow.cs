// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//
//
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------



public partial class MainWindow {

    private Gtk.UIManager UIManager;

    private Gtk.Action LircConfigAction;

    private Gtk.Action DialogAction;

    private Gtk.VBox vbox2;

    private Gtk.MenuBar menubar2;

    protected virtual void Build() {
        Stetic.Gui.Initialize(this);
        // Widget MainWindow
        this.UIManager = new Gtk.UIManager();
        Gtk.ActionGroup w1 = new Gtk.ActionGroup("Default");
        this.LircConfigAction = new Gtk.Action("LircConfigAction", Mono.Unix.Catalog.GetString("Lirc Config"), null, null);
        this.LircConfigAction.ShortLabel = Mono.Unix.Catalog.GetString("Lirc Config");
        w1.Add(this.LircConfigAction, null);
        this.DialogAction = new Gtk.Action("DialogAction", Mono.Unix.Catalog.GetString("Dialog"), null, null);
        this.DialogAction.ShortLabel = Mono.Unix.Catalog.GetString("Dialog");
        w1.Add(this.DialogAction, null);
        this.UIManager.InsertActionGroup(w1, 0);
        this.AddAccelGroup(this.UIManager.AccelGroup);
        this.Name = "MainWindow";
        this.Title = Mono.Unix.Catalog.GetString("MainWindow");
        this.WindowPosition = ((Gtk.WindowPosition)(4));
        // Container child MainWindow.Gtk.Container+ContainerChild
        this.vbox2 = new Gtk.VBox();
        this.vbox2.Name = "vbox2";
        this.vbox2.Spacing = 6;
        // Container child vbox2.Gtk.Box+BoxChild
        this.UIManager.AddUiFromString("<ui><menubar name='menubar2'><menu name='LircConfigAction' action='LircConfigAction'><menuitem name='DialogAction' action='DialogAction'/></menu></menubar></ui>");
        this.menubar2 = ((Gtk.MenuBar)(this.UIManager.GetWidget("/menubar2")));
        this.menubar2.Name = "menubar2";
        this.vbox2.Add(this.menubar2);
        Gtk.Box.BoxChild w2 = ((Gtk.Box.BoxChild)(this.vbox2[this.menubar2]));
        w2.Position = 0;
        w2.Expand = false;
        w2.Fill = false;
        this.Add(this.vbox2);
        if ((this.Child != null)) {
            this.Child.ShowAll();
        }
        this.DefaultWidth = 400;
        this.DefaultHeight = 300;
        this.Show();
        this.DeleteEvent += new Gtk.DeleteEventHandler(this.OnDeleteEvent);
        this.DialogAction.Activated += new System.EventHandler(this.OnDialog);
    }
}
