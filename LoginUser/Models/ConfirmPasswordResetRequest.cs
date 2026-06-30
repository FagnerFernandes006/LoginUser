namespace LoginUser.Models;

public class ConfirmPasswordResetRequest
{
    public string Email { get; set; } = "";

    public string Code { get; set; } = "";

    public string NewPassword { get; set; } = "";
}
