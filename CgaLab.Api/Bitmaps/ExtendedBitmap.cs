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

        public ExtendedBitmap(int width, int height)
        {
            Source = new(width, height, PixelFormat.Format32bppArgb);
            Width = Source.Width;
            Height = Source.Height;
            BytesPerPixel = Source.PixelFormat == PixelFormat.Format32bppArgb ? 4 : 0;
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
