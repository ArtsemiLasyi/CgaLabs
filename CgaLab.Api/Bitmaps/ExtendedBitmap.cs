using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;

namespace CgaLab.Api.Bitmaps
{
    public unsafe class ExtendedBitmap : IEnumerable
    {
        private BitmapData bitmapData { get; set; }
        private byte* scan0 { get; set; }
        private int BitmapDataStride { get; set; }
        private int BytesPerPixel { get; set; }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public Bitmap Source { get; private set; }

        private static int GetBytesPerPixel(PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.Format64bppArgb or PixelFormat.Format64bppPArgb:
                    return 64 / 8;

                case PixelFormat.Format48bppRgb:
                    return 48 / 8;

                case PixelFormat.Format32bppArgb or PixelFormat.Format32bppPArgb or PixelFormat.Format32bppRgb:
                    return 32 / 8;

                case PixelFormat.Format24bppRgb:
                    return 24 / 8;

                case PixelFormat.Format16bppArgb1555 or PixelFormat.Format16bppGrayScale or PixelFormat.Format16bppRgb555 or PixelFormat.Format16bppRgb565:
                    return 16 / 8;

                case PixelFormat.Format8bppIndexed:
                    return 8 / 8;

                default:
                    return 0;
            }
        }

        public ExtendedBitmap(int width, int height)
        {
            Source = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Width = Source.Width;
            Height = Source.Height;
            BytesPerPixel = GetBytesPerPixel(Source.PixelFormat);
        }

        public ExtendedBitmap(string filename)
        {
            Source = new(filename, true);
            Width = Source.Width;
            Height = Source.Height;
            BytesPerPixel = GetBytesPerPixel(Source.PixelFormat);
        }

        public void LockBits()
        {
            bitmapData = Source.LockBits(
                new Rectangle(0, 0, Width, Height),
                ImageLockMode.ReadWrite,
                Source.PixelFormat
            );
            SetScanAndStride();
        }

        public void UnlockBits()
        {
            Source.UnlockBits(bitmapData);
        }

        private unsafe byte* GetAddress(int x, int y)
        {
            return scan0 + y * BitmapDataStride + x * BytesPerPixel;
        }

        public unsafe Color this[int x, int y]
        {
            get
            {
                byte* address = GetAddress(x, y);
                return Color.FromArgb(
                    address[3],
                    address[0],
                    address[1],
                    address[2]
                );
            }
            set
            {
                if (x < Width && y < Height && x >= 0 && y >= 0)
                {
                    byte* address = GetAddress(x, y);
                    address[0] = value.R;
                    address[1] = value.G;
                    address[2] = value.B;
                    address[3] = value.A;
                }
            }
        }

        public unsafe Vector3 GetRGBVector(int x, int y)
        {
            byte* address = GetAddress(x, y);
            return new Vector3(address[0], address[1], address[2]);
        }

        private void SetScanAndStride()
        {
            scan0 = (byte*)bitmapData.Scan0;
            BitmapDataStride = bitmapData.Stride;
        }

        public IEnumerator<Color> GetEnumerator()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    yield return this[x, y];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
