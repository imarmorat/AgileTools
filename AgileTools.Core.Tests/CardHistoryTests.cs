using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgileTools.Core.Models;
using System.Collections.Generic;

namespace AgileTools.Core.Tests
{
    [TestClass]
    public class CardHistoryTests
    {
        [TestMethod]
        public void TestHistoryWhenNoChangeDone()
        {
            var card = new Card("fakeCard1", new Dictionary<CardFieldMeta, object>()
            {
                { CardFieldMeta.Title, "initial title" },
                { CardFieldMeta.Created, DateTime.Parse("1/1/2000") }
            });

            Assert.AreSame("initial title", card.GetFieldAtDate<string>(CardFieldMeta.Title, DateTime.Parse("1/1/2020")));
        }

        [TestMethod]
        public void TestHistoryWhenDatePriorToCreationDate()
        {
            var card = new Card("fakeCard1", new Dictionary<CardFieldMeta, object>()
            {
                { CardFieldMeta.Title, "initial title" },
                { CardFieldMeta.Created, DateTime.Parse("1/1/2000") }
            });

            Assert.AreEqual(null, card.GetFieldAtDate<string>(CardFieldMeta.Title, DateTime.Parse("1/1/1999")));
        }

        [TestMethod]
        public void TestHistoryWhenChangeBeforeAndAfterAtDate()
        {
            var card = new Card("fakeCard1", new Dictionary<CardFieldMeta, object>()
            {
                { CardFieldMeta.Title, "initial title" },
                { CardFieldMeta.Created, DateTime.Parse("1/1/2000") }
            });

            card.History.Add(new Card.HistoryItem { Field = CardFieldMeta.Title, From = "initial title", To = "updated title", On = DateTime.Parse("1/1/2001") });
            card.History.Add(new Card.HistoryItem { Field = CardFieldMeta.Title, From = "initial title", To = "final title", On = DateTime.Parse("1/1/2003") });

            Assert.AreSame("updated title", card.GetFieldAtDate<string>(CardFieldMeta.Title, DateTime.Parse("1/1/2002")));
        }

        [TestMethod]
        public void TestHistoryWhenChangeAfterAtDate()
        {
            var card = new Card("fakeCard1", new Dictionary<CardFieldMeta, object>()
            {
                { CardFieldMeta.Title, "initial title" },
                { CardFieldMeta.Created, DateTime.Parse("1/1/2000") }
            });

            card.History.Add(new Card.HistoryItem { Field = CardFieldMeta.Title, From = "initial title", To = "updated title", On = DateTime.Parse("1/1/2003") });

            Assert.AreSame("initial title", card.GetFieldAtDate<string>(CardFieldMeta.Title, DateTime.Parse("1/1/2002")));
        }
    }
}
