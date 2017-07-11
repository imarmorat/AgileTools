using System.Collections.Generic;

namespace AgileTools.Core.Models
{
    public interface IModelConverter
    {
        JiraField ConvertField(dynamic field);
        Card ConvertTicket(dynamic issue, IEnumerable<JiraField> fieldsMeta, IJiraClient client);
        CardStatus ConvertStatus(dynamic status);
        User ConvertUser(dynamic data);
        Sprint ConvertSprint(dynamic sprint);
    }
}