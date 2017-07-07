namespace AgileTools.Core.Models
{
    public class User
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }

        public override string ToString()
        {
            return $"{FullName} ({Id})";
        }
    }
}