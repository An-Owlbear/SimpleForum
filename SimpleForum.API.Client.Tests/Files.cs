using System;
using System.IO;
using System.Threading.Tasks;
using SimpleForum.Common;

namespace SimpleForum.API.Client.Tests
{
    public partial class Tests
    {
        // Tests downloading an image
        private static async Task TestProfileImage()
        {
            Console.Write("Enter the ID of the user to download the image for\n> ");
            int ID = int.Parse(Console.ReadLine());

            Result<Stream> result = await client.GetProfileImg(ID);
            if (result.Failure)
            {
                DisplayItems.DisplayError(result);
                return;
            }

            await using FileStream fileStream = File.Create("image.jpg");
            result.Value.Seek(0, SeekOrigin.Begin);
            result.Value.CopyTo(fileStream);
        }
    }
}