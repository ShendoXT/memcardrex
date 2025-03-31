using System;
using System.Drawing;

namespace MemcardRex.Core
{
    public class BmpBuilder
    {
        public BmpBuilder()
        {
        }

        byte ReverseBits(byte b)
        {
            byte reversed = 0;
            for (int i = 0; i < 8; i++)
            {
                // Shift the reversed byte to the left
                reversed <<= 1;
                // Get the least significant bit of the original byte and add it to the reversed byte
                reversed |= (byte)(b & 1);
                // Shift the original byte to the right
                b >>= 1;
            }
            return reversed;
        }

        /// <summary>
        /// Create a 32x32 monochrome BMP image
        /// </summary>
        /// <param name="RawImageData"></param>
        /// <returns></returns>
        public byte[] BuildBmp(byte[] RawImageData)
        {
            byte[] monoBmpHeader = {
                0x42, 0x4D, 0xBE, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3E, 0x00,
                0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00,
                0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF,
                0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };

            byte[] monoBmpImage = new byte[190];

            //Copy BMP header to bmp data array
            Array.Copy(monoBmpHeader, monoBmpImage, monoBmpHeader.Length);

            for (int i = 0; i < 0x80; i += 4)
            {
                monoBmpImage[monoBmpImage.Length - 4 - i] = (byte)~ReverseBits(RawImageData[0 + i]);
                monoBmpImage[monoBmpImage.Length - 3 - i] = (byte)~ReverseBits(RawImageData[1 + i]);
                monoBmpImage[monoBmpImage.Length - 2 - i] = (byte)~ReverseBits(RawImageData[2 + i]);
                monoBmpImage[monoBmpImage.Length - 1 - i] = (byte)~ReverseBits(RawImageData[3 + i]);
            }

            return monoBmpImage;
        }

        /// <summary>
        /// Create 16x16 ARGB BMP image
        /// </summary>
        /// <param name="RawImageData"></param>
        /// <returns></returns>
		public byte[] BuildBmp(Color[] RawImageData)
        {
            byte[] argbBmpHeader = new byte[] // All values are little-endian
            {
                0x42, 0x4D,             // Signature 'BM'
                0x8a, 0x40, 0x00, 0x00, // Size: 1162 bytes
                0x00, 0x00,             // Unused
                0x00, 0x00,             // Unused
                0x8a, 0x00, 0x00, 0x00, // Offset to image data

                0x7c, 0x00, 0x00, 0x00, // DIB header size (124 bytes)
                0x10, 0x00, 0x00, 0x00, // Width (16px)
                0x10, 0x00, 0x00, 0x00, // Height (16px)
                0x01, 0x00,             // Planes (1)
                0x20, 0x00,             // Bits per pixel (32)
                0x03, 0x00, 0x00, 0x00, // Format (bitfield = use bitfields | no compression)
                0x00, 0x04, 0x00, 0x00, // Image raw size (1024 bytes)
                0x13, 0x0B, 0x00, 0x00, // Horizontal print resolution (2835 = 72dpi * 39.3701)
                0x13, 0x0B, 0x00, 0x00, // Vertical print resolution (2835 = 72dpi * 39.3701)
                0x00, 0x00, 0x00, 0x00, // Colors in palette (none)
                0x00, 0x00, 0x00, 0x00, // Important colors (0 = all)
                0x00, 0x00, 0xFF, 0x00, // R bitmask (00FF0000)
                0x00, 0xFF, 0x00, 0x00, // G bitmask (0000FF00)
                0xFF, 0x00, 0x00, 0x00, // B bitmask (000000FF)
                0x00, 0x00, 0x00, 0xFF, // A bitmask (FF000000)
                0x42, 0x47, 0x52, 0x73, // sRGB color space

                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, // Unused R, G, B entries for color space

                0x00, 0x00, 0x00, 0x00, // Unused Gamma X entry for color space
                0x00, 0x00, 0x00, 0x00, // Unused Gamma Y entry for color space
                0x00, 0x00, 0x00, 0x00, // Unused Gamma Z entry for color space

                0x00, 0x00, 0x00, 0x00, // Unknown
                0x00, 0x00, 0x00, 0x00, // Unknown
                0x00, 0x00, 0x00, 0x00, // Unknown
                0x00, 0x00, 0x00, 0x00, // Unknown
            };

            byte[] bmpImage = new byte[1162];

            //Copy BMP header to bmp data array
            Array.Copy(argbBmpHeader, bmpImage, argbBmpHeader.Length);

            Color[] FlippedRawImageData = new Color[RawImageData.Length];

            //Y flip color data for compatibility with BMP
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    // Convert 2D (y, x) to 1D index
                    int indexTop = y * 16 + x;           // Current position
                    int indexBottom = (15 - y) * 16 + x; // Mirrored position

                    // Swap the colors
                    FlippedRawImageData[indexTop] = RawImageData[indexBottom];
                    FlippedRawImageData[indexBottom] = RawImageData[indexTop];
                }
            }

            int index = 255;
            for (int i = 0; i < 256 * 4; i += 4)
            {
                bmpImage[bmpImage.Length - i - 1] = FlippedRawImageData[index].A;
                bmpImage[bmpImage.Length - i - 2] = FlippedRawImageData[index].R;
                bmpImage[bmpImage.Length - i - 3] = FlippedRawImageData[index].G;
                bmpImage[bmpImage.Length - i - 4] = FlippedRawImageData[index].B;
                index--;
            }

            return bmpImage;

        }
    }
}
