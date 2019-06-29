using System;
using SkiaSharp;

namespace EVESovMap.Rendering
{
    public class SolarSystemsMapRenderTask : IMapRenderTask
    {
        public void Render(SKCanvas canvas, MapDataContext mapDataContext)
        {
            using (var paint = new SKPaint())
            {
                paint.Style = SKPaintStyle.StrokeAndFill;
                paint.Color = new SKColor(0xB0, 0xB0, 0xFF);
                paint.IsAntialias = true;

                foreach(var solarSystem in mapDataContext.SolarSystems)
                {
                    var random = new Random();
                    var randomBytes = new Byte[3];
                    random.NextBytes(randomBytes);
                    // paint.Color = new SKColor(randomBytes[0], randomBytes[1], randomBytes[2]);
                    canvas.DrawOval(MapRenderUtils.EveCartesianToScreenPoint(solarSystem.x, solarSystem.z), new SKSize(1.0f, 0.5f), paint);
                }
                canvas.DrawLine(new SKPoint(0.0f, 0.0f), new SKPoint(100.0f, 100.0f), paint);
            }

        }
    }
}