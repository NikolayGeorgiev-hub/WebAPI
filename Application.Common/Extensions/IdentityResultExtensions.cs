using Microsoft.AspNetCore.Identity;
using System.Text;

namespace Application.Common.Extensions;

public static class IdentityResultExtensions
{
    public static string GetIdentityResultMessages(this IdentityResult identityResult)
    {
        StringBuilder identityResultMessage = new();
        IEnumerable<string> identityResultMessages = identityResult.Errors.Select(x => x.Description);
        identityResultMessages.ToList().ForEach(x => identityResultMessage.AppendLine(x));

        return identityResultMessage.ToString();
    }
}
