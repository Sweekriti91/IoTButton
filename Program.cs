using System;
using System.Device.Gpio;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace led_blink
{
    class Program
    {
        //Azure IoT Hub setup
        const string DeviceConnectionString = "<Your Azure IoT Hub Connection String>";
        // Replace with the device id you used when you created the device in Azure IoT Hub
        const string DeviceId = "<Your Device Id>";
        static DeviceClient _deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Mqtt);
        static int _msgId = 0;
        const int messageThreshold = 8;
            
        static async Task Main(string[] args)
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
                        await SendMsgIotHub("Hello from .NET IoT! at " + DateTime.Now.ToShortTimeString() +" on " + DateTime.Now.ToShortDateString()) ;
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
    
        private static async Task SendMsgIotHub(string message)
        {
            var telemetry = new Telemetry() { MessageData=message, MessageId = _msgId++ };
            string json = JsonConvert.SerializeObject(telemetry);
            Console.WriteLine($"Sending {json}");
            Message eventMessage = new Message(Encoding.UTF8.GetBytes(json));
            eventMessage.Properties.Add("Action", "tweet");
            if(_msgId > messageThreshold)
            {
                await _deviceClient.SendEventAsync(eventMessage).ConfigureAwait(false);
                _msgId = 0;
            }
            else
                Console.WriteLine(eventMessage);
        }
        class Telemetry
        {
            [JsonPropertyAttribute (PropertyName="messageData")] 
            public string MessageData { get; set; } = "";
            [JsonPropertyAttribute (PropertyName="messageId")] 
            public int MessageId { get; set; } = 0;
            [JsonPropertyAttribute (PropertyName="deviceId")] 
            public string DeviceId {get; set;} = Program.DeviceId;
        }
    }
}
