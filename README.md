# AsciiCam
Prints webcam frames as ascii text to terminal window. Written in F# and targets .NET 7

## NOTE: This has only been tested on my machine (Linux - PopOS 22.04 LTS)

## Depends On:
- FlashCap for accessing webcam
- FSharp.FlashCap for the F# APIs for the above package
- SkiaSharp for converting raw byte array to Bitmap image
- SkiaSharp.NativeAssets.Linux so the above package works on Linux
