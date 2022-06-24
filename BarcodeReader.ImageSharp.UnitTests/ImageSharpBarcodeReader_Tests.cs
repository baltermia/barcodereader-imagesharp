using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BarcodeReader.ImageSharp.UnitTests
{
    [TestFixture]
    public class ImageSharpBarcodeReader_Tests
    {
        private const string CodeText = "ImageSharpBarcodeReader";
        private readonly string FolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "codes");

        private BarcodeReader<Rgba32> reader1D;
        private BarcodeReader<Rgba32> readerQR;

        private Image<Rgba32> barcodeImage;
        private Image<Rgba32> qrcodeImage;

        private byte[] barcodeBytes;
        private byte[] qrcodeBytes;

        private string barcodeBase64;
        private string qrcodeBase64;

        [SetUp]
        public void Setup()
        {
            barcodeImage = Image.Load<Rgba32>(Path.Combine(FolderPath, "barcode.png"));
            qrcodeImage = Image.Load<Rgba32>(Path.Combine(FolderPath, "qrcode.png"));

            barcodeBase64 = barcodeImage.ToBase64String(PngFormat.Instance);
            qrcodeBase64 = qrcodeImage.ToBase64String(PngFormat.Instance);

            barcodeBytes = Convert.FromBase64String(barcodeBase64.Split(',')[1]);
            qrcodeBytes = Convert.FromBase64String(qrcodeBase64.Split(',')[1]);

            reader1D = new BarcodeReader<Rgba32>();
            readerQR = new BarcodeReader<Rgba32>(types: ZXing.BarcodeFormat.QR_CODE);
        }

        [Test]
        public void Decode_BarcodeImage() => Test(() => reader1D.Decode(barcodeImage));

        [Test]
        public void DecodeAsync_QRcodeImage() => Test(() => readerQR.DecodeAsync(qrcodeImage).Result);

        [Test]
        public void Decode_QRcodeBytes() => Test(() => readerQR.Decode(qrcodeBytes));

        [Test]
        public void DecodeAsync_BarcodeBytes() => Test(() => reader1D.DecodeAsync(barcodeBytes).Result);

        [Test]
        public void Decode_BarcodeBase64() => Test(() => reader1D.Decode(barcodeBase64));

        [Test]
        public void DecodeAsync_QRcodeBase64() => Test(() => readerQR.DecodeAsync(qrcodeBase64).Result);

        [Test]
        public void DecodeWithEvent()
        {
            try
            {
                string value = null;

                reader1D.DetectedBarcode += (sender, e) => value = e.Result.Value;

                reader1D.Decode(barcodeBase64);

                switch (value)
                {
                    case CodeText:
                        Assert.Pass("Barcode successfuly decoded, event was fired.", value);
                        break;
                    case null:
                        Assert.Fail($"No event was fired.");
                        break;
                    default:
                        Assert.Fail($"Barcode got decoded but the wrong value was returned. Expected: '{CodeText}', Recieved: '{value}'.");
                        break;
                }
            }
            catch (SuccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Assert.Fail($"Case threw an exception. Message: {ex.Message}", ex);
            }
        }

        [Test]
        public async Task DecodeWithEventAsync()
        {
            try
            {
                string value = null;

                reader1D.DetectedBarcode += (sender, e) => value = e.Result.Value;

                await reader1D.DecodeAsync(barcodeBase64);

                switch (value)
                {
                    case CodeText:
                        Assert.Pass("Barcode successfuly decoded, event was fired.", value);
                        break;
                    case null:
                        Assert.Fail($"No event was fired.");
                        break;
                    default:
                        Assert.Fail($"Barcode got decoded but the wrong value was returned. Expected: '{CodeText}', Recieved: '{value}'.");
                        break;
                }
            }
            catch (SuccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Assert.Fail($"Case threw an exception. Message: {ex.Message}", ex);
            }
        }

        private static void Test(Func<BarcodeResult<Rgba32>> func)
        {
            try
            {
                BarcodeResult<Rgba32> result = func.Invoke();

                switch (result.Status)
                {
                    case Status.Found:
                        if (result.Value == CodeText)
                            Assert.Pass("Barcode successfuly decoded.", result.Value);
                        Assert.Fail($"Barcode got decoded but the wrong value was returned. Expected: '{CodeText}', Recieved: '{result.Value}'.");
                        break;
                    case Status.NotFound:
                        Assert.Fail(result.Message);
                        break;
                    case Status.Error:
                        Assert.Fail($"An error occured while decoding. Message: {result.Message}");
                        break;
                }
            }
            catch (SuccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Assert.Fail($"Case threw an exception. Message: {ex.Message}", ex);
            }
        }
    }
}