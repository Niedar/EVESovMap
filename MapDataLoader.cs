using System.IO;
using System.Linq;
using CsvHelper;

namespace EVESovMap
{
    public class MapDataLoader
    {
        public MapDataContext LoadData()
        {
            var mapDataContext = new MapDataContext();
            using (var reader = new StreamReader("mapSolarSystems.csv"))
            using (var csvReader = new CsvReader(reader))
            {
                mapDataContext.SolarSystems = csvReader.GetRecords<SolarSystem>().ToList();
            }
            using (var reader = new StreamReader("mapSolarSystemJumps.csv"))
            using (var csvReader = new CsvReader(reader))
            {
                mapDataContext.SolarSystemJumps = csvReader.GetRecords<SolarSystemJumps>().ToList();
            }
            using (var reader = new StreamReader("mapRegions.csv"))
            using (var csvReader = new CsvReader(reader))
            {
                mapDataContext.Regions = csvReader.GetRecords<Region>().ToList();
            }
            return mapDataContext;
        }
    }
}
