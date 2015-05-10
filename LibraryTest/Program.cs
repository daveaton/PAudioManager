using AudioManager;
using AudioManager.Helpers;
using CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryTest
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"c:\Windows\Media\chimes.wav");
            player.PlayLooping();
            bool isListening = false;
            VolumeChangedEventHandler handler = new VolumeChangedEventHandler(VolumeHandler);
            VolumeManager manager = new VolumeManager("LibraryTest.vshost");
            string input;
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine("System Information:");
            Console.WriteLine("System name: {0}", SysInfo.FullSystemName);
            Console.WriteLine("Process bits: {0}", SysInfo.Is64BitsProcess ? "64 bits" : "32 bits");
            Console.WriteLine("System architecture: {0}", SysInfo.Is64BitsSystem ? "x64" : "x86");
            Console.WriteLine("System OS: {0}", SysInfo.Os);
            Console.WriteLine("Service Pack: {0}", SysInfo.ServicePack);
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine("Process information:");
            Console.WriteLine("Process id: {0}", ProcessInfo.Id);
            Console.WriteLine("Process session id: {0}", ProcessInfo.SessionId);
            Console.WriteLine("Process name: {0}", ProcessInfo.Name);
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine("Manager information:");
            Console.WriteLine("Volume: {0}", manager.Volume);
            Console.WriteLine("Muted: {0}", manager.Mute);
            Console.WriteLine();
            Console.WriteLine("Press +/- to change volume, M for mute/unmute, S for events, Exit to exit.");
            int volume = manager.Volume;
            bool mute = manager.Mute;
            
            do
            {
                input = Console.ReadLine();
                
                switch (input)
                {
                    case "+":
                        {
                            Console.SetCursorPosition(8, 14);
                            if (volume < 100) 
                            {
                                volume = volume + 10;
                                Console.Write(volume);
                                manager.Volume = volume;
                            }
                            break;
                        }
                    case "-":
                        {
                            Console.SetCursorPosition(8, 14);
                            if (volume > 0)
                            {
                                volume = volume - 10;
                                Console.Write(volume);
                                manager.Volume = volume;
                            }
                            break;
                        }
                    case "m": case "M" :
                        {
                            Console.SetCursorPosition(7, 15);
                            mute = !mute;
                            Console.Write(mute);
                            manager.Mute = mute;
                            break;
                        }
                    case "s": case "S":
                        {
                            if (manager.CanListenEvents)
                            {
                                if (!isListening)
                                    manager.VolumeChanged += manager_VolumeChanged;
                                else
                                    manager.VolumeChanged -= manager_VolumeChanged;
                                isListening = !isListening;
                            }
                            break;
                        }
                }
                Console.SetCursorPosition(0, 18);
            } while (input.ToUpper() != "EXIT");
            manager.Dispose();
        }

        static void manager_VolumeChanged(object sender, CoreAudioApi.Helpers.VolumeData data)
        {
            Console.SetCursorPosition(0, 20);
            Console.WriteLine("New Volume: {0} - New Mute: {1}", data.Volume, data.Mute);
            Console.SetCursorPosition(0, 18);
        }

        private static void VolumeHandler(object sender, CoreAudioApi.Helpers.VolumeData data)
        {
            Console.SetCursorPosition(0, 20);
            Console.WriteLine("New Volume: {0} - New Mute: {1}", data.Volume, data.Mute);
            Console.SetCursorPosition(0, 18);
        }
    }
}
