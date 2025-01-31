using BokaInteDirekt.Models;

namespace BokaInteDirekt.DTO
{
    public class BookingUserRequest
    {
        public BookingRequest? Request { get; set; }
        public User? User { get; set; }
    }
}
