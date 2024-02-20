namespace ScheduleManagement.Api.S3;

public interface IS3Service
{
	Task PutFileFromRequest(string bucket, string fileName, IFormFile file);

	Task<(byte[], string)> GetFileFromBucket(string bucket, string key);

	Task DeleteFileFromBucket(string bucket, string key);
}
