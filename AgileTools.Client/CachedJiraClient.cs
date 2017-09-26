using AgileTools.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgileTools.Core.Models;
using log4net;

namespace AgileTools.Client
{
    /// <summary>
    /// TODO: preload check on every method is ugly, replace it with nicer solution
    /// </summary>
    public class CachedJiraClient : ICardManagerClient
    {
        #region Private

        private static ILog _logger = LogManager.GetLogger(typeof(CachedJiraClient));
        private ICardManagerClient _client;
        private IEnumerable<JiraField> _fieldCache;
        private IEnumerable<CardStatus> _statusCache;
        private IList<User> _userCache;
        private IList<Card> _cardCache;
        private IList<Sprint> _sprintCache;
        private bool _preloadCompleted = false;

        #endregion

        public string Id { get => _client.Id; set => _client.Id = value; }

        public IModelConverter ModelConverter { get => _client.ModelConverter; set => _client.ModelConverter = value; }

        public IList<string> InitParameters => _client.InitParameters;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client"></param>
        public CachedJiraClient(ICardManagerClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _cardCache = new List<Card>();
            _sprintCache = new List<Sprint>();
            _userCache = new List<User>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initParam"></param>
        public void Init(Dictionary<string,string> initParam)
        {
            _client.Init(initParam);
        }

        /// <summary>
        /// Preloading some reference data
        /// </summary>
        private void PreloadData()
        {
            _logger.Info($"Starting data preloading");

            _fieldCache = _client.GetFields().ToList();
            _logger.Info($"Caching {_fieldCache.Count()} fields");

            _statusCache = _client.GetStatuses().ToList();
            _logger.Info($"Caching {_statusCache.Count()} statuses");

            _preloadCompleted = true;

            _logger.Info($"Preloading complete");
        }

        public void CommentTicket(string ticketId, string comment, string author = null)
        {
            _client.CommentTicket(ticketId, comment, author);
        }

        public IEnumerable<JiraField> GetFields()
        {
            if (!_preloadCompleted)
                PreloadData();

            return _fieldCache;
        }

        public Sprint GetSprint(string sprintId)
        {
            if (!_preloadCompleted)
                PreloadData();

            var match = _sprintCache.FirstOrDefault(spr => spr.Id == sprintId);
            if (match == null)
            {
                var sprint = _client.GetSprint(sprintId);
                if (sprint == null)
                    return null;

                _sprintCache.Add(sprint);
                _logger.Debug($"Caching sprint {sprint}");
                return sprint;
            }
            else
                return match;
        }

        public CardStatus GetStatus(string statusId)
        {
            if (!_preloadCompleted)
                PreloadData();

            var match = _statusCache.FirstOrDefault(s => s.Id == statusId);
            if (match != null)
                return match;

            // reload the cache if one is not found in it
            _statusCache = _client.GetStatuses();
            var status = _client.GetStatus(statusId);
            return status;
        }

        public IEnumerable<CardStatus> GetStatuses()
        {
            if (!_preloadCompleted)
                PreloadData();

            return _statusCache;
        }

        public Card GetTicket(string ticketId)
        {
            if (!_preloadCompleted)
                PreloadData();

            var match = _cardCache.FirstOrDefault(c => c.Id == ticketId);
            if (match != null)
                return match;

            var card = _client.GetTicket(ticketId);
            _cardCache.Add(card);
            _logger.Debug($"Caching card {card}");

            return card;
        }

        public IEnumerable<Card> GetTickets(string query)
        {
            if (!_preloadCompleted)
                PreloadData();

            return _client.GetTickets(query);
        }

        public User GetUser(string userId)
        {
            if (!_preloadCompleted)
                PreloadData();

            var match = _userCache.FirstOrDefault(s => s.Id == userId);
            if (match != null)
                return match;

            // reload the cache if one is not found in it
            var user = _client.GetUser(userId);
            _userCache.Add(user);
            _logger.Debug($"Caching user {user}");
            return user;
        }

        public bool TryCheckConnection()
        {
            return _client.TryCheckConnection();
        }
    }
}
