using System;
using CAOS;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.IO;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;

namespace ConsoleApp1
{
    class Demo
    {
        static void Main(string[] args)
        {
            CaosInjector injector = new CaosInjector("Docking Station");

            if (injector.CanConnectToGame())
            {
                TryCatchStrategy(injector);
                //TryReturnBoolStrategy(injector);
            }
            else
            {
                Console.WriteLine("Couldn't connect to game.");
            }
            Console.ReadKey();
        }

        private static string uriString { get; } = "https://lemurware.tech/api/v1/creatures/";
        private static string documentsDockingStation { get; }

        static Demo()
        {
            string gog = (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/creatures/docking station/");

            if (Directory.Exists(gog))
            {
                documentsDockingStation = gog;
            }
            else
            {
                documentsDockingStation = ConfigurationManager.AppSettings["documentsDockingStation"];
            }
        }

        private static void TryCatchStrategy(CaosInjector injector)
        {
            try
            {
                CaosResult result = injector.ExecuteCaos(
 @"
outs wnam
sets va70 rtif rtim ""%Y%m%d%H%M%S""
adds va70 "".crdb.json""
file oope 0 va70 0
sets va00 """"
outs ""[""
loop
    sets va50 va00
    sets va00 hist next va00
    doif va50 <> """" AND hist mute va00 <> -1
        outs "",""
    endi

    doif hist mute va00 <> -1
        outs ""{""

        outs ""\""moniker\"":\""""
	    outs va00
        outs ""\"",""

        outs ""\""name\"":\""""
	    outs hist name va00
        outs ""\"",""

        outs ""\""crossoverPointMutations\"":""
	    outv hist cros va00
        outs "",""

        outs ""\""pointMutations\"":""
	    outv hist mute va00
        outs "",""

        outs ""\""gender\"":""
	    outv hist gend va00
        outs "",""

        outs ""\""genus\"":""
	    outv hist gnus va00
        outs "",""

        outs ""\""events\"":[""

        setv va01 0
        setv va60 hist coun va00
        loop

            doif va01 <> 0
                outs "",""
            endi
            
            outs ""{""

            outs ""\""histEventType\"":""
	        outv hist type va00 va01
            outs "",""

            outs ""\""lifeStage\"":""
	        outv hist cage va00 va01
            outs "",""

            outs ""\""photo\"":\""""
	        outs hist foto va00 va01
            outs ""\"",""

            outs ""\""moniker1\"":\""""
	        outs hist mon1 va00 va01
            outs ""\"",""

            outs ""\""moniker2\"":\""""
	        outs hist mon2 va00 va01
            outs ""\"",""

            outs ""\""timeUtc\"":""
	        outv hist rtim va00 va01
            outs "",""

            outs ""\""tickAge\"":""
	        outv hist tage va00 va01
            outs "",""

            outs ""\""worldTick\"":""
	        outv hist wtik va00 va01
            outs "",""

            outs ""\""worldName\"":\""""
	        outs hist wnam va00 va01
            outs ""\"",""

            outs ""\""worldId\"":\""""
	        outs hist wuid va00 va01
            outs ""\"",""

            outs ""\""userText\"":\""""
	        outs hist utxt va00 va01
            outs ""\""""

            outs ""}""

            addv va01 1
        untl va01 >= va60

        outs ""]}""

    endi
untl va00 = """"
outs ""]""
file oclo

");
                if (result.Success)
                {
                    string worldName = result.Content.Replace("\0", string.Empty);

                    string worldDirectory = documentsDockingStation + "/My Worlds/" + worldName;
                    string worldJournalDirectory = worldDirectory + "/Journal";
                    string worldImagesDirectory = worldDirectory + "/Images";


                    foreach (string fileEntry in Directory.GetFiles(worldJournalDirectory))
                    {
                        if (fileEntry.EndsWith(".crdb.json"))
                        {
                            /*using (StreamReader r = new StreamReader(fileEntry))
                            {
                                string json = r.ReadToEnd();
                                dynamic array = JsonConvert.DeserializeObject(json);
                                foreach (var item in array)
                                {
                                    // Create a new WebClient instance.
                                    WebClient myWebClient = new WebClient();
                                    //string postData = item;

                                    // Apply ASCII Encoding to obtain the string as a byte array.
                                    Console.WriteLine("PUTting {0} ...", item.ToString(Formatting.None));

                                    byte[] postArray = Encoding.UTF8.GetBytes(item.ToString(Formatting.None));
                                    Console.WriteLine("Uploading to {0} ...", uriString + item.moniker);
                                    myWebClient.Headers.Add("Content-Type", "application/json");

                                    //UploadData implicitly sets HTTP POST as the request method.




                                    byte[] responseArray = myWebClient.UploadData(uriString + item.moniker, "PUT", postArray);

                                    // Decode and display the response.
                                    Console.WriteLine("\nResponse received was :{0}", Encoding.ASCII.GetString(responseArray));
                                }
                            }*/
                            Console.WriteLine("Deleting: " + fileEntry);
                            File.Delete(fileEntry);
                        }
                    }

                    foreach (string fileEntry in Directory.GetFiles(worldImagesDirectory))
                    {
                        using (BinaryReader r = new BinaryReader(File.Open(fileEntry, FileMode.Open)))
                        {
                            Int32 rgbFormat = (Int32)r.ReadUInt32();
                            Int32 numberofImages = (Int32)r.ReadUInt16();
                            Int32 offsetToImageData = (Int32)r.ReadUInt32();
                            Int32 width = (Int32)r.ReadUInt16();
                            Int32 height = (Int32)r.ReadUInt16();

                            using (Bitmap bm = new Bitmap(width, height))
                            {
                                for (int y = 0; y < height; y++)
                                {
                                    for (int x = 0; x < width; x++)
                                    {
                                        UInt16 rgbUInt16 = r.ReadUInt16();
                                        Int32 red = (int)(((UInt32)rgbUInt16 & 0xf800) >> 8);
                                        Int32 green = (int)(((UInt32)rgbUInt16 & 0x07e0) >> 3);
                                        Int32 blue = (int)(((UInt32)rgbUInt16 & 0x001f) << 3);
                                        bm.SetPixel(x, y, Color.FromArgb(red, green, blue));
                                        //Console.WriteLine($"r:{red} g:{green} b:{blue}");
                                    }
                                }
                                using (WebClient client = new WebClient())
                                using (var ms = new MemoryStream())
                                {
                                    client.Headers.Add("Content-Type", "image/png");
                                    bm.Save(ms, ImageFormat.Png);
                                    Console.WriteLine($"{uriString}images/{Path.GetFileNameWithoutExtension(fileEntry)}");
                                    client.UploadData($"{uriString}images/{Path.GetFileNameWithoutExtension(fileEntry)}", "PUT", ms.ToArray());
                                }
                            }
                        }
                        Console.WriteLine("Image: " + fileEntry);

                    }
                }
                else
                {
                    Console.WriteLine($"Error Code: {result.ResultCode}");
                }
            }
            catch (NoGameCaosException e)
            {
                Console.WriteLine($"Game exited unexpectedly. Error message: {e.Message}");
            }
        }

        private static void TryReturnBoolStrategy(CaosInjector injector)
        {
            CaosResult result;
            if (injector.TryExecuteCaos("outs \"End file output. Begin upload.\"", out result))
            {
                if (result.Success)
                {
                    Console.WriteLine(result.Content);
                    //Just try to do it, we don't care about the results
                    injector.TryExecuteCaos("targ norn doif targ <> null sezz \"Yo yo! What up?\" endi");
                }
                else
                {
                    Debug.Assert(result.ResultCode != 0);
                    Console.WriteLine($"Error Code: {result.ResultCode}");
                }
            }
            else
            {
                Console.WriteLine("Execution failed. Game may have exited.");
            }
        }
    }
}
