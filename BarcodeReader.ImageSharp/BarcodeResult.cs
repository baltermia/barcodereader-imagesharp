using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace BarcodeReader.ImageSharp
{
    /// <summary>
    /// Class representing the results after (trying) decoding a barcode from a image
    /// </summary>
    public class BarcodeResult
    {
        /// <summary>
        /// String value of the barcode if one could be decoded
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Status of the operation
        /// </summary>
        public Status Status { get; }

        /// <summary>
        /// The message. Only really usefull if the Status is Error (2)
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// The image
        /// </summary>
        public Image<Rgba32> Image { get; }

        /// <summary>
        /// Creates a new instance of BarcodeResult
        /// </summary>
        public BarcodeResult(string value, Status status, string message, Image<Rgba32> image)
        {
            Value = value;
            Status = status;
            Message = message;
            Image = image;
        }
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
