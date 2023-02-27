module AsciiCam.AsciiPrint

open System.Threading.Tasks
open SkiaSharp
open System

/// methods for turning image byte array into its ascii text representation
type AsciiPrint(width: int, height: int, scaleX: int, scaleY: int) =
    let h, w = (height / scaleY, width / scaleX)
    let density = " .:-=+*#%@".ToCharArray()
    let mutable buffer: char[] = Array.zeroCreate (h * (w + 1))
    
    let calcBrightness (r: byte) (g: byte) (b: byte) : char =
        // perceived brightness formula
        let b = (0.2126 * float r + 0.7152 * float g + 0.0722 * float b) / 255.0
        
        // map brightness to character density
        let i = Math.Floor(b * float density.Length) |> int
        density[i]
    
    member this.asciiPrint (imageBytes: byte[]) =
        let bitmap = SKBitmap.Decode(imageBytes)
        
        for j in 0 .. (h - 1) do
            for i in 0 .. (w - 1) do
                buffer[j * (w + 1) + i] <- bitmap.GetPixel(i * scaleX, j * scaleY)
                                           |> fun p -> calcBrightness p.Red p.Green p.Blue
    
            buffer[j * (w + 1) + w] <- '\n'
        
        new string(buffer)
        
    member this.asciiPrintParallel (threadDim: int) (imageBytes: byte[]): string =
       let bitmap = SKBitmap.Decode(imageBytes)
       
       Parallel.ForEach([0..(threadDim * threadDim - 1)], fun threadId ->
           let threadX = threadId % threadDim
           let threadY = threadId / threadDim
           
           let startY = threadY * (h / threadDim)
           let endY = Math.Min(startY + (h/threadDim) - 1, h-1)
           let startX = threadX * (w / threadDim)
           let endX = Math.Min(startX + (w/threadDim) - 1, w - 1)
           
           for j in startY..endY do
               for i in startX..endX do
                   buffer[j * (w+1) + i] <- bitmap.GetPixel(i * scaleX, j * scaleY)
                                            |> fun p -> calcBrightness p.Red p.Green p.Blue 
               if endX = w-1 then
                  buffer[j * (w + 1) + w] <- '\n' 
           ()) |> ignore
       

       new string(buffer)

