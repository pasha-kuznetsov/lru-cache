using System.Linq;
using NUnit.Framework;
using Tydbits.LruCache;

namespace Tydbits.LRUCache.Tests
{
    [TestFixture]
    internal class ProgramTests
    {
        private Program _program;

        [SetUp]
        public void SetUp()
        {
            _program = new Program();
        }

        [TestCase(@"
SIZE 3
GET foo
 set foo 1
GET foo
 SET  foo  1.1
get foo
  SET  spam  2 
GET spam
SET ham third
SET parrot four
GET foo
get spam
GET ham
GET ham parrot
GET parrot",
            @"
SIZE OK
NOTFOUND
SET OK
GOT 1
SET OK
GOT 1.1
SET OK
GOT 2
SET OK
SET OK
NOTFOUND
GOT 2
GOT third
ERROR
GOT four")]
        public void TestScenario(string input, string expectedOutput)
        {
            var commands = SplitStrings(input);
            var expectedResults = SplitStrings(expectedOutput);
            for (var i = 0; i < commands.Length - 1; i++)
                Assert.That(_program.Process(commands[i]), Is.EqualTo(expectedResults[i]), commands[i]);
            Assert.That(_program.Process("EXIT"), Is.Null, "EXIT");
        }

        private static string[] SplitStrings(string input)
        {
            return input.Trim().Split('\r', '\n').Where(NotBlank).ToArray();
        }

        private static bool NotBlank(string s)
        {
            return !string.IsNullOrWhiteSpace(s);
        }
    }
}
