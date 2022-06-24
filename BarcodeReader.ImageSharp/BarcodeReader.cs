using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Threading.Tasks;
using ZXing;
using ZXing.Common;

namespace BarcodeReader.ImageSharp
{
    public class BarcodeReader<T> where T : unmanaged, IPixel<T>
    {
        /// <summary>
        /// Represents the method that will handle the event raised when a barcode is detected
        /// </summary>
        /// <param name="sender">The object which raises the event</param>
        /// <param name="e">Arguments such as the barcode value and bitmap are given through the EventArgs</param>
        public delegate void BarcodeDetectedEventHandler<U>(object sender, BarcodeEventArgs<U> e) where U : unmanaged, IPixel<U>;

        /// <summary>
        /// Event which is raised when a barcode is detected. It is only raised with the Status Found (0).
        /// </summary>
        public event BarcodeDetectedEventHandler<T> DetectedBarcode;

        /// <summary>
        /// ZXing BarcodeReader object which is being used for decoding possible barcodes in a bitmap
        /// </summary>
        private readonly ZXing.ImageSharp.BarcodeReader<T> reader;

        /// <summary>
        /// Creates a new instance of the BarcodeReader class. The Constructor will set any settings for decoding barcodes with the Decode() method
        /// </summary>
        /// <param name="type">What type of barcode the provided bitmap should be searched for. By defualt all one-dimensional barcodes will be detected</param>
        /// <param name="performanceMode">The decoder will take less resources to search for the barcodes. By default the mode is set to performance</param>
        public BarcodeReader(bool performanceMode = true, params BarcodeFormat[] types)
        {
            reader = new ZXing.ImageSharp.BarcodeReader<T>()
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
            reader = new ZXing.ImageSharp.BarcodeReader<T>()
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
        public BarcodeResult<T> Decode(Image<T> image)
        {
            try
            {
                using (image)
                {
                    Result result = reader.Decode(image);

                    if (result?.Text != null)
                    {
                        BarcodeResult<T> barcodeResult = new BarcodeResult<T>
                        {
                            Message = result.Text,
                            Status = Status.Found,
                            Value = result.Text,
                            Image = image
                        }; 

                        // '?.Invoke' is needed as oherwise an exception would be thrown if ther are no subscribers to the DetectedBarcode event
                        DetectedBarcode?.Invoke(this, new BarcodeEventArgs<T> { Result = barcodeResult});

                        return barcodeResult;
                    }
                    else
                    {
                        return new BarcodeResult<T>
                        {
                            Message = "The provided image did not contain a readable barcode",
                            Status = Status.NotFound,
                            Image = image
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                image?.Dispose();

                return new BarcodeResult<T>
                {
                    Message = ex.Message,
                    Status = Status.Error
                };
            }
        }

        /// <summary>
        /// Decodes a barcode asynchronously from the provided ImageSharp.Image. Make sure to await this method
        /// </summary>
        /// <returns>Result holding the value if a barcode is found or the error message</returns>
        public Task<BarcodeResult<T>> DecodeAsync(Image<T> image) => Task.Run(() => Decode(image));

        /// <summary>
        /// Decodes a barcode from the provided ImageSharp.Image
        /// </summary>
        /// <param name="data">Array of bytes representing the image</param>
        /// <returns>Result holding the value if a barcode is found or the error message</returns>
        public BarcodeResult<T> Decode(byte[] data) => Decode(Image.Load<T>(data));

        /// <summary>
        /// Decodes a barcode from the provided ImageSharp.Image
        /// </summary>
        /// <param name="data">Array of bytes representing the image</param>
        /// <returns>Result holding the value if a barcode is found or the error message</returns>
        public Task<BarcodeResult<T>> DecodeAsync(byte[] data) => DecodeAsync(Image.Load<T>(data));

        /// <summary>
        /// Decodes a barcode from the provided ImageSharp.Image
        /// </summary>
        /// <param name="data">Base64 string representing the image</param>
        /// <returns>Result holding the value if a barcode is found or the error message</returns>
        public BarcodeResult<T> Decode(string data) => Decode(Convert.FromBase64String(RemoveBase64Prefix(data)));

        /// <summary>
        /// Decodes a barcode asynchronously from the provided ImageSharp.Image. Make sure to await this method
        /// </summary>
        /// <param name="data">Base64 string representing the image</param>
        /// <returns>Result holding the value if a barcode is found or the error message</returns>
        public Task<BarcodeResult<T>> DecodeAsync(string data) => DecodeAsync(Image.Load<T>(Convert.FromBase64String(RemoveBase64Prefix(data))));

        /// <summary>
        /// Cuts off the prefix of a base64 image string if it has one
        /// </summary>
        private static string RemoveBase64Prefix(string img) => img.Substring(img.IndexOf(",") + 1);
    }
}