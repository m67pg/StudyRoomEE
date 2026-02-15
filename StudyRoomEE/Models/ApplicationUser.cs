namespace StudyRoomEE.Models
{
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser
    {
        // 追加: 0=管理者, 1以上=生徒
        public int StudentId { get; set; }
    }
}
