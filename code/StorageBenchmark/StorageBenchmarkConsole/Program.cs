using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using System.Diagnostics;

namespace StorageBenchmarkConsole
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage:  <Source Root Blob Url>");
            }
            else
            {
                var sourceRoot = args[0];

                Console.WriteLine($"Source Root Blob Url:  {sourceRoot}");
                Console.WriteLine();

                try
                {
                    var sourceBlobClient = new BlockBlobClient(
                        new Uri(sourceRoot),
                        new DefaultAzureCredential());
                    var totalElapsed = TimeSpan.Zero;
                    var loopLength = 5;
                    var loop = loopLength;

                    while (loop-- != 0)
                    {
                        totalElapsed += await ListBlobsAsync(sourceBlobClient);
                    }

                    Console.WriteLine();
                    Console.WriteLine($"Average elapsed:  {totalElapsed / loopLength}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }
            }
        }

        private static async Task<TimeSpan> ListBlobsAsync(BlockBlobClient sourceBlobClient)
        {
            var container = sourceBlobClient.GetParentBlobContainerClient();
            var prefix = sourceBlobClient.Name;
            var pageable = container.GetBlobsAsync(prefix: prefix);
            var watch = new Stopwatch();
            var blobCount = 0;

            watch.Start();
            await foreach (var item in pageable)
            {
                ++blobCount;
            }

            var elaspsed = watch.Elapsed;

            Console.WriteLine($"{blobCount} blobs in {elaspsed}");

            return elaspsed;
        }
    }
}