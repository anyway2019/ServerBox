using System.Text;
using Aliyun.OSS;

namespace ServerBox.Web.Utils;

public class OssHelper
{
    private static readonly OssClient ossClient = new OssClient(ConfigHelper.OSSHost, ConfigHelper.OSSKeyId, ConfigHelper.OSSKeySecret);

    /// <summary>
    /// upload file to oss
    /// </summary>
    /// <param name="key">file name</param>
    /// <param name="stream">file stream</param>
    /// <param name="folder">relative path to bucket</param>
    /// <param name="prefixName"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public static PutObjectResult PutStream(string key, Stream stream, string folder = "", string prefixName = null,
        string contentType = "")
    {
        var objectMetadata = new ObjectMetadata();
        if (!string.IsNullOrWhiteSpace(contentType))
        {
            objectMetadata.ContentType = contentType;
            return ossClient.PutObject(ConfigHelper.BucketName, folder + key, stream, objectMetadata);
        }

        return ossClient.PutObject(string.IsNullOrEmpty(prefixName) ? ConfigHelper.BucketName : prefixName, folder + key, stream);
    }

    /// <summary>
    /// upload text to oss
    /// </summary>
    /// <param name="key">file name</param>
    /// <param name="content">text need to upload</param>
    /// <param name="folder">file path</param>
    /// <param name="prefixName">bucket prefix</param>
    /// <returns></returns>
    public static PutObjectResult PutString(string key, string content, string folder = "", string prefixName = null)
    {
        var binaryData = Encoding.ASCII.GetBytes(content);
        var requestContent = new MemoryStream(binaryData);
        return PutStream(key, requestContent, folder, prefixName, "text/plain");
    }

    /// <summary>
    /// get thumb image url
    /// </summary>
    /// <param name="key">file name</param>
    /// <param name="prefixName"></param>
    /// <param name="h">height of image</param>
    /// <param name="w">width of iamge</param>
    /// <returns></returns>
    public static string GetThumb(string key, string prefixName = null, int h = 500, int w = 100)
    {
        return string.IsNullOrEmpty(prefixName) ? $"http://{ConfigHelper.BucketName}.{ConfigHelper.OSSHost}/{key}?x-oss-process=image/resize,m_mfit,h_{h},w_{w}" : $"http://{prefixName}.{ConfigHelper.OSSHost}/{key}?x-oss-process=image/resize,m_mfit,h_{h},w_{w}";
    }

    /// <summary>
    /// get big image url
    /// </summary>
    /// <param name="key">file name</param>
    /// <param name="prefixName"></param>
    /// <returns></returns>
    public static string GetBig(string key, string prefixName = null)
    {
        return string.IsNullOrEmpty(prefixName) ? $"http://{ConfigHelper.BucketName}.{ConfigHelper.OSSHost}/{key}?x-oss-process=style/big" : $"http://{prefixName}.{ConfigHelper.OSSHost}/{key}?x-oss-process=style/big";
    }
}