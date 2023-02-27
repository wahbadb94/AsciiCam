open System
open AsciiCam.AsciiPrint
open FlashCap

let descriptors = CaptureDevices().enumerateDescriptors()

let main () =
    async {
        // have user choose their device
        let descriptor, characteristics =
            printfn "Choose a device descriptor number. (Avoid choosing one with 0 characteristics listed)"
            descriptors
            |> Seq.iteri (fun i s -> printfn $"{i}: {s}") // print options
            |> Console.ReadLine // user choice
            |> int
            |> fun i ->
                let index = if i < Seq.length descriptors then i else 0
                let descriptor = Seq.item index descriptors
                let characteristics =
                    descriptor.Characteristics
                    |> Seq.reduce (fun a b -> if a.Height < b.Height then a else b)
                (descriptor, characteristics)
                
        // setup up the ascii printer
        let printer = AsciiPrint (characteristics.Width, characteristics.Height, 2, 3)
                
        // register callback to invoke when a frame is received
        use! device =
            descriptor.openAsync (
                characteristics,
                (fun bufferScope ->
                    async {
                        Console.Clear()
                        bufferScope.Buffer.extractImage() |> printer.asciiPrint |> fun s -> printfn $"{s}"
                    })
            )
        
        // start/stop directions
        printfn "Press any key to start webcam. Press again to stop." |> Console.ReadKey |> ignore
        
        // run the capture device until user quits, then clean up
        do! device.startAsync ()
        Console.ReadKey() |> ignore
        do! device.stopAsync ()
    }

main () |> Async.RunSynchronously
