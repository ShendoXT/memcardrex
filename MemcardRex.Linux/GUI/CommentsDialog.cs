using System;
using Gtk;

namespace MemcardRex.Linux;

public class CommentsDialog
{
    private readonly Dialog _window;
    private readonly Button _btnOk;
    private readonly Button _btnCancel;
    private readonly TextView _txtComments;

    private bool okClicked = false;

    public CommentsDialog(Window parent)
    {
        var builder = new Builder("MemcardRex.Linux.GUI.CommentsDialog.ui");

        _window = (Dialog)builder.GetObject("CommentsDialog")!;
        _btnOk = (Button)builder.GetObject("btnOk")!;
        _btnCancel = (Button)builder.GetObject("btnCancel")!;
        _txtComments = (TextView)builder.GetObject("txtComments")!;

        _window.SetTransientFor(parent);

        _btnOk.OnClicked += (s, e) => {
            okClicked = true;
            _window.Close();
        };

        _btnCancel.OnClicked += (s, e) => 
        {
            okClicked = false;
            _window.Close();
        };
    }

    public void SetComments(string title, string comments){
        _window.SetTitle(title);
        if(comments != null) _txtComments.Buffer!.Text = comments;
    }

    public string GetComments(){
        return _txtComments.Buffer!.Text!;
    }

    public bool Run()
    {
        okClicked = false;
        _window.Present();
        
        // Wait for dialog to close
        var loop = GLib.MainLoop.New(null, false);
        _window.OnCloseRequest += (sender, args) =>
        {
            loop.Quit();
            return false;
        };
        loop.Run();
        
        return okClicked;
    }

    public void Present()
    {
        _window.Present();
    }
}