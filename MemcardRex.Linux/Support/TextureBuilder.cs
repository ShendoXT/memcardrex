using Gdk;
using GdkPixbuf;
using Gio;
using GLib;

namespace MemcardRex.Linux;

public static class TextureBuilder
{
    public static Texture? BmpToTexture(byte[] bmpData)
    {
        try
        {
            var inputStream = MemoryInputStream.NewFromBytes(Bytes.New(bmpData));
            var pixbuf = Pixbuf.NewFromStream(inputStream, null);
            
            if (pixbuf == null)
                return null;
                
            var texture = Texture.NewForPixbuf(pixbuf);
            return texture;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error converting BMP to texture: {ex.Message}");
            return null;
        }
    }
}