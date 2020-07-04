using System;
using System.Device.Gpio;
using System.Threading;

namespace led_blink
{
    class Program
    {
        static void Main(string[] args)
        {
            // var pin = 17;
            var ledPin = 6;
            var buttonPin = 17;
            var buttonSleepTime = 500;
            var lightTimeInMilliseconds = 1000;
            var dimTimeInMilliseconds = 200;
            
            // Console.WriteLine($"Let's blink an LED!");
            Console.WriteLine($"Let's press a Button!");
            using (GpioController controller = new GpioController())
            {
                controller.OpenPin(ledPin, PinMode.Output);
                controller.OpenPin(buttonPin, PinMode.Input);
                Console.WriteLine($"GPIO pin enabled for use: {buttonPin}");
                Console.WriteLine($"GPIO pin enabled for use: {ledPin}");

                Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs eventArgs) =>
                {
                    controller.Dispose();
                };

                while (true)
                {
                   

                    var buttonState = controller.Read(buttonPin);
                    Console.WriteLine("Button State" + buttonState);
                    Thread.Sleep(5000);
                    while(controller.Read(buttonPin) == PinValue.Low)
                    {
                        Console.WriteLine("Button Pressed!!");
                        Console.WriteLine($"Light for {lightTimeInMilliseconds}ms");
                        controller.Write(ledPin, PinValue.High);
                        Thread.Sleep(lightTimeInMilliseconds);
                        Console.WriteLine($"Dim for {dimTimeInMilliseconds}ms");
                        controller.Write(ledPin, PinValue.Low);
                        Thread.Sleep(dimTimeInMilliseconds);
                        Thread.Sleep(buttonSleepTime);
                    }
                }
            }
        }
    }
}
