using BarcodeReader.ImageSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;

namespace BarcodeReader.ImageSharp.Examples {
    internal class Program {
        private static string FolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "codes");
        
        static void Main(string[] args) {
            Console.WriteLine("Reading BarCode");           
            var filePath = Path.Combine(FolderPath, "barcode.png");
            var barcodeImage = Image.Load<Rgba32>(filePath);
            var barcodeReader = new BarcodeReader<Rgba32>();
            var response = barcodeReader.Decode(barcodeImage);
            Console.WriteLine("Value on the barcode: " + response.Message);
        }
    }
}