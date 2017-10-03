using System.Collections.Generic;

namespace AgileTools.Core.Models
{
    public interface IModelConverter
    {
        JiraField ConvertField(dynamic field);
        Card ConvertCard(dynamic issue, IEnumerable<JiraField> fieldsMeta);
        CardStatus ConvertStatus(dynamic status);
        User ConvertUser(dynamic data);
        Sprint ConvertSprint(dynamic sprint);
        Release ConvertRelease(dynamic data);
    }
}