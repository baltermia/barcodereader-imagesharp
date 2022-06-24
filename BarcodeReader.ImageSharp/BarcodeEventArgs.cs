using SixLabors.ImageSharp.PixelFormats;
using System;

namespace BarcodeReader.ImageSharp
{
    /// <summary>
    /// EventArgs for BarcodeDetectedEvents but also other events using Barcodes
    /// </summary>
    public class BarcodeEventArgs<T> : EventArgs where T : unmanaged, IPixel<T>
    {
        /// <summary>
        /// Decoded value from the barcode as a string
        /// </summary>
        public BarcodeResult<T> Result { get; set; }
    }
}
