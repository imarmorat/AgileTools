using System.Collections.Generic;

namespace JiraTools.Core.Models
{
    public interface IModelConverter
    {
        JiraField ConvertField(dynamic field);
        Card ConvertTicket(dynamic issue, IEnumerable<JiraField> fieldsMeta);
        CardStatus ConvertStatus(dynamic status);
    }
}