using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using SkiaSharp;
using EVESovMap.Rendering;

namespace EVESovMap
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataLoader = new MapDataLoader();
            var mapDataContext = dataLoader.LoadData();
            var mapRenderer = new MapRenderer(mapDataContext);

            mapRenderer.Render();
        }
    }
}
