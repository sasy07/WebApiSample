namespace WebApiSample.Entities;

public class Post:BaseEntity<Guid>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public int CategoryId { get; set; }
    public int AuthorId { get; set; }

    public Category? Category { get; set; }
    public User? Author { get; set; }
}
