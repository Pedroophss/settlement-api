using RetSettlementDates.Domain.Abstractions;
using RetSettlementDates.Domain.DataObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RetSettlementDates.Domain.Binders
{
    /// <summary>
    /// This type will bind all of the configured data services with settlements from the CSV file
    /// </summary>
    public class CsvFileBinder : IBinderService
    {
        public class Input
        {
            public string CsvPath { get; }
            public char Separator { get; }
            public CultureInfo FileFormat { get; }

            public Input(string csvPath, char separator, CultureInfo fileFormat)
            {
                CsvPath = csvPath;
                Separator = separator;
                FileFormat = fileFormat;
            }
        }

        Input Args { get; }
        IEnumerable<IDataService> Services { get; }

        public CsvFileBinder(Input input, IEnumerable<IDataService> services)
        {
            Args = input;
            Services = services;
        }

        public async Task Bind(CancellationToken token)
        {
            using var file = new CsvReader(Args.CsvPath, Args.Separator);

            file.Open();

            await foreach(var line in file.ReadLinesAsync())
            {
                if (line.Length != 7)
                    continue;

                var settlement = ParseFromLine(line);
                if (settlement == Settlement.InvalidRef)
                    continue;

                foreach(var ds in Services)
                    await ds.ProcessSettlement(settlement, token);
            }
        }

        internal Settlement ParseFromLine(string[] line)
        {
            const string dateFormat = "yyyy-MM-dd";
            const string dateTimeFormat = "yyyy-MM-dd hh:mm:ss";
            const DateTimeStyles none = DateTimeStyles.None;

            var service = DateTime.MinValue;
            var pricePerMV = float.MinValue;
            var volumeMWH = float.MinValue;
            var insert = DateTime.MinValue;
            var modified = DateTime.MinValue;

            var valid = int.TryParse(line[1], out int id)
                && DateTime.TryParseExact(line[2], dateFormat, Args.FileFormat, none, out service)
                && float.TryParse(line[3], NumberStyles.Any, Args.FileFormat, out pricePerMV)
                && float.TryParse(line[4], NumberStyles.Any, Args.FileFormat, out volumeMWH)
                && DateTime.TryParseExact(line[5], dateTimeFormat, Args.FileFormat, none, out insert)
                && DateTime.TryParseExact(line[6], dateTimeFormat, Args.FileFormat, none, out modified);

            if (!valid)
                return Settlement.InvalidRef;

            return new Settlement
            (
                settlementLocationName: line[0],
                settlementLocationID: id,
                dateOfService: service,
                pricePerMWh: pricePerMV,
                volumeMWh: volumeMWH,
                insertDate: insert,
                modifiedDate: modified
            );
        }
    }

    internal class CsvReader : IDisposable
    {
        string Path { get; }
        char Separator { get; }
        StreamReader Stream { get; set; }

        public CsvReader(string path, char separator)
        {
            Path = path;
            Separator = separator;
        }

        public void Open()
        {
            if (!File.Exists(Path))
                throw new FileNotFoundException(Path);

            Stream = new StreamReader(Path);
        }

        public async IAsyncEnumerable<string[]> ReadLinesAsync()
        {
            if (Stream is null)
                throw new Exception("The reader should be open first");

            while(!Stream.EndOfStream)
            {
                var line = await Stream.ReadLineAsync();
                yield return line.Split(Separator);
            }
        }

        public void Dispose()
        {
            if (Stream != null)
                Stream.Dispose();
        }
    }
}
