using AgileTools.Core.Models;
using System.Collections.Generic;

namespace AgileTools.Core
{
    public interface ICardManagerClient
    {
        string  Id { get; set; }

        IList<string> InitParameters { get; }

        void Init(Dictionary<string, string> initParams);

        IModelConverter ModelConverter { get; set; }

        IEnumerable<CardStatus> GetStatuses();

        IEnumerable<JiraField> GetFields();

        void CommentTicket(string ticketId, string comment, string author = null);

        IEnumerable<Card> GetTickets(string query);

        Card GetTicket(string cardId);

        Sprint GetSprint(string sprintId);

        CardStatus GetStatus(string statusId);

        User GetUser(string userId);

        bool TryCheckConnection();

        Release GetRelease(string releaseId);
    }
}
