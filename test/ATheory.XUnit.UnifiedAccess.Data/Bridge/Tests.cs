using ATheory.UnifiedAccess.Data.Core;
using System;
using System.Collections.Generic;
using Xunit;
using static ATheory.UnifiedAccess.Data.Core.ServiceEnums;

namespace ATheory.XUnit.UnifiedAccess.Data.Bridge
{
    public class Tests
    {
        [Fact]
        public void PushSingle()
        {
            var bridge = Prepare.GetBridge();
            var result = bridge.Push<Author, AuthorMongo>(a => a.Id == 8, (s) => new AuthorMongo { Name = s.Name, Description = s.description });

            Assert.Equal(BridgeResult.Success, result);
        }

        [Fact]
        public void PullSingle()
        {
            var bridge = Prepare.GetBridge();
            var result = bridge.Pull<Author, AuthorMongo>(a => a.Index == 4, (s) => new Author { Name = s.Name, description = s.Description,index = s.Index + 20 });

            Assert.Equal(BridgeResult.Success, result);
        }
    }
}
