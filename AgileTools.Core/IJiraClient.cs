﻿using AgileTools.Core.Models;
using System.Collections.Generic;

namespace AgileTools.Core
{
    public interface IJiraClient
    {
        IEnumerable<CardStatus> GetStatuses();

        IEnumerable<JiraField> GetFields();

        void CommentTicket(string ticketId, string comment, string author = null);

        IEnumerable<Card> GetTickets(string query);

        Card GetTicket(string ticket);
    }
}