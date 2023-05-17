<h1 align="center">BarcodeReader.ImageSharp</h1>
<div align="center">
  
[![Nuget](https://img.shields.io/nuget/v/BarcodeReader.ImageSharp?style=flat-square)](https://www.nuget.org/packages/BarcodeReader.ImageSharp/)
[![Downloads](https://img.shields.io/nuget/dt/BarcodeReader.ImageSharp.svg?style=flat-square)](https://www.nuget.org/packages/BarcodeReader.ImageSharp/)
![Build Status](https://img.shields.io/github/actions/workflow/status/baltermia/barcodereader-imagesharp/dotnet.yml?style=flat-square)
  
A barcode reader compatible with SixLabors.ImageSharp using ZXing. Trying to get off System.Drawing.Common.
</div>

I created this library specifically to use with blazor but you can totally use this for other use cases!

## Features

This is a basic facade-library for the [ZXing.Bindings.ImageSharp.V2](https://github.com/micjahn/ZXing.Net) library which uses [SixLabors.ImageSharp](https://github.com/SixLabors/ImageSharp).

Included Features are:
- Barcode reading functionality that supports all ImageSharp image formats
- Decode Barcodes synchronously and asynchronously
- Get rich results from the decoding
- Fired events for better support in decoding loops

## .NET Support
The library supports a vast majority of up-to-date .NET versions:
- .NET Standard 2.0+
- .NET Framework 4.7.2+
- .NET Core 3.1+
- .NET 5+

## Installation

There are two ways to add the [BarcodeReader.ImageSharp](https://www.nuget.org/packages/BarcodeReader.ImageSharp/) library to your projects:

1. Open the command line and go into the directoy where your .csproj file is located, then execute this command:
```
dotnet add package BarcodeReader.ImageSharp
```

2. Or add it in the GUI of Visual Studio 20XX:  
`Tools` -> `Nuget Package Manager` -> `Manage Nuget Packages for Solution...`

---

Then add the following using to your C# files:
```csharp
using BarcodeReader.ImageSharp;
```
And depending on your code, you probably need one or more of the following usings:
```csharp
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;
```

## How to Use

In the following code we use one of ImageSharps pixelformats `IPixel<Rgba32>` which we will use to load PNG images from files.
```csharp
string path = @"C:\images\qrcode.png";

// first we need to Load a image
Image<Rgba32> image = Image.Load<Rgba32>(path);

// then we create a new BarcodeReader object
BarcodeReader<Rgba32> reader = new BarcodeReader<Rgba32>(types: ZXing.BarcodeFormat.QR_CODE);

// then we can get a result by decoding
BarcodeResult<Rgba32> result = await reader.DecodeAsync(image);

// then we can print out the result
if (result.Status == Status.Found)
{
    Console.WriteLine(result.Value);
}
else if (result.Status == Status.NotFound)
{
    Console.WriteLine(result.Message);
}
else if (result.Status == Status.Error)
{
    Console.WriteLine("An error occured while decoding barcode");
}
```

If you're using the reader in a while loop, you can use the `DetectedBarcode` event to only recieve `BarcodeResult`s with a `Status.Found`:
```csharp
// add handler to event
reader.DetectedBarcode += Recieved_Handler;

// infite loop for demonstration purposes
while (true)
{
    // recieve image from any method (not defined here)
    Image<Rgba32> img = GetNewImage();
    
    // decode the image. DetectedBarcode events are fired when a barcode could be found
    await reader.DecodeAsync(img);
}

// handler which is called when a barcode could be detected by any Decode call
void Recieved_Handler(object sender, BarcodeEventArgs<Rgba32> e) { /* Do something with the result */ }
```
