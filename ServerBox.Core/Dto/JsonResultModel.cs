namespace ServerBox.Core.Dto;

public class JsonResultModel<T>
{
    public int code { get; set; }
    public T data { get; set; }
    public string message { get; set; }
}