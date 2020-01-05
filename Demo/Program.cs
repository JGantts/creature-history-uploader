using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.IO;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace ConsoleApp1
{
    class Demo
    {
        private static string uriString { get; } = "https://lemurware.tech/api/v1/creatures/";
        private static string documentsDockingStationWorlds { get; }

        static Demo()
        {
            string gog = (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/creatures/docking station/My Worlds");

            if (Directory.Exists(gog))
            {
                documentsDockingStationWorlds = gog;
            }
            else
            {
                documentsDockingStationWorlds = ConfigurationManager.AppSettings["documentsDockingStationWorlds"];
            }
        }

        static void Main(string[] args)
        {
            var worldDirectories = Directory.GetDirectories(documentsDockingStationWorlds).Where(x => !x.EndsWith("\\Startup"));

            foreach (string worldDirectory in worldDirectories)
            {
                string worldJournalDirectory = worldDirectory + "/Journal/";
                string worldImagesDirectory = worldDirectory + "/Images/";

                Console.WriteLine("Searching: " + worldJournalDirectory);

                foreach (string fileEntry in Directory.GetFiles(worldJournalDirectory).ToList())
                {
                    if (fileEntry.EndsWith(".creature.crdb1.json") || fileEntry.EndsWith(".creatureEvents.crdb1.json"))
                    {
                        using (StreamReader r = new StreamReader(fileEntry))
                        {
                            string json = r.ReadToEnd();
                            dynamic creature = JsonConvert.DeserializeObject(json);

                            byte[] postArray = Encoding.UTF8.GetBytes(creature.ToString(Formatting.None));

                            //UploadData implicitly sets HTTP POST as the request method.

                            bool done = false;
                            while (!done)
                            {
                                try
                                {
                                    WebClient myWebClient = new WebClient();
                                    string uriStringfinal = uriString + creature.moniker; 
                                    Console.WriteLine("Uploading to {0} ...", uriStringfinal);
                                    myWebClient.Headers.Add("Content-Type", "application/json");
                                    byte[] responseArray = myWebClient.UploadData(uriStringfinal, "PUT", postArray);
                                    // Decode and display the response.
                                    Console.WriteLine("\nResponse received was :{0}", Encoding.ASCII.GetString(responseArray));
                                    done = true;
                                }
                                catch (Exception error)
                                {
                                    Console.WriteLine(error.ToString());
                                    Console.ReadKey();
                                }
                            }

                            foreach (dynamic lifeEvent in creature.events)
                            {
                                if (lifeEvent.photo != "")
                                {
                                    uploadPhoto(worldImagesDirectory + lifeEvent.photo + ".s16");
                                }
                            }

                        }
                        Console.WriteLine("Deleting: " + fileEntry);
                        File.Delete(fileEntry);
                    }
                    else if (fileEntry.EndsWith(".event.crdb1.json"))
                    {
                        using (StreamReader r = new StreamReader(fileEntry))
                        {
                            string json = r.ReadToEnd();
                            dynamic lifeEvent = JsonConvert.DeserializeObject(json);

                            byte[] postArray = Encoding.UTF8.GetBytes(lifeEvent.ToString(Formatting.None));

                            //UploadData implicitly sets HTTP POST as the request method.

                            bool done = false;
                            while (!done)
                            {
                                try
                                {
                                    WebClient myWebClient = new WebClient();
                                    string uriStringFinal = uriString + lifeEvent.moniker + "/events/" + lifeEvent.eventNumber;
                                    Console.WriteLine("Uploading to {0} ...", uriStringFinal);
                                    myWebClient.Headers.Add("Content-Type", "application/json");
                                    byte[] responseArray = myWebClient.UploadData(uriStringFinal, "PUT", postArray);
                                    // Decode and display the response.
                                    Console.WriteLine("\nResponse received was :{0}", Encoding.ASCII.GetString(responseArray));
                                    done = true;
                                }
                                catch (Exception error)
                                {
                                    Console.WriteLine(error.ToString());
                                    Console.ReadKey();
                                }
                            }

                            if (lifeEvent.photo != "")
                            {
                                uploadPhoto(worldImagesDirectory + lifeEvent.photo + ".s16");
                            }

                        }
                        Console.WriteLine("Deleting: " + fileEntry);
                        File.Delete(fileEntry);
                    }
                    /*else if (fileEntry.EndsWith(".utxt.crdb1.json"))
                    {
                        using (StreamReader r = new StreamReader(fileEntry))
                        {
                            string utxt = r.ReadToEnd();
                            JObject utxtJson = new JObject(
                                new JProperty("utxt", new JValue(utxt))
                            );
                            byte[] postArray = Encoding.UTF8.GetBytes(utxtJson.ToString(Formatting.None));

                            //UploadData implicitly sets HTTP POST as the request method.

                            bool done = false;
                            while (!done)
                            {
                                try
                                {
                                    WebClient myWebClient = new WebClient();
                                    Console.WriteLine("Uploading to {0} ...", uriString + creature.moniker);
                                    myWebClient.Headers.Add("Content-Type", "application/json");
                                    byte[] responseArray = myWebClient.UploadData(uriString + creature.moniker, "PUT", postArray);
                                    // Decode and display the response.
                                    Console.WriteLine("\nResponse received was :{0}", Encoding.ASCII.GetString(responseArray));
                                    done = true;
                                }
                                catch (Exception error)
                                {
                                    Console.WriteLine(error.ToString());
                                    Console.ReadKey();
                                }
                            }

                            foreach (dynamic lifeEvent in creature.events)
                            {
                                if (lifeEvent.photo != "")
                                {
                                    uploadPhoto(worldImagesDirectory + lifeEvent.photo);
                                }
                            }

                        }
                        Console.WriteLine("Deleting: " + fileEntry);
                        File.Delete(fileEntry);
                    }
                    else if (fileEntry.EndsWith(".name.crdb1.json"))
                    {
                        using (StreamReader r = new StreamReader(fileEntry))
                        {
                            string json = r.ReadToEnd();
                            dynamic creature = JsonConvert.DeserializeObject(json);

                            byte[] postArray = Encoding.UTF8.GetBytes(creature.ToString(Formatting.None));

                            //UploadData implicitly sets HTTP POST as the request method.

                            bool done = false;
                            while (!done)
                            {
                                try
                                {
                                    WebClient myWebClient = new WebClient();
                                    Console.WriteLine("Uploading to {0} ...", uriString + creature.moniker);
                                    myWebClient.Headers.Add("Content-Type", "application/json");
                                    byte[] responseArray = myWebClient.UploadData(uriString + creature.moniker, "PUT", postArray);
                                    // Decode and display the response.
                                    Console.WriteLine("\nResponse received was :{0}", Encoding.ASCII.GetString(responseArray));
                                    done = true;
                                }
                                catch (Exception error)
                                {
                                    Console.WriteLine(error.ToString());
                                    Console.ReadKey();
                                }
                            }

                            foreach (dynamic lifeEvent in creature.events)
                            {
                                if (lifeEvent.photo != "")
                                {
                                    uploadPhoto(worldImagesDirectory + lifeEvent.photo);
                                }
                            }

                        }
                        Console.WriteLine("Deleting: " + fileEntry);
                        File.Delete(fileEntry);
                    }*/
                }
            }

            Console.WriteLine("Done.");
            Console.ReadKey();
        }

        static void uploadPhoto(string imagePath)
        {
            using (BinaryReader r = new BinaryReader(File.Open(imagePath, FileMode.Open)))
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
                        client.UploadData($"{uriString}images/{Path.GetFileNameWithoutExtension(imagePath)}", "PUT", ms.ToArray());
                    }
                }
            }
            Console.WriteLine("Image: " + imagePath);
        }
    }
}
