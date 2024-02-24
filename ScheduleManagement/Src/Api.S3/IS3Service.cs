namespace ScheduleManagement.Api.S3;

public interface IS3Service
{
	Task PutFileFromRequest(string bucket, string fileName, byte[] file);

	Task DeleteFileFromBucket(string bucket, string key);
}
