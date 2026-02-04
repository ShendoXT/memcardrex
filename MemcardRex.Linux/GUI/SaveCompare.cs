using System;
using Gtk;
using GObject;

namespace MemcardRex.Linux
{
    public class SaveCompare
    {
        private Dialog _dialog;
        private ColumnView _columnView;
        private string[][] _data;

        public SaveCompare(Window parent, string save1Name, string save2Name, string[][] data)
        {
            _data = data;
            var builder = new Builder("MemcardRex.Linux.GUI.SaveCompare.ui");
            
            _dialog = (Dialog)builder.GetObject("SaveCompareDialog")!;
            _columnView = (ColumnView)builder.GetObject("DiffColumnView")!;
            var btnOk = (Button)builder.GetObject("BtnOk")!;

            if (builder.GetObject("LabelSave1") is Label label1)
                label1.SetText($"Save 1: {save1Name}");

            if (builder.GetObject("LabelSave2") is Label label2)
                label2.SetText($"Save 2: {save2Name}");

            _dialog.SetTransientFor(parent);

            btnOk.OnClicked += (s, e) => _dialog.Destroy();

            SetupColumns();
        }

        private void SetupColumns()
        {
            var stringList = StringList.New(Array.ConvertAll(_data, x => string.Join("|", x)));
            var selectionModel = SingleSelection.New(stringList);
            _columnView.SetModel(selectionModel);

            string[] titles = { "Offset (hex, int)", "Save 1 (hex, int)", "Save 2 (hex, int)" };
            for (int i = 0; i < titles.Length; i++)
            {
                int colIndex = i;
                var factory = SignalListItemFactory.New();
                
                factory.OnSetup += (s, e) => {
                    var item = (ListItem)e.Object;
                    item.SetChild(new Label { Xalign = 0, MarginStart = 8 });
                };

                factory.OnBind += (s, e) => {
                    var item = (ListItem)e.Object;
                    
                    if (item.GetItem() is StringObject stringObj && item.GetChild() is Label label)
                    {
                        var rowText = stringObj.String;
                        if (!string.IsNullOrEmpty(rowText))
                        {
                            var parts = rowText.Split('|');
                            if (colIndex < parts.Length)
                            {
                                label.SetText(parts[colIndex]);
                            }
                        }
                    }
                };

                var column = ColumnViewColumn.New(titles[i], factory);
                column.SetExpand(true); 
                _columnView.AppendColumn(column);
            }
        }

        public void Show() => _dialog.Show();
    }
}