using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;


Console.WriteLine("Azure Blob Storage exercise\n");

// Run the examples asynchronously, wait for the results before proceeding
ProcessAsync().GetAwaiter().GetResult();

Console.WriteLine("Press enter to exit the sample application.");
Console.ReadLine();



static async Task ProcessAsync()
{
    BlobContainerClient containerClient = await CreateBlobContainerAsync();
    await UploadBlobToContainer(containerClient);
    await ReadBlobFromContainer(containerClient);
    await DownloadedBlobs(containerClient);
    await DeleteContainer(containerClient);
}

static async Task<BlobContainerClient> CreateBlobContainerAsync()
{
    // Copy the connection string from the portal in the variable below.
    string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=mslearnstorageexercise;AccountKey=wXnTOfdH5SnwuER3iSEOqgoZXYGMyk/EhDcdque6PaUT83jg7/NhVjJ0YN5/VfgfLfO1bX9bvXfk+AStjszS6w==;EndpointSuffix=core.windows.net";

    // Create a client that can authenticate with a connection string
    var blobServiceClient = new BlobServiceClient(storageConnectionString);

    //Create a unique name for the container
    string containerName = "wtblob" + Guid.NewGuid().ToString();

    // Create the container and return a container client object
    BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);

    Console.WriteLine("A container named '" + containerClient.Name + "' has been created. " +
    "\nTake a minute and verify in the portal." +
    "\nNext a file will be created and uploaded to the container.");
    Console.WriteLine("Press 'Enter' to continue.");
    Console.ReadLine();


    return containerClient;
}

static async Task UploadBlobToContainer(BlobContainerClient containerClient)
{
    // Create a local file in the ./data/ directory for uploading and downloading
    string localPath = "./data/";
    string fileName = "wtfile" + Guid.NewGuid().ToString() + ".txt";
    string localFilePath = Path.Combine(localPath, fileName);

    // Write text to the file
    await File.WriteAllTextAsync(localFilePath, "Hello, World!");

    // Get a reference to the blob
    BlobClient blobClient = containerClient.GetBlobClient(fileName);

    Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);

    // Open the file and upload its data
    using FileStream uploadFileStream = File.OpenRead(localFilePath);
    await blobClient.UploadAsync(uploadFileStream, true);
    uploadFileStream.Close();

    Console.WriteLine("\nThe file was uploaded. We'll verify by listing" +
            " the blobs next.");
    Console.WriteLine("Press 'Enter' to continue.");
    Console.ReadLine();
}


static async Task ReadBlobFromContainer(BlobContainerClient containerClient)
{
    Console.WriteLine("Listing blobs...");
    await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
    {
        Console.WriteLine("\t" + blobItem.Name);
    }

    Console.WriteLine("\nYou can also verify by looking inside the " +
            "container in the portal." +
            "\nNext the blob will be downloaded with an altered file name.");
    Console.WriteLine("Press 'Enter' to continue.");
    Console.ReadLine();
}

static async Task DownloadedBlobs(BlobContainerClient containerClient)
{
    // Download the blob to a local file
    // Append the string "DOWNLOADED" before the .txt extension 

    string localPath = "./data/";
    string fileName = "wtfile" + Guid.NewGuid().ToString() + "DOWNLOADED.txt";
    string localFilePath = Path.Combine(localPath, fileName);

    Console.WriteLine("\nDownloading blob to\n\t{0}\n", localFilePath);

    // Download the blob's contents and save it to a file
    BlobClient blobClient = containerClient.GetBlobClient(fileName);
    BlobDownloadInfo download = await blobClient.DownloadAsync();

    using (FileStream downloadFileStream = File.OpenWrite(localFilePath))
    {
        await download.Content.CopyToAsync(downloadFileStream);
        downloadFileStream.Close();
    }

    Console.WriteLine("\nLocate the local file to verify it was downloaded.");
    Console.WriteLine("The next step is to delete the container and local files.");
    Console.WriteLine("Press 'Enter' to continue.");
    Console.ReadLine();
}

static async Task DeleteContainer(BlobContainerClient containerClient)
{
    // Delete the container and clean up local files created
    Console.WriteLine("\n\nDeleting blob container...");
    await containerClient.DeleteAsync();
    Console.WriteLine("Finished cleaning up.");
}