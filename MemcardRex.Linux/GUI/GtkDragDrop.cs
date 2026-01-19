using System.Runtime.InteropServices;
using Gtk;
using Gdk;

namespace MemcardRex.Linux;

public static class GtkDragDrop
{
    [DllImport("libgtk-4.so.1")]
    private static extern IntPtr gdk_file_list_get_files(IntPtr file_list);

    [DllImport("libgio-2.0.so.0")]
    private static extern IntPtr g_file_get_path(IntPtr file);

    [DllImport("libglib-2.0.so.0")]
    private static extern IntPtr g_slist_nth_data(IntPtr list, uint n);

    [DllImport("libglib-2.0.so.0")]
    private static extern uint g_slist_length(IntPtr list);

    public static void AddFileDropTarget(Widget widget, Action<string[]> onFilesDropped)
    {
        var dropTarget = DropTarget.New(Gdk.FileList.GetGType(), DragAction.Copy);

        dropTarget.OnDrop += (sender, args) => 
        {
            try 
            {
                var paths = GetDroppedFilePaths(args.Value);
                if (paths.Length > 0)
                {
                    onFilesDropped(paths);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Drop error: {ex.Message}");
            }
            
            return false;
        };

        widget.AddController(dropTarget);
    }

private static string[] GetDroppedFilePaths(GObject.Value value)
{
    var boxedPtr = value.GetBoxed();
    if (boxedPtr == IntPtr.Zero)
        return Array.Empty<string>();

    var filesPtr = gdk_file_list_get_files(boxedPtr);
    var length = g_slist_length(filesPtr);
    var paths = new string[length];
    
    for (uint i = 0; i < length; i++)
    {
        var filePtr = g_slist_nth_data(filesPtr, i);
        var pathPtr = g_file_get_path(filePtr);
        paths[i] = Marshal.PtrToStringUTF8(pathPtr) ?? string.Empty;
    }
    
    return paths;
}
}