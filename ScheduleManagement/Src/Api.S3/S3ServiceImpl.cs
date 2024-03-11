using Amazon.S3;
using Amazon.S3.Transfer;
using ScheduleManagement.Api.Config;

namespace ScheduleManagement.Api.S3;

public class S3ServiceImpl : IS3Service
{
	private readonly AmazonS3Client _client;

	public S3ServiceImpl()
	{
		var config = new AmazonS3Config
		{
			ServiceURL = ApiConfig.S3.Url,
			AuthenticationRegion = ApiConfig.S3.Region,
			ForcePathStyle = true
		};
		_client = new AmazonS3Client(ApiConfig.S3.AccessKey, ApiConfig.S3.SecretKey, config);
	}

	public async Task PutFileFromRequest(string bucket, string fileName, byte[] file)
	{
		using var memoryStream = new MemoryStream();

		await memoryStream.WriteAsync(file);
		memoryStream.Position = 0;

		var fileTransferUtility = new TransferUtility(_client);
		await fileTransferUtility.UploadAsync(memoryStream, bucket, fileName);
	}

	public async Task DeleteFileFromBucket(string bucket, string key)
	{
		await _client.DeleteObjectAsync(bucket, key);
	}
}
