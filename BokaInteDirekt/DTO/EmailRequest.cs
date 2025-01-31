namespace BokaInteDirekt.DTO
{
    public class EmailRequest
    {
        public required string Receiver { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
    }
}
