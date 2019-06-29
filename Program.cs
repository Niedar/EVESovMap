using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using SkiaSharp;

namespace EVESovMap
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            IEnumerable<SolarSystem> solarSystems;
            IEnumerable<SolarSystemJumps> solarSystemJumps;
            IEnumerable<Region> regions;

            using (var reader = new StreamReader("mapSolarSystems.csv"))
            using (var csvReader = new CsvReader(reader))
            {
                solarSystems = csvReader.GetRecords<SolarSystem>().ToList();
            }
            using (var reader = new StreamReader("mapSolarSystemJumps.csv"))
            using (var csvReader = new CsvReader(reader))
            {
                solarSystemJumps = csvReader.GetRecords<SolarSystemJumps>().ToList();
            }
            using (var reader = new StreamReader("mapRegions.csv"))
            using (var csvReader = new CsvReader(reader))
            {
                regions = csvReader.GetRecords<Region>().ToList();
            }


            var imageInfo = new SKImageInfo(MapConstants.HORIZONTAL_SIZE, MapConstants.VERTICAL_SIZE, SKImageInfo.PlatformColorType, SKAlphaType.Premul);

            using (var surface = SKSurface.Create(imageInfo)) {
                SKCanvas myCanvas = surface.Canvas;
                myCanvas.Clear(SKColors.Black);

                using (var paint = new SKPaint())
                {
                    paint.Style = SKPaintStyle.StrokeAndFill;
                    paint.Color = new SKColor(0xB0, 0xB0, 0xFF);
                    paint.IsAntialias = true;

                    RenderSolarSystemJumps(myCanvas, paint, solarSystemJumps, solarSystems);
                    RenderSolarSystems(myCanvas, paint, solarSystems);
                    RenderRegionNames(myCanvas, paint, regions);
                    

                    using (SKImage image = surface.Snapshot())
                    using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
                    using (var fileStream = File.Create("sovMap.png"))
                    {
                        fileStream.Write(data.ToArray(), 0, data.ToArray().Length);
                    }
                    // using (MemoryStream mStream = new FileStream(data.ToArray()))
                    // {
                    //     Bitmap bm = new Bitmap(mStream, false);
                    //     pictureBox1.Image = bm;
                    // }                                      
                }

                // Your drawing code goes here.
            }
        }

        static void RenderSolarSystems(SKCanvas canvas, SKPaint paint, IEnumerable<SolarSystem> solarSystems)
        {
            paint.Style = SKPaintStyle.StrokeAndFill;
            paint.Color = new SKColor(0xB0, 0xB0, 0xFF);
            paint.IsAntialias = true;

            foreach(var solarSystem in solarSystems)
            {
                var random = new Random();
                var randomBytes = new Byte[3];
                random.NextBytes(randomBytes);
                // paint.Color = new SKColor(randomBytes[0], randomBytes[1], randomBytes[2]);
                canvas.DrawOval(EveCartesianToScreenPoint(solarSystem.x, solarSystem.z), new SKSize(1.0f, 0.5f), paint);
            }
            canvas.DrawLine(new SKPoint(0.0f, 0.0f), new SKPoint(100.0f, 100.0f), paint);
        }

        public static void RenderRegionNames(SKCanvas canvas, SKPaint paint, IEnumerable<Region> regions)
        {
            paint.Style = SKPaintStyle.StrokeAndFill;
            paint.Color = SKColors.White.WithAlpha(0x7E);
            paint.IsAntialias = true;
            paint.TextSize = 12.0f;
            foreach (var region in regions)
            {
                canvas.DrawText(region.regionName,EveCartesianToScreenPoint(region.x, region.z), paint);
            }
        }

        static void RenderSolarSystemJumps(SKCanvas canvas, SKPaint paint, IEnumerable<SolarSystemJumps> solarSystemJumps, IEnumerable<SolarSystem> solarSystems)
        {
            paint.Style = SKPaintStyle.Stroke;
            paint.Color = new SKColor(0xB0, 0xB0, 0xFF, 0x1E);
            paint.IsAntialias = true;
            foreach (var solarSystemJump in solarSystemJumps)
            {
                var fromSolarSystem = solarSystems.First(x => x.solarSystemID == solarSystemJump.fromSolarSystemID);
                var toSolarSystem = solarSystems.First(x => x.solarSystemID == solarSystemJump.toSolarSystemID);

                if (solarSystemJump.IsRegionalJump)
                {
                    paint.Color = SKColors.LightPink.WithAlpha(0x1E);
                }
                else if (solarSystemJump.IsConstellationJump)
                {
                    paint.Color = SKColors.Red.WithAlpha(0x1E);
                }
                else
                {
                    paint.Color = SKColors.Blue.WithAlpha(0x1E);
                }


                canvas.DrawLine(EveCartesianToScreenPoint(fromSolarSystem.x, fromSolarSystem.z), EveCartesianToScreenPoint(toSolarSystem.x, toSolarSystem.z), paint);
            }
        }

        static SKPoint EveCartesianToScreenPoint(double x, double y)
        {
            var xCoord = Math.Floor((x / MapConstants.SCALE ) + (MapConstants.HORIZONTAL_SIZE / 2) + MapConstants.HORIZONTAL_OFFSET) + 0.5;
            var yCoord = Math.Floor((MapConstants.VERTICAL_SIZE / 2) - (y / MapConstants.SCALE) + MapConstants.VERTICAL_OFFSET) + 0.5;

            return new SKPoint((float)xCoord, (float)yCoord);
        }
    }

    public class SolarSystem
    {
        public string solarSystemID {get; set;}
        public double x {get; set;}
        public double y {get; set;}
        public double z {get; set;}
    }

    public class SolarSystemJumps
    {
        public string fromRegionID {get; set;}
        public string fromConstellationID {get; set;}
        public string fromSolarSystemID {get; set;}
        public string toSolarSystemID {get; set;}
        public string toConstellationID {get; set;}
        public string toRegionID {get; set;}

        public bool IsConstellationJump => (fromConstellationID != toConstellationID) && !IsRegionalJump;
        public bool IsRegionalJump => fromRegionID != toRegionID;
    }

    public class Region
    {
        public string regionID { get; set;}
        public string regionName {get; set;}
        public double x {get; set;}
        public double y {get; set;}
        public double z {get; set;}
    }

    public class MapConstants
    {	
        //	Sample rate for text placement algorithm, samples every sampleRate pixels
        public static int SAMPLE_RATE = 8; 

        //	width
        public static int HORIZONTAL_SIZE = 928*2;
        
        //	height
        public static int VERTICAL_SIZE = 1024*2;
        
        //	vertical offset
        public static int HORIZONTAL_OFFSET = 208;
        
        //	horizontal offset
        public static int VERTICAL_OFFSET = 0;
        
        //	Scaling factor
        public static double SCALE = 4.8445284569785E17/((VERTICAL_SIZE - 20) / 2.0);
    }
}
