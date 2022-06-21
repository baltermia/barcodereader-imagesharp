using System;
using SixLabors;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using ZXing.Common;
using ZXing;
using System.Threading.Tasks;

namespace BarcodeReader.ImageSharp
{
    public class BarcodeReader
    {
        /// <summary>
        /// Represents the method that will handle the event raised when a barcode is detected
        /// </summary>
        /// <param name="sender">The object which raises the event</param>
        /// <param name="e">Arguments such as the barcode value and bitmap are given through the EventArgs</param>
        public delegate void BarcodeDetectedEventHandler(object sender, BarcodeEventArgs e);

        /// <summary>
        /// Event which is raised when a barcode is detected. It is only raised with the Status Found (0).
        /// </summary>
        public event BarcodeDetectedEventHandler DetectedBarcode;

        /// <summary>
        /// ZXing BarcodeReader object which is being used for decoding possible barcodes in a bitmap
        /// </summary>
        private readonly ZXing.ImageSharp.BarcodeReader<Rgba32> reader;

        /// <summary>
        /// Creates a new instance of the BarcodeReader class. The Constructor will set any settings for decoding barcodes with the Decode() method
        /// </summary>
        /// <param name="type">What type of barcode the provided bitmap should be searched for. By defualt all one-dimensional barcodes will be detected</param>
        /// <param name="performanceMode">The decoder will take less resources to search for the barcodes. By default the mode is set to performance</param>
        public BarcodeReader(bool performanceMode = true, params BarcodeFormat[] types)
        {
            reader = new ZXing.ImageSharp.BarcodeReader<Rgba32>()
            {
                AutoRotate = !performanceMode,
                Options = new DecodingOptions()
                {
                    TryHarder = !performanceMode,
                    PossibleFormats = types.Length >= 1 ? types : new BarcodeFormat[] { BarcodeFormat.All_1D }
                }
            };
        }

        /// <summary>
        /// Creates a new instance of the BarcodeReader class. The Constructor will set any settings for decoding barcodes with the Decode() method
        /// </summary>
        /// <param name="tryHarder">The decoder will try harder decoding a barcode from the bitmap, thus also using more resources.</param>
        /// <param name="autoRotate">The decoder will rotate the bitmap to look for any barcodes. This will take more resources if enabled</param>
        /// <param name="type">What type of barcode the provided bitmap should be searched for. By defualt all one-dimensional barcodes will be detected</param>
        public BarcodeReader(bool tryHarder, bool autoRotate, params BarcodeFormat[] types)
        {
            reader = new ZXing.ImageSharp.BarcodeReader<Rgba32>()
            {
                AutoRotate = autoRotate,
                Options = new DecodingOptions()
                {
                    TryHarder = tryHarder,
                    PossibleFormats = types.Length >= 1 ? types : new BarcodeFormat[] { BarcodeFormat.All_1D }
                }
            };
        }

        /// <summary>
        /// Decodes a barcode from the provided ImageSharp.Image
        /// </summary>
        /// <returns>Result holding the value if a barcode is found or the error message</returns>
        public BarcodeResult Decode(Image<Rgba32> image)
        {
            try
            {
                using (image)
                {
                    Result result = reader.Decode(image);

                    if (result?.Text != null)
                    {
                        BarcodeResult barcodeResult = new BarcodeResult(result.Text, Status.Found, "Barcode found in image", image);

                        // '?.Invoke' is needed as oherwise an exception would be thrown if ther are no subscribers to the DetectedBarcode event
                        DetectedBarcode?.Invoke(this, new BarcodeEventArgs(barcodeResult));

                        return barcodeResult;
                    }
                    else
                    {
                        return new BarcodeResult(null, Status.NotFound, "The provided image did not contain a readable barcode", image);
                    }
                }
            }
            catch (Exception ex)
            {
                image?.Dispose();

                return new BarcodeResult(null, Status.Error, ex.Message, null);
            }
        }

        /// <summary>
        /// Decodes a barcode asynchronously from the provided ImageSharp.Image. Make sure to await this method
        /// </summary>
        /// <returns>Result holding the value if a barcode is found or the error message</returns>
        public Task<BarcodeResult> DecodeAsync(Image<Rgba32> image) => Task.Run(() => Decode(image));

        /// <summary>
        /// Decodes a barcode from the provided ImageSharp.Image
        /// </summary>
        /// <param name="data">Array of bytes representing the image</param>
        /// <returns>Result holding the value if a barcode is found or the error message</returns>
        public BarcodeResult Decode(byte[] data) => Decode(Image.Load(data));

        /// <summary>
        /// Decodes a barcode from the provided ImageSharp.Image
        /// </summary>
        /// <param name="data">Array of bytes representing the image</param>
        /// <returns>Result holding the value if a barcode is found or the error message</returns>
        public Task<BarcodeResult> DecodeAsync(byte[] data) => DecodeAsync(Image.Load(data));

        /// <summary>
        /// Decodes a barcode from the provided ImageSharp.Image
        /// </summary>
        /// <param name="data">Base64 string representing the image</param>
        /// <returns>Result holding the value if a barcode is found or the error message</returns>
        public BarcodeResult Decode(string data) => Decode(Convert.FromBase64String(RemoveBase64Prefix(data)));

        /// <summary>
        /// Decodes a barcode asynchronously from the provided ImageSharp.Image. Make sure to await this method
        /// </summary>
        /// <param name="data">Base64 string representing the image</param>
        /// <returns>Result holding the value if a barcode is found or the error message</returns>
        public Task<BarcodeResult> DecodeAsync(string data) => DecodeAsync(Image.Load(Convert.FromBase64String(RemoveBase64Prefix(data))));

        /// <summary>
        /// Cuts off the prefix of a base64 image string if it has one
        /// </summary>
        private static string RemoveBase64Prefix(string img) => img.Substring(img.IndexOf(",") + 1);
    }
}