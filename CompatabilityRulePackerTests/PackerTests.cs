using CompatabilityRulePacker;

namespace CompatabilityRulePackerTests
{
    public class PackerTests
    {
        static readonly int[] items = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];

        [TestCaseSource(nameof(PackItemsWithCompatabilityRulesTestCases))]
        public void PackItemsWithCompatabilityRulesTest(Func<int, int, bool> itemsAreCompatable, int[] expectedUnassignedItems, int expectedBinCount)
        {
            (var bins, var unassignedItems) = Packer.PackItemsWithCompatabilityRules(items, itemsAreCompatable);

            Assert.Multiple(() =>
            {
                Assert.That(unassignedItems, Is.EquivalentTo(expectedUnassignedItems));
                Assert.That(bins, Has.Length.EqualTo(expectedBinCount));
                Assert.That(bins.SelectMany(c => c).Concat(unassignedItems), Is.EquivalentTo(items));
            });
            
            foreach (var bin in bins)
            {
                for (var i = 0; i < bin.Length; i++)
                {
                    for (var j = i + 1; j < bin.Length; j++)
                    {
                        Assert.That(itemsAreCompatable(bin[i], bin[j]));
                    }
                }
            }
        }

        public static IEnumerable<TestCaseData> PackItemsWithCompatabilityRulesTestCases
        {
            get
            {
                yield return new TestCaseData(
                    (int i1, int i2) => true,
                    items,
                    0
                ).SetName("All items compatible - {2} Bin(s)");

                yield return new TestCaseData(
                    (int i1, int i2) => false,
                    Array.Empty<int>(),
                    10
                ).SetName("All items incompatible - {2} Bin(s)");

                yield return new TestCaseData(
                    (int i1, int i2) => Math.Abs(i2 - i1) != 1,
                    Array.Empty<int>(),
                    2
                ).SetName("Chain incompatibility - {2} Bin(s)");

                yield return new TestCaseData(
                    (int i1, int i2) => i1 > 5 || (i1 + 4) % 5 != i2 && (i1 + 6) % 5 != i2,
                    new int[] { 5, 6, 7, 8, 9 },
                    3
                ).SetName("Loop of five incompatibility - {2} Bin(s)");

                yield return new TestCaseData(
                    (int i1, int i2) => i1 * i2 != 0,
                    Array.Empty<int>(),
                    2
                ).SetName("All incompatible with one item - {2} Bin(s)");

                yield return new TestCaseData(
                    (int i1, int i2) => i1 / 2 == i2 / 2 && Math.Abs(i1 - i2) == 1,
                    Array.Empty<int>(),
                    5
                ).SetName("Adjacent pairs only - {2} Bin(s)");
            }
        }
    }
}