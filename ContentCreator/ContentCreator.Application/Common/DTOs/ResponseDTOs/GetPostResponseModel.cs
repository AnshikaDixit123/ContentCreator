public class GetPostResponseModel
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public string PostDescription { get; set; }
    public string Media { get; set; }
    public int LikeCount { get; set; }
    public string UserName { get; set; }
    public bool IsLiked { get; set; }
    public Guid? SharedBy { get; set; }
    public Guid? ParentId { get; set; }
    public DateTime DatePosted { get; set; }
    public bool IsReshared { get; set; }
    public OriginalPostModel OriginalPost { get; set; }
}

public class OriginalPostModel
{
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string PostDescription { get; set; }
    public string Media { get; set; }
}