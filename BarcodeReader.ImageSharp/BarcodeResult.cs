using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace BarcodeReader.ImageSharp
{
    /// <summary>
    /// Class representing the results after (trying) decoding a barcode from a image
    /// </summary>
    public class BarcodeResult<T> where T : unmanaged, IPixel<T>
    {
        /// <summary>
        /// String value of the barcode if one could be decoded
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Status of the operation
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// The message. Only really usefull if the Status is Error (2)
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The image
        /// </summary>
        public Image<T> Image { get; set; }
    }

    /// <summary>
    /// StatusCodes for BarcodeResult
    /// </summary>
    public enum Status
    {
        Found = 0,
        NotFound = 1,
        Error = 2
    }
}
