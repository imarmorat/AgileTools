﻿using AgileTools.CommandLine.Common.Commands;
using AgileTools.Core;
using AgileTools.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgileTools.CommandLine.Common
{
    public class Context
    {
        private ICardManagerClient _cardService;

        public event EventHandler CardServiceChanged;

        public IList<CardManagerConfig> AvailableCardServices { get; set; }

        public ICardManagerClient CardService
        {
            get => _cardService;
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                _cardService = value;
                CardServiceChanged?.Invoke(this, null);
            }
        }

        public IList<Card> LoadedCards { get; set; }

        public CommandManager CmdManager { get; set; }

        public VariableManager VariableManager { get; set; }

        public Context()
        {
            LoadedCards = new List<Card>();
            AvailableCardServices = new List<CardManagerConfig>();
        }
    }
}
