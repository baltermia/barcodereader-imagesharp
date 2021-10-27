using System;

namespace BarcodeReader.ImageSharp
{
    /// <summary>
    /// EventArgs for BarcodeDetectedEvents but also other events using Barcodes
    /// </summary>
    public class BarcodeEventArgs : EventArgs
    {
        /// <summary>
        /// Decoded value from the barcode as a string
        /// </summary>
        public BarcodeResult Result { get; }

        /// <summary>
        /// Creates a new instance of the BarcodeEventArgs class 
        /// </summary>
        public BarcodeEventArgs(BarcodeResult result)
        {
            Result = result;
        }
    }
}
