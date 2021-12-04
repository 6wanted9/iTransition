using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Couresework.Models
{
    public class Comment
    {
        public Comment(int reviewId, string authorId, string userComment)
        {
            ReviewId = reviewId;
            AuthorId = authorId;
            UserComment = userComment;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int Id { get; set; }
        [Required]
        public int ReviewId { get; set; }
        public Review Review { get; set; }
        [Required]
        public string AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public AspNetUsers AspNetUsers { get; set; }
        [Required]
        public string UserComment { get; set; }
    }
}
