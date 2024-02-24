using System.Net;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using ScheduleManagement.Api.Config;
using ScheduleManagement.Api.Exception;

namespace ScheduleManagement.Api.S3;

public class S3ServiceImpl : IS3Service
{
	private readonly AmazonS3Client _client;

	public S3ServiceImpl()
	{
		var credentials = new BasicAWSCredentials(
			ApiConfig.S3.AccessKey,
			ApiConfig.S3.SecretKey
		);
		var config = new AmazonS3Config
		{
			ServiceURL = ApiConfig.S3.Url,
			RegionEndpoint = RegionEndpoint.GetBySystemName(ApiConfig.S3.Region),
			ForcePathStyle = true
		};
		_client = new AmazonS3Client(credentials, config);
	}

	public async Task PutFileFromRequest(string bucket, string fileName, byte[] file)
	{
		using var memoryStream = new MemoryStream();

		await memoryStream.WriteAsync(file);
		memoryStream.Position = 0;

		var fileTransferUtility = new TransferUtility(_client);
		await fileTransferUtility.UploadAsync(memoryStream, bucket, fileName);
	}

	public async Task<(byte[], string)> GetFileFromBucket(string bucket, string key)
	{
		var response = await _client.GetObjectMetadataAsync(bucket, key);
		var contentType = response.Headers.ContentType;

		var getObjectRequest = new GetObjectRequest
		{
			BucketName = bucket,
			Key = key
		};
		using var getObjectResponse = await _client.GetObjectAsync(getObjectRequest);
		if (getObjectResponse == null)
		{
			throw new RestApiException("Nie znaleziono szukanego pliku.", HttpStatusCode.NotFound);
		}
		using var memoryStream = new MemoryStream();

		await getObjectResponse.ResponseStream.CopyToAsync(memoryStream);
		var fileBytes = memoryStream.ToArray();

		return (fileBytes, contentType);
	}

	public async Task DeleteFileFromBucket(string bucket, string key)
	{
		await _client.DeleteObjectAsync(bucket, key);
	}
}
