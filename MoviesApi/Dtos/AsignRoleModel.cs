namespace MoviesApi.Dtos
{
    public class AsignRoleModel
    {
        [Required]
        public string RoleName { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}
