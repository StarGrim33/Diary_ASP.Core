namespace FonTech.Domain.Result
{
    public class BaseResult
    {
        public bool IsSuccess => ErrorMessage == null;

        public string ErrorMessage { get; set; }

        public int ErrorCode { get; set; }
    }
}
