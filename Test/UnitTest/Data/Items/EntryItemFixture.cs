﻿using NUnit.Framework;
using Sitecore.Modules.WeBlog.Data.Items;
using System;

namespace Sitecore.Modules.WeBlog.UnitTest.Data.Items
{
    [TestFixture]
    public class EntryItemFixture
    {
        [Test]
        public void Ctor_NullItem_ThrowsException()
        {
            // arrange
            Action sutAction = () => new EntryItem(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(new TestDelegate(sutAction));
            Assert.That(ex.ParamName, Is.EqualTo("innerItem"));
        }
    }
}
